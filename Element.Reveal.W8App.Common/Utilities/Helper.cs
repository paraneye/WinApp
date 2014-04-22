using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core; 
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Imaging;
using WinRTXamlToolkit.Composition;
using Windows.UI.Xaml.Media;

namespace WinAppLibrary.Utilities
{
    public class Helper
    {
        private static Uri _baseUri = new Uri("ms-appx:///");
        public static Uri BaseUri { get { return _baseUri; } }
        public const string AppParametrUser = "user";
        public static string UserStatusPhotoUrl = "";
        public static string UserPhotoUrl = "";
        public static DateTime DateTimeMinValue
        {
            get
            {
                return new DateTime(1753, 1, 1);
            }
        }

        #region "Storage Helper"
        public static string DBInstance
        {
            get
            {
                var instance = GetValueFromStorage("RevealDBInstance");
                if (string.IsNullOrEmpty(instance))
                    return new ResourceLoader("WinAppLibrary/Resources").GetString("RevealDBInstance");
                else
                    return instance;
            }

            set { SetValueInStorage("RevealDBInstance", value); }
        }

        public static string ServiceUrl
        {
            get
            {
                var instance = GetValueFromStorage("RevealService");
                if (string.IsNullOrEmpty(instance))
                    return new ResourceLoader("WinAppLibrary/Resources").GetString("RevealService");
                else
                    return instance;
            }

            set { SetValueInStorage("RevealService", value); }
        }

        public static bool ActivateOffMode
        {
            get
            {
                bool value = false;
                bool.TryParse(GetValueFromStorage("OffMode"), out value);
                return value;
            }
            set { SetValueInStorage("OffMode", value); }
        }

        public static bool DownloadedData
        {
            get
            {
                bool value = false;
                bool.TryParse(GetValueFromStorage("Downloaded"), out value);
                return value;
            }
            set { SetValueInStorage("Downloaded", value); }
        }

        public static string LoginID
        {
            get
            {
                return GetValueFromStorage("LoginID");
            }

            set { SetValueInStorage("LoginID", value); }
        }

        public static string PinCode
        {
            get
            {
                return GetValueFromStorage("PinCode");
            }

            set { SetValueInStorage("PinCode", value); }
        }

        public static string JsonAddListItem
        {
            get { return new ResourceLoader("WinAppLibrary/Resources").GetString("AddListItemJSONStr"); }
        }

        public static string JsonAddList
        {
            get { return new ResourceLoader("WinAppLibrary/Resources").GetString("AddListJSONStr"); }
        }

        public static string GetResourceString(string name)
        {
            return new ResourceLoader("WinAppLibrary/Resources").GetString(name);
        }

        public static string GetValueFromStorage(string key)
        {
            string retValue = string.Empty;

            try
            {
                ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
                retValue = roamingSettings.Values[key].ToString();
            }
            catch
            {
                // dont crash if we cannot retrieve the site url 
            }
            return retValue;
        }

        public static void SetValueInStorage(string key, string value)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[key] = value;
        }

