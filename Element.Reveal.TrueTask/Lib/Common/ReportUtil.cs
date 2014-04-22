using oz.api;

using Element.Reveal.TrueTask.Lib.Common;

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace Element.Reveal.TrueTask.Lib.Common
{
    public class ReportUtil
    {

        public const string DEFAULT_LANG = "en/us";
        public const string DEFAULT_SEVERPATH = "http://dev.elementindustrial.com/oz60/server.aspx\n";

        /// <summary>
        /// MakeParameterForOnline : Make Parameter For Reports on Server
        /// </summary>
        /// <param name="rdsParams">Data Object</param>
        /// <returns>Parameter String</returns>
        public static string MakeParameterForOnline(ReportDS rdsParams)
        {
            return MakeParameterString(rdsParams);
        }

        /// <summary>
        /// MakeParameterForOffLine : Make Parameter For Reports on Device
        /// </summary>
        /// <returns>Parameter String</returns>
        public static async Task<string> MakeParameterForOffLine()
        {
            string strOzdPath = string.Empty;
            string strFinalPath = string.Empty;

            try
            {
                FileOpenPicker foPicker = new FileOpenPicker();

                foPicker.ViewMode = PickerViewMode.List;
                foPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                foPicker.FileTypeFilter.Add(".ozd");

                StorageFile sfiFile = await foPicker.PickSingleFileAsync();
                StorageFolder sfoFolder = ApplicationData.Current.LocalFolder;
                
                if (sfiFile != null)
                {
                    // await sfiFile.CopyAsync(sfoFolder, sfiFile.Name, NameCollisionOption.ReplaceExisting);
                    await sfiFile.CopyAsync(sfoFolder, "insert.ozd", NameCollisionOption.ReplaceExisting);

                    //strOzdPath = ApplicationData.Current.LocalFolder.Path + "\\" + sfiFile.Name;
                    strOzdPath = ApplicationData.Current.LocalFolder.Path + "\\insert.ozd";
                    StorageFile sfoTmpFile = await StorageFile.GetFileFromPathAsync(strOzdPath);
                    strFinalPath = "connection.openfile=" + strOzdPath;
                }                
            }
            catch (Exception ex)
            {
                strFinalPath = "";
            }

            return strFinalPath;
        }

        /// <summary>
        /// MakeParameterForOffLine : Make Parameter For Reports on Device
        /// </summary>
        /// <param name="rdsParams">Data Object</param>
        /// <returns>Parameter String</returns>
        public static async Task<string> MakeParameterForOffLine(ReportDS rdsParams)
        {
            string strOzdPath = string.Empty;
            string strFinalPath = string.Empty;

            try
            {
                FileOpenPicker foPicker = new FileOpenPicker();

                foPicker.ViewMode = PickerViewMode.List;
                foPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                foPicker.FileTypeFilter.Add(".ozd");

                StorageFile sfiFile = await foPicker.PickSingleFileAsync();
                StorageFolder sfoFolder = ApplicationData.Current.LocalFolder;

                if (sfiFile != null)
                {
                    // await sfiFile.CopyAsync(sfoFolder, sfiFile.Name, NameCollisionOption.ReplaceExisting);
                    await sfiFile.CopyAsync(sfoFolder, "insert.ozd", NameCollisionOption.ReplaceExisting);

                    //strOzdPath = ApplicationData.Current.LocalFolder.Path + "\\" + sfiFile.Name;
                    strOzdPath = ApplicationData.Current.LocalFolder.Path + "\\insert.ozd";
                    StorageFile sfoTmpFile = await StorageFile.GetFileFromPathAsync(strOzdPath);

                    rdsParams.OzdName = strOzdPath;
                    rdsParams.OdiName = "";
                    rdsParams.ReportName = "";
                    rdsParams.ServerYn = "N";
                    rdsParams.ProjectCode = "";
                    rdsParams.Params = null;

                    strFinalPath = MakeParameterString(rdsParams);
                }
            }
            catch (Exception ex)
            {
                strFinalPath = "";
            }

            return strFinalPath;
        }

        /// <summary>
        /// MakeParameterString : Make Parameter For Reports
        /// </summary>
        /// <param name="rdsParams">Data Object</param>
        /// <returns>Parameter String</returns>
        public static string MakeParameterString(ReportDS rdsParams)
        {
            string strParam = string.Empty;
            string strProjectCd = string.Empty;
            string strTempText = string.Empty;

            // Our Server : DEFAULT_SEVERPATH ( http://dev.elementindustrial.com/oz60/server.aspx\n )
            // Nothing for Local Files
            if (!string.IsNullOrEmpty(rdsParams.ServerPath))
            {
                strParam = strParam + "connection.servlet=" + rdsParams.ServerPath + "\n";
            }
            else
            {
                if (!string.IsNullOrEmpty(rdsParams.ServerYn))
                {
                    if (rdsParams.ServerYn.ToUpper().Equals("Y"))
                    {
                        strParam = strParam + "connection.servlet=" + DEFAULT_SEVERPATH;
                    }
                }
            }

            if (!string.IsNullOrEmpty(rdsParams.ProjectCode))
            {
                strProjectCd = "/" + rdsParams.ProjectCode;
            }

            // Only Online
            if (!string.IsNullOrEmpty(rdsParams.ReportName))
            {
                strParam = strParam + "connection.reportname=" + strProjectCd + rdsParams.ReportName + "\n";
            }

            // Only Online
            if (!string.IsNullOrEmpty(rdsParams.OdiName))
            {
                // Odi File Check
                strTempText = rdsParams.OdiName.EndsWith(".odi") ? rdsParams.OdiName.Substring(0, rdsParams.OdiName.Length - 4) : rdsParams.OdiName;
                strParam = strParam + "odi.odinames=" + strTempText + "\n";

                // Parameter Check
                if (rdsParams.Params != null)
                {
                    if (rdsParams.Params.Count() > 0)
                    {
                        int intTempCnt = 0;
                        strParam = strParam + "odi." + strTempText + ".pcount=" + rdsParams.Params.Count().ToString() + "\n";

                        foreach (object ojbParam in rdsParams.Params)
                        {
                            intTempCnt++;
                            strParam = strParam + "odi." + strTempText + ".args" + intTempCnt.ToString() + "=" + ojbParam.ToString() + "\n";
                        }
                    }
                }
            }

            // For Local Files
            if (!string.IsNullOrEmpty(rdsParams.OzdName))
            {
                strParam = strParam + "connection.openfile=" + rdsParams.OzdName + "\n";
            }

            #region // When it needs more function of OZ.dll

            //// For Local Files
            //if (!string.IsNullOrEmpty(rdsParams.ViewMode))
            //{
            //    string strTemp = string.Empty;
            //    string strModeText = string.Empty;

            //    strTemp = rdsParams.ViewMode.ToLower().IndexOf("print") > -1 ? "print/" : "";
            //    strModeText = strModeText + strTemp;
            //    strTemp = rdsParams.ViewMode.ToLower().IndexOf("export") > -1 ? "export/" : "";
            //    strModeText = strModeText + strTemp;
            //    strTemp = rdsParams.ViewMode.ToLower().IndexOf("preview") > -1 ? "preview/" : "";
            //    strModeText = strModeText + strTemp;
            //    strModeText = strModeText.Substring(0, strModeText.Length - 1);

            //    strParam = strParam + "viewer.mode=" + strModeText + "\n";
            //    //strParam = strParam + "viewer.mode=preview/export\n";
            //}

            #endregion

            // Language Setting
            strTempText = !string.IsNullOrEmpty(rdsParams.Language) ? rdsParams.Language : DEFAULT_LANG;
            strParam = strParam + "nglobal.language=" + strTempText + "\n";

            // When we Sign
            if (!rdsParams.IsNoSignZoom)
            {
                strParam = strParam + "eform.signpad_type=zoom\n";
            }

            #region // When we use Tool Bar

            string strToolbarUseYn = "Y";

            if (!string.IsNullOrEmpty(rdsParams.ToolBarUseYn))
            {
                strToolbarUseYn = rdsParams.ToolBarUseYn.ToUpper();
            }

            if (strToolbarUseYn.Equals("N"))
            {
                strParam = strParam + "viewer.usetoolbar=false\n";
            }
            else
            {
                // if UserMenuUseYn is not "N", we can suggest the following causes.
                // Save button in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarSaveYn))
                {
                    if (rdsParams.ToolBarSaveYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.save=false\n";
                    }
                }
                // Print in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarPrintYn))
                {
                    if (rdsParams.ToolBarPrintYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.print=false\n";
                    }
                }
                // Data Save in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarDataSaveYn))
                {
                    if (rdsParams.ToolBarDataSaveYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.option=false\n";
                    }
                }
                // Page Selection in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarPageMoveYn))
                {
                    if (rdsParams.ToolBarPageMoveYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.pageselection=false\n";
                        strParam = strParam + "toolbar.page=false\n";
                    }
                }
                // Zoom in & out in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarZoomYn))
                {
                    if (rdsParams.ToolBarZoomYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.zoom=false\n";
                    }
                }
                // Page Controls in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarPageControlYn))
                {
                    if (rdsParams.ToolBarPageControlYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.pagecontrol=false\n";
                    }
                }
                // Page width in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarWithControlYn))
                {
                    if (rdsParams.ToolBarWithControlYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.singlepage_fittoframe=false\n";
                        strParam = strParam + "toolbar.singlepagecontinuous_fittowidth=false\n";
                    }
                }
                // Find in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarFindYn))
                {
                    if (rdsParams.ToolBarFindYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.finds=false\n";
                    }
                }
                // Other Menus in Viewer Menu
                if (!string.IsNullOrEmpty(rdsParams.ToolBarOtherMenuYn))
                {
                    if (rdsParams.ToolBarOtherMenuYn.ToUpper().Equals("N"))
                    {
                        strParam = strParam + "toolbar.etc=false\n";
                    }
                }
            }

            #endregion

            strParam = strParam + "comment.all=true";
            strParam = strParam + "?ozurltype=url\n";

            return strParam;
        }

        /// <summary>
        /// RunReport : Make OZReportViewer Object for Display
        /// </summary>
        /// <param name="brdParam">Boarder Object</param>
        /// <param name="strParam">Parameter string</param>
        /// <returns>OZ ReportViewer</returns>
        /// public OZViewer RunReport(Windows.UI.Xaml.Controls.Border brdParam, string param)
        public static OZReportViewer RunReport(Windows.UI.Xaml.Controls.Border brdParam, string strParam)
        {
            OZReportViewer ozReturnViwer = null;

            //strParam = "connection.servlet=http://dev.elementindustrial.com/oz60/server.aspx\nconnection.reportname=/RDMIInspection.ozr\nodi.odinames=QaqcFormEntire\ncomment.all=true\nglobal.language=ko/kr\neform.signpad_type=zoom?ozurltype=url\n";

            try
            {
                if (!string.IsNullOrEmpty(strParam))
                {
                    ozReturnViwer = OZReportAPI.CreateViewer(brdParam, strParam);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                string strTrace = ex.StackTrace; 
            }

            return ozReturnViwer;
        }

        /// <summary>
        /// MakeMessageDialog : Make Message Dialogue
        /// </summary>
        /// <param name="strMessage">Message</param>
        /// <param name="strTitle">Popup Title</param>
        /// <param name="objParam">[Text, Boolean]</param>
        /// <returns>bool</returns>
        public static async Task<bool> MakeMessageDialog(string strMessage, string strTitle, object[,] objParam)
        {
            bool result = false;

            try
            {
                MessageDialog msgDialog = new MessageDialog(strMessage, strTitle);

                for (int intTmpI = 0; intTmpI < objParam.Length; intTmpI++)
                {
                    msgDialog.Commands.Add(new UICommand(objParam[intTmpI, 0].ToString(), new UICommandInvokedHandler((cmd) => result = (bool)objParam[intTmpI, 1])));
                }

                await msgDialog.ShowAsync();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// MakeMessageDialog : Make Message "OK" Dialogue
        /// </summary>
        /// <param name="strMessage">Message</param>
        /// <param name="strTitle">Popup Title</param>
        /// <returns>bool</returns>
        public static async Task<bool> MakeMessageDialog(string strMessage, string strTitle)
        {
            bool result = false;

            try
            {
                MessageDialog msgDialog = new MessageDialog(strMessage, strTitle);

                msgDialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler((cmd) => result = true)));

                await msgDialog.ShowAsync();
            }
            catch { }

            return result;
        }

        /// <summary>
        /// MakeWraningAboutReportLoad : Display a Warning Message when User made a something wrong in a report.
        /// </summary>
        /// <returns>bool</returns>
        public static async Task<bool> MakeWraningAboutReport(string strWarningText)
        {
            object[,] objDialog = new object[1, 2];

            objDialog[0, 0] = "OK";
            objDialog[0, 1] = true;

            bool isReturn = await ReportUtil.MakeMessageDialog(strWarningText, "Warning", objDialog);

            return isReturn;
        }
        
    }
}
