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

using System.Reflection;
using Windows.UI.Input;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WinAppLibrary.ServiceModels;



using WinAppLibrary.Extensions;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input.Inking;
using Windows.UI;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Element.Reveal.Meg.Discipline.MTO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrawingViewerMarkup : WinAppLibrary.Controls.LayoutAwarePage
    {
        private int _projectid, _moduleid, category;

        Uri _imguri;
        private uint _penID;
        private uint _touchID;
        string _filename, _fileserver;
        public event RangeBaseValueChangedEventHandler SlideValueChanged;
        private bool _isopen = false;
        double ratio = 1;
        bool _editmode = false;
        private Point _previousContactPt;
        private Point _currentContactPt;
        private int _drawingmode = WinAppLibrary.Utilities.DrawingMode.Pen;
        public bool IsOpen
        {
            get { return _isopen; }
            set { _isopen = value; }
        }
        private Windows.UI.Color _brushcolor = Windows.UI.Colors.Red;
        public Windows.UI.Color BrushColor
        {
            get { return _brushcolor; }
            set { _brushcolor = value; }
        }

        private double _brushwidth = 1.0;
        public double BrushWidth
        {
            get { return _brushwidth; }
            set { _brushwidth = value; }
        }

        public double PenWidth
        {
            get { return slWidth.Value; }
            set { slWidth.Value = value; }
        }

        Lib.DrawingDataSource _drawingsource = new Lib.DrawingDataSource();
        Lib.UI.DrawingGrouping _drawinggrouping = new Lib.UI.DrawingGrouping();
        Lib.UI.DrawingInfo _drawinginfo = new Lib.UI.DrawingInfo();
        Lib.UI.DrawingSorting _drawingsort = new Lib.UI.DrawingSorting();
        WinAppLibrary.UI.StickyNote _stickynote = new WinAppLibrary.UI.StickyNote();

        Stack<UIElement> SuUndo = new Stack<UIElement>();
        Stack<UIElement> SuRedo = new Stack<UIElement>();
        private Dictionary<Windows.UI.Xaml.UIElement, WinAppLibrary.Utilities.ManipulationManager> _manipulationManager;

        InkManager MyInkManager = new InkManager();
        double X1, X2, Y1, Y2;
        Ellipse NewEllipse;
        Rectangle NewRectangle;


        Color BorderColor = Colors.Red;

        Point startingTranslation;
        double startingRotation;
        double startingSacle;

        public DrawingViewerMarkup()
        {
            this.InitializeComponent();
            this._manipulationManager = new Dictionary<UIElement, WinAppLibrary.Utilities.ManipulationManager>();
           // RenderConfiguration(cvDrawing, cvContainer);
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model 
            _projectid = Login.UserAccount.CurProjectID;
            _moduleid = Login.UserAccount.CurModuleID;
            UriKind uld = new UriKind();
            LoadDrawing("http://reveal.elementindustrial.com/Element.Reveal.Server.WS/IFC_Images/260KV/", "DD29D-P-0175-0001.jpg", uld);

            LoadingComponent();
            LoadOptionOnMode();
            
         //   StartRender();
        }

        private void LoadingComponent()
        {
            slWidth.Value = 2;
            var colors = typeof(Windows.UI.Colors).GetTypeInfo().DeclaredProperties;
            foreach (var item in colors)
            {
                cbPenColor.Items.Add(item);
            }

            cbPenColor.SelectionChanged += cbPenColor_SelectionChanged;
            IList<FontFamily> fontFamilies = new List<string>
            {
                "Arial",
                "Calibri",
                "Cambria",
                "Candara",
                "Consolas",
                "Corbel",
                "Courier New",
                "Georgia",
                "Gigi",
                "Jokerman",
                "Lucida Sans Unicode",
                "Magneto",
                "Precursor Alphabet",
                "Quartz MS",
                "Segoe UI",
                "Times New Roman",
                "Trebuchet MS",
            }.Select(s => new FontFamily(s)).ToList<FontFamily>();

            //     InitiateStoryBoard();
        }

        private async void LoadOptionOnMode()
        {
            var common = new Lib.ServiceModel.CommonModel();
            try
            {
                await _drawingsource.LoadOptionOnMode(_projectid, _moduleid);
                _drawingsort.BindInfo(GetSortingCategories());
                Loaddrawing();
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "LoadGroup", "There was an error load drawing. Pleae contact administrator", "Error!");
            }

            Login.MasterPage.SetBottomAppbar(WinAppLibrary.UI.TrueTaskMasterPage.BottomAppBarButton.Download, Visibility.Visible);
            //Login.MasterPage.AppbarClicked += MasterPage_AppbarClicked;
            Login.MasterPage.Loading(false, this);
        }

        private ObservableCollection<DataGroup> GetSortingCategories()
        {
            ObservableCollection<DataGroup> retValue = new ObservableCollection<DataGroup>();

            var group = new DataGroup("Sotring", "Sorting Drawing", "");
            retValue.Add(group);
            retValue[0].Items.Add(new DataItem("DrawingName", "DrawingName", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingPlantNo", "Plant Number", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingSubject", "Drawing Title", "", "", group));
            retValue[0].Items.Add(new DataItem("CWPName", "CWP", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingTypeLUID", "Drawing Type", "", "", group));
            retValue[0].Items.Add(new DataItem("DrawingNo", "Drawing Number", "", "", group));

            return retValue;
        }

        #region "Drawing"
        private async void Loaddrawing()
        {
            // Login.MasterPage.Loading(true, this);


            if (Login.LoginMode == WinAppLibrary.UI.LogMode.OnMode)
            {
                await _drawingsource.GetDrawingOnMode(_projectid,
                                _drawingsource.GroupList[Lib.HashKey.Key_CWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_FIWP].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawingsource.GroupList[Lib.HashKey.Key_DrawingType].Where(x => x.ParentID > 0).Select(x => x.DataID).ToList(),
                                _drawinggrouping.EngineerTag, _drawinggrouping.DrawingTitle,
                                _drawingsort.SelectedItem == null ? "" : (_drawingsort.SelectedItem as DataItem).UniqueId,
                    //  (int)DrawingSlider.Value);
                                1);
                //  await _drawingsource.GetDrawingOffMode(1);
            }
            else
            {
                //await _drawingsource.GetDrawingOffMode((int)DrawingSlider.Value);
                await _drawingsource.GetDrawingOffMode(1);
            }

            GrouingDrawing(3);

            GC.Collect();
            Login.MasterPage.Loading(false, this);
        }

        private void GrouingDrawing(int sortindex)
        {
            List<DataGroup> source = null;
            DataGroup datagroup = null;
            bool isshow = false;

            if (_drawingsource.DrawingPage != null && _drawingsource.DrawingPage.drawing != null)
            {
                switch (sortindex)
                {
                    //Group by CWP
                    case 0:
                        source = _drawingsource.DrawingPage.drawing.GroupBy(x => new { x.CWPName, x.CWPID }).Select(x => datagroup = new DataGroup(x.Key.CWPID.ToString(), x.Key.CWPName, "Assets/semantic_cwp.png")
                        {
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFilePath + y.DrawingFileURL, y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    //Group by 
                    case 1:
                        source = _drawingsource.DrawingPage.drawing.GroupBy(x => new { x.CWPName, x.CWPID }).Select(x => datagroup = new DataGroup(x.Key.CWPID.ToString(), x.Key.CWPName, "Assets/semantic_cwp.png")
                        {
                            Items = x.Select(y => new DataItem(y.DrawingID.ToString(), y.DrawingName, y.DrawingFileURL + y.DrawingFilePath, y.Description, datagroup) { }).ToObservableCollection()
                        }).ToList();
                        break;
                    default:
                        datagroup = new DataGroup("Drawing", "", "");
                        datagroup.Items = _drawingsource.DrawingPage.drawing.Select(x =>
                            new DataItem(x.DrawingID.ToString(), x.DrawingName, x.DrawingFilePath + x.DrawingFileURL, x.Description, datagroup) { }).ToObservableCollection();
                        source = new List<DataGroup>();
                        source.Add(datagroup);
                        break;
                }

                isshow = true;
            }
            else
                WinAppLibrary.Utilities.Helper.SimpleMessage("There were no drawing.", "Alert!");

            cvDrawingList.Source = source;
            /* this.DefaultViewModel["Drawings"] = source;
             this.DrawingSlider.Value = _drawingsource.DrawingPage.CurrentPage;
             this.DrawingSlider.Maxnumber = _drawingsource.DrawingPage.TotalPageCount;
             this.DrawingSlider.SwingShow(isshow);
             this.FlipView.SelectedItem = this.gvDrawing.SelectedItem = null;
             this.gvViewType.SelectedIndex = 0;*/


        }

        #endregion

        void cbPenColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var pro = e.AddedItems[0] as System.Reflection.PropertyInfo;
                this.BrushColor = (Windows.UI.Color)pro.GetValue(null);
            }
        }

        #region "Event Handler"

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.BrushWidth = e.NewValue;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DrawingViewer));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Frame.Navigate(typeof(DrawingViewer));
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            _drawingmode = WinAppLibrary.Utilities.DrawingMode.Rectangle;
            //   StartEditing();
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            _drawingmode = WinAppLibrary.Utilities.DrawingMode.Ellipse;
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            _drawingmode = WinAppLibrary.Utilities.DrawingMode.Pen;
            //    StartEditing();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (cvDraw.Children.Count > 0)
            {
                cvDraw.Children.RemoveAt(cvDraw.Children.Count - 1);
                SuRedo.Push(SuUndo.Pop());
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (cvDraw.Children.Count < SuUndo.Count + SuRedo.Count)
            {
                SuUndo.Push(SuRedo.Peek());
                cvDraw.Children.Add(SuRedo.Pop());
            }
        }

        #endregion

        public void StartEditing()
        {
            StopRender();

            cvDraw.PointerPressed += new PointerEventHandler(cvDrawing_PointerPressed);
            cvDraw.PointerMoved += new PointerEventHandler(cvDrawing_PointerMoved);
            cvDraw.PointerReleased += new PointerEventHandler(cvDrawing_PointerReleased);
            cvDraw.PointerExited += new PointerEventHandler(cvDrawing_PointerReleased);
            
           // _editmode = true;
            ClearBorder();           
        }

        private void ClearBorder()
        {
        //    transformTextBlock = false;
        }

        #region 포인터이동 관련
        Windows.UI.Xaml.Shapes.Path _Current;
        private void cvDrawing_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(cvDraw);
            _previousContactPt = pt.Position;

            if (!pt.Properties.IsLeftButtonPressed)
                return;

            switch (_drawingmode)
            {
                case WinAppLibrary.Utilities.DrawingMode.Pen:
                    ProcessPenDown(pt);
                    break;
                case WinAppLibrary.Utilities.DrawingMode.Rectangle:
                    
                    NewRectangle = new Rectangle();
                    X1 = e.GetCurrentPoint(cvDraw).Position.X;
                    Y1 = e.GetCurrentPoint(cvDraw).Position.Y;
                    X2 = X1;
                    Y2 = Y1;
                    NewRectangle.Width = X2 - X1;
                    NewRectangle.Height = Y2 - Y1;
                    NewRectangle.StrokeThickness = this._brushwidth;;
                    NewRectangle.Stroke = new SolidColorBrush(this._brushcolor);

                    SuRedo.Clear();
                    SuUndo.Push(NewRectangle);
                    cvDraw.Children.Add(NewRectangle);
                    break;
                case WinAppLibrary.Utilities.DrawingMode.Ellipse:

                    NewEllipse = new Ellipse();
                    X1 = e.GetCurrentPoint(cvDraw).Position.X;
                    Y1 = e.GetCurrentPoint(cvDraw).Position.Y;
                    X2 = X1;
                    Y2 = Y1;
                    NewEllipse.Width = X2 - X1;
                    NewEllipse.Height = Y2 - Y1;
                    NewEllipse.StrokeThickness = this._brushwidth;
                    NewEllipse.Stroke = new SolidColorBrush(this._brushcolor);
                
                    SuRedo.Clear();
                    SuUndo.Push(NewEllipse);
                    cvDraw.Children.Add(NewEllipse);                   

                    break;
                        
            }
            e.Handled = true;
            ClearBorder();
        }

        private void ProcessPenDown(PointerPoint pt)
        {
            // Pass the pointer information to the InkManager.
            //_inkManager.ProcessPointerDown(pt);

            //_penID = pt.PointerId;

            //hitapia - s
            if (!pt.Properties.IsLeftButtonPressed)
                return;

            _Current = new Windows.UI.Xaml.Shapes.Path
            {
                Data = new PathGeometry
                {
                    Figures = { 
                    new PathFigure { StartPoint = pt.Position, 
                    Segments = { new PolyLineSegment() } } }
                },
                Stroke = new SolidColorBrush(_brushcolor),
                StrokeThickness = this._brushwidth
            };
            SuRedo.Clear();
            SuUndo.Push(_Current);

            cvDraw.Children.Add(_Current);
            //hitapia - e
        }

        private void cvDrawing_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == _penID || _Current != null || e.GetCurrentPoint(cvDraw).Properties.IsLeftButtonPressed == true)
            {
                PointerPoint pt = e.GetCurrentPoint(cvDraw);

                switch (_drawingmode)
                {
                    case WinAppLibrary.Utilities.DrawingMode.Pen:
                        ProcessPenMoving(pt);
                        break;
                    case WinAppLibrary.Utilities.DrawingMode.Rectangle:
                        // Rectangle
                        if (e.GetCurrentPoint(cvDraw).Properties.IsLeftButtonPressed == true)
                        {
                            X2 = e.GetCurrentPoint(cvDraw).Position.X;
                            Y2 = e.GetCurrentPoint(cvDraw).Position.Y;
                            if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                NewRectangle.Margin = new Thickness(X1, Y1, X2, Y2);
                            else if ((X2 - X1) < 0)
                                NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                            else if ((Y2 - Y1) < 0)
                                NewRectangle.Margin = new Thickness(X1, Y2, X2, Y1);
                            else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                            NewRectangle.Width = Math.Abs(X2 - X1);
                            NewRectangle.Height = Math.Abs(Y2 - Y1);
                        }
                        break;
                    case WinAppLibrary.Utilities.DrawingMode.Ellipse :
                        if (e.GetCurrentPoint(cvDraw).Properties.IsLeftButtonPressed == true)
                        {
                            X2 = e.GetCurrentPoint(cvDraw).Position.X;
                            Y2 = e.GetCurrentPoint(cvDraw).Position.Y;
                            if ((X2 - X1) > 0 && (Y2 - Y1) > 0)
                                NewEllipse.Margin = new Thickness(X1, Y1, X2, Y2);
                            else if ((X2 - X1) < 0)
                                NewEllipse.Margin = new Thickness(X2, Y1, X1, Y2);
                            else if ((Y2 - Y1) < 0)
                                NewEllipse.Margin = new Thickness(X1, Y2, X2, Y1);
                            else if ((X2 - X1) < 0 && (Y2 - Y1) < 0)
                                NewRectangle.Margin = new Thickness(X2, Y1, X1, Y2);
                            NewEllipse.Width = Math.Abs(X2 - X1);
                            NewEllipse.Height = Math.Abs(Y2 - Y1);
                        }
                        break;
                    case WinAppLibrary.Utilities.DrawingMode.Text:
                        break;
                }
            }

            else if (e.Pointer.PointerId == _touchID)
            {
                // Process touch input
            }
        }

        private void ProcessPenMoving(PointerPoint pt)
        {
            // Render a red line on the canvas as the pointer moves. 
            // Distance() is an application-defined function that tests
            // whether the pointer has moved far enough to justify 
            // drawing a new line.

            /*
            _currentContactPt = pt.Position;
            _x1 = _previousContactPt.X;
            _y1 = _previousContactPt.Y;
            _x2 = _currentContactPt.X;
            _y2 = _currentContactPt.Y;

            if (Distance(_x1, _y1, _x2, _y2) > 2.0)
            {
                Windows.UI.Xaml.Shapes.Line line = new Windows.UI.Xaml.Shapes.Line()
                {
                    X1 = _x1,
                    Y1 = _y1,
                    X2 = _x2,
                    Y2 = _y2,
                    StrokeThickness = this._brushwidth,
                    Stroke = new SolidColorBrush(_brushcolor)
                };

                _previousContactPt = _currentContactPt;

                // Draw the line on the canvas by adding the Line object as
                // a child of the Canvas object.
                cvDrawing.Children.Add(line);

                // Pass the pointer information to the InkManager.
                //_inkManager.ProcessPointerUpdate(pt);
            }
            */

            //hitapia - s
            if (!pt.Properties.IsLeftButtonPressed || _Current == null)
                return;
            var segments = (_Current.Data as PathGeometry).Figures.First().Segments.First() as PolyLineSegment;
            segments.Points.Add(pt.Position);
            //hitapia - e 
        }

        private void cvDrawing_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (e.Pointer.PointerId == _penID)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(ImgDrawing);

                // Pass the pointer information to the InkManager. 
                //_inkManager.ProcessPointerUp(pt);
            }

            else if (e.Pointer.PointerId == _touchID)
            {
                // Process touch input
            }

            _touchID = 0;
            _penID = 0;

            // Call an application-defined function to render the ink strokes.


            e.Handled = true;
            */

            //hitapia - s 
            _Current = null;
            //hitapia - e 
        }
        #endregion

        #region 이미지 관련

        public async void LoadDrawing(string url, string filename, UriKind kind)
        {
            Loader.Show(this);
            bool failed = false;
            try
            {
                if (_fileserver != url || _filename != filename)
                    LoadImage(await GetImageSource(url, filename, kind));

                else
                    ResetManipulation();
            }
            catch (Exception e)
            {
                failed = true;
            }


            Loader.Hide(this);

        }

        private void LoadImage(WriteableBitmap source)
        {
            //   ResetManipulation();
            ImgDrawing.Source = null;
            ImgDrawing.Source = source;

            ratio = source.PixelWidth / grView.ActualWidth; //무조건 현재의 Layout의 해상도에서 그리기를 시작한다.

            ImgDrawing.Width = cvDrawing.Width = cvDraw.Width = grView.ActualWidth;
            ImgDrawing.Height = cvDrawing.Height = cvDraw.Height = grView.ActualHeight;



        }

        private void OnImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var image = sender as Windows.UI.Xaml.Controls.Image;
            var canvas = image.Parent as Windows.UI.Xaml.Controls.Canvas;
            var root = canvas.Parent as Windows.UI.Xaml.Controls.Canvas;
            ResetManipulation();
            //SetScale(ImgDrawing.ActualWidth > 0 ? LayoutRoot.ActualWidth / ImgDrawing.ActualWidth : 1,
            //        ImgDrawing.ActualHeight > 0 ? LayoutRoot.ActualHeight / ImgDrawing.ActualHeight : 1);
            Loader.Hide(this);
        }

        private async void ImgDrawing_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImgDrawing.Source = await GetImageSource(WinAppLibrary.Utilities.Helper.BaseUri + "Assets/", "Default.jpg", UriKind.Absolute);
            Loader.Hide(this);
        }

        private void ResetManipulation()
        {
            this._manipulationManager[cvDrawing].ResetManipulation();
        }

        async Task<WriteableBitmap> GetImageSource(string uri, string filename, UriKind kind)
        {
            try
            {
                _imguri = new Uri(uri + filename, kind);
                _filename = filename;
                _fileserver = uri;
                return await (new WinAppLibrary.Utilities.Helper()).GetWriteableBitmapFromUri(_imguri);
            }
            catch (Exception e)
            {
                (new WinAppLibrary.Utilities.Helper()).ExceptionHandler(e, "DrawingEditor GetImageSource");
                throw e;
            }
        }
        #endregion

        private void StopRender()
        {
            try
            {
                this._manipulationManager[cvDrawing].Configure(false, false, false, false);
            }
            catch { }
        }

        private void StartRender()
        {
            try
            {
                this._manipulationManager[cvDrawing].Configure(true, true, true, true);
            }
            catch { }
        }

        private void RenderConfiguration(FrameworkElement element, Canvas container)
        {
            element.RenderTransform = new Windows.UI.Xaml.Media.TransformGroup
            {
                Children = {                  
                    new Windows.UI.Xaml.Media.TranslateTransform(),
                    new Windows.UI.Xaml.Media.RotateTransform
                    {
                        CenterX = container.ActualWidth / 2,
                        CenterY = container.ActualHeight / 2
                    },
                    new Windows.UI.Xaml.Media.ScaleTransform
                    {
                        CenterX = container.ActualWidth / 2,
                        CenterY = container.ActualHeight / 2,
                        ScaleX = 1,
                        ScaleY = 1
                    },  
                }
            };

            // Create and configure manipulation manager for this image
            var manManager = new WinAppLibrary.Utilities.ManipulationManager(element, container);
            manManager.OnFilterManipulation = WinAppLibrary.Utilities.ManipulationFilter.RotateAboutCenter;
            manManager.Configure(true, true, true, true);
            element.IsHoldingEnabled = false;
            this._manipulationManager[element] = manManager;
        }

        private void btnMarkup_Click(object sender, RoutedEventArgs e)
        {
            if (_editmode)
            {
                _editmode = false;

             //   StartRender();
                cvDraw.PointerPressed -= new PointerEventHandler(cvDrawing_PointerPressed);
                cvDraw.PointerMoved -= new PointerEventHandler(cvDrawing_PointerMoved);
                cvDraw.PointerReleased -= new PointerEventHandler(cvDrawing_PointerReleased);
                cvDraw.PointerExited -= new PointerEventHandler(cvDrawing_PointerReleased);

                cvContainer.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All;
                cvContainer.ManipulationDelta += cvContainer_ManipulationDelta;
                cvContainer.ManipulationStarted += cvContainer_ManipulationStarted;
            }
            else
            {
                _editmode = true;
                StartEditing();

                cvContainer.ManipulationDelta -= cvContainer_ManipulationDelta;
                cvContainer.ManipulationStarted -= cvContainer_ManipulationStarted;
            }
        }

        void cvContainer_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            startingTranslation = new Point { X = transform.TranslateX, Y = transform.TranslateY };
            startingRotation = transform.Rotation;
            startingSacle = transform.ScaleX;
        }

        void cvContainer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            transform.TranslateX = startingTranslation.X + e.Cumulative.Translation.X;
            transform.TranslateY = startingTranslation.Y + e.Cumulative.Translation.Y;

            transform.Rotation = startingRotation + e.Cumulative.Rotation;
            transform.ScaleX = transform.ScaleY = startingSacle * e.Cumulative.Scale;
        }

       

        

       
    }
}
