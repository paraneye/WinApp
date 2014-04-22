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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.TrueTask.Discipline.Schedule.AssembleIWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatedBinder : WinAppLibrary.Controls.LayoutAwarePage
    {
        Lib.CWPDataSource _cwp = new Lib.CWPDataSource();
        private int _projectid, _moduleid;

        public CreatedBinder()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
        }

        #region "Event Handler"

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        #endregion

        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
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

                Login.MasterPage.Loading(true, this);

                bool islogin = WinAppLibrary.Utilities.SPDocument.IsLogin ? true : await (new WinAppLibrary.Utilities.SPDocument()).SignInSharepoint(Login.UserAccount.SPURL, Login.UserAccount.SPUser, Login.UserAccount.SPPassword);

                if (islogin)
                {
                    try
                    {
                        // Ensure the stream is disposed once the image is loaded
                        using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {

                            var reader = new DataReader(fileStream.GetInputStreamAt(0));
                            var bytes = new byte[fileStream.Size];
                            await reader.LoadAsync((uint)fileStream.Size);
                            reader.ReadBytes(bytes);
                            var stream = new MemoryStream(bytes);


                            await (new WinAppLibrary.Utilities.SPDocument()).SaveJpegContent(Login.UserAccount.SPURL + "/" + WinAppLibrary.Utilities.SPCollectionName.Drawing + "/", "131016AssemblyIWP_test.jpg", stream);
                            // Set the image source to the selected bitmap
                            BitmapImage bitmapImage = new BitmapImage();
                            //bitmapImage.DecodePixelHeight = decodePixelHeight;
                            //bitmapImage.DecodePixelWidth = decodePixelWidth;

                            await bitmapImage.SetSourceAsync(fileStream);
                            Img3D.Source = bitmapImage;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                    WinAppLibrary.Utilities.Helper.SimpleMessage("Your account for sharepoint is not valid. Please login again.", "Caution!");

                Login.MasterPage.Loading(false, this);
            }
        }

        private void btnIWPEquip_Click(object sender, RoutedEventArgs e)
        {

        }


        #region "Private Method"

        #endregion
    }
}
