using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WinAppLibrary.Utilities
{
    public class JsonHelper
    {
        public const string ProjectService = "Prev/ProjectService.svc/rest/";
        public const string CommonService = "Prev/CommonService.svc/rest/";
        public const string UserService = "Prev/UserService.svc/rest/";
        public const string P6ManagerService = "Prev/P6ManagerService.svc/rest/";
        public const string WorkflowService = "Common/Workflow.svc/rest/";

        /// <summary>
        /// JSON string을 T형태로 deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        /// <summary>
        /// object를 JSON string으로 serialize
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                {
                    return _Reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// dictionary를 JSON string으로 변환
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        private static string DictionaryToJson(Dictionary<string, List<dynamic>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }

        /// <summary>
        /// dictionary를 JSON string으로 변환
        /// </summary>
        /// <param name="dParams"></param>
        /// <returns></returns>
        private static string DictionaryToJson(Dictionary<string, dynamic> dParams)
        {
            string jsonString = "{";
            int countParam = 0;

            if (dParams != null && dParams.Count() > 0)
            {
                foreach (KeyValuePair<string, dynamic> param in dParams)
                {
                    jsonString = jsonString + "\"" + param.Key + "\"" +
                        (param.Value == null ? ":\"\"" : ":" + Serialize(param.Value));

                    countParam++;
                    if (countParam != dParams.Count())
                    {
                        jsonString = jsonString + ",";
                    }
                }

                jsonString = jsonString + "}";
            }

            return jsonString;
        }

        /// <summary>
        /// List를 Array 형태의 string으로 변환
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static string ListToArray(List<dynamic> lParam)
        {
            string result = "{";
            int pCount = 0;

            foreach (dynamic param in lParam)
            {
                pCount++;
                result = result + param;
                if (pCount != lParam.Count())
                {
                    result = result + ",";
                }
            }

            result = result + "}";

            return result;
        }

        /// <summary>
        /// GET에서 사용할 URL
        /// </summary>
        /// <param name="method"></param>
        /// <param name="dParams"></param>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        private static string MakeUrl(string method, List<dynamic> dParams, string servicetype)
        {
            string base_url = (Helper.ServiceUrl.Substring(Helper.ServiceUrl.Length - 1) == "/") ? Helper.ServiceUrl : Helper.ServiceUrl + "/";

            string requestUrl = base_url + servicetype + method;
            string jsonParam;

            if (dParams != null && dParams.Count() > 0)
            {
                foreach (dynamic param in dParams)
                {
                    if (Object.ReferenceEquals(param, null) || string.IsNullOrEmpty(param.ToString()))
                    {
                        jsonParam = "{}";
                    }
                    else
                    {
                        jsonParam = Serialize(param);
                    }

                    if (jsonParam.Contains("{") == false && jsonParam.Contains("[") == true)  // 단순 배열의 경우 []를 {}로 교체
                    {
                        jsonParam = jsonParam.Replace("[", "{").Replace("]", "}");
                    }

                    if (jsonParam.Contains("{") == true || jsonParam.Contains("[") == true)  // Json으로 Serialize되면 그대로 사용
                    {
                        requestUrl = requestUrl + "/" + jsonParam;
                    }
                    else
                    {
                        if (param.GetType() == typeof(DateTime))
                        {
                            requestUrl = requestUrl + "/" + param.ToString("yyyyMMddHHmmss");
                        }
                        else
                        {
                            requestUrl = requestUrl + "/" + param;
                        }
                    }
                }
            }

            return requestUrl;
        }

        /// <summary>
        /// POST, PUT에서 사용할 URL
        /// </summary>
        /// <param name="method"></param>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        private static string MakeUrl(string method, string servicetype)
        {
            string base_url = (Helper.ServiceUrl.Substring(Helper.ServiceUrl.Length - 1) == "/") ? Helper.ServiceUrl : Helper.ServiceUrl + "/";

            return base_url + servicetype + method;
        }

        /// <summary>
        /// Debug-log 만들기
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        private static async void MakeLog(string message)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + "Logfile.log";

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                string log = Environment.NewLine + DateTime.Now.ToString("yyyy/MM/dd H:mm:ss") + "    " + message;
                await FileIO.AppendTextAsync(file, log);
            }
            catch { }
        }

        public static async Task<T> GetDataAsync<T>(string method, List<dynamic> dParams, string servicetype)
        {
            
            T result;
            
            string requestUrl = MakeUrl(method, dParams, servicetype);
            MakeLog("GET - Url: " + requestUrl);
                
            HttpClient hClient = new HttpClient();
            var response = await hClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            if (true == content.Contains(method + "Result"))  // Json에서 method + result 제거
            {
                content = content.Replace("{\"" + method + "Result" + "\":", "");
                content = content.Remove(content.Length - 1);
            }

            // Generic type이 아니면서 []로 묶여 있으면 이를 제거
            if (typeof(T).IsConstructedGenericType == false &&
                content.Contains("},") == false &&
                content.Substring(0, 1) == "[")
            {
                content = content.Remove(0, 1).Remove(content.Remove(0, 1).Length - 1);
            }

            result = Deserialize<T>(content);

            return result;
        }

        public static async Task<T> PutDataAsync<T>(string method, Dictionary<string, dynamic> dParams, string servicetype)
        {
            T result;
            string requestUrl = MakeUrl(method, servicetype);
            MakeLog("PUT - Url: " + requestUrl);

            string sContent = DictionaryToJson(dParams);
            MakeLog("PUT - Json: " + sContent);

            HttpClient hClient = new HttpClient();
            var response = await hClient.PutAsync(requestUrl, new StringContent(sContent, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            if (true == content.Contains(method + "Result"))  // Json에서 method + result 제거
            {
                content = content.Replace("{\"" + method + "Result" + "\":", "");
                content = content.Remove(content.Length - 1);
            }

            // Generic type이 아니면서 []로 묶여 있으면 이를 제거
            if (typeof(T).IsConstructedGenericType == false &&
                content.Contains("},") == false &&
                content.Substring(0, 1) == "[")
            {
                content = content.Remove(0, 1).Remove(content.Remove(0, 1).Length - 1);
            }

            result = Deserialize<T>(content);

            return result;
        }

        public static async Task<T> PostDataAsync<T>(string method, Dictionary<string, dynamic> dParams, string servicetype)
        {
            T result;
            string requestUrl = MakeUrl(method, servicetype);
            MakeLog("POST - Url: " + requestUrl);

            string sContent = DictionaryToJson(dParams);
            MakeLog("POST - Json: " + sContent);

            HttpClient hClient = new HttpClient();
            var response = await hClient.PostAsync(requestUrl, new StringContent(sContent, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            if (true == content.Contains(method + "Result"))  // Json에서 method + result 제거
            {
                content = content.Replace("{\"" + method + "Result" + "\":", "");
                content = content.Remove(content.Length - 1);
            }

            // Generic type이 아니면서 []로 묶여 있으면 이를 제거
            if (typeof(T).IsConstructedGenericType == false &&
                content.Contains("},") == false &&
                content.Substring(0, 1) == "[")
            {
                content = content.Remove(0, 1).Remove(content.Remove(0, 1).Length - 1);
            }

            result = Deserialize<T>(content);

            return result;
        }

        public static async void UploadFileAsync(string method, string filename, string servicetype)
        {
            string requestUrl = MakeUrl(method, servicetype);
            MakeLog("POST - Url: " + requestUrl);

            HttpClient hClient = new HttpClient();
            StorageFile file = await KnownFolders.DocumentsLibrary.GetFileAsync(filename);
            using (IRandomAccessStream stream = await file.OpenReadAsync ())
            {
                //MultipartContent form = new MultipartContent ();
                StreamContent fileContent = new StreamContent (stream.AsStreamForRead ());
                //StringContent stringContent = new StringContent (" test");
                //form.Add(fileContent);
                //form.Add(stringContent);
                var response = await hClient.PostAsync(requestUrl, fileContent);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
            }
        }
  
    }
}
