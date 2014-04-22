using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using WinAppLibrary.Utilities;

namespace WinAppLibrary.Converters
{
    public sealed class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    var image = await this.ImageStreamAsyn((string)value);
                    return image;
                });

                return new TaskCompletionNotifier<BitmapImage>(task); ;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }

        private async Task<BitmapImage> ImageStreamAsyn(string url)
        {
            var response = await (new SPDocument()).GetDocument(url);

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
                writer.WriteBytes(response);
                await writer.StoreAsync();
                BitmapImage b = new BitmapImage();
                b.SetSource(stream);
                return b;
            }
        }
    }

    public sealed class PortraitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                int id = -1;
                Uri retValue;
                string sub = "Foreman";

                if (string.Equals(parameter, "Crew"))
                    sub = "Crew";
                else if (string.Equals(parameter, "Crewbrass"))
                    sub = "Crewbrass";
                else if (string.Equals(parameter, "CrewPicture"))
                    sub = "CrewPicture";

                try
                {
                    int.TryParse(value.ToString(), out id);
                }
                catch { }

                if (sub == "CrewPicture")
                {
                    if (id > -1)
                        retValue = new Uri(string.Format("{0}/{1}/{2}.jpg", new Uri(Windows.Storage.ApplicationData.Current.LocalFolder.Path), sub, id));
                    else
                        retValue = new Uri(string.Format("{0}/{1}/{2}.jpg", new Uri(Windows.Storage.ApplicationData.Current.LocalFolder.Path), sub, (string)value));
                }
                else
                {
                    if (id > -1)
                        retValue = new Uri(string.Format("{0}Assets/{1}/{2}.png", Utilities.Helper.BaseUri, sub, id));
                    else
                        retValue = new Uri(string.Format("{0}Assets/{1}/{2}.png", Utilities.Helper.BaseUri, sub, (string)value));
                }

                

                return retValue;
            }
            catch
            {
                return new Uri(Utilities.Helper.BaseUri + "Assets/DefaultCrew.png");
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            //throw new NotImplementedException();
            return value;
        }
    }

    public sealed class StickyNoteConverter : IValueConverter
    {
        Random random = new Random((int)DateTime.Now.Ticks);
        public object Convert(object value, Type targetType,
                          object parameter, string culture)
        {
            try
            {
                string[] param = parameter.ToString().Split(';');
                Uri retValue = new Uri(param[0] + random.Next(int.Parse(param[1])) + ".png");
                return retValue;
            }
            catch
            {
                return new Uri("");
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            //throw new NotImplementedException();
            return value;
        }
    }
}
