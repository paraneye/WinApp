using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WinAppLibrary.Utilities
{
    public class ServiceHelper
    {
        public const string ProjectService = "ProjectService.svc/custom";
        public const string CommonService = "CommonService.svc/custom";
        public const string UserService = "UserService.svc/custom";
        public const string P6ManagerService = "P6ManagerService.svc/custom";

        private static int type = BindType.Custom;

        public static T GetServiceClient<T>(string servicetype)
        {
            T retValue;
            string ws_url = Helper.ServiceUrl;

            switch (type)
            {
                case BindType.Basic:
                    retValue = GetServiceClient_Basic<T>(ws_url + servicetype);
                    break;
                case BindType.Custom:
                    retValue = GetServiceClient_Custom<T>(ws_url + servicetype);
                    break;
                //case BindType.WebHttp:
                //    retValue = GetServiceClient_WebHttp<T>(ws_url);
                //    break;
                default:
                    retValue = GetServiceClient_Basic<T>(ws_url + servicetype);
                    break;
            }

            return retValue;
        }

        private static T GetServiceClient_Custom<T>(string ws_url)
        {
            CustomBinding binding = new CustomBinding(
                new BinaryMessageEncodingBindingElement(),
                new HttpTransportBindingElement { MaxBufferSize = int.MaxValue, MaxReceivedMessageSize = int.MaxValue });

            return (T)(Activator.CreateInstance(typeof(T), new object[] { binding, new EndpointAddress(new Uri(ws_url)) }));
        }

        private static T GetServiceClient_Basic<T>(string ws_url)
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding(
                    (new Uri(ws_url)).Scheme.Equals("https", StringComparison.CurrentCultureIgnoreCase)
                    ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None);
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.CloseTimeout = new TimeSpan(00, 10, 00);
                binding.OpenTimeout = new TimeSpan(00, 10, 00);
                binding.ReceiveTimeout = new TimeSpan(00, 10, 00);
                binding.SendTimeout = new TimeSpan(00, 10, 00);

                return (T)(Activator.CreateInstance(typeof(T), new object[] { binding, new EndpointAddress(new Uri(ws_url)) }));
            }
            catch
            {
                return (T)(Activator.CreateInstance(typeof(T)));
            }
        }

        //wsHttpbinding is suppored for security reason.
        /*
        private static T GetServiceClient_WebHttp<T>(string ws_url)
        {
            CustomBinding binding = new CustomBinding(
                new TextMessageEncodingBindingElement(
                    MessageVersion.Soap12WSAddressing10,
                    System.Text.Encoding.UTF8),
                new HttpCookieContainerBindingElement(),
                new HttpTransportBindingElement { MaxBufferSize = int.MaxValue, MaxReceivedMessageSize = int.MaxValue });

            return (T)(Activator.CreateInstance(typeof(T), new object[] { binding, new EndpointAddress(new Uri(ws_url)) }));
        }
        */
    }

    public struct BindType
    {
        public const int Basic = 0;
        public const int WebHttp = 1;
        public const int Custom = 2;
    }
}
