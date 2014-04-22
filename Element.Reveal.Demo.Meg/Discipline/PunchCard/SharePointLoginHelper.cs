using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Net.Http;

namespace Element.Reveal.Meg.Discipline.PunchCard
{
     public class SharePointLoginHelper
    {

        #region Variables

        Uri spSiteUrl;
        string samlXML;
        WebResponse response;
        string username;
        string password;
        string libraryname;
        const string msoAuthUrl = "https://login.microsoftonline.com/extSTS.srf";
        const string spLoginUrl = "/_forms/default.aspx?wa=wsignin1.0";
        const string nssoap = "http://www.w3.org/2003/05/soap-envelope";
        const string nswstrust = "http://schemas.xmlsoap.org/ws/2005/02/trust";
        const string nswssecurity = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        const string nswsutility = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        const string nswsaddressing = "http://www.w3.org/2005/08/addressing";
        const string nssaml = "urn:oasis:names:tc:SAML:1.0:assertion";
        const string nswspolicy = "http://schemas.xmlsoap.org/ws/2004/09/policy";
        const string nspssoapf = "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault";
        Stream stream;
        string tokenRequestXml;
        string token = null;
        Uri wsSigninUrl;
        CookieContainer cookieJar = new CookieContainer();
        static SharePointLoginHelper authobj;

        #endregion

        #region Properties

        public static SharePointLoginHelper AuthObj
        {
            get
            {
                return authobj;
            }
        }

        public Uri SiteUrl
        {
            get
            {
                return this.spSiteUrl;
            }
        }

        public string LibraryName
        {
            get
            {
                return this.libraryname;
            }
        }

        #endregion

        public SharePointLoginHelper(Uri sharepointSiteUrl, string loginUserName, string loginPassword)
        {
            spSiteUrl = sharepointSiteUrl;
            username = loginUserName;
            password = loginPassword;
        }


        public SharePointLoginHelper(Uri siteurl, string username, string password, string libraryname)
        {
            this.username = username;
            this.spSiteUrl = siteurl;
            this.password = password;
            this.libraryname = libraryname;
        }

        /// <summary>
        /// Function to get the cookies from sharepoint site by passing username and password
        /// </summary>
        /// <returns></returns>
        public async Task<CookieContainer> GetCookieContainer()
        {
            wsSigninUrl = new Uri(String.Format("{0}://{1}{2}", spSiteUrl.Scheme, spSiteUrl.Authority, spLoginUrl));
            string binaryToken = await GetBinaryToken(AuthObj.username, AuthObj.password);

            var cookies = await SubmitBinaryToken(token);
            if (cookies.GetCookies(spSiteUrl)["FedAuth"] != null)
            {
                return cookies;
            }

            return null;
        }

        /// <summary>
        /// Get the binary access token from  sharepoint site by passing username and password
        /// </summary>
        /// <param name="userName">microsoft credentials</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> GetBinaryToken(string userName, string password)
        {

            if (string.IsNullOrEmpty(samlXML))
            {
                XDocument loadedData = XDocument.Load("DataModel/SAML.xml");
                samlXML = loadedData.ToString();
            }
            tokenRequestXml = string.Format(samlXML, userName, password, spSiteUrl.Host);
            var request = WebRequest.CreateHttp(msoAuthUrl);
            request.Method = HttpMethod.Post;
            stream = await request.GetRequestStreamAsync();
            using (StreamWriter w = new StreamWriter(stream))
            {
                w.Write(tokenRequestXml);
            }
            response = await request.GetResponseAsync();

            var xDoc = XDocument.Load(response.GetResponseStream());
            var body = xDoc.Descendants(XName.Get("Body", nssoap)).FirstOrDefault();
            if (body != null)
            {
                var fault = body.Descendants(XName.Get("Fault", nssoap)).FirstOrDefault();
                if (fault != null)
                {
                    var error = fault.Descendants(XName.Get("text", nspssoapf)).FirstOrDefault();
                    if (error != null)
                        throw new Exception(error.Value);
                }
                else
                {
                    var binaryToken = body.Descendants(XName.Get("BinarySecurityToken", nswssecurity)).FirstOrDefault();
                    if (binaryToken != null)
                        token = binaryToken.Value;
                }
            }

            return token;
        }

