using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinAppLibrary.ServiceModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThumbnailViewer : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid; private string _disciplineCode;
        
        public ThumbnailViewer()
        {
            this.InitializeComponent();
            
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _disciplineCode = Login.UserAccount.CurDisciplineCode;
            LoadThumbnail();
        }

        private async void LoadThumbnail() 
        {
            try 
            {
                List<DataGroup> source = null;
                DataGroup datagroup = null;

                List<DataLibrary.ComboBoxDTO> thumbnailviewr = new List<DataLibrary.ComboBoxDTO>();
                thumbnailviewr = await (new Lib.ServiceModel.CommonModel()).GetDrawingByCWP(Lib.CWPDataSource.selectedCWP, Lib.CommonDataSource.selectedMaterialCategory, "", _projectid, _disciplineCode);
                thumbnailviewr[0].ExtraValue2 = "PL21-P-8391-1%20REV%201B.jpg";
                thumbnailviewr[0].ExtraValue3 = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/PL21-P-8391-1%20REV%201B.jpg";
                //thumbnailviewr[0].ExtraValue2 = "http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/PL21-P-8391-1%20REV%201B.jpg";
               // thumbnailviewr[0].ExtraValue2 = "PL21-P-8391-1%20REV%201B.jpg";
                
                thumbnailviewr.Add(thumbnailviewr[0]);
                for (int i = 0; i < 30; i++)
                {
                    thumbnailviewr.Add(thumbnailviewr[0]);
                }
                gvDrawing.ItemsSource = thumbnailviewr;
            }
            catch (Exception ex)
            { 

            }
        }

        #region Button Event
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectTask));
        }
        #endregion

        private void gvDrawing_ItemClick(object sender, ItemClickEventArgs e)
        {
            Lib.CommonDataSource.selectedDrawing = ((DataLibrary.ComboBoxDTO)e.ClickedItem).DataID;
            this.Frame.Navigate(typeof(DrawingViewer));
        }
    }
}
