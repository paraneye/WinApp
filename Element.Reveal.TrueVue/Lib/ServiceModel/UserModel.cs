using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAppLibrary.Utilities;

namespace Element.Reveal.TrueVue.Lib.ServiceModel
{
    public class UserModel
    {
        #region "LoginAccount"
        public async Task<RevealUserSvc.LoginaccountDTO> GetLoginaccountByLoginName(string loginName)
        {
            RevealUserSvc.UserServiceClient user = ServiceHelper.GetServiceClient<RevealUserSvc.UserServiceClient>(ServiceHelper.UserService);
            var retValue = await user.GetLoginaccountByLoginNameAsync(Helper.DBInstance, loginName);
            await user.CloseAsync();
            return retValue;
        }

        public async Task<RevealUserSvc.LoginaccountDTO> GetLoginaccountByPesonnelID(int personnelId)
        {
            RevealUserSvc.UserServiceClient user = ServiceHelper.GetServiceClient<RevealUserSvc.UserServiceClient>(ServiceHelper.UserService);
            var retValue = await user.GetLoginaccountByPesonnelIDAsync(Helper.DBInstance, personnelId);
            await user.CloseAsync();
            return retValue;
        }

        public async Task<RevealUserSvc.LoginaccountAndModuleUserGroup> GetUserLogin(string loginName, string password)
        {
            RevealUserSvc.LoginaccountAndModuleUserGroup retValue = null;

            RevealUserSvc.UserServiceClient u = ServiceHelper.GetServiceClient<RevealUserSvc.UserServiceClient>(ServiceHelper.UserService);
            // Take the user's information and attempt to sign into Reveal Project
            var login = await u.GetLoginaccountByLoginNameAsync(Helper.DBInstance, loginName);
            if (login != null)
            {
                byte[] bpassword = null;
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
                }


                retValue = await u.GetUserLoginAsync(Helper.DBInstance, loginName, bpassword, isdomain);
            }

            await u.CloseAsync();
            return retValue;
        }

        public async Task<RevealUserSvc.MobileLoginDTO> MobileGetUserLogin(string loginName, string password)
        {
            RevealUserSvc.MobileLoginDTO retValue = null;

            RevealUserSvc.UserServiceClient u = ServiceHelper.GetServiceClient<RevealUserSvc.UserServiceClient>(ServiceHelper.UserService);
            // Take the user's information and attempt to sign into Reveal Project
            var login = await u.GetLoginaccountByLoginNameAsync(Helper.DBInstance, loginName);
            if (login != null)
            {
                byte[] bpassword = null;
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
                }

                retValue = await u.MobileGetUserLoginAsync(Helper.DBInstance, loginName, bpassword, isdomain, DateTime.Now, false);
            }

            await u.CloseAsync();
            return retValue;
        }
        #endregion

    }
}
