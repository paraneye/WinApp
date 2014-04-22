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
using Element.Reveal.Crew.RevealProjectSvc;
using Element.Reveal.Crew.RevealCommonSvc;

using WinAppLibrary.ServiceModels;
using WinAppLibrary.Utilities;
using WinAppLibrary.Extensions;

using System.Reflection;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Crew.Discipline.Administrator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToolBoxTalkSelection : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.DataSource.ToolBoxTalkSelectionDataSource _document = new Lib.DataSource.ToolBoxTalkSelectionDataSource();

        private bool _isitemtrue = false;
        private bool _isopened = false;
        public bool IsOpened
        {
            get { return _isopened; }
        }

        public ToolBoxTalkSelection()
        {
            Login.MasterPage.SetPageTitle("Safety Document");
            this.InitializeComponent();
        }

        Lib.ServiceModel.ProjectModel _projectSM = new Lib.ServiceModel.ProjectModel();

        List<DailytoolboxDTO> _dailytoolboxdto = new List<DailytoolboxDTO>();

        bool login = false;
        int _dailybassid = 0;

        #region "PageLoad"
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                _dailybassid = Convert.ToInt32(navigationParameter.ToString()); // 74; 

                switch (Login.LoginMode)
                {
                    case WinAppLibrary.UI.LogMode.OnMode:
                        BindDocumentList();
                        break;
                    case WinAppLibrary.UI.LogMode.OffMode:
                        BindDocumentListOff();
                        break;
                }
                SharePointLogin();
            }
            catch (Exception ex)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "LoadState");
            }
        }
        #endregion

        #region "조회"
        private async void BindDocumentList()
        {
            Login.MasterPage.Loading(true, this);
            try
            {
                ////DailyToolBox 기존 데이타가 있는지 확인
                _dailytoolboxdto = await (new Lib.ServiceModel.ProjectModel()).GetDailytoolboxByDailyBras(_dailybassid);
            }
            catch(Exception e)
            {
            }
            
            try
            {
                //Group View
                List<DataGroup> source = null;
                await _document.GetSafetyDocumentsList(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);
                source = _document.GetGroupSharePoint(); 

                this.DefaultViewModel["Document"] = source;
                
                //this.gvDocument.SelectedItem = null;

                var docuList = await (new Lib.ServiceModel.ProjectModel()).GetSafetyDocumentsList(Login.UserAccount.CurProjectID, Login.UserAccount.CurModuleID);

                var result = from t in docuList
                             join x in _dailytoolboxdto on t.SPCollectionID equals x.SPCollectionID
                             select t;

                // Display results.
                foreach (var r in result)
                {
                    foreach (DataItem item in gvDocument.Items)
                    {
                        if (item.Content == r.Description)
                        {
                            gvDocument.SelectedItems.Add(item);
                            Lib.DataSource.ToolBoxTalkSelectionDataSource.strFileName = item.Content;
                        }
                    }                                       
                }               
            }
            catch (Exception e)
            {                
            }

            if (gvDocument.SelectedItems.Count > 0)
            {
                _isitemtrue = true;
            }

            Login.MasterPage.Loading(false, this);
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var top = (LayoutRoot.RowDefinitions[1].ActualHeight - 470) / 2;
            //ScrollViewer.Padding = new Thickness(0, top, 0, 0);
        }

        private async void BindDocumentListOff()
        {           
            Login.MasterPage.Loading(false, this);
        }
        #endregion

        private void btnDocuDown_Click(object sender, RoutedEventArgs e)
        {
            //Select SharePoint Document DownLoad
            if (gvDocument.SelectedItems.Count == 0)
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please select Document", "Caution!");
            else
            {
                if (!login)
                    SharePointLogin();

                try
                {
                    //if (login)
                    //{
                        List<DailytoolboxDTO> dtolist = new List<DailytoolboxDTO>();
                        foreach (DataItem item in gvDocument.SelectedItems)
                        {
                            //DownloadDocument(item.LocationURL, item.Description);
                            DownloadDocument(item.ImagePath, item.Content);

                            if (_isitemtrue == false)
                            {
                                DocumentDTO newitem = new DocumentDTO();

                                DailytoolboxDTO dto = new DailytoolboxDTO();
                                dto.DailyBrassID = _dailybassid; // 67; _dailybassid;
                                dto.DocumentName = item.Content;
                                dto.DocumentVersion = "";
                                dto.SPCollectionID = Convert.ToInt32(item.UniqueId);
                                dto.DTOStatus = (int)WinAppLibrary.Utilities.RowStatus.New;

                                _dailytoolboxdto.Add(dto);
                            }
                        }

                        if (_dailytoolboxdto.Count > 0 && _isitemtrue == false)
                            SaveDailyToolBoxTalk(_dailytoolboxdto);

                        WinAppLibrary.Utilities.Helper.SimpleMessage("Download Complete", "Success");
                    //}
                    //else
                    //    WinAppLibrary.Utilities.Helper.SimpleMessage("SharePoint Login Fail", "Caution!");
                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "DownloadDocument");
                }
            }

        }

        private async void SaveDailyToolBoxTalk(List<DailytoolboxDTO> dtolist)
        {
            try
            {
                _dailytoolboxdto = await _projectSM.SaveDailyToolBox(dtolist);
                //await (new Lib.DataSource.ProjectModuleSource()).SaveFileToolbox(_dailytoolboxdto, Lib.HashKey.Key_ToolboxIn);
            }
            catch (Exception ex)
            {
            }
        }

        private async void SharePointLogin()
        {
            login = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);
        }

        private async void DownloadDocument(string strurl, string strFileName)
        {
            try
            {
                Lib.DataSource.ToolBoxTalkSelectionDataSource.strFileName = strFileName;
                byte[] bytes = await (new WinAppLibrary.Utilities.SPDocument()).GetDocumentWithLogin(strurl, Login.UserAccount.SPUser, Login.UserAccount.SPPassword) ; //.GetDocument(strurl);

                if (bytes.Length > 0)
                {
                    //var source = await (new WinAppLibrary.Utilities.Helper()).GetWriteableBitmapFromBytes(bytes);
                    //var stream = await (new WinAppLibrary.Utilities.Helper()).GetJpegStreamFromWriteableBitmap(source);

                    //if (source != null && source.PixelWidth > 0 && source.PixelHeight > 0)
                    //{
                    //    await (new WinAppLibrary.Utilities.Helper()).SaveFileStream(Element.Reveal.Crew.Lib.ContentPath.OffModeFolder, strFileName, stream);

                    //}
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(strFileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                    
                    await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
                }
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "Document Download", "We failed to save the download document.", "Error!");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CrewBrassIn), Login.UserAccount.PersonnelID);
        }
                 
        private void btnDailyToolBox_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DailyToolBoxTalk), _dailybassid);
        }
        
        private async void DownloadImg(string strurl, string strFileName)
        {
            byte[] bytes = await (new WinAppLibrary.Utilities.SPDocument()).GetDocumentWithLogin(strurl, Login.UserAccount.SPUser, Login.UserAccount.SPPassword); // GetDocument(strurl);            

            try
            {
                //var source = await (new WinAppLibrary.Utilities.Helper()).GetWriteableBitmapFromBytes(bytes);
                //imgPopup.Source = source;
                
                var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(strFileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);

                await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
                               
                var stream = await (new WinAppLibrary.Utilities.Helper()).GetFileStream(Windows.Storage.ApplicationData.Current.LocalFolder, strFileName);
                PdfViewer.LoadDocument(stream);

            }
            catch (Exception ex)
            {  
            }
        }

        private void btnExitPopup_Click(object sender, RoutedEventArgs e)
        {
            PopupPannel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var item = (sender as StackPanel).DataContext as DataItem;

            if (!login)
                SharePointLogin();
            
            //if (login)
            //{
                try
                {
                    DownloadImg(item.ImagePath, item.Content);
                    PopupPannel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                catch (Exception ex)
                {
                    (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(ex, "Image View", "Image File Error.", "Error!");
                }
            //}
            //else
            //    WinAppLibrary.Utilities.Helper.SimpleMessage("SharePoint Login Fail", "Caution!");
        }

    }
}
