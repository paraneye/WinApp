using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinAppLibrary.Utilities
{
    public class FileDocument
    {
        public async Task<byte[]> GetDocument(string siteUrl)
        {
            // async 방식으로 지정 file  가져옴
            HttpClient hClient = new HttpClient();
            var response = await hClient.GetAsync(siteUrl);

            response.EnsureSuccessStatusCode();
            byte[] content = await response.Content.ReadAsByteArrayAsync();

            return content;
        }

        public async Task<byte[]> SaveJpegContent(string siteUrl, string docname, Stream contents)
        {
            return await SaveDocument(siteUrl.Replace("\\\\", "/").Replace("\\", "/"), docname, "image/jpeg", contents);
        }

        private async Task<byte[]> SaveDocument(string siteUrl, string docname, string contenttype, Stream contents)
        {
            var response = await HttpUtility.SendODataHttpRequestWithCanary(
                new Uri(siteUrl.Replace("\\\\", "/").Replace("\\", "/") + docname),
                HttpMethod.Put,
                contents,
                contenttype,
                new HttpClientHandler(),
                SPOAuthUtility.Current);

            return response;
        }
    }
}
