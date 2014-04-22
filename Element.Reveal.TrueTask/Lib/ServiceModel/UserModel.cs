using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueTask.Lib.ServiceModel
{
    public class UserModel
    {
        #region "LoginAccount"
        public async Task<DataLibrary.LoginaccountDTO> GetLoginaccountByLoginName(string loginName)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(loginName);

            return await JsonHelper.GetDataAsync<DataLibrary.LoginaccountDTO>("JsonGetLoginaccountByLoginName", param, JsonHelper.UserService);
        }


        public async Task<DataLibrary.LoginaccountDTO> GetLoginaccountByTempOwnerPesonnelID(int personnelId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(personnelId);

            return await JsonHelper.GetDataAsync<DataLibrary.LoginaccountDTO>("JsonGetLoginaccountByTempOwnerPesonnelID", param, JsonHelper.UserService);
        }

        public async Task<DataLibrary.LoginaccountDTO> GetLoginaccountByPesonnelID(int personnelId)
        {
            List<dynamic> param = new List<dynamic>();
            param.Add(personnelId);

            return await JsonHelper.GetDataAsync<DataLibrary.LoginaccountDTO>("JsonGetLoginaccountByPesonnelID", param, JsonHelper.UserService);
        }

        public async Task<DataLibrary.LoginaccountAndModuleUserGroup> GetUserLogin(string loginName, string password)
        {
            DataLibrary.LoginaccountAndModuleUserGroup retValue = null;
            // Take the user's information and attempt to sign into Reveal Project

            List<dynamic> param = new List<dynamic>();
            param.Add(loginName);
            var login = await JsonHelper.GetDataAsync<DataLibrary.LoginaccountDTO>("JsonGetLoginaccountByLoginName", param, JsonHelper.UserService);
            if (login != null)
            {
                byte[] bpassword = null;
                string sPassword = null;
                bool isdomain = login.IsDomainUser == 0 ? false : true;
                if (isdomain)
                    bpassword = System.Text.UTF8Encoding.UTF8.GetBytes(password);
                else
                {
                    var alg = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm("MD5");
                    Windows.Storage.Streams.IBuffer buff = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(password, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                    var hashed = alg.HashData(buff);
                    bpassword = new byte[hashed.Length];
                    Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(hashed, out bpassword);
                    sPassword = Convert.ToBase64String(bpassword);
                }

                param.Add(sPassword);
                param.Add(isdomain);
                retValue = await JsonHelper.GetDataAsync<DataLibrary.LoginaccountAndModuleUserGroup>("JsonGetUserLogin", param, JsonHelper.UserService);

            }

            return retValue;
        }

        public async Task<DataLibrary.MobileLoginDTO> MobileGetUserLogin(string loginName, string password)
        {
            DataLibrary.MobileLoginDTO retValue = null;
            // Take the user's information and attempt to sign into Reveal Project

            List<dynamic> param = new List<dynamic>();
            param.Add(loginName);
            var login = await JsonHelper.GetDataAsync<DataLibrary.LoginaccountDTO>("JsonGetLoginaccountByLoginName", param, JsonHelper.UserService);
            if (login != null && !string.IsNullOrEmpty(login.PersonnelId))
            {
                byte[] bpassword = null;
                string sPassword = null;
                bool isdomain = login.IsDomainUser == 0 ? false : true;

                // 패스워드 아직 웹에서 적용 안되서 암호화 하는 것을 막아 놓음 차후 적용 하여야 함.
                //sPassword = password;
                if (isdomain)
                    bpassword = System.Text.UTF8Encoding.UTF8.GetBytes(password);
                else
                {
                    var alg = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm("MD5");
                    Windows.Storage.Streams.IBuffer buff = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(password, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                    var hashed = alg.HashData(buff);
                    bpassword = new byte[hashed.Length];
                    Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(hashed, out bpassword);
                    sPassword = Convert.ToBase64String(bpassword);
                }

                param.Add(sPassword);
                param.Add(isdomain);
                param.Add(DateTime.Now);
                param.Add(false);
                retValue = await JsonHelper.GetDataAsync<DataLibrary.MobileLoginDTO>("JsonMobileGetUserLogin", param, JsonHelper.UserService);

                List<dynamic> param1 = new List<dynamic>();
                param1.Add(loginName);
                DataLibrary.ComboCodeBoxDTO userphoto = await JsonHelper.GetDataAsync<DataLibrary.ComboCodeBoxDTO>("JsonGetSigmaUserPhotoBySigmaUserId", param1, JsonHelper.CommonService);
                WinAppLibrary.Utilities.Helper.UserStatusPhotoUrl = userphoto.DataName.Replace("\\\\", "/").Replace("\\", "/");
            }

            return retValue;
        }

        public async Task<DataLibrary.ComboCodeBoxDTO> GetPersonnelPhoto(string loginName)
        {
            List<dynamic> param1 = new List<dynamic>();
            param1.Add(loginName);
            DataLibrary.ComboCodeBoxDTO userphoto = await JsonHelper.GetDataAsync<DataLibrary.ComboCodeBoxDTO>("JsonGetSigmaUserPhotoBySigmaUserId", param1, JsonHelper.CommonService);
            WinAppLibrary.Utilities.Helper.UserPhotoUrl = userphoto.DataName;

            return userphoto;
        }
        #endregion

    }
}
