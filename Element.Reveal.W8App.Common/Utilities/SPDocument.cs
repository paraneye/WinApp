using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Storage.Streams;

namespace WinAppLibrary.Utilities
{
    public class SPDocument
    {
        public static bool IsLogin 
        { 
            get 
            { 
                if(SPOAuthUtility.Current == null) 
                    return false;
                else
                    return true;
            }
        }

        public async Task<bool> SignInSharepoint(string siteUrl, string username, string password)
        {
            bool retValue = false;

            try
            {
                // If both creds and site url are found try to sign in the user to SharePoint Online
                await SPOAuthUtility.Create(new Uri(siteUrl), username, password);
                retValue = true;
            }
            catch { }

            return retValue;
        }

        public async Task<byte[]> GetDocument(string siteUrl)
        {
            //if (SPOAuthUtility.Current != null)
            //{
                var resonse = await HttpUtility.SendODataJsonRequest(
                        new Uri(siteUrl),
                       HttpMethod.Get,
                       null,
                       new HttpClientHandler(),
                       SPOAuthUtility.Current);

                return resonse;
            //}
            //else
            //    return null;
        }

        public async Task<byte[]> SaveJpegContent(string siteUrl, string docname, Stream contents)
        {
            if (SPOAuthUtility.Current != null)
            {
                return await SaveDocument(siteUrl, docname, "image/jpeg", contents);
            }
            else
                return null;
        }

        private async Task<byte[]> SaveDocument(string siteUrl, string docname, string contenttype, Stream contents)
        {
            var response = await HttpUtility.SendODataHttpRequestWithCanary(
                new Uri(siteUrl + docname),
                HttpMethod.Put,
                contents,
                contenttype,
                new HttpClientHandler(),
                SPOAuthUtility.Current);

            return response;
        }

        public async Task<byte[]> GetDocumentWithLogin(string siteUrl, string username, string password)
        {
            if (SPOAuthUtility.Current != null)
            {
            }
            else
                SignInSharepoint(siteUrl, username, password);

            if (SPOAuthUtility.Current != null)
            {
                var resonse = await HttpUtility.SendODataJsonRequest(
                        new Uri(siteUrl),
                       HttpMethod.Get,
                       null,
                       new HttpClientHandler(),
                       SPOAuthUtility.Current);

                return resonse;
            }
            else
                return null;
        }

        //This remain to be seen as its searching query is uncertain.
        public async Task<byte[]> SaveDocumentWithCanary(string siteUrl, string collection, string docname, IRandomAccessStream contents)
        {
            if (SPOAuthUtility.Current != null)
            {
                string addItemJsonString = Helper.JsonAddListItem;
                string imagestring = (new Helper()).GetStringFromImageRandomAccessStream(contents);

                addItemJsonString = addItemJsonString.Replace("[title]", collection);
                addItemJsonString = addItemJsonString.Replace("[Name]", docname);

                var resonse = await HttpUtility.SendODataJsonRequestWithCanary(
                       new Uri(siteUrl + docname),
                       HttpMethod.Post,
                       new MemoryStream(Encoding.UTF8.GetBytes(addItemJsonString)),
                       new HttpClientHandler(),
                       SPOAuthUtility.Current);

                return resonse;
            }
            else
                return null;
        }
    }
}
