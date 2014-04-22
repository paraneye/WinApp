using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Data.Json;

namespace WinAppLibrary.Utilities
{
    internal class HttpUtility
    {
        /// <summary>
        /// Sends a JSON OData request appending the SharePoint canary to the request header.
        /// Appending the canary to the request is necessary to perform write operations (e.g. create, update, delete list items)
        /// The canary is a security measure to prevent cross site scripting attacks
        /// </summary>
        public static async Task<byte[]> SendODataJsonRequestWithCanary(Uri uri, HttpMethod method, Stream requestContent, HttpClientHandler clientHandler, SPOAuthUtility authUtility)
        {
            // Make a post request to {siteUri}/_api/contextinfo to get the canary
            var response = await HttpUtility.SendODataJsonRequest(
                new Uri(String.Format("{0}/_api/contextinfo", SPOAuthUtility.Current.SiteUrl)),
                HttpMethod.Post,
                null,
                clientHandler,
                SPOAuthUtility.Current);

            Dictionary<String, IJsonValue> dict = new Dictionary<string, IJsonValue>();
            HttpUtility.ParseJson(JsonObject.Parse(Encoding.UTF8.GetString(response, 0, response.Length)), dict);

            string canary = dict["FormDigestValue"].GetString();

            // Make the OData request passing the canary in the request headers
            return await HttpUtility.SendODataJsonRequest(
                uri,
                method,
                requestContent,
                clientHandler,
                SPOAuthUtility.Current,
                new Dictionary<string, string> { 
                { "X-RequestDigest", canary  } 
                });
        }

        /// <summary>
        /// Sends a JSON OData request appending SPO auth cookies to the request header.
        /// </summary>
        public static async Task<byte[]> SendODataJsonRequest(Uri uri, HttpMethod method, Stream requestContent, HttpClientHandler clientHandler, SPOAuthUtility authUtility, Dictionary<string, string> headers = null)
        {
            if (clientHandler.CookieContainer == null)
                clientHandler.CookieContainer = new CookieContainer();

            //CookieContainer cookieContainer = await authUtility.GetCookieContainer();

            //foreach (Cookie c in cookieContainer.GetCookies(uri))
            //{
            //    clientHandler.CookieContainer.Add(uri, c);
            //}

            return await SendHttpRequest(
                uri,
                method,
                requestContent,
                "application/json;odata=verbose;charset=utf-8",
                clientHandler,
                headers);
        }

        public static async Task<byte[]> SendODataHttpRequestWithCanary(Uri uri, HttpMethod method, Stream requestContent, string contentType,
            HttpClientHandler clientHandler, SPOAuthUtility authUtility, Dictionary<string, string> headers = null)
        {
            // Make a post request to {siteUri}/_api/contextinfo to get the canary
            var response = await HttpUtility.SendODataJsonRequest(
                new Uri(String.Format("{0}/_api/contextinfo", SPOAuthUtility.Current.SiteUrl)),
                HttpMethod.Post,
                null,
                clientHandler,
                SPOAuthUtility.Current);

            Dictionary<String, IJsonValue> dict = new Dictionary<string, IJsonValue>();
            HttpUtility.ParseJson(JsonObject.Parse(Encoding.UTF8.GetString(response, 0, response.Length)), dict);

            string canary = dict["FormDigestValue"].GetString();

            // Make the OData request passing the canary in the request headers
            return await HttpUtility.SendODataHttpRequest(
                uri,
                method,
                requestContent,
                contentType,
                clientHandler,
                SPOAuthUtility.Current,
                new Dictionary<string, string> { 
                { "X-RequestDigest", canary  } 
                });
        }

        public static async Task<byte[]> SendODataHttpRequest(Uri uri, HttpMethod method, Stream requestContent, string contentType,
            HttpClientHandler clientHandler, SPOAuthUtility authUtility, Dictionary<string, string> headers = null)
        {
            if (clientHandler.CookieContainer == null)
                clientHandler.CookieContainer = new CookieContainer();

            CookieContainer cookieContainer = await authUtility.GetCookieContainer();

            foreach (Cookie c in cookieContainer.GetCookies(uri))
            {
                clientHandler.CookieContainer.Add(uri, c);
            }

            return await SendHttpRequest(
                uri,
                method,
                requestContent,
                contentType,
                clientHandler,
                headers);
        }

        /// <summary>
        /// Sends an http request to the specified uri and returns the response as a byte array 
        /// </summary>
        public static async Task<byte[]> SendHttpRequest(Uri uri, HttpMethod method, Stream requestContent = null, string contentType = null, HttpClientHandler clientHandler = null, Dictionary<string, string> headers = null)
        {
            var req = clientHandler == null ? new HttpClient() : new HttpClient(clientHandler);
            var message = new HttpRequestMessage(method, uri);
            byte[] response;

            message.Headers.Add("Accept", contentType);

            if (requestContent != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Delete))
            {
                message.Content = new StreamContent(requestContent);

                if (!string.IsNullOrEmpty(contentType))
                {
                    message.Content.Headers.Add("Content-Type", contentType);
                }
            }

            // append additional headers to the request
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (message.Headers.Contains(header.Key))
                    {
                        message.Headers.Remove(header.Key);
                    }

                    message.Headers.Add(header.Key, header.Value);
                }
            }

            // Send the request and read the response as an array of bytes
            using (var res = await req.SendAsync(message))
            {
                response = await res.Content.ReadAsByteArrayAsync();
            }

            return response;
        }

        /// <summary>
        /// Parses a JSON object recursively into a dictionary of name value pairs
        /// </summary>
        public static Dictionary<String, IJsonValue> ParseJson(JsonObject jObj, Dictionary<String, IJsonValue> result)
        {
            var keys = jObj.Keys.GetEnumerator();

            while (keys.MoveNext())
            {
                String key = keys.Current;
                if (jObj[key].ValueType == JsonValueType.Object)
                {
                    JsonObject value = jObj[key].GetObject();
                    ParseJson(value, result);
                }
                else if (jObj[key].ValueType != JsonValueType.Null)
                {
                    if (!result.ContainsKey(key))
                        result.Add(key, jObj[key]);
                }
            }

            return result;
        }

        public static async Task<HttpResponseMessage> GetHttpResponseMessage(Uri urisource)
        {
            HttpClient httpClient = new HttpClient();
            CreateHttpClient(ref httpClient);
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, urisource);
            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return response;
        }

        public static void CreateHttpClient(ref HttpClient httpClient)
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }

            // HttpClient functionality can be extended by plugging multiple handlers together and providing
            // HttpClient with the configured handler pipeline.
            System.Net.Http.HttpMessageHandler handler = new System.Net.Http.HttpClientHandler();
            handler = new PlugInHandler(handler);
            httpClient = new HttpClient(handler);

            // The following line sets a "User-Agent" request header as a default header on the HttpClient instance.
            // Default headers will be sent with every request sent from this HttpClient instance.
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TrueVue", "v8"));
        }
    }

    public class PlugInHandler : MessageProcessingHandler
    {
        public PlugInHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        // Process the request before sending it
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                request.Headers.Add("TrueVue-Header", "TrueVueRequestValue");
            }
            return request;
        }

        // Process the response before returning it to the user
        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, System.Threading.CancellationToken cancellationToken)
        {
            if (response.RequestMessage.Method == HttpMethod.Get)
            {
                response.Headers.Add("TrueVue-Header", "TrueVueResponseValue");
            }
            return response;
        }
    }
}
