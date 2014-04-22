using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;

using WinAppLibrary;
using Element.Reveal.Crew.RevealProjectSvc;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.ITR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FillOutSubmitITR : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid;
        private string _loginid;
        //Fillout or Submit mode check
        string fmode = "";

        private int departmentId = 0;
        private Windows.Storage.StorageFolder BaseFolder;
        List<QaqcformtemplateDTO> _ofiles = new List<QaqcformtemplateDTO>();
        List<QaqcformDTO> _oservices = new List<QaqcformDTO>();

        Lib.DataSource.FillOutSubmitITRDataSource _FillOutSubmitITR = new Lib.DataSource.FillOutSubmitITRDataSource();

        public FillOutSubmitITR()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            _loginid = string.IsNullOrEmpty(WinAppLibrary.Utilities.Helper.LoginID) ? Login.UserAccount.LoginName.ToString() : WinAppLibrary.Utilities.Helper.LoginID;
            BaseFolder = Lib.ContentPath.OffModeUserFolder;
            //Login.MasterPage.DoBeforeBack += MasterPage_DoBeforeBack;
            Login.MasterPage.ShowBackButton = false;
            try
            {
                btnRemove.Visibility = Visibility.Visible;
                departmentId = WinAppLibrary.Utilities.Department.Foreman;
                // 1. QC Manager의 경우 foreman이 작업한 내용만 list up
                fmode = navigationParameter != null ? navigationParameter.ToString() : "1";

                switch (fmode)
                {
                    case "1":  // FillOut Mode
                        Login.MasterPage.SetPageTitle("Select Download Inspection & Test Record");
                        txtTitle.Text = "Fill Out Inspection & Test Record";
                        btnFillout.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        btnSubmit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        BindList();
                        break;
                        
                    case "2":  // Submit Mode
                        Login.MasterPage.SetPageTitle("Inspection & Test Record To Submit");
                        txtTitle.Text = "Submit Inspection & Test Record";
                        btnFillout.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        btnSubmit.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        BindList();
                        break;

                    case "3":  // Approval Mode
                        Login.MasterPage.SetPageTitle("Inspection & Test Record To Approval");
                        txtTitle.Text = "Approval Inspection & Test Record";
                        btnFillout.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        btnSubmit.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        btnRemove.Visibility = Visibility.Collapsed;
                        departmentId = WinAppLibrary.Utilities.Department.QualityManagement;
                        bindListfromService();
                        break;
                }
                    
                    

                //departmentId = await GetDepartmentID();
                //if (departmentId == 14)  // Foreman
                //{
                //    btnRemove.Visibility = Visibility.Visible;
                //    BindList();
                //}

                //if (departmentId == 18)  // Quality Management
                //{
                //    btnRemove.Visibility = Visibility.Collapsed;
                //}
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
            }
        }

        private async Task<int> GetDepartmentID()
        {
            // Department ID를 이용하여 Foreman인지.. Quality Management인지 구분하기 위함
            int dID = -1;
            Lib.ServiceModel.CommonModel common = new Lib.ServiceModel.CommonModel();
            List<RevealCommonSvc.DepartstructureDTO> listDepartment = new List<RevealCommonSvc.DepartstructureDTO>();
            listDepartment = await common.GetDepartStructureByPersonnelID(Login.UserAccount.PersonnelID);
                
            if (listDepartment.Count > 0)
            {
                dID = listDepartment.Where(c => c.DepartmentID == WinAppLibrary.Utilities.Department.Foreman).Count() > 0 ?
                    WinAppLibrary.Utilities.Department.Foreman : listDepartment.Where(c => c.DepartmentID == WinAppLibrary.Utilities.Department.QualityManagement).Count() > 0 ?
                    WinAppLibrary.Utilities.Department.QualityManagement : listDepartment[0].DepartmentID;
            }

            return dID;
        }

        //void MasterPage_DoBeforeBack(object sender, object e)
        //{
        //    Login.MasterPage.DoBeforeBack -= MasterPage_DoBeforeBack;
        //    this.Frame.Navigate(typeof(ITRMenu), Login.UserAccount.PersonnelID);    
        //}


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.ShowBackButton = true;
            this.Frame.Navigate(typeof(ITRMenu), Login.UserAccount.PersonnelID);
        }

        #region "File Save and Load"
        public async Task<List<QaqcformtemplateDTO>> LoadToQaqcformtemplate()
        {
            List<QaqcformtemplateDTO> filedto = new List<QaqcformtemplateDTO>();
            try
            {
                if (BaseFolder.GetFileAsync(Lib.ITRList.DownloadList) != null)
                {
                    WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
                    var stream = await helper.GetFileStream(BaseFolder, Lib.ITRList.DownloadList);
                    filedto = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<List<RevealProjectSvc.QaqcformtemplateDTO>>(stream);
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("File Download First!", "Error");
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadToQaqcformtemplate");
                throw e;
            }

            return filedto;
        }

        public async Task<QaqcformDTO> LoadToQaqcform(string filename)
        {
            QaqcformDTO filedto = new QaqcformDTO();
            try
            {
                if (BaseFolder.GetFileAsync(filename) != null)
                {
                    WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
                    var stream = await helper.GetFileStream(BaseFolder, filename);
                    filedto = await (new WinAppLibrary.Utilities.Helper()).EncryptDeserializeFrom<RevealProjectSvc.QaqcformDTO>(stream);
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Not Found Download File", "Error");
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadToQaqcform");
                throw e;
            }

            return filedto;
        }

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

        #endregion
        
        //Grid Bind
        private async void BindList()
        {
            //Download List에 저장될 내용 추가
            //파일명(키값) = QaqcformtemplateDTO.QAQCFormTemplateID_QaqcformtemplateDTO.QAQCTypeLUID
            //QaqcformtemplateDTO.Rev = FiwpName
            //QaqcformtemplateDTO.QAQCFormTemplateID = QAQCFormID
            //QaqcformtemplateDTO.QAQCTypeLUID = QAQCFormTemplateID
            //QaqcformtemplateDTO.Description = Download Date
            //QaqcformtemplateDTO.QAQCFormCode = Status(1:Download, 2:Saved, 3:Ready to Submit)
            //QaqcformtemplateDTO.QAQCFormTemplateName = QAQCFormTemplateName
                        
            List<QaqcformtemplateDTO> lfiles = new List<QaqcformtemplateDTO>();
            try
            {
                _ofiles = await LoadToQaqcformtemplate();
                if (_ofiles.Count > 0)
                {
                    foreach (QaqcformtemplateDTO item in _ofiles)
                    {
                        // 서버에 적용되지 않은 삭제 상태의 item은 binding에서 제외
                        if (item.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete)
                        {
                            item.QAQCFormCode = item.QAQCFormCode == "1" ? "Download" : item.QAQCFormCode == "2" ? "Saved" : "Ready to Submit";
                            lfiles.Add(item);
                        }
                    }
                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Not Found Data", "Not Found Data!");
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Bind Error", "Error!");
            }
            //this.DefaultViewModel["ITRList"] = lfiles;

            //Order by Document Name, Download Date, Status 
            gvDocument.ItemsSource = lfiles.OrderBy(x => x.QAQCFormTemplateName).ThenBy(y => y.Description).ThenBy(z => z.QAQCFormCode);

            //if (gvDocument.Items.Count > 10)
            //{
                
            //    gvDocument.ScrollIntoView(true, ScrollIntoViewAlignment.Leading);
            //}
        }

        /// <summary>
        /// bind list from service for Quality Management
        /// </summary>
        private async void bindListfromService()
        {
            List<QaqcformtemplateDTO> dto = new List<QaqcformtemplateDTO>();

            Lib.ServiceModel.ProjectModel project = new Lib.ServiceModel.ProjectModel();
            try
            {
                _oservices = await project.GetQaqcformByQcManager(_projectid, _moduleid, Convert.ToInt16(_loginid));
                if (_oservices.Count < 0)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Not Found Data", "Not Found Data!");
                    return;
                }

                List<FiwpqaqcDTO> itrs = await project.GetITRListByFiwp(Login.UserAccount.FIWPID);
                foreach (QaqcformDTO item in _oservices)
                {
                    QaqcformtemplateDTO data = new QaqcformtemplateDTO();

                    data.QAQCFormTemplateID = item.QAQCFormID;
                    data.QAQCTypeLUID = item.QAQCFormTemplateID;
                    data.QAQCFormRev = item.FIWPName;
                    data.Description = DateTime.Now.ToString();
                    data.QAQCFormCode = "Ready to Approval";  // 4 : Ready to Approval"

                    foreach (FiwpqaqcDTO ditem in itrs)
                    {
                        if (item.QAQCFormTemplateID == ditem.QAQCFormTemplateID)
                        {
                            data.QAQCFormTemplateName = ditem.QAQCFormTemplateName;
                            break;
                        }
                    }

                    dto.Add(data);
                }

                // Bind
                gvDocument.ItemsSource = dto.OrderBy(x => x.QAQCFormTemplateName).ThenBy(y => y.Description).ThenBy(z => z.QAQCFormCode);
            }
            catch
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Bind Error", "Error!");
            }

        }
        
        #region Button Event
        private async void btnFillout_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            try
            {
                if (gvDocument.SelectedItems.Count > 0)
                {
                    QaqcformtemplateDTO dto = (QaqcformtemplateDTO)gvDocument.SelectedItems.ToList().FirstOrDefault();
                    switch (departmentId)
                    {
                        case WinAppLibrary.Utilities.Department.Foreman:
                            filename = dto.QAQCFormTemplateID.ToString() + "_" + dto.QAQCTypeLUID.ToString() + ".xml";

                            Login.MasterPage.ShowBackButton = true;
                            
                            break;

                        case WinAppLibrary.Utilities.Department.QualityManagement:
                            foreach (QaqcformDTO item in _oservices)
                            {
                                if (item.QAQCFormID == dto.QAQCFormTemplateID)
                                {
                                    filename = item.QAQCFormID.ToString() + "_" + dto.QAQCTypeLUID.ToString() + ".xml";
                                    await SaveToQaqcForm(item, BaseFolder, filename);
                                    break;
                                }
                            }
                            break;
                    }

                    Dictionary<string, Object> nParams = new Dictionary<string, object>();
                    nParams.Add("filename", filename);
                    nParams.Add("departmentid", departmentId);
                    this.Frame.Navigate(typeof(ITR_Master), nParams);
                }
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("FillOut Error!", "Error!");
            }
        }

        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            _ofiles = await LoadToQaqcformtemplate();

            RemoveITR();
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            _ofiles = await LoadToQaqcformtemplate();

            SubmitITR();            
        }

        private void gvDocument_Holding(object sender, HoldingRoutedEventArgs e)
        {
            string filename = "";

            try
            {
                var dto = (sender as ListView).SelectedItem as QaqcformtemplateDTO;

                if (dto != null)
                {
                    filename = dto.QAQCFormTemplateID.ToString() + "_" + dto.QAQCTypeLUID.ToString() + ".xml";

                    if (filename.Replace(".xml", "") != "")
                    {
                        Login.MasterPage.ShowBackButton = true;
                        this.Frame.Navigate(typeof(ITR_Master), filename);
                    }
                }
            }
            catch (Exception ex)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Holding Error!", "Error!");
            }
        }        
        #endregion

        private async void RemoveITR()
        {
            List<QaqcformDTO> removeDto = new List<QaqcformDTO>();
            List<QaqcformtemplateDTO> forKeep =new List<QaqcformtemplateDTO>();
            List<QaqcformtemplateDTO> forRemove = new List<QaqcformtemplateDTO>();
            string filename = "";
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                if (gvDocument.SelectedItems.Count > 0)
                {
                    List<QaqcformtemplateDTO> targets = (List<QaqcformtemplateDTO>)gvDocument.SelectedItems;

                    if (_ofiles.Count <= 0)
                    {
                        return;
                    }

                    forRemove = _ofiles.Intersect(targets).ToList();  // 삭제
                    forKeep = _ofiles.Except(targets).ToList();  // 유지

                    foreach (QaqcformtemplateDTO data in _ofiles)
                    {
                        if (targets.Contains(data) == true)
                        {
                            QaqcformDTO dto = new QaqcformDTO();
                            filename = data.QAQCFormTemplateID.ToString() + "_" + data.QAQCTypeLUID.ToString() + ".xml";
                            dto = await LoadToQaqcform(filename);
                            dto.UpdatedDate = DateTime.Now;
                            dto.UpdatedBy = Login.UserAccount.UserName;
                            dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;  // QaqcformDTO Delete mark
                            dto.QaqcfromDetails.Select(c => { c.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete; return c; });  // QaqcformdetailDTO Delete mark

                            removeDto.Add(dto);

                            data.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Delete;
                            data.QAQCFormCode = "3";  // Status for Submit

                            // Update local file
                            await SaveToQaqcForm(dto, BaseFolder, filename);
                        }
                    }

                    switch (Login.LoginMode)
                    {
                        // OnMode일 때 서버로 내용 전달하고 file 삭제 / List 갱신
                        case WinAppLibrary.UI.LogMode.OnMode:
                            //Save To Server
                            await _FillOutSubmitITR.SaveQaqcformForSubmit(removeDto);

                            // Delete files
                            foreach (QaqcformtemplateDTO item in forRemove)
                            {
                                filename = item.QAQCFormTemplateID.ToString() + "_" + item.QAQCTypeLUID.ToString() + ".xml";
                                await helper.DeleteFileStream(BaseFolder, filename);
                            }

                            // Update for keep FileList
                            await SaveToQaqcformtemplate(forKeep, BaseFolder, Lib.ITRList.DownloadList);
                            break;
                        case WinAppLibrary.UI.LogMode.OffMode:
                            // Update for All FileList
                            await SaveToQaqcformtemplate(_ofiles, BaseFolder, Lib.ITRList.DownloadList);
                            break;
                    }

                    BindList();

                    WinAppLibrary.Utilities.Helper.SimpleMessage("Remove Complete", "Complete!");
                }
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "btnRemove_Click");
            }
        }

        private async void SubmitITR()
        {
            bool retvalue = false;
            List<QaqcformDTO> ReadyDto = new List<QaqcformDTO>();
            List<QaqcformtemplateDTO> NotReadyToSubmit = new List<QaqcformtemplateDTO>();
            List<QaqcformtemplateDTO> ReadyToSubmit = new List<QaqcformtemplateDTO>();
            string filename = "";
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();

            try
            {
                if (_ofiles.Count > 0)
                {
                    ReadyToSubmit = _ofiles.Where(x => x.QAQCFormCode == "3").ToList();
                    NotReadyToSubmit = _ofiles.Where(x => x.QAQCFormCode != "3").ToList();
                }
                //Search : Ready To Sumbmit List 
                foreach (QaqcformtemplateDTO item in ReadyToSubmit)
                {
                    QaqcformDTO dto = new QaqcformDTO();
                    filename = item.QAQCFormTemplateID.ToString() + "_" + item.QAQCTypeLUID.ToString() + ".xml";
                    dto = await LoadToQaqcform(filename);
                    if (item.DTOStatus != (int)WinAppLibrary.Utilities.RowStatus.Delete)
                    {
                        dto.UpdatedDate = DateTime.Now;
                        dto.UpdatedBy = Login.UserAccount.UserName;
                        dto.IsSubmitted = 1;  //0=Download / 1=Submit
                        dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                    }
                    ReadyDto.Add(dto);
                }

                //Save To Server
                retvalue = await _FillOutSubmitITR.SaveQaqcformForSubmit(ReadyDto);

                //Delete Files
                foreach (QaqcformtemplateDTO item in _ofiles.Where(x => x.QAQCFormCode == "3"))
                {
                    filename = item.QAQCFormTemplateID.ToString() + "_" + item.QAQCTypeLUID.ToString() + ".xml";
                    await helper.DeleteFileStream(BaseFolder, filename);
                }

                //Update FileList
                await SaveToQaqcformtemplate(NotReadyToSubmit, BaseFolder, Lib.ITRList.DownloadList);

                BindList();

                WinAppLibrary.Utilities.Helper.SimpleMessage("Submit Complete", "Complete!");
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "SubmitITR");
            }
                        
        }

    }
}