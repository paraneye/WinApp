using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using C1.Xaml.DateTimeEditors;
using C1.Xaml;
using Windows.UI.Core;
using System.IO;
using System.Xml.Serialization;

namespace Element.Reveal.Meg.Discipline.ITR
{
    public class CControlSerializer
    {
        public Dictionary<string, string> UserData { get; set; }
        public Dictionary<string, string> UserDataforWCF { get; set; }
        public string JsonUserData { get; set; }
        public event EventHandler Event_NFCAssigne;

        public CControlSerializer() { }

        #region Serializer
        /// <summary>
        /// 최상위 Element를 지정하고 하위의 Type에 해당하는 값들을 name : value 형식으로 만든다.
        /// </summary>
        /// <param name="_grd">최상위 Element</param>
        public void Serialize(Grid _grd)
        {
            JsonUserData = "";
            UserData = new Dictionary<string, string>();
            MakeDictionay(_grd, new List<Type> { typeof(TextBox), typeof(CheckBox), typeof(RadioButton) , typeof(ComboBox), typeof(C1DatePicker)});
            var entries = UserData.Select(d =>
                    string.Format("{0}|:ITRS:|{1}", d.Key, d.Value)
                );
            JsonUserData = string.Join("|:ITRITEMS:|", entries);
        }

        /// <summary>
        /// 최상위 Element를 지정하고 하위의 Type에 해당하는 값들을 name : value 형식으로 만든다.
        /// </summary>
        /// <param name="_reference">최상위 Element</param>
        /// <param name="_findType">조사할 Type들 </param>
        private void MakeDictionay(FrameworkElement _reference, List<Type> _findType)
        {
            if (_reference != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(_reference);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(_reference, i);
                    if (_findType.IndexOf(child.GetType()) > -1)
                        GetUserData((FrameworkElement)child);
                    else
                        MakeDictionay((FrameworkElement)child, _findType);
                }
            }
        }

        private void GetUserData(FrameworkElement _element)
        {
            string value = "";

            if (_element.GetType() == typeof(TextBox))
            {
                value = ((TextBox)_element).Text;
            }
            else if (_element.GetType() == typeof(ComboBox))
            {
                value = (((ComboBox)_element).SelectedItem == null) ? "" : ((ComboBox)_element).SelectedItem.ToString();
            }
            else if (_element.GetType() == typeof(CheckBox))
            {
                value = (((CheckBox)_element).IsChecked == true) ? "Y" : "N";
            }
            else if (_element.GetType() == typeof(RadioButton))
            {
                value = (((RadioButton)_element).IsChecked == true) ? "Y" : "N";
            }
            else if (_element.GetType() == typeof(C1DatePicker))
            {
                value = ((C1DatePicker)_element).Text;
            }

            UserData.Add(_element.Name, value);
        }
        #endregion

        #region Deserializer
        /// <summary>
        /// Data String을 Element에 Assign 한다.
        /// </summary>
        /// <param name="_parent">상위 Framework Element</param>
        /// <param name="_data">Key Value User Data</param>
        public void Deserialize(FrameworkElement _parent, Dictionary<string,string> _data)
        {
            UserData = _data;
            if (UserData.Count > 0)
            {
                foreach (KeyValuePair<string, string> p in UserData)
                {
                    FrameworkElement element = (FrameworkElement)_parent.FindName(p.Key);
                    SetUserData(element, p.Value);
                }
            }
        }

