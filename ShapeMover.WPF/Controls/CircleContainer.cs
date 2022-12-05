using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeMover.WPF.Controls;

[TemplatePart(Name = "PART_CircleCanvas", Type = typeof(Canvas))]
public class CircleContainer : Control
{
    private Canvas circleCanvas;
    private const int DIAMETER = 40;
    private const int CIRCUMFERENCE = 3;
    private int circleDraggedID;

    public Dictionary<int, Point> Circles
    {
        get { return (Dictionary<int, Point>)GetValue(CirclesProperty); }
        set { SetValue(CirclesProperty, value); }
    }

    public static readonly DependencyProperty CirclesProperty =
        DependencyProperty.Register("Circles", typeof(Dictionary<int, Point>), typeof(CircleContainer), new PropertyMetadata(new Dictionary<int, Point>(), CirclesPropertyChanged));

    private static void CirclesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CircleContainer circleContainer = d as CircleContainer;

        if (circleContainer == null)
            return;

        circleContainer.AddCircles();
    }


    public ICommand MoveCircleCommand
    {
        get { return (ICommand)GetValue(CircleMovedCommandProperty); }
        set { SetValue(CircleMovedCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CircleMovedCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CircleMovedCommandProperty =
        DependencyProperty.Register("MoveCircleCommand", typeof(ICommand), typeof(CircleContainer), new PropertyMetadata(null));


    //public static RoutedEvent CircleMovedEvent = EventManager.RegisterRoutedEvent("CircleMoved", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(CircleContainer));

    //public event RoutedEventHandler CircleMoved
    //{
    //    add 
    //    {
    //        AddHandler(CircleMovedEvent, value);
    //    }
    //    remove
    //    {
    //        RemoveHandler(CircleMovedEvent, value);
    //    }
    //}

    //public Size CanvasSize
    //{
    //    get { return (Size)GetValue(CanvasSizeProperty); }
    //    set { SetValue(CanvasSizeProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for CanvasSize.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty CanvasSizeProperty =
    //    DependencyProperty.Register("CanvasSize", typeof(Size), typeof(CircleContainer), new PropertyMetadata(new Size()));


    static CircleContainer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleContainer), new FrameworkPropertyMetadata(typeof(CircleContainer)));        
    }   

    //Todo: This whole method can probably be removed, just for testing.
    private void AddCircles()
    {
        if (circleCanvas == null)
            return;

        circleCanvas.Children.Clear();
        foreach (int circleKey in Circles.Keys)
        {
            Ellipse circle = new Ellipse()
            {
                Width = DIAMETER,
                Height = DIAMETER,
                Stroke = Brushes.Black,
                StrokeThickness = CIRCUMFERENCE,
                Tag = circleKey
            };

            circle.MouseMove += Circle_MouseMove;            
            circleCanvas.Children.Add(circle);

            Canvas.SetLeft(circle, Circles[circleKey].X);
            Canvas.SetTop(circle, Circles[circleKey].Y);
        }
    }

    private void CircleCanvas_Drop(object sender, DragEventArgs e)
    {
        //RaiseEvent(new RoutedEventArgs(CircleMovedEvent, this));
        if (MoveCircleCommand == null)
            return;

        Point dropPosition = e.GetPosition(circleCanvas);
        MoveCircleCommand.Execute((circleDraggedID, dropPosition));

        Debug.WriteLine("CircleCanvas_Drop");
    }

    private void Circle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Ellipse circle = sender as Ellipse;

        if (circle == null)
            return;

        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            circleDraggedID = (int)circle.Tag;
            DragDrop.DoDragDrop(circle, new DataObject(DataFormats.Serializable, circle), DragDropEffects.Move);
            
            //Debug.WriteLine($"circleDraggedID = {circleDraggedID}");
        }
    }

    public override void OnApplyTemplate()
    {
        circleCanvas = Template.FindName("PART_CircleCanvas", this) as Canvas;
        circleCanvas.DragOver += CircleCanvas_Over;
        circleCanvas.Drop += CircleCanvas_Drop;
        AddCircles();

        base.OnApplyTemplate();
    }

    private void CircleCanvas_Over(object sender, DragEventArgs e)
    {
        Ellipse circle = e.Data.GetData(DataFormats.Serializable) as Ellipse;

        if (circle == null)
            return;

        Point dropPosition = e.GetPosition(circleCanvas);

        Canvas.SetLeft(circle, dropPosition.X);
        Canvas.SetTop(circle, dropPosition.Y);
    }
}
