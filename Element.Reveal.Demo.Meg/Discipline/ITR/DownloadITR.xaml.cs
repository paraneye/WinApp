using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Core;

using WinAppLibrary;
using Element.Reveal.Meg.RevealProjectSvc;
using Element.Reveal.Meg.RevealCommonSvc;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using WinAppLibrary.Extensions;

using System.Reflection;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadITR : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.DataSource.DownloadITRDataSource _ITRList = new Lib.DataSource.DownloadITRDataSource();

        List<QaqcformDTO> _QaqcForm = new List<QaqcformDTO>();
        List<QaqcformtemplateDTO> _QaqcFormTemplate = new List<QaqcformtemplateDTO>();
        List<FiwpqaqcDTO> _SelectITRs = new List<FiwpqaqcDTO>();

        private string _savedFileName = "";
        private Windows.Storage.StorageFolder BaseFolder;
        private int _fiwpID = 0; //Convert.ToInt32(navigationParameter.ToString());

        public DownloadITR()
        {
            this.InitializeComponent();
            Login.MasterPage.SetPageTitle("Select Inspection & Test Record");
            BaseFolder = Lib.ContentPath.OffModeUserFolder;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                var Parameter = navigationParameter.ToString().Split('/');
                txtFiwpName.Text = Parameter[0].ToString();
                _fiwpID = Convert.ToInt32(Parameter[1].ToString());

                BindITRList();
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
            }
        }

        //Grid Binding
        private async void BindITRList()
        {
            try
            {
                Lib.ServiceModel.CommonModel cd = new Lib.ServiceModel.CommonModel();

                List<FiwpqaqcDTO> source = null;
                await _ITRList.GetITRListByFiwp(_fiwpID);
                source = _ITRList.GetITRList();

                this.DefaultViewModel["ITRList"] = source;

                if (source == null || source.Count < 1)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Not Exist Downlad Data", "Caution!");
                }
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FillOutSubmitITR), "1");   //"1" : Fillout Mode  "2" : Submit Mode
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            _SelectITRs = new List<FiwpqaqcDTO>();

            if (gvITRList.SelectedItems.Count < 1)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please Select Download ITR", "Caution!");
            }
            else
            {
                try
                {
                    List<QaqcformtemplateDTO> QaqcList = new List<QaqcformtemplateDTO>();
                    var dtos = gvITRList.SelectedItems.ToList();

                    //select ITR Download
                    foreach (FiwpqaqcDTO item in dtos)
                    {
                        QaqcformtemplateDTO selectQaqc = new QaqcformtemplateDTO();
                        selectQaqc.QAQCFormTemplateID = Convert.ToInt32(item.QAQCFormTemplateID);
                        selectQaqc.QAQCTypeLUID = Convert.ToInt32(item.QAQCTypeLUID);
                        selectQaqc.QAQCFormCode = item.QAQCFormCode;
                        QaqcList.Add(selectQaqc);
                        _SelectITRs.Add(item);
                    }

                    DownloadITRList(QaqcList);

                }
                catch (Exception ex)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Download Failed", "Caution!");
                }
            }

        }

        private async void DownloadITRList(List<QaqcformtemplateDTO> list)
        {
            Login.MasterPage.Loading(true, this);
            _QaqcForm = new List<QaqcformDTO>();
            try
            {
                await _ITRList.GetDownloadQaqc(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID, 0, _fiwpID, list, Login.UserAccount.UserName, WinAppLibrary.Utilities.QAQCDataType.ITR);
                _QaqcForm = _ITRList.GetDownloadQaqcList();

                await SaveITRList();
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("DownloadITRList Error", "Error!");
            }
            Login.MasterPage.Loading(false, this);
        }

        private async Task<bool> SaveITRList()
        {
            bool retvalue = true;

            if (_QaqcForm == null || _QaqcForm.Count < 1)
                WinAppLibrary.Utilities.Helper.SimpleMessage("Downloaded ITR Count 0. Please Check Data.", "Error!");
            else
            {
                List<QaqcformtemplateDTO> saveQaqcformtemplateDTO = new List<QaqcformtemplateDTO>();
                try
                {
                    //Save QaqcForm + Download List
                    foreach (QaqcformDTO item in _QaqcForm)
                    {
                        string formCode = "";
                        foreach (FiwpqaqcDTO ditem in _SelectITRs)
                        {
                            if (item.QAQCFormTemplateID == ditem.QAQCFormTemplateID)
                                formCode = ditem.QAQCFormCode;
                        }
                        //파일명 = QAQCFormID_QAQCFormTemplateID.xml
                        _savedFileName = item.QAQCFormID.ToString() + "_" + formCode + ".xml";

                        bool blSaved = false;
                        blSaved = await SaveToQaqcForm(item, BaseFolder, _savedFileName);

                        if (blSaved)
                        {
                            //Download List에 저장될 내용 추가
                            //파일명(키값) = QaqcformtemplateDTO.QAQCFormTemplateID_QaqcformtemplateDTO.QAQCTypeLUID
                            //QaqcformtemplateDTO.Rev = FiwpName
                            //QaqcformtemplateDTO.QAQCFormTemplateID = QAQCFormID
                            //QaqcformtemplateDTO.QAQCTypeLUID = QAQCFormTemplateID
                            //QaqcformtemplateDTO.Description = Download Date
                            //QaqcformtemplateDTO.QAQCFormCode = Status(1:Download, 2:Saved, 3:Ready to Submit)
                            //QaqcformtemplateDTO.QAQCFormTemplateName = QAQCFormTemplateName
                            QaqcformtemplateDTO list = new QaqcformtemplateDTO();

                            list.QAQCFormTemplateID = item.QAQCFormID;
                            list.QAQCFormRev = txtFiwpName.Text;
                            list.Description = DateTime.Now.ToString();
                            list.QAQCFormCode = "1";  //"3" - Ready to Submit

                            foreach (FiwpqaqcDTO ditem in _SelectITRs)
                            {
                                if (item.QAQCFormTemplateID == ditem.QAQCFormTemplateID)
                                {
                                    list.QAQCFormTemplateName = ditem.QAQCFormTemplateName + "_" + item.KeyValue;
                                    list.ExceptionMessage = ditem.QAQCFormCode;
                                }
                            }

                            saveQaqcformtemplateDTO.Add(list);
                        }
                        else
                        {
                            retvalue = false;
                            WinAppLibrary.Utilities.Helper.SimpleMessage("Save to LocalStorage Error!", "Error");
                        }

                    }

                    _QaqcFormTemplate = saveQaqcformtemplateDTO;

                    await SaveFileList();
                }
                catch (Exception ex)
                {
                    retvalue = false;
                    WinAppLibrary.Utilities.Helper.SimpleMessage("SaveITRList Error", "Error!");
                }
            }
            return retvalue;
        }

        private async Task<bool> SaveFileList()
        {
            bool retvalue = true;
            if (_QaqcFormTemplate != null && _QaqcFormTemplate.Count > 0)
            {
                List<QaqcformtemplateDTO> ofiles = new List<QaqcformtemplateDTO>();

                try
                {
                    //기존 FileList 가 있으면 불러오기
                    ofiles = await LoadToQaqcformtemplate();

                    //불러온 이후 저장될 내용 추가
                    if (ofiles != null && ofiles.Count > 0)
                    {
                        foreach (QaqcformtemplateDTO dto in _QaqcFormTemplate)
                        {
                            ofiles.Add(dto);
                        }
                    }
                    else
                    {
                        ofiles = _QaqcFormTemplate;
                    }

                    //File List 저장
                    bool blSaved = false;
                    blSaved = await SaveToQaqcformtemplate(ofiles, BaseFolder, Lib.ITRList.DownloadList);

                    if (!blSaved)
                    {
                        WinAppLibrary.Utilities.Helper.SimpleMessage("Save to LocalStorage Error!", "Error");
                    }

                    WinAppLibrary.Utilities.Helper.SimpleMessage("Download Succeed", "Success!");

                    BindITRList();
                }
                catch (Exception ex)
                {
                    retvalue = false;
                    WinAppLibrary.Utilities.Helper.SimpleMessage("SaveFileList Error!", "Error");
                }

            }
            return retvalue;
        }

        #region "File Save and Load"
        //QaqcForm Save To File
        public async Task<bool> SaveToQaqcForm(QaqcformDTO dto, Windows.Storage.StorageFolder _path, string _filename)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = FormSerialize.EncryptHashSerializeTo<QaqcformDTO>(dto);
                await helper.SaveFileStream(_path, _filename, xmlstream);
                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveToQaqcForm");
                throw e;
            }

            return retValue;
        }

        //Qaqcformtemplate Save To File
        public async Task<bool> SaveToQaqcformtemplate(List<QaqcformtemplateDTO> dto, Windows.Storage.StorageFolder _path, string _filename)
        {
            bool retValue = false;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                var xmlstream = FormSerialize.EncryptHashSerializeTo<List<QaqcformtemplateDTO>>(dto);
                await helper.SaveFileStream(_path, _filename, xmlstream);
                retValue = true;
            }
            catch (Exception e)
            {
                helper.ExceptionHandler(e, "SaveToQaqcformtemplate");
                throw e;
            }

            return retValue;
        }

        //Qaqcformtemplate Load From File
        public async Task<List<QaqcformtemplateDTO>> LoadToQaqcformtemplate()
        {
            List<QaqcformtemplateDTO> filedto = new List<QaqcformtemplateDTO>();
            try
            {
                WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
                var stream = await helper.GetFileStream(BaseFolder, Lib.ITRList.DownloadList);
                filedto = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.QaqcformtemplateDTO>>(stream);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadToQaqcformtemplate");
                throw e;
            }

            return filedto;
        }

        #endregion


    }
}
