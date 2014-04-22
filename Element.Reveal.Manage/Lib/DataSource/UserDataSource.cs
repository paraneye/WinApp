using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Manage.Lib.DataSource
{
    class UserDataSource
    {
        public async Task<bool> SaveFileLoginAccount(object dtolist, string strkeyvalue)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = helper.EncryptSerializeTo<RevealUserSvc.MobileLoginDTO>(dtolist);
                await helper.SaveFileStream(ContentPath.OffModeLoginFolder, Lib.ContentPath.LoginAccount, xmlstream);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_LoginAccount, strkeyvalue);

                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, strkeyvalue);
                throw e;
            }

            return retValue;
        }
    }
}
