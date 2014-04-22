using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinAppLibrary.ServiceModels;

namespace Element.Reveal.Crew.Lib.DataSource
{
    class BrassInOutDataSource
    {
        public static int BrassID { get; set; }

        private static List<RevealProjectSvc.DailybrassDTO> _brassList;
        public static List<RevealProjectSvc.DailybrassDTO> BrassList
        {
            get
            {
                if (_brassList == null)
                    return new List<RevealProjectSvc.DailybrassDTO>();
                else
                    return _brassList;
            }
            set
            {
                _brassList = value;
            }
        }

        private static List<RevealProjectSvc.DailybrasssignDTO> _brasssignList;
        public static List<RevealProjectSvc.DailybrasssignDTO> BrassSignList
        {
            get
            {
                if (_brasssignList == null)
                    return new List<RevealProjectSvc.DailybrasssignDTO>();
                else
                    return _brasssignList;
            }
            set
            {
                _brasssignList = value;
            }
        }

        public async Task<bool> SaveFileDailyBrass(object dtolist, string strkeyvalue)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = helper.EncryptSerializeTo<List<RevealProjectSvc.DailybrassDTO>>(dtolist);
                await helper.SaveFileStream(ContentPath.OffModeUserFolder, Lib.ContentPath.BrassIn, xmlstream);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_ForemanBrassIn, strkeyvalue);

                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, strkeyvalue);
                throw e;
            }

            return retValue;
        }

        public async Task<bool> SaveFileDayilyBrassSign(object dtolist, string strkeyvalue)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = helper.EncryptSerializeTo<List<RevealProjectSvc.DailybrasssignDTO>>(dtolist);
                await helper.SaveFileStream(ContentPath.OffModeUserFolder, Lib.ContentPath.BrassSignIn, xmlstream);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_CrewBrassIn, strkeyvalue);

                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, strkeyvalue);
                throw e;
            }

            return retValue;
        }

        public async Task<bool> SaveFileToolbox(object dtolist, string strkeyvalue)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = helper.EncryptSerializeTo < List<RevealProjectSvc.ToolboxsignDTO>>(dtolist);
                await helper.SaveFileStream(ContentPath.OffModeUserFolder, Lib.ContentPath.ToolBoxTalk, xmlstream);
                WinAppLibrary.Utilities.Helper.SetValueInStorage(Lib.HashKey.Key_ToolboxIn, strkeyvalue);

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
