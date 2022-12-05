using System.Collections.Generic;
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
    private const int CIRCLELINEWEIGHT = 3;
    private int circleDraggedID;

    public Dictionary<int, Point> Circles
    {
        get { return (Dictionary<int, Point>)GetValue(CirclesProperty); }
        set { SetValue(CirclesProperty, value); }
    }

    public static readonly DependencyProperty CirclesProperty =
        DependencyProperty.Register(nameof(Circles), typeof(Dictionary<int, Point>), typeof(CircleContainer), new PropertyMetadata(new Dictionary<int, Point>(), CirclesPropertyChanged));

    public ICommand MoveCircleCommand
    {
        get { return (ICommand)GetValue(CircleMovedCommandProperty); }
        set { SetValue(CircleMovedCommandProperty, value); }
    }

    public static readonly DependencyProperty CircleMovedCommandProperty =
        DependencyProperty.Register(nameof(MoveCircleCommand), typeof(ICommand), typeof(CircleContainer), new PropertyMetadata(null));



    public double MyHeight
    {
        get { return (double)GetValue(MyHeightProperty); }
        set { SetValue(MyHeightProperty, value); }
    }

    public static readonly DependencyProperty MyHeightProperty =
        DependencyProperty.Register(nameof(MyHeight), typeof(double), typeof(CircleContainer), new PropertyMetadata(0d));


    public double MyWidth
    {
        get { return (double)GetValue(MyWidthProperty); }
        set { SetValue(MyWidthProperty, value); }
    }

    public static readonly DependencyProperty MyWidthProperty =
        DependencyProperty.Register(nameof(MyWidth), typeof(double), typeof(CircleContainer), new PropertyMetadata(0d));

    private static void CirclesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CircleContainer circleContainer = d as CircleContainer;

        if (circleContainer == null)
            return;

        circleContainer.AddCircles();
    }

    static CircleContainer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleContainer), new FrameworkPropertyMetadata(typeof(CircleContainer)));
    }

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
                StrokeThickness = CIRCLELINEWEIGHT,
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
        if (MoveCircleCommand == null)
            return;

        Point dropPosition = e.GetPosition(circleCanvas);
        MoveCircleCommand.Execute((circleDraggedID, dropPosition));
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
        }
    }

    public override void OnApplyTemplate()
    {
        circleCanvas = Template.FindName("PART_CircleCanvas", this) as Canvas;
        circleCanvas.DragOver += CircleCanvas_Over;
        circleCanvas.Drop += CircleCanvas_Drop;
        circleCanvas.SizeChanged += CircleCanvas_SizeChanged;
        AddCircles();

        base.OnApplyTemplate();
    }

    private void CircleCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        MyWidth = ActualWidth - DIAMETER;
        MyHeight = ActualHeight - DIAMETER;
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
