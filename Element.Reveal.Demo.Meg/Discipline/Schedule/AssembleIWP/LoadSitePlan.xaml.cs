using System;
using System.Collections;
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
using WinAppLibrary.ServiceModels;
using WinAppLibrary.Converters;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadSitePlan : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        private int _projectid, _moduleid;
        private MemoryStream _stream;

        public LoadSitePlan()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;

            InitPage();
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage, Lib.CommonDataSource.selPackageTypeLUID, true);

            if (Lib.WizardDataSource.PreviousMenu != null)
                this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
        }

        #endregion

        private async void InitPage()
        {
           FileOpenPicker openPicker = new FileOpenPicker();
           // openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();


            // Ensure a file was selected
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {

                    var reader = new DataReader(fileStream.GetInputStreamAt(0));
                    var bytes = new byte[fileStream.Size];
                    await reader.LoadAsync((uint)fileStream.Size);
                    reader.ReadBytes(bytes);
                    _stream = new MemoryStream(bytes);

                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.DecodePixelHeight = 500;
                    bitmapImage.DecodePixelWidth = 800;

                    await bitmapImage.SetSourceAsync(fileStream);
                   
                    Img3D.Source = bitmapImage;

                }
            }
            else
            {
                Lib.WizardDataSource.SetTargetMenu(WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage, Lib.CommonDataSource.selPackageTypeLUID, true);

                if (Lib.WizardDataSource.PreviousMenu != null)
                    this.Frame.Navigate(Lib.WizardDataSource.PreviousMenu);
            }
        }

        private async void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            Login.MasterPage.Loading(true, this);

            //exist data check
            var edoc = await (new Lib.ServiceModel.ProjectModel()).GetDocumentForFIWPByDocType(Lib.DocType.ModelView, Lib.IWPDataSource.selectedIWP, _projectid, _moduleid);

            //1. Call Service(document,blank image upload to sharepoint)
            RevealProjectSvc.DocumentDTO document = new RevealProjectSvc.DocumentDTO();
            document.DocumentTypeLUID = Lib.DocType.ModelView;
            document.CWPID = Lib.CWPDataSource.selectedCWP;
            document.ProjectID = _projectid;
            document.ModuleID = _moduleid;
            document.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
            document.FIWPID = Lib.IWPDataSource.selectedIWP;
            document.Description = "3D Model View Image";
            document.UpdatedBy = Login.UserAccount.UserName;
            document.UpdatedDate = DateTime.Now;

            if (edoc.Count > 0) {
                document.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
                document.SPCollectionID = edoc[0].SPCollectionID; 
                document.DocumentID = edoc[0].DocumentID;
            }
            else
            {
                document.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;
                document.SPCollectionID = 0;
                document.DocumentID = 0;
            }            

            string docName = string.Empty;

            List<RevealProjectSvc.FiwpDTO> fiwpdto = new List<RevealProjectSvc.FiwpDTO>();
            fiwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedIWP).ToList();

            fiwpdto[0].DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.Update;
            fiwpdto[0].DocEstablishedLUID = WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage;

            RevealProjectSvc.DocumentDTO rtn = await (new Lib.ServiceModel.ProjectModel()).SaveProjectDocumentWithSharePointForModelView(fiwpdto, document, string.Format("{0}-{1}.jpg",
                Lib.CWPDataSource.selectedCWPName, Lib.IWPDataSource.selectedIWPName), Lib.CWPDataSource.selectedCWPName, Lib.IWPDataSource.selectedIWPName);

            //2. Client Image Update to SharePoint
            bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

            if (islogin)
            {
                try
                {
                    var rst = await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.ProjectDoc + "/",
                        string.Format("{0}-{1}.jpg", Lib.CWPDataSource.selectedCWPName, Lib.IWPDataSource.selectedIWPName), _stream);



                    RevealProjectSvc.DocumentDTO documents = new RevealProjectSvc.DocumentDTO();
                    documents.DocumentTypeLUID = Lib.DocType.ModelView;
                    documents.CWPID = Lib.CWPDataSource.selectedCWP;
                    documents.ProjectID = _projectid;
                    documents.ModuleID = _moduleid;
                    documents.ProjectScheduleID = Lib.ScheduleDataSource.selectedSchedule;
                    documents.FIWPID = Lib.IWPDataSource.selectedIWP;
                    documents.UpdatedBy = Login.UserAccount.UserName;
                    documents.UpdatedDate = DateTime.Now;

                    await (new Lib.ServiceModel.ProjectModel()).SaveProjectDocumentWithSharePointForWFP(document,
                        Lib.CWPDataSource.selectedCWPName, Lib.IWPDataSource.selectedIWPName, WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage);

                    Lib.IWPDataSource _iwp = new Lib.IWPDataSource();
                    await _iwp.GetFiwpByScheduleIDOnMode(Lib.ScheduleDataSource.selectedSchedule);
                    RevealProjectSvc.FiwpDTO iwpdto = new RevealProjectSvc.FiwpDTO();
                    iwpdto = Lib.IWPDataSource.iwplist.Where(x => x.FiwpID == Lib.IWPDataSource.selectedIWP).FirstOrDefault();

                    Lib.IWPDataSource.selectedIWP = iwpdto.FiwpID;
                    Lib.IWPDataSource.selectedIWPName = iwpdto.FiwpName;
                    Lib.IWPDataSource.isWizard = iwpdto.DocEstablishedLUID == WinAppLibrary.Utilities.DocEstablishedForApp.SiteImage ? false : true;


                    this.Frame.Navigate(typeof(Discipline.Schedule.AssembleIWP.AssembleIWP));


                }
                catch (Exception ex)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("There was an error Image Load . Pleae contact administrator", "Error!");
                }
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

            Login.MasterPage.Loading(false, this);

           
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            InitPage();
        }

        #region "Private Method"

        #endregion
    }
}
