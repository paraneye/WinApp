using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using NdefLibrary.Ndef;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace Element.Reveal.Crew.Lib
{
    class ProximityHandler
    {
        public event EventHandler<object> OnException;
        public event EventHandler<object> OnMessage;

        #region "Properties"
        private Windows.UI.Core.CoreDispatcher proximityDispatcher = Window.Current.CoreWindow.Dispatcher;
        private ProximityDevice _proximity;
        private bool _triggeredConnectSupported = false;
        private bool _browseConnectSupported = false;
        private bool _peerFinderStarted = false;
        private long _subscribedMessageId = -1;
        private long _publishedMessageId = -1;

        private PeerInformation _requestingPeer;
        private StreamSocket _socket = null;
        private bool _socketClosed = true;
        private DataWriter _dataWriter = null;
        #endregion

        public ProximityHandler()
        {
            _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) ==
                                         PeerDiscoveryTypes.Triggered;
            _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) ==
                                      PeerDiscoveryTypes.Browse;
        }
        
        public void SetProximityDevice(ProximityDevice pdevice)
        {
            if (pdevice != null)
            {
                _proximity = pdevice;
                _subscribedMessageId = _proximity.SubscribeForMessage("NDEF", messageReceived);
                _peerFinderStarted = true;
            }
            else
                this.NotifyUser("NFC Device could not be found...", NotifyType.ErrorMessage);
        }

        public void DesetProximityDevice()
        {
            if (_proximity != null)
            {
                _proximity.StopSubscribingForMessage(_subscribedMessageId);
            }
        }

        public void SetPublishCrew(string personnel)
        {
            if (_proximity != null)
            {
                if (_subscribedMessageId != -1)
                {
                    _proximity.StopSubscribingForMessage(_subscribedMessageId);

                    _subscribedMessageId = -1;
                }

                try
                {
                    
                    //In case for Crew
                    var record = GetPublishingMessage(typeof(NdefTextRecord), personnel.ToString());
                    //In case for foreman to auto login
                    //var record = GetPublishingMessage(typeof(NdefLaunchAppRecord), "user=" + personnelId);

                    var msg = new NdefMessage { record };
                    _proximity.PublishBinaryMessage("NDEF:WriteTag", msg.ToByteArray().AsBuffer(), messageTransmitted);
                    //NotifyUser("Publishing Message: \n" + record.ToString(), NotifyType.StatusMessage);

                    /*
                     * This section is based on MSDN and same function with using NDEF Library.
                     * But it was not verified
                     */
                    //string launchArgs = "user=" + personnelId;
                    //string praid = CoreApplication.Id; // The Application Id value from your package.appxmanifest.
                    //string appName = Package.Current.Id.FamilyName + "!" + praid;
                    //string launchAppMessage = launchArgs + "\tWindows\t" + appName;
                    //var dataWriter = new Windows.Storage.Streams.DataWriter();
                    //dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
                    //dataWriter.WriteString(launchAppMessage);
                    //_proximity.PublishBinaryMessage("LaunchApp:WriteTag", dataWriter.DetachBuffer(), messageTransmitted);
                    //NotifyUser("Publishing Message: \n" + launchArgs, NotifyType.StatusMessage);
                    
                }
                catch (Exception e)
                {
                    _subscribedMessageId = _proximity.SubscribeForMessage("NDEF", messageReceived);
                    this.NotifyUser(e.Message, NotifyType.ErrorMessage);
                }
            }
            else
                this.NotifyUser("NFC Device could not be found...", NotifyType.ErrorMessage);
        }

        public void SetPublishLaunch(string personnel)
        {
            if (_proximity != null)
            {
                if (_subscribedMessageId != -1)
                {
                    _proximity.StopSubscribingForMessage(_subscribedMessageId);
                    _subscribedMessageId = -1;
                }

                try
                {
                    string launchAppMessage = GetLaunchMessage("user=" + personnel);
                    
                    var dataWriter = new Windows.Storage.Streams.DataWriter();
                    dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;
                    dataWriter.WriteString(launchAppMessage);
                    _proximity.PublishBinaryMessage("LaunchApp:WriteTag", dataWriter.DetachBuffer(), messageTransmitted);

                }
                catch (Exception e)
                {
                    _subscribedMessageId = _proximity.SubscribeForMessage("NDEF", messageReceived);
                    this.NotifyUser(e.Message, NotifyType.ErrorMessage);
                }
            }
            else
                this.NotifyUser("NFC Device could not be found...", NotifyType.ErrorMessage);
        }

        public void SetPeerAccess()
        {
            if (_proximity != null && !_peerFinderStarted)
            {
                //This part is for handling Peer Device 
                _proximity.DeviceArrived += ProximityDeviceArrived;
                _proximity.DeviceDeparted += ProximityDeviceDeparted;

                PeerFinder.TriggeredConnectionStateChanged += new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // attach the incoming connection request event handler
                PeerFinder.ConnectionRequested += new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);
                // start listening for proximate peers
                PeerFinder.Start();
            }
            else
                this.NotifyUser("We couldn't find any connectable device!", NotifyType.ErrorMessage);
        }

        public void StopPublish()
        {
            if (_publishedMessageId != -1)
            {
                _proximity.StopPublishingMessage(_publishedMessageId);
                _publishedMessageId = -1;
            }

            if (_proximity != null && _subscribedMessageId == -1)
                _subscribedMessageId = _proximity.SubscribeForMessage("NDEF", messageReceived);
        }
        #region "Message Handler"
        private void messageReceived(Windows.Networking.Proximity.ProximityDevice sender, Windows.Networking.Proximity.ProximityMessage message)
        {
            var rawMsg = message.Data.ToArray();
            var ndefMessage = NdefMessage.FromByteArray(rawMsg);

            if (ndefMessage != null && ndefMessage.Count > 0)
            {
                string tagmsg = string.Empty;
                // Loop over all records contained in the NDEF message
                foreach (NdefRecord record in ndefMessage)
                    tagmsg = HandleNDEFRecord(record);

                //This is aimed for handling only one crew in NFC Data
                //Because one NFC card is taken by one crew
                if (OnMessage != null)
                    OnMessage(NotifyType.NdefMessage, tagmsg);
            }
        }

        private void messageTransmitted(Windows.Networking.Proximity.ProximityDevice sender, long messageId)
        {
            _publishedMessageId = messageId;
            StopPublish();
            this.NotifyUser("NFC message was successfully transferred.!", NotifyType.PublishMessage);
        }

        #region "Peer Device Handler"
        private void ProximityDeviceArrived(Windows.Networking.Proximity.ProximityDevice device)
        {
            this.NotifyUser("Proximate device arrived. id = " + device.DeviceId, NotifyType.PeerMessage);
        }

        private void ProximityDeviceDeparted(Windows.Networking.Proximity.ProximityDevice device)
        {
            this.NotifyUser("Proximate device departed. id = " + device.DeviceId, NotifyType.PeerMessage);
        }

        //connection states
        string[] rgConnectState = {"PeerFound", 
                                   "Listening",
                                   "Connecting",
                                   "Completed",
                                   "Canceled",
                                   "Failed"};

        private async void TriggeredConnectionStateChangedEventHandler(object sender, TriggeredConnectionStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case TriggeredConnectState.PeerFound:
                    // Use this state to indicate to users that the tap is complete and
                    // they can pull their devices away.
                    //this.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage);
                    break;
                case TriggeredConnectState.Completed:
                    this.NotifyUser("Socket connect success!", NotifyType.StatusMessage);
                    // Start using the socket that just connected.
                    await proximityDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.PeerFinder_StartSendReceive(e.Socket);
                    });
                    break;
                case TriggeredConnectState.Failed:
                    this.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage);
                    break;
                default:
                    this.NotifyUser("TriggeredConnectionStateChangedEventHandler - " + rgConnectState[(int)e.State], NotifyType.PeerMessage);
                    break;
            }
        }

        private async void PeerConnectionRequested(object sender, ConnectionRequestedEventArgs e)
        {
            _requestingPeer = e.PeerInformation;

            await proximityDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.NotifyUser("Connection requested from peer " + e.PeerInformation.DisplayName, NotifyType.PeerMessage);
                AccepPeer();
                //this.PeerFinder_SendButton.Visibility = Visibility.Collapsed;
            });
        }

        protected async void AccepPeer()
        {
            this.NotifyUser("Connecting to " + _requestingPeer.DisplayName + "....", NotifyType.StatusMessage);
            try
            {
                StreamSocket socket = await PeerFinder.ConnectAsync(_requestingPeer);
                this.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                PeerFinder_StartSendReceive(socket);
            }
            catch (Exception err)
            {
                this.NotifyUser("Connection to " + _requestingPeer.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
            }
        }

        // Start the send receive operations
        void PeerFinder_StartSendReceive(StreamSocket socket)
        {
            _socket = socket;
            // If the scenario was switched just as the socket connection completed, just close the socket.
            if (!_peerFinderStarted)
            {
                CloseSocket();
                return;
            }

            _dataWriter = new DataWriter(_socket.OutputStream);
            _socketClosed = false;
            PeerFinder_StartReader(new DataReader(_socket.InputStream));
        }

        async void PeerFinder_StartReader(DataReader socketReader)
        {
            try
            {
                uint bytesRead = await socketReader.LoadAsync(sizeof(uint));
                if (bytesRead > 0)
                {
                    uint strLength = (uint)socketReader.ReadUInt32();
                    bytesRead = await socketReader.LoadAsync(strLength);
                    if (bytesRead > 0)
                    {
                        String message = socketReader.ReadString(strLength);
                        this.NotifyUser("Got message: " + message, NotifyType.PeerMessage);
                        PeerFinder_StartReader(socketReader); // Start another reader
                    }
                    else
                    {
                        SocketError("The remote side closed the socket");
                        socketReader.Dispose();
                    }
                }
                else
                {
                    SocketError("The remote side closed the socket");
                    socketReader.Dispose();
                }
            }
            catch (Exception e)
            {
                if (!_socketClosed)
                {
                    SocketError("Reading from socket failed: " + e.Message);
                }
                socketReader.Dispose();
            }
        }
        #endregion

        #endregion

        #region "Private Method"
        private string HandleNDEFRecord(NdefRecord record)
        {
            string tag = string.Empty;
            var specializedType = record.CheckSpecializedType(false);

            if (specializedType == typeof(NdefTextRecord))
            {
                var oRecord = new NdefTextRecord(record);
                try
                {
                    tag = oRecord.Text;
                    this.NotifyUser(tag, NotifyType.PublishMessage);
                }
                catch { }
            }
            else if (specializedType == typeof(NdefSpRecord))
            {
                var oRecord = new NdefSpRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else if (specializedType == typeof(NdefUriRecord))
            {
                var oRecord = new NdefUriRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else if (specializedType == typeof(NdefTelRecord))
            {
                var oRecord = new NdefTelRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Tel:" + oRecord.TelNumber + ")");
            }
            else if (specializedType == typeof(NdefMailtoRecord))
            {
                var oRecord = new NdefMailtoRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Subject:" + oRecord.Subject + ")");
            }
            else if (specializedType == typeof(NdefAndroidAppRecord))
            {
                var oRecord = new NdefAndroidAppRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Package:" + oRecord.PackageName + ")");
            }
            else if (specializedType == typeof(NdefLaunchAppRecord))
            {
                var oRecord = new NdefLaunchAppRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Platform:" + oRecord.PlatformIds + ")");
            }
            else if (specializedType == typeof(NdefSmsRecord))
            {
                var oRecord = new NdefSmsRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Sms:" + oRecord.SmsBody + ")");
            }
            else if (specializedType == typeof(NdefSmartUriRecord))
            {
                var oRecord = new NdefSmartUriRecord(record);
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else
            {
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supporting Type");
            }

            return tag;
        }

        private NdefRecord GetPublishingMessage(Type type, string data)
        {
            NdefRecord retValue = null;

            if (type == typeof(NdefTextRecord))
            {
                var oRecord = new NdefTextRecord();
                oRecord.Text = data;
                retValue = oRecord;
            }
            else if (type == typeof(NdefSpRecord))
            {
                var oRecord = new NdefSpRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else if (type == typeof(NdefUriRecord))
            {
                var oRecord = new NdefUriRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else if (type == typeof(NdefTelRecord))
            {
                var oRecord = new NdefTelRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Tel:" + oRecord.TelNumber + ")");
            }
            else if (type == typeof(NdefMailtoRecord))
            {
                var oRecord = new NdefMailtoRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Subject:" + oRecord.Subject + ")");
            }
            else if (type == typeof(NdefAndroidAppRecord))
            {
                var oRecord = new NdefAndroidAppRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Package:" + oRecord.PackageName + ")");
            }
            else if (type == typeof(NdefLaunchAppRecord))
            {
                var oRecord = new NdefLaunchAppRecord();         
     
                string praid = CoreApplication.Id; // The Application Id value from your package.appxmanifest.
                string appName = Package.Current.Id.FamilyName + "!" + praid;

                NdefLaunchAppRecord launch = new NdefLaunchAppRecord();
                launch.AddPlatformAppId("Windows", appName);
                launch.Arguments = data;
                retValue = oRecord;
            }
            else if (type == typeof(NdefSmsRecord))
            {
                var oRecord = new NdefSmsRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Sms:" + oRecord.SmsBody + ")");
            }
            else if (type == typeof(NdefSmartUriRecord))
            {
                var oRecord = new NdefSmartUriRecord();
                //this.NotifyUser(NotifyType.PeerMessage, "Not Supportint Type, Value (Uri:" + oRecord.Uri + ")");
            }
            else
            {
                //
            }

            return retValue;
        }

        private void SocketError(String errMessage)
        {
            this.NotifyUser(errMessage, NotifyType.ErrorMessage);

            if (_browseConnectSupported)
            {
                //PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible;
            }

            CloseSocket();
        }

        private void CloseSocket()
        {
            if (_socket != null)
            {
                _socketClosed = true;
                _socket.Dispose();
                _socket = null;
            }

            if (_dataWriter != null)
            {
                _dataWriter.Dispose();
                _dataWriter = null;
            }
        }

        private string GetLaunchMessage(string launchArgs)
        {
            string launchAppMessage = string.Format("{0}\tWindows\t{1}!{2}", launchArgs, Package.Current.Id.FamilyName, CoreApplication.Id);
            return launchAppMessage;
        }

        public void Dispose()
        {
            CloseSocket();
        }

        private void NotifyUser(string msg, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.NdefMessage:
                case NotifyType.PeerMessage:
                case NotifyType.StatusMessage:
                case NotifyType.PublishMessage:
                    if (OnMessage != null)
                        OnMessage(type, msg);
                    break;
                case NotifyType.ErrorMessage:
                    if (OnException != null)
                        OnException(type, msg);
                    break;
            }
        }
        #endregion
    }
}
