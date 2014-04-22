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

using Element.Reveal.TrueTask.Lib.Common;
using oz.api;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Net;
using System.Net.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssembleReport : WinAppLibrary.Controls.LayoutAwarePage
    {
        OZReportViewer ozViewer = null;
        protected string FileName;
        protected string OdiName = "AssembleIWP";
        protected string FileType, AssembleStep;
        private int _projectid, _fiwpid, _fileStoreID;
        protected List<DataLibrary.DocumentDTO> ReportList;
        protected string _filePath = "";
        protected string strParam = "";

        public AssembleReport()
        {
            this.InitializeComponent();

        }
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _projectid = Login.UserAccount.CurProjectID;
            _fiwpid = Lib.IWPDataSource.selectedIWP;

            //Report Type 구분
            //새로 타입 추가될 때 1) 각 메뉴의 BackButton / Save/Next 이동메뉴 확인 필요시  파라미터 추가 2)SelectIWP에서도 타입 구분 추가
            AssembleStep = navigationParameter.ToString();
            if (AssembleStep == DataLibrary.Utilities.AssembleStep.SUMMARY)
            {
                FileType = DataLibrary.Utilities.FileType.SUMMARY;
                FileName = "IWPSummary";
                tbpageTitle.Text = "IWP Summary";
            }
            else if (AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_CHECK)
            {
                FileType = DataLibrary.Utilities.FileType.SAFETY_CHECK;
                FileName = "SafetyChecklist";
                tbpageTitle.Text = "Safety Checklist";
            }
            else if (AssembleStep == DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK)
            {
                FileType = DataLibrary.Utilities.FileType.SCAFFOLD_CHECK;
                FileName = "ScaffoldChecklist";
                tbpageTitle.Text = "Scaffold Checklist";
            }

            LoadReport();

            Lib.WizardDataSource.SetTargetMenu(AssembleStep, Lib.CommonDataSource.selPackageTypeLUID, true);

            this.ButtonBar.CurrentMenu = AssembleStep;
            this.ButtonBar.Load();
        }

        private async void LoadReport()
        {
            Login.MasterPage.Loading(true, this);

            ReportDS dsReport = new ReportDS();

            //ozd 파일 get
            ReportList = await (new Lib.ServiceModel.ProjectModel()).GetIwpDocumentByIwpProjectFileType(_fiwpid, _projectid, FileType, "Y", DataLibrary.Utilities.FileCategory.REPORT, "0");

            if (ReportList != null)
            {
                foreach (DataLibrary.DocumentDTO _dto in ReportList.OrderByDescending(x => x.DocumentID).ToList())
                {
                    if (_dto.FileExtension.ToLower().Equals("ozd"))
                    {
                        _filePath = _dto.LocationURL;
                        _fileStoreID = _dto.FileStoreId;
                        break;
                    }
                }
            }

            //1. ozd 파일 있으면 ozd 로딩 
            if (!string.IsNullOrEmpty(_filePath))
            {
                //IIS 세팅 시 마임타입 추가(ozd : text/ozd)
                //_filePath = "http://localhost/SigmaStorage/SigmaDoc/Yellow/PJTname/FILE_TYPE_SUMMARY/IWPSummary/1/IWPSummary.ozd";

                try
                {
                    Uri pathCheckUri = new Uri(_filePath);
                    var client = new HttpClient();
                    //파일 있는지 확인
                    string page = await client.GetStringAsync(pathCheckUri);
                    
                    strParam = "connection.openfile=" + _filePath + "\nviewer.usetoolbar=false\nviewer.errorcommand=false";
                }
                catch (Exception ex)
                {   
                    strParam = "";
                }

            }
            else
            {
                //2. 처음 작성 시 ozr 파일 로딩
                DataLibrary.rptProjectCwaIwpDTO dto = await (new Lib.ServiceModel.ProjectModel()).JsonGetProjectCwaIwpByIwp(Lib.IWPDataSource.selectedIWP.ToString());

                object[] objParam = new object[13];

                objParam[0] = "FIWPNM=" + dto.FiwpName; //107-10-21-01";
                objParam[1] = "ScheduleID=" + dto.ProjectScheduleName;//TBO";
                objParam[2] = "Title=" + dto.Description;// CWA 107 Cut and Cap Piles - Tank 3A-T-113";
                objParam[3] = "ClientNM=" + dto.ClientCompanyName;// Meg Energy Corp.";
                objParam[4] = "ProjectNM=" + dto.ClientProjectName;// CLRP - Phase 3A";
                objParam[5] = "LedcorProject=" + dto.ProjectName;// 3615465";
                objParam[6] = "CWARef=" + dto.CwaName;// 1100-10-S-01-107";
                objParam[7] = "ReleasedNM=" + Login.UserAccount.UserName;
                objParam[8] = "AssignedNM=" + dto.LeaderName;
                objParam[9] = "Manhour=" + dto.TotalManhours;
                objParam[10] = "ScheduleDT=" + dto.StartDate.ToString("d/M/yyyy") + "-" + dto.EndDate.ToString("d/M/yyyy");
                objParam[11] = "ReleasedDT=test";
                objParam[12] = "AssignedDT=test";
                dsReport.Params = objParam;

                dsReport.ServerYn = "Y";
                dsReport.ProjectCode = "Element";// "LedCore";
                dsReport.ReportName = "/" + FileName + ".ozr";
                dsReport.OdiName = OdiName;
                dsReport.ToolBarUseYn = "N";
                dsReport.ToolBarOtherMenuYn = "N";

                strParam = ReportUtil.MakeParameterForOnline(dsReport);
            }

            try
            {
                if (ozViewer != null)
                {
                    ozViewer.Dispose();
                }

                if (strParam != "")
                    ozViewer = ReportUtil.RunReport(brdViewer, strParam);
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("There is a problem loading the " + FileName + " - Please try again later", "Loading Error");
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Load " + FileName, "There is a problem loading the " + FileName + " - Please try again later", "Loading Error");
            }

            Login.MasterPage.Loading(false, this);
        }

        private void Button_Clicked(object sender, object e)
        {
            string tag = e != null ? e.ToString() : string.Empty;

            switch (tag)
            {
                
                case "Save":
                    Save();
                    break;
                case "Next":
                    Save();
                    break;
            }
        }

        //ozd, jpg 함께 저장
        private async void Save()
        {
            Login.MasterPage.Loading(true, this);

            try
            {
                //로컬에 임시로 저장
                StorageFolder folder = ApplicationData.Current.TemporaryFolder;
                string fullPath = folder.Path + "\\";
                string paramJPG = "export.format=jpg;export.mode=silent;export.path=" + fullPath + ";export.filename=" + FileName + ".jpg;export.confirmsave=false";
                ozViewer.ScriptEx("save", paramJPG, ";");
                string paramPDF = "export.format=pdf;export.mode=silent;export.path=" + fullPath + ";export.filename=" + FileName + ".pdf;export.confirmsave=false";
                ozViewer.ScriptEx("save", paramPDF, ";");
                string paramOZD = "export.format=ozd;export.mode=silent;export.path=" + fullPath + ";export.filename=" + FileName + ".ozd;export.confirmsave=false";
                ozViewer.ScriptEx("save", paramOZD, ";");

                //업로드용 bytes
                StorageFile fileJPG = await StorageFile.GetFileFromPathAsync(fullPath + FileName + ".jpg");
                byte[] bytesJPG = await WinAppLibrary.Utilities.Helper.GetImageBytesFromStorageFile(fileJPG);
                StorageFile filePDF = await StorageFile.GetFileFromPathAsync(fullPath + FileName + ".pdf");
                byte[] bytesPDF = await WinAppLibrary.Utilities.Helper.GetImageBytesFromStorageFile(filePDF);
                StorageFile fileOZD = await StorageFile.GetFileFromPathAsync(fullPath + FileName + ".ozd");
                byte[] bytesOZD = await WinAppLibrary.Utilities.Helper.GetImageBytesFromStorageFile(fileOZD);

                //fiwpdto
                List<DataLibrary.FiwpDTO> fiwpdto = new List<DataLibrary.FiwpDTO>();

                if (Lib.IWPDataSource.iwplist == null)
                {
                    Login.MasterPage.Loading(false, this);
                    return;
                }
                fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedIWP).ToList();

                fiwpdto[0].DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;

                //upFileCollectionDTO
                DataLibrary.UpfileDTOS upFileCollection = new DataLibrary.UpfileDTOS(); ;
                upFileCollection.fileStoreDTOList = new List<DataLibrary.FileStoreDTO>();
                upFileCollection.uploadedFileDTOList = new List<DataLibrary.UploadedFileInfoDTO>();

                #region fileStoreDTO
                DataLibrary.FileStoreDTO fileStoreDTOList = new DataLibrary.FileStoreDTO();

                if (_fileStoreID > 0)
                    fileStoreDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                else
                    fileStoreDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;

                fileStoreDTOList.CreatedBy = Login.UserAccount.UserName;
                fileStoreDTOList.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                fileStoreDTOList.FileCategory = DataLibrary.Utilities.FileCategory.REPORT;
                fileStoreDTOList.FileDescription = Login.UserAccount.CurProjectID + " " + Login.UserAccount.FIWPID + " " + FileName;    //내용 확인 필요
                fileStoreDTOList.FileStoreId = _fileStoreID;
                fileStoreDTOList.FileTitle = FileName;
                fileStoreDTOList.FileTypeCode = FileType;
                fileStoreDTOList.ProjectId = Login.UserAccount.CurProjectID;
                fileStoreDTOList.UpdatedBy = Login.UserAccount.UserName;
                fileStoreDTOList.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");

                upFileCollection.fileStoreDTOList.Add(fileStoreDTOList);
                #endregion

                #region uploadFileDTO

                DataLibrary.UploadedFileInfoDTO uploadedFileDTOList = new DataLibrary.UploadedFileInfoDTO();
                if (_fileStoreID > 0)
                    uploadedFileDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                else
                    uploadedFileDTOList.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                uploadedFileDTOList.byteFile = bytesJPG;
                uploadedFileDTOList.FileExtension = "jpg";
                uploadedFileDTOList.Size = bytesJPG.Length;
                uploadedFileDTOList.CreatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList.FileStoreId = _fileStoreID;
                uploadedFileDTOList.Name = FileName;
                uploadedFileDTOList.Path = fullPath;
                uploadedFileDTOList.UpdatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList.UploadedBy = Login.UserAccount.UserName;
                uploadedFileDTOList.UploadedDate = DateTime.Now;
                //uploadedFileDTOList.Revision
                //uploadedFileDTOList.UploadedFileInfoId

                upFileCollection.uploadedFileDTOList.Add(uploadedFileDTOList);

                DataLibrary.UploadedFileInfoDTO uploadedFileDTOList2 = new DataLibrary.UploadedFileInfoDTO();
                if (_fileStoreID > 0)
                    uploadedFileDTOList2.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                else
                    uploadedFileDTOList2.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                uploadedFileDTOList2.byteFile = bytesPDF;
                uploadedFileDTOList2.CreatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList2.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList2.FileExtension = "pdf";
                uploadedFileDTOList2.FileStoreId = _fileStoreID;
                uploadedFileDTOList2.Name = FileName;
                uploadedFileDTOList2.Path = fullPath;
                //uploadedFileDTOList2.Revision
                uploadedFileDTOList2.Size = bytesPDF.Length;
                uploadedFileDTOList2.UpdatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList2.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList2.UploadedBy = Login.UserAccount.UserName;
                uploadedFileDTOList2.UploadedDate = DateTime.Now;
                //uploadedFileDTOList2.UploadedFileInfoId

                upFileCollection.uploadedFileDTOList.Add(uploadedFileDTOList2);

                DataLibrary.UploadedFileInfoDTO uploadedFileDTOList3 = new DataLibrary.UploadedFileInfoDTO();
                if (_fileStoreID > 0)
                    uploadedFileDTOList3.DTOStatus = (int)DataLibrary.Utilities.RowStatus.Update;
                else
                    uploadedFileDTOList3.DTOStatus = (int)DataLibrary.Utilities.RowStatus.New;
                uploadedFileDTOList3.byteFile = bytesOZD;
                uploadedFileDTOList3.CreatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList3.CreatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList3.FileExtension = "ozd";
                uploadedFileDTOList3.FileStoreId = _fileStoreID;
                uploadedFileDTOList3.Name = FileName;
                uploadedFileDTOList3.Path = fullPath;
                //uploadedFileDTOList3.Revision
                uploadedFileDTOList3.Size = bytesOZD.Length;
                uploadedFileDTOList3.UpdatedBy = Login.UserAccount.UserName;
                uploadedFileDTOList3.UpdatedDate = DateTime.Now.ToString("dd-MM-yyyy");
                uploadedFileDTOList3.UploadedBy = Login.UserAccount.UserName;
                uploadedFileDTOList3.UploadedDate = DateTime.Now;
                //uploadedFileDTOList3.UploadedFileInfoId

                upFileCollection.uploadedFileDTOList.Add(uploadedFileDTOList3);
                #endregion


                //결과값 사용하지 않음
                await (new Lib.ServiceModel.ProjectModel()).SaveDocumentWithOZformForAssembleIWP(fiwpdto, upFileCollection, AssembleStep, Login.UserAccount.PersonnelId);

                //현재 단계 저장
                if (!fiwpdto[0].DocEstablishedLUID.Equals(DataLibrary.Utilities.AssembleStep.APPROVER))
                    fiwpdto[0].DocEstablishedLUID = AssembleStep;

                Lib.WizardDataSource.SetTargetMenu(AssembleStep, Lib.CommonDataSource.selPackageTypeLUID, true);


                //공통 파일 - AssembleDocument나 AssembleReport로 이동 시 파라미터 추가
                if (Lib.WizardDataSource.NextMenu != null)
                {
                    //IWPSummary -> SafetyChecklist
                    if (AssembleStep == DataLibrary.Utilities.AssembleStep.SUMMARY)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SAFETY_CHECK);
                    //SafetyChecklist -> Safety Formlist
                    else if(AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_CHECK)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SAFETY_FORM);
                    //ScaffoldChecklist -> Specs & Details
                    else if(AssembleStep == DataLibrary.Utilities.AssembleStep.SCAFFOLD_CHECK)
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu, DataLibrary.Utilities.AssembleStep.SPEC);
                    else
                        this.Frame.Navigate(Lib.WizardDataSource.NextMenu);
                }
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Save " + FileName, "There is a problem saving the " + FileName + " - Please try again later", "Error");
            }
            Login.MasterPage.Loading(false, this);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            //공통 파일 - AssembleDocument나 AssembleReport로 이동 시 파라미터 추가
            if (Lib.WizardDataSource.PreviousMenu != null)
            {
                if(AssembleStep == DataLibrary.Utilities.AssembleStep.SAFETY_CHECK)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu, DataLibrary.Utilities.AssembleStep.SUMMARY);
                else
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
            }
        }
    }
}