        public static void SetValueInStorage(string key, object value)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[key] = value == null ? null : value.ToString();
        }

        public static string GetStringValueFromResourceByKey(string key)
        {
            string retValue = string.Empty;

            try
            {
                retValue = Application.Current.Resources[key].ToString();
            }
            catch { }

            return retValue;
        }
        #endregion

        #region "Image Helper"
        public async Task<WriteableBitmap> GetWriteableBitmapFromBitmapImage(BitmapImage bitImg)
        {
            WriteableBitmap wrtBit = null;
            if (bitImg != null)
            {
                try
                {
                    wrtBit = new WriteableBitmap(1, 1);
                    await wrtBit.FromBitmapImage(bitImg);
                }
                catch (Exception e)
                {
                    wrtBit = null;
                    //Lib.Utility.Helper.ExceptionHandler(e, "GetWriteableBitmapFromBitmapImage");
                }
            }
            return wrtBit;
        }

        public async Task<WriteableBitmap> GetWriteableBitmapFromFile(BitmapImage bitImg)
        {
            WriteableBitmap wrtBit = null;
            StorageFile file = await GetStorageFileFromUri(bitImg.UriSource);

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                wrtBit = new WriteableBitmap(bitImg.PixelWidth, bitImg.PixelHeight);
                await wrtBit.SetSourceAsync(stream);
            }

            return wrtBit;
        }

        private async Task<StorageFile> GetStorageFileFromUri(Uri urisource)
        {
            StorageFile file = null;

            try
            {
                if (string.Compare(BaseUri.Scheme, urisource.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
                    file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(BaseUri.Scheme + "://" + urisource.LocalPath));
                else
                    file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(urisource);
            }
            catch { }

            return file;
        }

        public async Task<WriteableBitmap> GetWriteableBitmapFromUri(Uri urisource)
        {
            WriteableBitmap wrtBit = null;
            if (urisource != null)
            {
                try
                {
                    if (!urisource.IsFile && urisource.Port > 0)
                        wrtBit = await GetWriteableBitmapFromHttp(urisource);
                    else
                        wrtBit = await GetWriteableBitmapFromFile(urisource);
                }
                catch (Exception e)
                {
                    //Lib.Utility.Helper.ExceptionHandler(e, "GetWriteableBitmapFromBitmapImage");
                }
            }
            return wrtBit;
        }

        //This function is not completed and throws error. This needs to be done more.
        private async Task<WriteableBitmap> GetWriteableBitmapFromhHttp(BitmapImage bitImg)
        {
            WriteableBitmap wrtBit = null;

            var rass = RandomAccessStreamReference.CreateFromUri(bitImg.UriSource);
            using (IRandomAccessStream stream = await rass.OpenReadAsync())
            {
                wrtBit = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(bitImg.PixelWidth, bitImg.PixelHeight);
                await wrtBit.SetSourceAsync(stream);
            }

            return wrtBit;
        }

        private async Task<WriteableBitmap> GetWriteableBitmapFromFile(Uri urisource)
        {
            WriteableBitmap wrtBit = null;
            StorageFile file = await GetStorageFileFromUri(urisource);

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                BitmapImage bitImg = new BitmapImage();
                var clone = stream.CloneStream();
                bitImg.SetSource(clone);
                wrtBit = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(bitImg.PixelWidth, bitImg.PixelHeight);
                clone.Dispose();
                await wrtBit.SetSourceAsync(stream);
            }

            return wrtBit;
        }

        //This function is not completed and throws error. This needs to be done more.
        private async Task<WriteableBitmap> GetWriteableBitmapFromHttp(Uri urisource)
        {
            WriteableBitmap wrtBit = null;

            var rass = RandomAccessStreamReference.CreateFromUri(urisource);
            using (IRandomAccessStream stream = await rass.OpenReadAsync())
            {
                wrtBit = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(1, 1);                
                await wrtBit.SetSourceAsync(stream);
            }
            return wrtBit;

            //This is reserved for future help in case of converting stream to IRandomAccessStream

            //byte[] bytes = await http.GetByteArrayAsync(bitImg.UriSource);
            //using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            //{
            //    await stream.AsStreamForWrite().WriteAsync(bytes, 0, bytes.Length);
            //    await stream.FlushAsync();
            //    stream.Seek(0);
            //    wrtBit = new Windows.UI.Xaml.Media.Imaging.WriteableBitmap(bitImg.PixelWidth, bitImg.PixelHeight);
            //    await wrtBit.SetSourceAsync(stream);
            //}

            //using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
            //{
            //    var response = await HttpUtility.GetHttpResponseMessage(urisource);
            //    DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
            //    writer.WriteBytes(await response.Content.ReadAsByteArrayAsync());
            //    await writer.StoreAsync();
            //}
        }

        public async Task<WriteableBitmap> GetWriteableBitmapFromBytes(byte[] bytes)
        {
            WriteableBitmap retValue = null;

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                retValue = new WriteableBitmap(1, 1);
                await retValue.SetSourceAsync(stream);
                //retValue.FromByteArray(bytes);
            }

            return retValue;
        }

        //This is returning Pixel Binaries so need to be confirmed for its compatibility.
        public byte[] GetBytesFromWritableBitmap(WriteableBitmap wrtImg)
        {
            byte[] retValue = null;

            using (Stream stream = wrtImg.PixelBuffer.AsStream())
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                retValue = memoryStream.ToArray();
                wrtImg.Invalidate();
            }

            return retValue;
        }

        public async Task<BitmapImage> GetBitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage retValue = null;

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                retValue = new BitmapImage();
                retValue.SetSource(stream);
            }

            return retValue;
        }

        public async Task<BitmapImage> GetBitmapImageFromBytes(byte[] bytes, int decodedWidth, int decodedHeight)
        {
            BitmapImage retValue = null;

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                retValue = new BitmapImage();
                retValue.DecodePixelWidth = decodedWidth;
                retValue.DecodePixelHeight = decodedHeight;
                retValue.SetSource(stream);
            }

            return retValue;
        }

        public async Task<BitmapImage> GetBitmapImageFromBytesWithRatio(byte[] bytes, int decodedWidth, int decodedHeight)
        {
            BitmapImage retValue = null;

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(bytes);
                await writer.StoreAsync();
                BitmapImage temp = new BitmapImage();
                
                temp.SetSource(stream);

                if (temp.PixelHeight > temp.PixelWidth)
                {
                    decodedWidth = (int)Math.Round((double)decodedHeight * ((double)temp.PixelWidth / (double)temp.PixelHeight));
                }
            }

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(bytes);
                await writer.StoreAsync();

                retValue = new BitmapImage();
                retValue.DecodePixelHeight = decodedHeight;
                retValue.DecodePixelWidth = decodedWidth;
                retValue.SetSource(stream);
            }

            return retValue;
        }

        public async Task<BitmapImage> GetRenderedBitmapImage(WriteableBitmap wrtImg, uint width, uint height)
        {
            BitmapImage retValue = null;

            if (wrtImg != null)
            {
                Windows.Graphics.Imaging.BitmapDecoder decoder = null;

                using (InMemoryRandomAccessStream rstream = new InMemoryRandomAccessStream())
                {
                    var stream = wrtImg.PixelBuffer.AsStream();
                    //byte[] pixels = new byte[stream.Length];
                    //ConvertToRGBA(wrtImg.PixelHeight, wrtImg.PixelWidth, pixels);

                    //await stream.ReadAsync(pixels, 0, pixels.Length);

                    var randomAccessStream = new InMemoryRandomAccessStream();
                    var outputStream = randomAccessStream.GetOutputStreamAt(0);
                    await RandomAccessStream.CopyAndCloseAsync(stream.AsInputStream(), outputStream);

                    //var imsWriter = rstream.AsStreamForRead(pixels.Length);
                    //await Task.Factory.StartNew(() => stream.CopyTo(imsWriter));
                    //await stream.FlushAsync();

                    decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(rstream);
                }

                if (decoder != null)
                {
                    using (InMemoryRandomAccessStream wstream = new InMemoryRandomAccessStream())
                    {
                        var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId, wstream);
                        var pixeldata = await decoder.GetPixelDataAsync();
                        encoder.SetPixelData(decoder.BitmapPixelFormat, Windows.Graphics.Imaging.BitmapAlphaMode.Straight, width, height, decoder.DpiX, decoder.DpiY,
                            pixeldata.DetachPixelData().ToArray());
                        await encoder.FlushAsync();

                        retValue = new BitmapImage();
                        await retValue.SetSourceAsync(wstream);
                    }
                }

                #region "Alternative Decoding"
                /*
                Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(fileStream);
                // Scale image to appropriate size 

                Windows.Graphics.Imaging.BitmapTransform transform = new Windows.Graphics.Imaging.BitmapTransform()
                {
                    ScaledWidth = Convert.ToUInt32(wrtImg.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(wrtImg.PixelHeight)
                };
                Windows.Graphics.Imaging.PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, // WriteableBitmap uses BGRA format 
                    Windows.Graphics.Imaging.BitmapAlphaMode.Straight,
                    transform,
                    Windows.Graphics.Imaging.ExifOrientationMode.IgnoreExifOrientation, // This sample ignores Exif orientation 
                    Windows.Graphics.Imaging.ColorManagementMode.DoNotColorManage
                );

                // An array containing the decoded image data, which could be modified before being displayed 
                byte[] sourcePixels = pixelData.DetachPixelData();

                // Open a stream to copy the image contents to the WriteableBitmap's pixel buffer 
                using (Stream stream = wrtImg.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                }
                */
                #endregion
            }

            return retValue;
        }

        private void ConvertToRGBA(int pixelHeight, int pixelWidth, byte[] pixels)
        {
            if (pixels == null)
                return;

            int offset;

            for (int row = 0; row < (uint)pixelHeight; row++)
            {
                for (int col = 0; col < (uint)pixelWidth; col++)
                {
                    offset = (row * (int)pixelWidth * 4) + (col * 4);

                    byte B = pixels[offset];
                    byte G = pixels[offset + 1];
                    byte R = pixels[offset + 2];
                    byte A = pixels[offset + 3];

                    // convert to RGBA format for BitmapEncoder
                    pixels[offset] = R; // Red
                    pixels[offset + 1] = G; // Green
                    pixels[offset + 2] = B; // Blue
                    pixels[offset + 3] = A; // Alpha
                }
            }
        }

        public Windows.UI.Xaml.Media.SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            Windows.UI.Xaml.Media.SolidColorBrush retValue = null;

            if (!string.IsNullOrEmpty(hexaColor))
            {
                var hex = hexaColor.Replace(@"#", "");
                switch (hex.Length)
                {
                    case 6:
                        retValue = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(
                                        255,
                                        Convert.ToByte(hex.Substring(0, 2), 16),
                                        Convert.ToByte(hex.Substring(2, 2), 16),
                                        Convert.ToByte(hex.Substring(4, 2), 16)));
                        break;
                    case 8:
                        retValue = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(
                                        Convert.ToByte(hex.Substring(0, 2), 16),
                                        Convert.ToByte(hex.Substring(2, 2), 16),
                                        Convert.ToByte(hex.Substring(4, 2), 16),
                                        Convert.ToByte(hex.Substring(6, 2), 16)));
                        break;
                }
            }

            return retValue;
        }

        public async Task<Stream> GetStreamFromtWriteableBitmap(WriteableBitmap bitmap)
        {
            WriteableBitmap bmp = bitmap;

            using (Stream stream = bmp.PixelBuffer.AsStream())
            {
                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream;
            }
        }


        public async Task<Stream> GetJpegStreamFromWriteableBitmap(WriteableBitmap writableImage)
        {
            //StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Temp.jpg", CreationCollisionOption.ReplaceExisting);
            //await writableImage.SaveToFile(file, Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId);

            using (IRandomAccessStream ranstream = new InMemoryRandomAccessStream())
            {
                Windows.Graphics.Imaging.BitmapEncoder encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateAsync(Windows.Graphics.Imaging.BitmapEncoder.JpegEncoderId, ranstream);
                Stream pixelStream = writableImage.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];

                pixelStream.Seek(0, SeekOrigin.Begin);
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(
                    Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                    Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied,
                    (uint)writableImage.PixelWidth,
                    (uint)writableImage.PixelHeight,
                    96,
                    96,
                    pixels);

                await encoder.FlushAsync();

                using (var outputStream = ranstream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();

                    var copy = ranstream.CloneStream();
                    ranstream.Dispose();
                    return copy.AsStream();
                }
            }
        }

        public string GetStringFromImageRandomAccessStream(IRandomAccessStream ranstream)
        {
            byte[] imageData = new byte[ranstream.Size];
            ranstream.Seek(0);

            for (int i = 0; i < imageData.Length; i++)
            {
                imageData[i] = (byte)ranstream.AsStreamForRead().ReadByte();
            }

            const int MAX_URI_LENGTH = 32766;
            string base64img = System.Convert.ToBase64String(imageData);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < base64img.Length; i += MAX_URI_LENGTH)
            {
                sb.Append(Uri.EscapeDataString(base64img.Substring(i, Math.Min(MAX_URI_LENGTH, base64img.Length - i))));
            }

            return sb.ToString();
        }

        public async Task<Stream> GetImageStreamFromUri(Uri imguri)
        {
            Stream retValue = null;

            try
            {
                var rass = RandomAccessStreamReference.CreateFromUri(imguri);
                using (IRandomAccessStream stream = await rass.OpenReadAsync())
                {
                    retValue = stream.CloneStream().AsStream();
                }
            }
            catch { }

            return retValue;
        }

        public async Task<byte[]> GetJpegBytesFromUri(Uri imguri)
        {
            byte[] retValue = null;
            try
            {
                RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(imguri);

                using (IRandomAccessStreamWithContentType fileStream = await streamRef.OpenReadAsync())
                {
                    retValue = new byte[fileStream.Size];
                    var buffer = await fileStream.ReadAsync(retValue.AsBuffer(), (uint)retValue.Length, InputStreamOptions.None);
                    retValue = buffer.ToArray();

                    #region "In case of needing encoding.
                    //Windows.Graphics.Imaging.BitmapDecoder decoder = null;
                    //decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(Windows.Graphics.Imaging.BitmapDecoder.JpegDecoderId, fileStream);

                    //if (decoder != null)
                    //{
                    //    using (InMemoryRandomAccessStream wstream = new InMemoryRandomAccessStream())
                    //    {
                    //        wstream.Seek(0);
                    //        BitmapImage img = new BitmapImage();
                    //        img.SetSource(wstream);
                    //        var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateForTranscodingAsync(wstream, decoder);
                    //        var pixeldata = await decoder.GetPixelDataAsync();
                    //        byte[] sourcePixels = pixeldata.DetachPixelData();
                    //        retValue = new byte[sourcePixels.Length];

                    //        encoder.SetPixelData(
                    //            decoder.BitmapPixelFormat,
                    //            Windows.Graphics.Imaging.BitmapAlphaMode.Straight,
                    //            decoder.PixelWidth,
                    //            decoder.PixelHeight,
                    //            decoder.DpiX,
                    //            decoder.DpiY,
                    //            sourcePixels);

                    //        await encoder.FlushAsync();

                    //        retValue = new byte[wstream.Size];
                    //        var buffer = await wstream.ReadAsync(retValue.AsBuffer(), (uint)retValue.Length, InputStreamOptions.None);
                    //        retValue = buffer.ToArray();
                    //    }
                    //}
                    #endregion
                }
            }
            catch { }

            return retValue;
        }

        public static async Task<byte[]> GetImageBytesFromStorageFile(StorageFile image)
        {
            IRandomAccessStream fileStream = await image.OpenAsync(FileAccessMode.Read);
            var reader = new Windows.Storage.Streams.DataReader(fileStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)fileStream.Size);

            byte[] pixels = new byte[fileStream.Size];

            reader.ReadBytes(pixels);

            return pixels;

        }

        #endregion

        #region "File IO Helper"
        public Stream EncryptSerializeTo<T>(object obj)
        {
            Stream retValue = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    string xml = Encrypt(writer.ToString(), Element.Reveal.DataLibrary.Utilities.HashKey.Key_DrawingList);
                    retValue = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                }
            }
            catch { }

            return retValue;
        }

        public async Task<T> EncryptDeserializeFrom<T>(Stream stream)
        {
            T retValue = default(T);

            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);

                string xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                using (StringReader reader = new StringReader(Decrypt(xml, Element.Reveal.DataLibrary.Utilities.HashKey.Key_DrawingList)))
                {
                    retValue = (T)deserializer.Deserialize(reader);
                }
            }
            catch { }

            return retValue;
        }

        public Stream EncryptHashSerializeTo<T>(object obj)
        {
            Stream retValue = null;

            try
            {
                System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));

                using (StringWriter stringwriter = new StringWriter())
                {
                    using (System.Xml.XmlWriter xmlwriter = System.Xml.XmlWriter.Create(stringwriter))
                    {
                        serializer.WriteObject(xmlwriter, obj);
                        string xml = Encrypt(stringwriter.ToString(), Element.Reveal.DataLibrary.Utilities.HashKey.Key_DrawingList);
                        retValue = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                    }
                }
            }
            catch { }

            return retValue;
        }

        public async Task<T> EncryptHashDeserializeFrom<T>(Stream stream)
        {
            T retValue = default(T);

            try
            {
                System.Runtime.Serialization.DataContractSerializer deserializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));

                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);

                string xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                xml = Decrypt(xml, Element.Reveal.DataLibrary.Utilities.HashKey.Key_DrawingList);

                using (StringReader reader = new StringReader(xml))
                {
                    using (System.Xml.XmlReader xmlreader = System.Xml.XmlReader.Create(reader))
                    {
                        retValue = (T)deserializer.ReadObject(xmlreader);
                    }
                }
            }
            catch { }

            return retValue;
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Returns an encrypted text.
        /// </summary>
        /// <param name="toEncrypt">String to be encrypted</param>
        /// <param name="key">Unique key for encryption/decryption</param>m>
        /// <returns>Returns encrypted string.</returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            string retValue = string.Empty;
            try    
            {
                // Get the MD5 key hash (you can as well use the binary of the key string)
                var keyHash = GetMD5Hash(key);         
                // Create a buffer that contains the encoded message to be encrypted.        
                var toDecryptBuffer = CryptographicBuffer.ConvertStringToBinary(toEncrypt, BinaryStringEncoding.Utf8);
                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);  
                var symetricKey = aes.CreateSymmetricKey(keyHash);         
                // The input key must be securely shared between the sender of the cryptic message and the recipient.
                var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, toDecryptBuffer, null);
                // We are using Base64 to convert bytes to string
                var strEncrypted = CryptographicBuffer.EncodeToBase64String(buffEncrypted);
                retValue = strEncrypted;    
            }    
            catch  { }

            return retValue;
        }
        
        /// <summary>
        /// All in Decrypt should be matched with encrypt procedure
        /// </summary>
        /// <param name="cipherString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherString, string key)
        {
            string retValue = string.Empty;
            try    
            {         
                var keyHash = GetMD5Hash(key);
                IBuffer toDecryptBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);
                SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);   
                var symetricKey = aes.CreateSymmetricKey(keyHash);         
                var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, toDecryptBuffer, null);         
                string strDecrypted = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
                retValue = strDecrypted;    
            }    
            catch { }

            return retValue;
        }

        private static IBuffer GetMD5Hash(string key)
        {    
            // Convert the key string to binary data.    
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.    
            if (buffHash.Length != objAlgProv.HashLength)    
            {        
                throw new Exception("There was an error creating the hash");    
            }  
  
            return buffHash;
        }

        public async Task<byte[]> GetBytesFromLocalUri(Uri uri)
        {
            byte[] retValue = null;

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                IBuffer buf = await FileIO.ReadBufferAsync(file);
                retValue = new byte[buf.Length];
                var dr = DataReader.FromBuffer(buf);
                dr.ReadBytes(retValue);
            }
            catch { }

            return retValue;
        }

        public async Task<Stream> GetFileStream(StorageFolder folder, string filename)
        {
            Stream retValue = null;

            try
            {
                var file = await StorageFile.GetFileFromPathAsync(folder.Path + "\\" + filename);
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    retValue = stream.CloneStream().AsStream();
                }

            }
            catch { }

            return retValue;
        }

        public async Task<StorageFile> CreateOrGetStorageFile(StorageFolder folder, string filename)
        {
            StorageFile retValue = null;

            try
            {
                retValue = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            }
            catch { }

            return retValue;
        }

        public async Task<bool> SaveWritableBitmapToFile(StorageFolder folder, string filename, WriteableBitmap writebitmap)
        {
            bool retValue = false;

            if (writebitmap != null)
            {
                try
                {
                    await writebitmap.SaveToFile(folder, filename);
                    retValue = true;
                }
                catch { }
            }

            return retValue;
        }

        public async Task<bool> SaveFileStream(StorageFolder folder, string filename, Stream stream)
        {
             bool retValue = false;

             try
             {
                 StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                 byte[] pixels = new byte[stream.Length];
                 await stream.ReadAsync(pixels, 0, pixels.Length);

                 using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                 {
                     using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                     {
                         using (DataWriter dataWriter = new DataWriter(outputStream))
                         {
                             dataWriter.WriteBytes(pixels);
                             await dataWriter.StoreAsync();
                             dataWriter.DetachStream();
                         }

                         await outputStream.FlushAsync();
                     }
                 }

                 retValue = true;
             }
             catch (Exception e)
             {
                 throw e;
             }

             return retValue;
        }

        public async Task<bool> DeleteFileStream(StorageFolder folder, string filename)
        {
            bool retValue = false;

            try
            {
                StorageFile file = await folder.GetFileAsync(filename);

                if (file != null)
                {
                    await file.DeleteAsync();
                }

                retValue = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return retValue;
        }

        public async Task<bool> DeleteAllUnder(StorageFolder folder)
        {
            bool retValue = false;

            try
            {
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

                foreach (StorageFile file in files)
                {
                    try
                    {
                        await file.DeleteAsync();
                    }
                    catch(Exception e)
                    {
                        ExceptionHandler(e, "DeleteAllUnder");
                    }
                }

                retValue = true;
            }
            catch(Exception e)
            {
                ExceptionHandler(e, "DeleteAllUnder GetFilesAsync");
            }

            return retValue;
        }
        #endregion

        #region "Message Helper"
        public static bool GlobalTryCatch(Action action)
        {
            Exception err = null;
            bool retValue = false;
            try
            {
                action.Invoke();
                retValue = true;
            }
            catch (Exception e)
            {
                err = e;
            }

            if (err != null)
                SimpleMessage(err.Message, "Error!");
            return retValue;
        }

        public async Task<Stream> GetErrorLog()
        {
            Stream retValue = null;
            try
            {
                retValue = await GetFileStream(Windows.Storage.ApplicationData.Current.LocalFolder, "Error.log");
            }
             catch { }

            return retValue;
        }

        public async void ExceptionHandler(Exception e, string source)
        {
            await RecordLog(e, source);
        }

        public async void ExceptionHandler(Exception e, string source, string msg, string title)
        {
            await RecordLog(e, source);
            SimpleMessage(msg, title);
        }

        private async Task<bool> RecordLog(Exception e, string source)
        {
            bool retValue = false;
            try
            {
                var file = await CreateOrGetStorageFile(Windows.Storage.ApplicationData.Current.LocalFolder, "Error.log");
                if (file != null)
                {
                    string seperator = "==========================================================================================";
                    StringBuilder content = new StringBuilder();
                    content.AppendLine(seperator);
                    content.AppendLine(DateTime.Now.ToString() + "->" + source);
                    content.AppendLine(e.Message);
                    content.AppendLine("");
                    content.AppendLine(e.StackTrace);
                    content.AppendLine(seperator);

                    //using (var randomstream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    //{
                    //    DataWriter datawriter = new DataWriter(randomstream);
                    //    datawriter.WriteString(content.ToString());
                    //}
                    await Windows.Storage.FileIO.AppendTextAsync(file, content.ToString());
                    file = null;
                
                }
                retValue = true;
            }
            catch { }

            return retValue;
        }

        public static async void SimpleMessage(string msg, string title)
        {
            try
            {
                MessageDialog dialog = new MessageDialog(msg, title);
                await dialog.ShowAsync();
            }
            catch { }
        }

        public static async Task<bool> YesOrNoMessage(string msg, string title)
        {
            bool result = false;

            try
            {
                MessageDialog dialog = new MessageDialog(msg, title);
                dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler((cmd) => result = true)));
                dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler((cmd) => result = false)));
                await dialog.ShowAsync();
            }
            catch { }

            return result;
        }

        public static async Task<bool> TrueFalseMessage(string msg, string title, string selectType1, string selectType2)
        {
            bool result = false;

            try
            {
                MessageDialog dialog = new MessageDialog(msg, title);
                dialog.Commands.Add(new UICommand(selectType1, new UICommandInvokedHandler((cmd) => result = true)));
                dialog.Commands.Add(new UICommand(selectType2, new UICommandInvokedHandler((cmd) => result = false)));
                await dialog.ShowAsync();
            }
            catch { }

            return result;
        }

        public static async Task<object> BuildMessage(string msg, string title, List<string> selectType)
        {
            try
            {                
                MessageDialog dialog = new MessageDialog(msg, title);
                for(int i =0; i < selectType.Count; i++)
                {
                    dialog.Commands.Add(new UICommand(selectType[i], null, i));
                }
                var commandChosen = await dialog.ShowAsync();
                return commandChosen.Id;
               
            }
            catch { }
            return null;
            
        }

        public static async Task<bool> OkMessage(string msg, string title)
        {
            bool result = false;

            try
            {
                MessageDialog dialog = new MessageDialog(msg, title);
                dialog.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await dialog.ShowAsync();
            }
            catch { }

            return result;
        }

        public static Popup GetPopupPane()
        {
            Popup retValue = new Popup()
            {
                ChildTransitions = new TransitionCollection
                                        {
                                            new AddDeleteThemeTransition(),
                                            new PopupThemeTransition {FromVerticalOffset = Window.Current.Bounds.Height}
                                        },
                IsLightDismissEnabled = true,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
            };

            var ttv = retValue.TransformToVisual(Window.Current.Content);
            ttv.TransformPoint(new Point(0, 0));

            return retValue;
        }
        #endregion
    }
}