        /// <summary>
        /// Asyn function to create http request to fetch videos from sharepoint site.
        /// </summary>
        /// <param name="httpmethod">http method type</param>
        /// <param name="requesturi">Uri of the sharepoint site along with the library name</param>
        /// <returns></returns>
        public async Task<byte[]> CreateHttpRequest(System.Net.Http.HttpMethod httpmethod, Uri requesturi)
        {
            byte[] responseStream;
            // Populates the list of videos from the sharepoint site .
            var httpRequestMessage = new HttpRequestMessage(httpmethod, requesturi);
            string contentType = "application/json;odata=verbose;charset=utf-8";
            var httpClientHandler = new HttpClientHandler();
            var request = new HttpClient(httpClientHandler);
            httpRequestMessage.Headers.Add("Accept", contentType); // set the content type of the request
            if (httpClientHandler.CookieContainer == null)
                httpClientHandler.CookieContainer = new CookieContainer();

            // get the auth cookies  after authenticating with Microsoft Online Services 
            CookieContainer cookieContainer = await
                     SharePointLoginHelper.AuthObj.GetCookieContainer();
            foreach (Cookie c in cookieContainer.GetCookies(SharePointLoginHelper.AuthObj.SiteUrl))
            {
                // append  auth cookies to the request
                httpClientHandler.CookieContainer.Add(SharePointLoginHelper.AuthObj.SiteUrl, c);
            }

            // Send the request and read the response as an array of bytes
            using (var resultData = await request.SendAsync(httpRequestMessage))
            {
                responseStream = await resultData.Content.ReadAsByteArrayAsync();
            }
            return responseStream;
        }

        /// <summary>
        /// function to get cookies by passing token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>Authentication cookies</returns>
        private async Task<CookieContainer> SubmitBinaryToken(string token)
        {
            var uriBuilder = new UriBuilder(spSiteUrl.Scheme, spSiteUrl.Host, spSiteUrl.Port);
            string signInUrl = uriBuilder.Uri + spLoginUrl;
            var request = HttpWebRequest.CreateHttp(signInUrl);
            request.CookieContainer = cookieJar;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";

            try
            {
                var requestStream = await request.GetRequestStreamAsync();
                using (var writer = new StreamWriter(requestStream))
                {
                    await writer.WriteAsync(token);
                }
                var response = await request.GetResponseAsync();

                uriBuilder.Path = null;
                response.Dispose();
            }
            catch
            {
            }

            return request.CookieContainer;

        }

        /// <summary>
        /// Initialize the authentication object when user logged in.
        /// </summary>
        /// <param name="spSiteUrl">sharepoint site URL</param>
        /// <param name="username">Microsoft account userr name</param>
        /// <param name="password">Password for the account</param>
        /// <param name="libraryname">Name of library created for videos in sharepoint site</param>
        /// <returns></returns>
        public static async Task<bool> InitAuthObj(Uri spSiteUrl, string username, string password, string libraryname)
        {
            var spauthobj = new SharePointLoginHelper(spSiteUrl, username, password, libraryname);
            var authobjcookie = new SharePointLoginHelper(spSiteUrl, username, password);
            authobj = spauthobj;
            CookieContainer cc = await authobjcookie.GetCookieContainer();
            var cookies = from Cookie c in cc.GetCookies(spSiteUrl) where c.Name == "FedAuth" select c;
            if (cookies.Count() > 0)
            {
                return true;
            }
            else
                throw new Exception("Could not retrieve Auth cookies");
        }

    }
    public static class HttpMethod
    {
        public static string Head { get { return "HEAD"; } }
        public static string Post { get { return "POST"; } }
        public static string Put { get { return "PUT"; } }
        public static string Get { get { return "GET"; } }
        public static string Delete { get { return "DELETE"; } }
        public static string Trace { get { return "TRACE"; } }
        public static string Options { get { return "OPTIONS"; } }
        public static string Connect { get { return "CONNECT"; } }
        public static string Patch { get { return "PATCH"; } }
    }
}
