using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsPos
{
    public class CustomLineGrid : Grid
    {
        public enum GridLinesVisibilityEnum
        {
            Both,
            Vertical,
            Horizontal,
            None
        }

        public static readonly DependencyProperty ShowCustomGridLinesProperty = 
            DependencyProperty.Register("ShowCustomGridLines", typeof(bool), typeof(CustomLineGrid), new UIPropertyMetadata(false));
        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(GridLinesVisibilityEnum), typeof(CustomLineGrid), new UIPropertyMetadata(GridLinesVisibilityEnum.Both));
        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(CustomLineGrid), new UIPropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty GridLineThicknessProperty =
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(CustomLineGrid), new UIPropertyMetadata(1.0));

        public bool ShowCustomGridLines
        {
            set { SetValue(ShowCustomGridLinesProperty, value); }
            get { return (bool)GetValue(ShowCustomGridLinesProperty); }
        }
        public GridLinesVisibilityEnum GridLinesVisibility
        {
            set { SetValue(GridLinesVisibilityProperty, value); }
            get { return (GridLinesVisibilityEnum)GetValue(GridLinesVisibilityProperty); }
        }
        public Brush GridLineBrush
        {
            set { SetValue(GridLineBrushProperty, value); }
            get { return (Brush)GetValue(GridLineBrushProperty); }
        }
        public double GridLineThickness
        {
            set { SetValue(GridLineThicknessProperty, value); }
            get { return (double)GetValue(GridLineThicknessProperty); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (ShowCustomGridLines)    // Grid Line 그릴건지
            {
                if (GridLinesVisibility == GridLinesVisibilityEnum.Both)
                {
                    foreach (var rowDefinition in RowDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                    }
                    foreach (var colDefinition in ColumnDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(colDefinition.Offset, 0), new Point(colDefinition.Offset, ActualHeight));
                    }
                    dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }
                else if (GridLinesVisibility == GridLinesVisibilityEnum.Vertical)
                {
                    foreach (var colDefinition in ColumnDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(colDefinition.Offset, 0), new Point(colDefinition.Offset, ActualHeight));
                    }
                    dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }
                else if (GridLinesVisibility == GridLinesVisibilityEnum.Horizontal)
                {
                    foreach (var rowDefinition in RowDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                    }
                    dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }
            }
            base.OnRender(dc);
        }


        static CustomLineGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomLineGrid), new FrameworkPropertyMetadata(typeof(CustomLineGrid)));
        }
    }
}
