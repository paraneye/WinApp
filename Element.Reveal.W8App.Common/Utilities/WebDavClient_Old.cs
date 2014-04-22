using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Data.Xml.Dom;

namespace WinAppLibrary.Utilities
{
    public class WebDavClient_Old
    {
        public string ServerUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        // Server url should be full url, including protocol, like http://localhost
        public WebDavClient_Old(string serverUrl, string userName, string password, string domain)
        {
            ServerUrl = serverUrl;
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public async Task<List<string>> GetListItems(string folderUrl)
        {
            var items = new List<string>();
            var rawContents = await GetRawListItems(folderUrl);
            using (Stream stream = new MemoryStream(rawContents))
            {
                XmlReader reader = XmlReader.Create(stream);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Convert.ToString(rawContents));

                // to get list of all <a:response> elements
                var nodeList = xmlDoc.ChildNodes[1].ChildNodes;
                var nsmgr = new XmlNamespaceManager(reader.NameTable);
                nsmgr.AddNamespace("a", "DAV:");
                IXmlNode tempNode;

                foreach (IXmlNode node in nodeList)
                {
                    tempNode = node.FirstChild;

                    // uncomment to get 'Display name' of item
                    // tempNode = node.SelectSingleNode("a:propstat/a:prop/a:displayname", nsmgr);
                    items.Add(tempNode.InnerText);
                }
            }
            return items;
        }

        public async void Copy(string copyWhat, string copyTo)
        {
            string fullCopyToUrl = ServerUrl.TrimEnd("/".ToCharArray()) + copyTo.TrimEnd("/".ToCharArray()) + "/" + Path.GetFileName(copyWhat);
            IDictionary<string, string> headers = new Dictionary<string, string> { { "Destination", fullCopyToUrl } };
            await SendRequest(copyWhat, "COPY", null, null, headers);
        }

        public async void Move(string moveWhat, string moveTo)
        {
            string fullMoveTo = ServerUrl.TrimEnd("/".ToCharArray()) + moveTo.TrimEnd("/".ToCharArray()) + "/" + Path.GetFileName(moveWhat);
            IDictionary<string, string> headers = new Dictionary<string, string> { { "Destination", fullMoveTo } };
            await SendRequest(moveWhat, "MOVE", null, null, headers);
        }

        public async Task<byte[]> ReadItemProperties(string itemPath)
        {
            byte[] result = await SendRequest(itemPath, "PROPFIND", null, "0", null);
            return result;
        }

        public async void UploadItem(string folderUrl, byte[] contents, string targetFileName = null)
        {
            var httpclient = GetHttpClient();
            await SendRequest(folderUrl, "PUT", contents, null, null);
        }

        public async Task<byte[]> UploadImageContent(string fullUrl, Stream content)
        {
            var results = await SendRequest(fullUrl, "PUT", "1", content);
            return results;
        }

        private HttpClient GetHttpClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.AllowAutoRedirect = true;
            clientHandler.Credentials = new CredentialCache
            {
                {
                    new Uri(ServerUrl), "Negotiate",
                    new NetworkCredential(UserName, Password)
                }
            };

            var httpclient = new HttpClient(clientHandler);
            //httpclient.DefaultRequestHeaders.Authorization = CreateBasicHeader(UserName, Password);
            //httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpeg"));
            httpclient.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft-WebDAV-MiniRedir/6.1.7600");

            return httpclient;
        }

        public AuthenticationHeaderValue CreateBasicHeader(string username, string password)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }


        public async Task<byte[]> DownloadItem(string remotePath)
        {
            //  SendRequest(remotePath, "GET", fullLocalPath, credentialName, null, null);          
            var result = await SendRequest(remotePath, "GET", null, null, null);
            return result;
        }

        public async Task<string> CreateWebDavFolder(string newFolderPath)
        {
            await SendRequest(newFolderPath, "MKCOL", null, null, null);
            return newFolderPath;
        }

        public async Task<string> CreateWebDavFolder(string folderPath, string folderName)
        {
            var fullPath = folderPath.TrimEnd("/".ToCharArray()) + @"/" + folderName;
            await SendRequest(fullPath, "MKCOL", null, null, null);
            return fullPath;
        }

        public async void Delete(string itemPath)
        {
            await SendRequest(itemPath, "DELETE", null, null, null);
        }

        public async Task<byte[]> GetRawListItems(string folder)
        {
            byte[] result = await SendRequest(folder, "PROPFIND", null, "1", null);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Http Address supporting Web Dav</param>
        /// <param name="method">Http Method</param>
        /// <param name="contents">Support only image format at this moment</param>
        /// <param name="depth">Additional directory to reach to destination</param>
        /// <param name="headers">Htttp protocol</param>
        /// <returns></returns>
        private async Task<byte[]> SendRequest(string url, string method, byte[] contents, string depth, IDictionary<string, string> headers)
        {
            string fullUrl = ServerUrl;
            fullUrl += url;

            HttpClient httpclient = GetHttpClient();
            HttpRequestMessage requestmsg = new HttpRequestMessage(new HttpMethod(method), fullUrl);

            if (contents != null)
            {
                Stream streamOutput = new MemoryStream();
                using (BinaryWriter writer = new BinaryWriter(streamOutput))
                {
                    writer.Write(contents);
                    requestmsg.Content = new StreamContent(streamOutput);
                }
            }

            // Need to send along headers?
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                    requestmsg.Headers.Add(key, headers[key]);
            }

            if (depth != null)
            {
                requestmsg.Headers.Add("Depth", depth);
            }

            requestmsg.Headers.Add("translate", "f");
            requestmsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpeg"));
            requestmsg.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8", 0.7));
            requestmsg.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us", 0.5));

            // execute the request
            var result = await httpclient.SendAsync(requestmsg);
            // we will read data via the response stream
            var content = await result.Content.ReadAsByteArrayAsync();

            return content;
        }

        /// <summary>
        /// Simple Image Stream Upload
        /// </summary>
        /// <param name="url">Http Address supporting Web Dav</param>
        /// <param name="method">Http Method</param>
        /// <param name="contentstream">Support only image format at this moment</param>
        /// <returns></returns>
        private async Task<byte[]> SendRequest(string url, string method, string depth, Stream contentstream)
        {
            HttpClient httpclient = GetHttpClient();
            HttpRequestMessage requestmsg = new HttpRequestMessage(new HttpMethod(method), url);

            if (contentstream != null)
                requestmsg.Content = new StreamContent(contentstream);

            if (depth != null)
            {
                requestmsg.Headers.Add("Depth", depth);
            }

            requestmsg.Headers.Add("translate", "f");
            requestmsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpeg"));
            requestmsg.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8", 0.7));
            requestmsg.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us", 0.5));

            // execute the request
            var result = await httpclient.SendAsync(requestmsg);

            // we will read data via the response stream
            var content = await result.Content.ReadAsByteArrayAsync();

            return content;
        }
    }
}
