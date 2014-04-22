using System.IO;
using System.Xml.Linq;
using WinAppLibrary.ServiceModels;

namespace WinAppLibrary.Menu
{
    public class MenuDataSource
    {
        public MenuDataSource()
        {
        }

        public static GroupModel GetDataSrouce()
        {
            GroupModel _datasource = new GroupModel();
            string meneXmlPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Lib/Data/MainMenu.xml");

            XDocument loadData = XDocument.Load(meneXmlPath);

            DataGroup dataGroup = null;
            var groups = loadData.Root.Elements();
            foreach (var group in groups)
            {
                dataGroup = new DataGroup(group.Attribute("uniqueId").Value, group.Attribute("title").Value, group.Attribute("imagePath").Value);
                foreach (var dataitem in group.Elements())
                {
                    dataGroup.Items.Add(new DataItem(dataitem.Attribute("uniqueId").Value,
                                                     dataitem.Attribute("title").Value,
                                                     dataitem.Attribute("imagePath").Value,
                                                     dataitem.Attribute("content").Value,
                                                     dataGroup));
                }

                _datasource.AllGroups.Add(dataGroup);
            }

            return _datasource;
        }
    }
}