        private void SetUserData(FrameworkElement _element, string _data)
        {
            if (_element.GetType() == typeof(TextBox))
            {
                ((TextBox)_element).Text = _data;
            }
            else if (_element.GetType() == typeof(CheckBox))
            {
                ((CheckBox)_element).IsChecked = (_data == "Y") ? true : false;
            }
            else if (_element.GetType() == typeof(ComboBox))
            {
                if(((ComboBox)_element).Items.Count > 0 && _data != "")
                    ((ComboBox)_element).SelectedItem = _data;
            }
            else if (_element.GetType() == typeof(RadioButton))
            {
                ((RadioButton)_element).IsChecked = (_data == "Y") ? true : false;
            }
            else if (_element.GetType() == typeof(C1DatePicker))
            {
                ((C1DatePicker)_element).Text = _data;
            }
        }
        #endregion
    }

    public static class FormSerialize
    {
        public static Stream EncryptHashSerializeTo<T>(object obj)
        {
            Stream retValue = null;
            WinAppLibrary.Utilities.Helper helper = new WinAppLibrary.Utilities.Helper();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter stringwriter = new StringWriter())
                {
                    using (System.Xml.XmlWriter xmlwriter = System.Xml.XmlWriter.Create(stringwriter))
                    {
                        serializer.Serialize(stringwriter, obj);
                        string xml = WinAppLibrary.Utilities.Helper.Encrypt(stringwriter.ToString(), WinAppLibrary.Utilities.HashKey.Key_DrawingList);
                        retValue = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                    }
                }
            }
            catch { }

            return retValue;
        }

        public static void GenDTO(int _group, List<List<FrameworkElement>>_elements, List<RevealProjectSvc.QaqcformdetailDTO> _dtolist)
        {
            CControlSerializer serialize = new CControlSerializer();
            int i = 1; int j = 1; int gno = 1;
            foreach (List<FrameworkElement> _element in _elements)
            {
                RevealProjectSvc.QaqcformdetailDTO dto = new RevealProjectSvc.QaqcformdetailDTO { InspectionLUID = _group, InspectedValue = gno};
                
                foreach (FrameworkElement _el in _element)
                {
                    if (_el.GetType() == typeof(C1DatePicker))
                    {
                        if (j == 1) { dto.DateValue1 = DateTime.Parse((string)FormSerialize.GetUserData(_el)); j++; continue; }
                        if (j == 2) { dto.DateValue2 = DateTime.Parse((string)FormSerialize.GetUserData(_el)); j++; continue; }
                    }
                    if (_el.GetType() == typeof(ListView))
                    {
                        ListView tmp = (ListView)_el;
                        WinAppLibrary.UI.ObjectNFCSign tmp2 = (WinAppLibrary.UI.ObjectNFCSign)tmp.Items[0];
                        dto.StringValue1 = tmp2.PersonnelName;
                        if(!string.IsNullOrEmpty(tmp2.SignedTime))
                            dto.DateValue1 = DateTime.Parse(tmp2.SignedTime);
                        _dtolist.Add(dto);
                        return;
                    }
                    if (i == 1) { dto.StringValue1 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 2) { dto.StringValue2 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 3) { dto.StringValue3 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 4) { dto.StringValue4 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 5) { dto.StringValue5 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 6) { dto.StringValue6 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 7) { dto.StringValue7 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 8) { dto.StringValue8 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 9) { dto.StringValue9 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 10) { dto.StringValue10 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 11) { dto.StringValue11 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 12) { dto.StringValue12 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 13) { dto.StringValue13 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 14) { dto.StringValue14 = FormSerialize.GetUserData(_el); i++; continue; }
                    if (i == 15) { dto.StringValue15 = FormSerialize.GetUserData(_el); i++; continue; }
                }
                _dtolist.Add(dto);
                gno++; i = 1; j = 1;
            }
        }

        public static void SetLoadData(List<FrameworkElement> _elements, RevealProjectSvc.QaqcformdetailDTO _dto)
        {
            int i = 1; int j = 1;
            
            foreach (FrameworkElement _el in _elements)
            {
                if (_el.GetType() == typeof(C1DatePicker))
                {
                    if (j == 1) { FormSerialize.SetData(_el, _dto.DateValue1.ToString()); j++; continue;}
                    if (j == 2) { FormSerialize.SetData(_el, _dto.DateValue2.ToString()); j++; continue;}
                }

                if (_el.GetType() == typeof(ListView))
                {
                    ListView tmp = (ListView)_el;
                  //  tmp.DataContext = new WinAppLibrary.UI.ObjectNFCSign { PersonnelName = _dto.StringValue1, SignedTime = _dto.DateValue1.ToString() };
                    tmp.DataContext = new List<WinAppLibrary.UI.ObjectNFCSign>{
                        new WinAppLibrary.UI.ObjectNFCSign{
                        PersonnelName = _dto.StringValue1, SignedTime = _dto.DateValue1.ToString(),
                        isSigned = "Signed",
                        SignedColor = new SolidColorBrush(Windows.UI.Colors.YellowGreen)
                        }};
                }

                if (i == 1) { FormSerialize.SetData(_el, _dto.StringValue1 == null ? "" : _dto.StringValue1 .ToString()); i++; continue;}
                if (i == 2) { FormSerialize.SetData(_el, _dto.StringValue2 == null ? "" : _dto.StringValue2.ToString()); i++; continue; }
                if (i == 3) { FormSerialize.SetData(_el, _dto.StringValue3 == null ? "" : _dto.StringValue3.ToString()); i++; continue; }
                if (i == 4) { FormSerialize.SetData(_el, _dto.StringValue4 == null ? "" : _dto.StringValue4.ToString()); i++; continue; }
                if (i == 5) { FormSerialize.SetData(_el, _dto.StringValue5 == null ? "" : _dto.StringValue5.ToString()); i++; continue; }
                if (i == 6) { FormSerialize.SetData(_el, _dto.StringValue6 == null ? "" : _dto.StringValue6.ToString()); i++; continue; }
                if (i == 7) { FormSerialize.SetData(_el, _dto.StringValue7 == null ? "" : _dto.StringValue7.ToString()); i++; continue; }
                if (i == 8) { FormSerialize.SetData(_el, _dto.StringValue8 == null ? "" : _dto.StringValue8.ToString()); i++; continue; }
                if (i == 9) { FormSerialize.SetData(_el, _dto.StringValue9 == null ? "" : _dto.StringValue9.ToString()); i++; continue; }
                if (i == 10) { FormSerialize.SetData(_el, _dto.StringValue10 == null ? "" : _dto.StringValue10.ToString()); i++; continue; }
                if (i == 11) { FormSerialize.SetData(_el, _dto.StringValue11== null ? "" : _dto.StringValue11.ToString()); i++; continue; }
                if (i == 12) { FormSerialize.SetData(_el, _dto.StringValue12 == null ? "" : _dto.StringValue12.ToString()); i++; continue; }
                if (i == 13) { FormSerialize.SetData(_el, _dto.StringValue13 == null ? "" : _dto.StringValue13.ToString()); i++; continue; }
                if (i == 14) { FormSerialize.SetData(_el, _dto.StringValue14 == null ? "" : _dto.StringValue14.ToString()); i++; continue; }
                if (i == 15) { FormSerialize.SetData(_el, _dto.StringValue15 == null ? "" : _dto.StringValue15.ToString()); i++; continue; }
            }
        }

        public static void Load(List<List<List<FrameworkElement>>> _elements, List<RevealProjectSvc.QaqcformdetailDTO> _dtolist)
        {
            int j = 0; int m = 0;
            for (int i = 0; i < _dtolist.Count; i++)
            {
                if (_elements[j].Count <= m) { j++; m = 0; }
                SetLoadData(_elements[j][m], _dtolist[i]);
                m++;
            }
        }

        public static void SetData(FrameworkElement _element, string _data)
        {
            if (_element.GetType() == typeof(TextBox))
            {
                ((TextBox)_element).Text = _data;
            }
            else if (_element.GetType() == typeof(CheckBox))
            {
                ((CheckBox)_element).IsChecked = (_data == "Y") ? true : false;
            }
            else if (_element.GetType() == typeof(ComboBox))
            {
                if(((ComboBox)_element).Items.Count > 0 && _data != "")
                    ((ComboBox)_element).SelectedItem = _data;
            }
            else if (_element.GetType() == typeof(RadioButton))
            {
                ((RadioButton)_element).IsChecked = (_data == "Y") ? true : false;
            }
            else if (_element.GetType() == typeof(C1DatePicker))
            {
                ((C1DatePicker)_element).Text = _data;
            }
            else if (_element.GetType() == typeof(TextBlock))
            {
                ((TextBlock)_element).Text = _data;
            }
        }

        public static string GetUserData(FrameworkElement _element)
        {
            string value = "";

            if (_element.GetType() == typeof(TextBox))
            {
                value = ((TextBox)_element).Text;
            }
            else if (_element.GetType() == typeof(ComboBox))
            {
                value = (((ComboBox)_element).SelectedItem == null) ? "" : ((ComboBox)_element).SelectedItem.ToString();
            }
            else if (_element.GetType() == typeof(CheckBox))
            {
                value = (((CheckBox)_element).IsChecked == true) ? "Y" : "N";
            }
            else if (_element.GetType() == typeof(RadioButton))
            {
                value = (((RadioButton)_element).IsChecked == true) ? "Y" : "N";
            }
            else if (_element.GetType() == typeof(C1DatePicker))
            {
                value = ((C1DatePicker)_element).Text;
            }
            return value;
        }
    }
}
