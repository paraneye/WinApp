using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Element.Reveal.DataLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.IWPSignoff
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignoffTitle : WinAppLibrary.Controls.LayoutAwarePage
    {
        int imgcount = 0;
        private bool ApproveYN = true;

        Lib.WorkFlowDataSource _workflow = new Lib.WorkFlowDataSource();
        DocumentAndDrawing DocumentDrawingDTO = new DocumentAndDrawing();

        public SignoffTitle()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            tbpageTitle.Text = Lib.WorkFlowDataSource.selectedTypeName;
            int iwpid = Lib.WorkFlowDataSource.selectedIwpID;
            int documentid = Lib.WorkFlowDataSource.selectedDocumentID;
            if (iwpid == documentid) //IWP Signoff
            {
                LoadIwpSignoff(iwpid);
            }
            else //ITR
            {
                LoadITR(iwpid, documentid);
            }

            if (Lib.WorkFlowDataSource.sentyn == "Y")
            {
                btnApprove.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnReject.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                btnApprove.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnReject.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            btnPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void LoadIwpSignoff(int iwpID)
        {
            await _workflow.GetFIWPDocDrawingsByFIWP(iwpID, Login.UserAccount.CurProjectID);
            DocumentDrawingDTO = _workflow.GetFIWPDocDrawings();

            ShowImage(imgcount);
        }

        private async void LoadITR(int iwpID, int documentID)
        {
            await _workflow.GetIwpDocumentByIwpProjectFileType(iwpID, Login.UserAccount.CurProjectID, documentID);
            DocumentDrawingDTO.documents = _workflow.GetIwpDocument();
            ShowImage(imgcount);
        }

        private void ShowImage(int imgcount)
        {
            if (DocumentDrawingDTO.documents != null && DocumentDrawingDTO.documents.Count > 0)
            {
                if (imgcount < 0)
                {
                    btnPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //WinAppLibrary.Utilities.Helper.SimpleMessage("First Image!", "Warning!");
                }
                else if (imgcount == DocumentDrawingDTO.documents.Count - 1)
                {
                    btnNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //WinAppLibrary.Utilities.Helper.SimpleMessage("Last Image!", "Warning!");
                }
                else
                {
                    btnNext.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    string url = DocumentDrawingDTO.documents[imgcount].LocationURL;

                    if (url == "")
                        url = WinAppLibrary.Utilities.Helper.BaseUri + "Assets\\Default.PNG";

                    imgView.UriSource = new Uri(url);
                }
            }
            else
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("There is No Data.", "Not Loading!");

                btnPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                string url = WinAppLibrary.Utilities.Helper.BaseUri + "Assets\\Default.PNG";
                imgView.UriSource = new Uri(url);
            }
        }


        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            string url = WinAppLibrary.Utilities.Helper.BaseUri + "Assets\\Default.PNG";
            imgView.UriSource = new Uri(url);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            imgcount = imgcount - 1;            

            btnNext.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (imgcount < 0)
                btnPrev.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            else
                btnPrev.Visibility = Windows.UI.Xaml.Visibility.Visible;

            ShowImage(imgcount);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            imgcount = imgcount + 1;            

            btnPrev.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (imgcount == DocumentDrawingDTO.documents.Count - 1)
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            else
                btnNext.Visibility = Windows.UI.Xaml.Visibility.Visible;

            ShowImage(imgcount);
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(IWPSignoffStatus));
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            txbPopTitle.Text = "Approve";
            txbPopSubTitle.Text = "Please type if you have any comment (Optional)";
            ApproveYN = true;

            stpPop.Visibility = Windows.UI.Xaml.Visibility.Visible;
            grPop.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            txbPopTitle.Text = "Reject";
            txbPopSubTitle.Text = "Please comment the reason why you reject the sign off request";
            ApproveYN = false;

            stpPop.Visibility = Windows.UI.Xaml.Visibility.Visible;
            grPop.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        
        //Main Close
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(IWPSignoffStatus));
        }
        
        //PopUp Approve
        private async void btnPopApprove_Click(object sender, RoutedEventArgs e)
        {
            if (!ApproveYN && txtComment.Text.Length < 1)
            {
                WinAppLibrary.Utilities.Helper.SimpleMessage("Please comment the reason why you reject the sign off request", "Warning!");
            }
            else
            {
                bool result = await (new Lib.ServiceModel.WorkflowModel()).SaveWorkflowForEasy(Lib.WorkFlowDataSource.PackageTypeCode, Lib.WorkFlowDataSource.selectedDocumentID, 0, ApproveYN, Login.UserAccount.PersonnelId, string.Empty, txtComment.Text);
                if (result)
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Save Complete", "Complete");

                    txtComment.Text = "";
                    stpPop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    grPop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    btnApprove.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    btnReject.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Save Failed!", "Warning!");
                }
            }
        }

        //Popup Close
        private void btnPopCancel_Click(object sender, RoutedEventArgs e)
        {
            txtComment.Text = "";
            stpPop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            grPop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }


    }
}
