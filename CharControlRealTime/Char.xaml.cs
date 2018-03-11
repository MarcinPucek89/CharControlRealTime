using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CharControlRealTime
{
    /// <summary>
    /// Logika interakcji dla klasy Char.xaml
    /// </summary>
    public partial class Char : UserControl, INotifyPropertyChanged
    {
        public Char()
        {
            InitializeComponent();

            UpdateChar +=  Char_update;
        }

        #region Options Control 
        public static readonly DependencyProperty _nameAxisX        = DependencyProperty.Register("NameAxisX",        typeof(string),    typeof(Char), new FrameworkPropertyMetadata("Name AxisX"));
        public static readonly DependencyProperty _nameAxisY        = DependencyProperty.Register("NameAxisY",        typeof(string),    typeof(Char), new FrameworkPropertyMetadata("Name AxisY"));
        public static readonly DependencyProperty _MaxValueX        = DependencyProperty.Register("_nameMaxX",        typeof(Double),    typeof(Char), new FrameworkPropertyMetadata(10.0));
        public static readonly DependencyProperty _MinValueX        = DependencyProperty.Register("_nameMinX",        typeof(Double),    typeof(Char), new FrameworkPropertyMetadata(0.0));
        public static readonly DependencyProperty _MaxValueY        = DependencyProperty.Register("_nameMaxY",        typeof(Double),    typeof(Char), new FrameworkPropertyMetadata(10.0));
        public static readonly DependencyProperty _MinValueY        = DependencyProperty.Register("_nameMinY",        typeof(Double),    typeof(Char), new FrameworkPropertyMetadata(0.0));
        public static readonly DependencyProperty _MaxPoint         = DependencyProperty.Register("_maxPointFIFO",    typeof(int),       typeof(Char), new FrameworkPropertyMetadata(10000));
        public static readonly DependencyProperty _PaddingDrawLine  = DependencyProperty.Register("_paddingDrawLine", typeof(Thickness), typeof(Char), new FrameworkPropertyMetadata(new Thickness(0.5, 1,0.5,1)));
        public static readonly DependencyProperty _ShowToolTipChar  = DependencyProperty.Register("_showToolTipChar", typeof(Boolean),   typeof(Char), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty _ColorChar        = DependencyProperty.Register("_colorChar",       typeof(Brush),     typeof(Char),   new FrameworkPropertyMetadata(Brushes.Blue));
        public static readonly DependencyProperty _StrokeThicknessChar = DependencyProperty.Register("_strokeThicknessChar", typeof(Double), typeof(Char), new FrameworkPropertyMetadata(1.0));
        public static readonly DependencyProperty _ScaleAxisRealTime = DependencyProperty.Register("_scaleAxisRealTime", typeof(bool), typeof(Char), new FrameworkPropertyMetadata(true));

        [Category("Preferences")]
        public bool ScaleAxisRealTime
        {
            get { return (bool)GetValue(_ScaleAxisRealTime); }
            set { SetValue(_ScaleAxisRealTime, value); UpdateChar?.Invoke("ScaleAxisRealTime"); }
        }

        [Category("Preferences")]
        public Double StrokeThicknessChar
        {
            get { return (Double)GetValue(_StrokeThicknessChar); }
            set { SetValue(_StrokeThicknessChar, value); UpdateChar?.Invoke("StrokeThicknessChar"); }
        }

        [Category("Preferences")]
        public Brush ColorChar
        {
            get { return (Brush)GetValue(_ColorChar); }
            set { SetValue(_ColorChar, value);  UpdateChar?.Invoke("ColorChar"); }
        }

        [Category("Preferences")]
        public Boolean ShowToolTipChar
        {
            get { return (Boolean)GetValue(_ShowToolTipChar); }
            set { SetValue(_ShowToolTipChar, value);  UpdateChar?.Invoke("ShowToolTipChar"); }
        }

        [Category("Preferences")]
        public Thickness PaddingDrawLine
        {
            get { return (Thickness)GetValue(_PaddingDrawLine); }
            set { SetValue(_PaddingDrawLine, value); UpdateChar?.Invoke("PaddingDrawLine"); }
        }

        [Category("Preferences")] 
        public int MaxPoint
        {
            get { return (int)GetValue(_MaxPoint); }
            set { SetValue(_MaxPoint, value); UpdateChar?.Invoke("MaxPoint"); }
        }

        [Category("Preferences")] 
        public Double Maximum_Y
        {
            get { return (Double)GetValue(_MaxValueY); }
            set { SetValue(_MaxValueY, value); UpdateChar?.Invoke("Maximum_Y"); }
        }

        [Category("Preferences")]
        public Double Minimum_Y
        {
            get { return (Double)GetValue(_MinValueY); }
            set { SetValue(_MinValueY, value); UpdateChar?.Invoke("Minimum_Y"); }
        }

        [Category("Preferences")] 
        public Double Maximum_X
        {
            get { return (Double)GetValue(_MaxValueX); }
            set { SetValue(_MaxValueX, value); UpdateChar?.Invoke("Maximum_X"); }
        }

        [Category("Preferences")]
        public Double Minimum_X
        {
            get { return (Double)GetValue(_MinValueX); }
            set { SetValue(_MinValueX, value); UpdateChar?.Invoke("Minimum_X"); }
        }

        [Category("Preferences")] 
        public string NameAxisX
        {
            get { return GetValue(_nameAxisX).ToString(); }
            set { SetValue(_nameAxisX, value); UpdateChar?.Invoke("NameAxisX"); }
        }

        [Category("Preferences")]
        public string NameAxisY
        {
            get { return GetValue(_nameAxisY).ToString(); }
            set { SetValue(_nameAxisY, value); UpdateChar?.Invoke("NameAxisY"); }
        }
        #endregion

        #region Variables

        private double widthChar;
        private double heightChar;
        private double PositionLineX_0;
        private double scalaX;
        private double scalaY;
        private double valueOsY;
        private double valueOsX;
        private double ChangeScaleAxisX, ChangeScaleAxisY;
        private bool click = false;
        private double X_0 = 0.0;
        private double Y_0 = 0.0;
        private bool mouseIsDown = false;
        private bool mouseIsUP = false;
        private bool changePositionToolbar = false;
        private double x1 = 0;
        private double y1 = 0;
        private double x2 = 0;
        private double y2 = 0;

        private RectangleGeometry rectangleGeometry = new RectangleGeometry();
        private Path path = new Path();
        private Line lineHorizontal, lineVertical;
        private Label value;
        private Polyline border_Char_Polyline;
        private ZoomAxisXY zoomAxisXY   = new ZoomAxisXY();
        private Polyline lineChar;
        private Queue<AxisXY> _Point    = new Queue<AxisXY>();
        private PositionXY AxisPosition = new PositionXY();
        private ContextMenu contextMenu = new ContextMenu();
        private DoubleCollection Dash   = new DoubleCollection() { 8, 10, };

        private const int WidthValueAxisX       = 35;
        private const int HeightValueAxisY      = 30;
        private const int heightToolBar         = 22;
        private DoubleAnimation doubleAnimation = new DoubleAnimation();
        private bool isCompleted                = false;
        private double ActualMarginesY;
        private double ActualMarginesX;
        private double ActualPositionX;
        private double ActualPositionY;
        private double ReadPositionX;
        private double ReadPositionY;
        private double tmp_actux;
        private double tmp_actuy;
        private double _current_MaxValue_Y;
        private double _current_MinValue_Y;
        private double _current_MaxValue_X;
        private double _current_MinValue_X;
        private double _temp_valueY = 10;
        private Label toolTipLabel = new Label();
        public event PropertyChangedEventHandler PropertyChanged;
        private Visibility _visibilityColorLegend;
        private Brush _colorCharLegend;

        public string LegendaTextBinding
        {
            get { return _legendaTextBinding; }
            set { _legendaTextBinding = value; OnPropertyChanged("LegendaTextBinding"); }
        }
        
        public Brush ColorCharLegend
        {
            get { return _colorCharLegend; }
            set { _colorCharLegend = value; OnPropertyChanged("ColorCharLegend"); }
        }
        
        public Visibility VisibilityColorLegend
        {
            get { return _visibilityColorLegend; }
            set
            {
                _visibilityColorLegend = value;
                OnPropertyChanged("VisibilityColorLegend");
            }
        }
        private string _legendaTextBinding;

        public delegate void UpdateCHarRealTime(string nameChangeProperties);
        private event UpdateCHarRealTime UpdateChar;

        private Double Copy_StrokeThicknessChar;
        private Brush Copy_ColorChar;
        private Boolean Copy_ShowToolTipChar;
        private Thickness Copy_PaddingDrawLine;
        private int Copy_MaxPoint;

        #endregion


        private void LoadToolTipCHar()
        {
            if (toolTipLabel == null) return;

            toolTipLabel.Background  = Brushes.Gray;
            toolTipLabel.BorderBrush = Brushes.Black;
            toolTipLabel.Foreground  = Brushes.White;
            toolTipLabel.BorderThickness = new Thickness(1);

            toolTipCanva.Children.Add(toolTipLabel);

            Panel.SetZIndex(toolTipCanva, 2);

            DeleteToolTipLabel();
        }

        private void DeleteToolTipLabel()
        {
            if (toolTipLabel == null) return;

            toolTipLabel.Visibility = Visibility.Hidden;
        }

        public void AddPoint(double pointX, double pointY)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new System.Action(() => {
                if (double.IsInfinity(pointX) || double.IsInfinity(pointY)) throw new AggregateException("Object x or y is Infinity");
                if (double.IsNaN(pointX) || double.IsNaN(pointY)) throw new AggregateException("Object x or y is NaN");

                if (lineChar == null) AddNewPolyline();

                using (AxisXY axisXY = new AxisXY())
                {
                    axisXY.Axis_X = pointX;
                    axisXY.Axis_Y = pointY;

                    _Point.Enqueue(axisXY);

                    if (pointY > _current_MaxValue_Y) _current_MaxValue_Y = pointY;
                    if (pointY < _current_MinValue_Y) _current_MinValue_Y = pointY;
                    if (pointX > _current_MaxValue_X) _current_MaxValue_X = pointX;
                    if (pointX < _current_MinValue_X) _current_MinValue_X = pointX;

                    //if the size of the buffer exceeds the declared MaxPoint size
                    if (_Point.Count >= Copy_MaxPoint && lineChar.Points.Count > 0)
                    {
                        //remove the first item from the queue and from the graph line 
                        _Point.Dequeue();
                        lineChar.Points.RemoveAt(0);

                        //if the graph is scaled in real time
                        if (ScaleAxisRealTime && _temp_valueY > pointY)
                        {
                            //data update after overflow of the determined buffer
                            _current_MaxValue_Y = _Point.Max().Axis_Y;
                            _current_MinValue_Y = _Point.Min().Axis_Y;
                            _current_MinValue_X = _Point.FirstOrDefault().Axis_X;
                            _current_MaxValue_X = pointX;
                        }
                    }

                    //if the graph is scaled in real time
                    if (ScaleAxisRealTime)
                    {
                        //if the chart value exceeds the maximum X or Y axis range
                        if (pointY >= _current_MaxValue_Y || pointY <= _current_MinValue_Y || pointX >= _current_MaxValue_X || pointX <= _current_MinValue_X)
                        {
                            Delete();
                            DrawLineAndValue(_current_MaxValue_X + Copy_PaddingDrawLine.Right, _current_MaxValue_Y + Copy_PaddingDrawLine.Top, _current_MinValue_X - Copy_PaddingDrawLine.Left, _current_MinValue_Y - Copy_PaddingDrawLine.Bottom);

                            AddNewPolyline();

                            foreach (var v in _Point)
                                DravPolyline(v.Axis_X, v.Axis_Y, _current_MaxValue_X, _current_MaxValue_Y);
                        }
                        else DravPolyline(axisXY.Axis_X, axisXY.Axis_Y);
                    }
                    else DravPolyline(axisXY.Axis_X, axisXY.Axis_Y);

                    CharDrawe.Children.Clear();
                    CharDrawe.Children.Add(lineChar);
                }
            }));
        }

        private void AddNewPolyline()
        {
            if(lineChar != null) lineChar.MouseMove -= LineChar_MouseMove;

            lineChar = new Polyline()
            {
                Stroke = Copy_ColorChar,
                StrokeThickness = Copy_StrokeThicknessChar
            };
            lineChar.MouseMove += LineChar_MouseMove;
        }

        private void DrawBorderChar()
        {
            border_Char_Polyline = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            //reading current Canvas dimensions
            widthChar = CharDrawe.ActualWidth;
            heightChar = CharDrawe.ActualHeight;
            PositionLineX_0 = 0;

            //line Y
            //vertical left line
            border_Char_Polyline.Points.Add(new Point(PositionLineX_0, PositionLineX_0));
            border_Char_Polyline.Points.Add(new Point(PositionLineX_0, heightChar));
            //line X
            //horizontal bottom line
            border_Char_Polyline.Points.Add(new Point(widthChar, heightChar));
            //line x right
            border_Char_Polyline.Points.Add(new Point(widthChar, PositionLineX_0));
            //line y top
            border_Char_Polyline.Points.Add(new Point(PositionLineX_0, PositionLineX_0));
            Chart_Border.Children.Add(border_Char_Polyline);
        }

        private void DrawLineAndValue(double x, double y, double odX = 0.0, double odY = 0.0)
        {
            if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(odX) || double.IsNaN(odY)) return;
            if (double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(odX) || double.IsInfinity(odY)) return;

            Delete();

            double _RangeX = x;
            double _RangeY = y;

            double StrokeThicknessLineY = 0.5;
            double StrokeThicknessLineX = 0.5;

            widthChar = CharDrawe.ActualWidth;
            heightChar = CharDrawe.ActualHeight;

            //calculating the scale from to and dividing it by range
            double zakresX = Math.Abs(_RangeX - odX);

            //checking the correctness of division - dividing by zero
            if (zakresX > 0) valueOsY = widthChar / zakresX;
            else valueOsY = widthChar;

            //calculating the scale from to and dividing it by range
            double zakresY = Math.Abs((_RangeY - odY));

            //checking the correctness of division - dividing by zero
            if (zakresY > 0) valueOsX = heightChar /  zakresY;
            else valueOsX = heightChar;

            const int width_X  = 60;
            const int height_X = 25;
            const int width_Y  = 60;
            const int height_Y = 25;

            //exchange of range
            if (_RangeX < odX){
                double Tmp_odX = odX;

                odX = _RangeX;
                _RangeX = Tmp_odX;
            }

            if (_RangeY < odY){
                double Tmp_odY = odY;

                odY = _RangeY;
                _RangeY = Tmp_odY;
            }

            //saving the axis range
            ChangeScaleAxisX = odX;
            ChangeScaleAxisY = odY;

            //counting the scale
            double temp_scalaX = 10;
            double temp_scalaY = 10;

            scalaY = zakresY / temp_scalaY;
            scalaX = zakresX / temp_scalaX;

            if (scalaX != 0){
                double positionLineY = 0;

                for (double i = odX; i <= _RangeX; i += scalaX){
                    //calculation of the jump and beginning of the axis from the point from X => deducting it from the variable and the so-called zeroing and division by scale
                    positionLineY = (valueOsY * (i - odX));

                    if (i > odX && i < _RangeX){
                        lineVertical = new Line(){
                            Stroke = Brushes.Gray,
                            StrokeDashArray = Dash,
                            StrokeThickness = StrokeThicknessLineX,

                            X1 = positionLineY,
                            Y1 = PositionLineX_0,
                            X2 = positionLineY,
                            Y2 = heightChar
                        };
                        LineDrawe.Children.Add(lineVertical);
                    }

                    //displaying results with an accuracy of 3 decimal places
                    double temp_i = Math.Round(i, 3);

                    value = new Label(){
                        Height = height_X,
                        Width = width_X,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        FontSize = 14,
                        FontFamily = new FontFamily("Arial"),
                        Content = temp_i.ToString(),
                       
                    };

                    double tmp_widthLabel =  value.Content.ToString().Trim().Length;

                    //centering the numbers displayed on the X axis
                    if (tmp_widthLabel <= 1) tmp_widthLabel = value.Width / 2 - 20;
                    else if (tmp_widthLabel == 2 || tmp_widthLabel == 3) tmp_widthLabel = value.Width / 2 - 15;
                    else if (tmp_widthLabel == 4) tmp_widthLabel = value.Width / 2 - 10;
                    else if (tmp_widthLabel > 4 && tmp_widthLabel <= 5) tmp_widthLabel = value.Width / 2 - 8;
                    else if (tmp_widthLabel > 5 && tmp_widthLabel <= 7) tmp_widthLabel = value.Width / 2 ;
                    
                    value.Margin = new Thickness(positionLineY - tmp_widthLabel, 0, 0, 0);

                    value_x.Children.Add(value);
                }
            }

            if (scalaY != 0){
                double positionLineX = 0;

                for (double i = odY; i <= _RangeY; i += scalaY){
                    //calculation of the jump and beginning of the axis from the point from Y => deducting it from the variable and the so-called zeroing
                    positionLineX = heightChar - ((valueOsX) * ((i - odY)));

                    if (i > odY && i < _RangeY){
                        lineHorizontal = new Line(){
                            Stroke = Brushes.Gray,
                            StrokeDashArray = Dash,
                            StrokeThickness = StrokeThicknessLineY,

                            X1 = PositionLineX_0,
                            Y1 = positionLineX,
                            X2 = widthChar,
                            Y2 = positionLineX
                        };

                        LineDrawe.Children.Add(lineHorizontal);
                    }

                    value = new Label(){
                        Height = height_Y,
                        Width = width_Y,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        FontSize = 14,
                        FontFamily = new FontFamily("Arial")
                    };

                    //displaying results with an accuracy of 3 decimal places
                    double temp_i = Math.Round(i, 3);

                    //if y = x ban on the chart
                    if (i != odX){
                        value.Content = temp_i.ToString();
                        value.Margin = new Thickness(-7, positionLineX - 12, 0, 0);

                        value_y.Children.Add(value);
                    }
                }
            }
        }

        private void DravPolyline(double x, double y, double maxValueX = 0, double maxValueY = 0)
        {
            if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(maxValueX) || double.IsNaN(maxValueY)) return;
            if (double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(maxValueX) || double.IsInfinity(maxValueY)) return;

            if (maxValueX == 0 || maxValueY == 0){
                maxValueY = _current_MaxValue_Y;
                maxValueX = _current_MaxValue_X;
            }

            double copyX = x;
            double copyY = y;

            using (MathXY mathPoint = new MathXY())
            {
                AxisPosition = mathPoint.GetAxisPosition(copyX, copyY, valueOsX, valueOsY, heightChar, ChangeScaleAxisX, ChangeScaleAxisY);

                if (AxisPosition != null)
                    lineChar.Points.Add(new Point(AxisPosition.PositionX, AxisPosition.PositionY));

                AxisPosition.Dispose();
            }
        }

        private void LineChar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Copy_ShowToolTipChar) return;

            double pointX = e.GetPosition(this.CharDrawe).X;
            double pointY = e.GetPosition(this.CharDrawe).Y;

            const int digitsx = 2, digitsy = 2;

            using (MathXY mathPoint = new MathXY())
            {
                var c = mathPoint.GetPointAB(pointX, pointY, 0, 0, heightChar, valueOsX, valueOsY, scalaX, scalaY, ChangeScaleAxisX, ChangeScaleAxisY);

                toolTipLabel.Visibility = Visibility.Visible;
                toolTipLabel.Content = string.Format("{0} ; {1}", Math.Round(c.Axis_AX, digitsx).ToString(), Math.Round(c.Axis_AY, digitsy).ToString());
                toolTipLabel.Margin = new Thickness(pointX + 5, pointY - 30, 0, 0);
            }
        }

        public void AddLegend(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            LegendaTextBinding    = text;
            VisibilityColorLegend = Visibility.Visible;
            Legenda.Visibility    = Visibility.Visible;
        }

        private void LoadLegend()
        {
            //show a square about the color of the chart
            if (string.IsNullOrEmpty(LegendaTextBinding))
            {
                VisibilityColorLegend = Visibility.Hidden;
                Legenda.Visibility    = Visibility.Hidden;
            }
            else
            {
                VisibilityColorLegend = Visibility.Visible;
                Legenda.Visibility    = Visibility.Visible;
            }

            ColorCharLegend = Copy_ColorChar;
        }

        #region Event

        private void CharControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadToolTipCHar();
            LoadLegend();
            DrawBorderChar();

            DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y);

            CharLegend.Children.Add(path);

            //menu initialization after clicking the right mouse button
            MenuItem menuReset = new MenuItem();
            MenuItem menuScale = new MenuItem();
            menuReset.Header = "Reset";
            menuScale.Header = "Scale";

            menuReset.Click += MenuReset_Click;
            menuScale.Click += MenuScale_Click;

            //adding menuItem to the context menu
            contextMenu.Items.Add(menuReset);
            contextMenu.Items.Add(menuScale);
        }

        private void MenuScale_Click(object sender, RoutedEventArgs e)
        {
            if (_Point.Count == 0) return;

            _current_MinValue_X = _Point.FirstOrDefault().Axis_X;
            _current_MaxValue_X = _Point.LastOrDefault().Axis_X;
            _current_MinValue_Y = _Point.Min().Axis_Y;
            _current_MaxValue_Y = _Point.Max().Axis_Y;

            //increase the range to improve readability by the value provided by the user in PaddingDrawLine
            DrawLineAndValue(_current_MaxValue_X + Copy_PaddingDrawLine.Right, _current_MaxValue_Y + Copy_PaddingDrawLine.Top, _current_MinValue_X - Copy_PaddingDrawLine.Left, _current_MinValue_Y - Copy_PaddingDrawLine.Bottom);

            AddNewPolyline();

            foreach (var v in _Point)
                DravPolyline(v.Axis_X, v.Axis_Y, _current_MaxValue_Y, _current_MaxValue_Y);

            CharDrawe.Children.Add(lineChar);
        }

        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            Delete();
            DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y);
            _Point.Clear();
        }

        private void CharLegend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (click) return;

            double pointX = e.GetPosition(this.CharDrawe).X;
            double pointY = e.GetPosition(this.CharDrawe).Y;

            X_0 = pointX;
            Y_0 = pointY;

            mouseIsDown = true;
            mouseIsUP   = false;
        }

        private void CharLegend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if the toolbar is moved - do not scale the graph
            if (changePositionToolbar) return;

            mouseIsDown = false;
            mouseIsUP = true;

            //reading the current cursor position ie the current "under the cursor" item
            double pointX = e.GetPosition(this.CharDrawe).X;
            double pointY = e.GetPosition(this.CharDrawe).Y;

            double Axis_A_X = 0;
            double Axis_A_Y = 0;
            double Axis_B_X = 0;
            double Axis_B_Y = 0;

            if (pointX == x1 || pointY == y1){
                //description if selecting the data on the chart is different than from the left corner to the right corner
                Axis_A_X = x1; Axis_A_Y = y1;
                Axis_B_X = x1 + x2; Axis_B_Y = y1 + y2;
            }
            else{
                Axis_A_X = x1; Axis_A_Y = y1;
                Axis_B_X = pointX; Axis_B_Y = pointY;
            }

            using (MathXY mathPoint = new MathXY()){
                zoomAxisXY = mathPoint.GetPointAB(Axis_A_X, Axis_A_Y, Axis_B_X, Axis_B_Y,  heightChar, valueOsX, valueOsY, scalaX, scalaY, ChangeScaleAxisX, ChangeScaleAxisY);

                if (zoomAxisXY == null) return;

                Axis_A_X = zoomAxisXY.Axis_AX;
                Axis_A_Y = zoomAxisXY.Axis_AY;
                Axis_B_X = zoomAxisXY.Axis_BX;
                Axis_B_Y = zoomAxisXY.Axis_BY;

                //change of scope
                //the function is repeated 2x ==> DrawLineAndValue
                if (Axis_B_X < Axis_A_X){
                    double Tmp_odX = Axis_A_X;

                    Axis_A_X = Axis_B_X;
                    Axis_B_X = Tmp_odX;
                }

                if (Axis_A_Y < Axis_B_Y){
                    double Tmp_odY = Axis_B_Y;

                    Axis_B_Y = Axis_A_Y;
                    Axis_A_Y = Tmp_odY;
                }

                //deleting the graph, i.e. the values of X and Y and the co-equivalents corresponding to them
                Delete();
                //drawing a new chart
                DrawLineAndValue(Axis_B_X, Axis_A_Y, Axis_A_X, Axis_B_Y);
            }

            //if a graph is plotted on the graph
            if (_Point.Count != 0){
                AddNewPolyline();

                //scaling the chart after all points
                foreach (var v in _Point)
                    DravPolyline(v.Axis_X, v.Axis_Y);

                CharDrawe.Children.Add(lineChar);
            }

            //removing a square from the chart
            rectangleGeometry.Rect = new Rect(0, 0, 0, 0);
        }

        private void Delete()
        {
            if (scalaX != 0) value_x.Children.Clear();
            if (scalaY != 0) value_y.Children.Clear();

            CharDrawe.Children.Clear();
            LineDrawe.Children.Clear();

            GC.SuppressFinalize(this);
        }

        private void DeleteBorderChar()
        {
            Chart_Border.Children.Clear();
            GC.SuppressFinalize(this);
        }

        private void CharLegend_MouseMove(object sender, MouseEventArgs e)
        {
            DeleteToolTipLabel();

            double pointX = e.GetPosition(this.CharDrawe).X;
            double pointY = e.GetPosition(this.CharDrawe).Y;

            // legend to the chart
            double positionLegendLeft = Legenda.Margin.Left;
            double positionLegendTop  = Legenda.Margin.Top;
            double widthLegend        = Legenda.ActualWidth + positionLegendLeft;
            double heightLegend       = heightToolBar + positionLegendTop;
            
            path.Fill             = Brushes.Transparent;
            path.Stroke           = Brushes.Black;
            path.StrokeThickness  = 0.5;
            path.Data             = rectangleGeometry;

            // determination of the range from smaller to larger X
            positionLegendLeft = positionLegendLeft >= widthLegend ? widthLegend : positionLegendLeft;
             widthLegend        = positionLegendLeft <= widthLegend ? widthLegend : positionLegendLeft;

            //determination of the range from smaller to larger Y
            positionLegendTop = positionLegendTop >= heightLegend ? heightLegend : positionLegendTop;
             heightLegend       = positionLegendTop <= heightLegend ? heightLegend : positionLegendTop;

            //If the mouse button is pressed and checking ranges, the mouse cursor is not in the legend toolbar area
            if (mouseIsDown && !mouseIsUP && !(pointX >= positionLegendLeft && widthLegend >= pointX && pointY >= positionLegendTop && heightLegend >= pointY))
            {
                changePositionToolbar = false;

                double widthRec = 0.0;
                double heightRec = 0.0;

                double actualX = 0.0;
                double actualY = 0.0;

                //positive height and width value
                widthRec = Math.Abs(pointX - X_0);
                heightRec = Math.Abs(pointY - Y_0);

                //determination of the values of parameters x1, x2, y1, y2
                //drawing squares in all directions
                if (X_0 < pointX && Y_0 < pointY)
                {
                    x1 = X_0; y1 = Y_0; x2 = widthRec; y2 = heightRec;
                    rectangleGeometry.Rect = new Rect(x1, y1, x2, y2);
                }
                if (X_0 > pointX)
                {
                    x1 = X_0 - widthRec; y1 = Y_0; x2 = widthRec; y2 = heightRec;
                    rectangleGeometry.Rect = new Rect(x1, y1, x2, y2);
                }
                if (Y_0 > pointY)
                {
                    x1 = X_0; y1 = Y_0 - heightRec; x2 = widthRec; y2 = heightRec;
                    rectangleGeometry.Rect = new Rect(x1, y1, x2, y2);
                }
                if (X_0 > pointX && Y_0 > pointY)
                {
                    x1 = X_0 - widthRec; y1 = Y_0 - heightRec; x2 = widthRec; y2 = heightRec;
                    rectangleGeometry.Rect = new Rect(x1, y1, x2, y2);
                }

                actualX = pointX;
                actualY = pointY;
            }
            else changePositionToolbar = true;
        }

        private void Legenda_MouseMove(object sender, MouseEventArgs e)
        {
            double positionMouseX = e.GetPosition(this.CharDrawe).X;
            double positionMouseY = e.GetPosition(this.CharDrawe).Y;

            doubleAnimation.Duration   = new Duration(TimeSpan.FromSeconds(1));
            doubleAnimation.Completed += DoubleAnimation_Completed;

            if (positionMouseY - heightToolBar <= Legenda.Margin.Top && TitleBarLegend.Height <= 2)
            {
                doubleAnimation.From = 0.0;
                doubleAnimation.To = 20;
                TitleBarLegend.BeginAnimation(StackPanel.HeightProperty, doubleAnimation);
                doubleAnimation.AutoReverse = false;
            }
            else if (TitleBarLegend.Height >= 3 && positionMouseY - heightToolBar >= Legenda.Margin.Top && isCompleted)
            {
                isCompleted = false;
                doubleAnimation.From = 20.0;
                doubleAnimation.To = 0;
                TitleBarLegend.BeginAnimation(StackPanel.HeightProperty, doubleAnimation);
                doubleAnimation.AutoReverse = false;
            }
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e) { isCompleted = true; }

        private void TitleBarLegend_MouseMove(object sender, MouseEventArgs e){
            if (click)
            {
                ReadPositionX = e.GetPosition(this.CharDrawe).X;
                ReadPositionY = e.GetPosition(this.CharDrawe).Y;

                double ActualPositionXX = ReadPositionX - ActualPositionX;
                double ActualPositionYY = ReadPositionY - ActualPositionY;

                tmp_actux = ActualMarginesX + ActualPositionXX;
                tmp_actuy = ActualMarginesY + ActualPositionYY;

                Legenda.Margin = new Thickness(tmp_actux, tmp_actuy, 0, 0);
            }
        }

        private void TitleBarLegend_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Panel.SetZIndex(CharLegend,3);

            click = true;

            ActualMarginesY = Legenda.Margin.Top;
            ActualMarginesX = Legenda.Margin.Left;

            ActualPositionX = e.GetPosition(this.CharDrawe).X;
            ActualPositionY = e.GetPosition(this.CharDrawe).Y;
        }

        private void TitleBarLegend_MouseUp(object sender, MouseButtonEventArgs e){ click = false; Panel.SetZIndex(CharLegend, 0); }

        private void Char_update(string nameChangeProperties)
        {
            _current_MinValue_X = Minimum_X;
            _current_MaxValue_X = Maximum_X;
            _current_MaxValue_Y = Maximum_Y;
            _current_MinValue_Y = Minimum_Y;

            switch (nameChangeProperties)
            {
                case "Maximum_Y": { Delete(); DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y); } break;
                case "Minimum_Y": { Delete(); DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y); } break;
                case "Maximum_X": { Delete(); DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y); } break;
                case "Minimum_X": { Delete(); DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y); } break;
                default:
                    {
                        Copy_StrokeThicknessChar    = StrokeThicknessChar;
                        Copy_ColorChar              = ColorChar;
                        Copy_ShowToolTipChar        = ShowToolTipChar;
                        Copy_PaddingDrawLine        = PaddingDrawLine;
                        Copy_MaxPoint               = MaxPoint;
                    }
                    break;
            }
        }

        private void CharControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Delete();
            DeleteBorderChar();
            DrawBorderChar();

            DrawLineAndValue(_current_MaxValue_X, _current_MaxValue_Y, _current_MinValue_X, _current_MinValue_Y);
        }

        private void CharLegend_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //assigning the menu context to the canvas
            CharLegend.ContextMenu = contextMenu;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}
