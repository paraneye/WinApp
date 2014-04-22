using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
using System.IO;


namespace WinAppLibrary.Utilities
{
    #region Token & Cookies
    internal class SamlSecurityToken
    {
        public byte[] BinarySecurityToken
        {
            get;
            set;
        }

        public DateTime Expires
        {
            get;
            set;
        }
    }

    internal class SPOAuthCookies
    {
        public string FedAuth
        {
            get;
            set;
        }

        public string RtFA
        {
            get;
            set;
        }

        public Uri Host
        {
            get;
            set;
        }

        public DateTime Expires
        {
            get;
            set;
        }
    }
    #endregion

    internal class SPOAuthUtility
    {
        Uri spSiteUrl;
        string username;
        string password;
        static SPOAuthUtility current;
        CookieContainer cookieContainer;
        SamlSecurityToken stsAuthToken;

        const string msoStsUrl = "https://login.microsoftonline.com/extSTS.srf";
        const string wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        const string wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        const string spowssigninUri = "_forms/default.aspx?wa=wsignin1.0";

        public static async Task<bool> Create(Uri spSiteUrl, string username, string password)
        {
            var utility = new SPOAuthUtility(spSiteUrl, username, password);
            CookieContainer cc = await utility.GetCookieContainer();
            var cookies = from Cookie c in cc.GetCookies(spSiteUrl) where c.Name == "FedAuth" select c;
            if (cookies.Count() > 0)
            {
                current = utility;
                return true;
            }
            else
                throw new Exception("Could not retrieve Auth cookies");
        }

        private SPOAuthUtility(Uri spSiteUrl, string username, string password)
        {
            this.spSiteUrl = spSiteUrl;
            this.username = username;
            this.password = password;
            stsAuthToken = new SamlSecurityToken();
        }

        public static SPOAuthUtility Current
        {
            get
            {
                return current;
            }
        }

        public Uri SiteUrl
        {
            get
            {
                return this.spSiteUrl;
            }
        }

        public async Task<CookieContainer> GetCookieContainer()
        {
            if (stsAuthToken != null)
            {
                if (DateTime.Now > stsAuthToken.Expires)
                {
                    this.stsAuthToken = await GetMsoStsSAMLToken();
                    SPOAuthCookies cookies = await GetSPOAuthCookies(this.stsAuthToken);
                    CookieContainer cc = new CookieContainer();

                    Cookie samlAuthCookie = new Cookie("FedAuth", cookies.FedAuth)
                    {
                        Path = "/",
                        Expires = this.stsAuthToken.Expires,
                        Secure = cookies.Host.Scheme.Equals("https"),
                        HttpOnly = true,
                        Domain = cookies.Host.Host
                    };

                    cc.Add(this.spSiteUrl, samlAuthCookie);

                    Cookie rtFACookie = new Cookie("rtFA", cookies.RtFA)
                    {
                        Path = "/",
                        Expires = this.stsAuthToken.Expires,
                        Secure = cookies.Host.Scheme.Equals("https"),
                        HttpOnly = true,
                        Domain = cookies.Host.Host
                    };

                    cc.Add(this.spSiteUrl, rtFACookie);

                    this.cookieContainer = cc;
                }
            }

            return this.cookieContainer;
        }

        private async Task<SPOAuthCookies> GetSPOAuthCookies(SamlSecurityToken stsToken)
        {

            Uri siteUri = this.spSiteUrl;
            Uri wsSigninUrl = new Uri(String.Format("{0}://{1}/{2}", siteUri.Scheme, siteUri.Authority, spowssigninUri));
            var clientHandler = new HttpClientHandler();

            await HttpUtility.SendHttpRequest(
                wsSigninUrl,
                HttpMethod.Post,
                new MemoryStream(stsToken.BinarySecurityToken),
                "application/x-www-form-urlencoded",
                clientHandler);

            SPOAuthCookies spoAuthCookies = new SPOAuthCookies();
            spoAuthCookies.FedAuth = clientHandler.CookieContainer.GetCookies(wsSigninUrl)["FedAuth"].Value;
            spoAuthCookies.RtFA = clientHandler.CookieContainer.GetCookies(wsSigninUrl)["rtFA"].Value;
            spoAuthCookies.Expires = stsToken.Expires;
            spoAuthCookies.Host = wsSigninUrl;

            return spoAuthCookies;
        }

        private async Task<SamlSecurityToken> GetMsoStsSAMLToken()
        {
            // Makes a request that conforms with the WS-Trust standard to 
            // Microsoft Online Services Security Token Service to get a SAML

            // generate the WS-Trust security token request SOAP message passing 
            //in the user's credentials and the site we want access to 
            byte[] saml11RTBytes = Encoding.UTF8.GetBytes(ParameterizeamlRTString(
                this.spSiteUrl.ToString(),
                this.username,
                this.password));

            byte[] response = await HttpUtility.SendHttpRequest(
                new Uri(msoStsUrl),
                HttpMethod.Post,
                new MemoryStream(saml11RTBytes),
                "application/soap+xml; charset=utf-8",
                null);

            StreamReader sr = new StreamReader(new MemoryStream(response));

            XDocument xDoc = XDocument.Parse(sr.ReadToEnd());
            var binaryST = from e in xDoc.Descendants()
                           where e.Name == XName.Get("BinarySecurityToken", wsse)
                           select e;

            var expires = from e in xDoc.Descendants()
                          where e.Name == XName.Get("Expires", wsu)
                          select e;

            SamlSecurityToken samlST = new SamlSecurityToken();
            samlST.BinarySecurityToken = Encoding.UTF8.GetBytes(binaryST.FirstOrDefault().Value);
            samlST.Expires = DateTime.Parse(expires.FirstOrDefault().Value);

            return samlST;
        }

        private string ParameterizeamlRTString(string url, string username, string password)
        {
            string samlRTString = Helper.GetResourceString("SAML11RequestTokenSOAPMsg");
            samlRTString = samlRTString.Replace("[username]", username);
            samlRTString = samlRTString.Replace("[password]", password);
            samlRTString = samlRTString.Replace("[url]", url);

            return samlRTString;
        }
    }
}
