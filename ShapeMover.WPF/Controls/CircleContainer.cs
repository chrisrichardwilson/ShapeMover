using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeMover.WPF.Controls;

/// <summary>
/// A control that draws circles on a canvas and allows the user to drag them to different positions.
/// </summary>
[TemplatePart(Name = "PART_CircleCanvas", Type = typeof(Canvas))]
public class CircleContainer : Control
{
    private Canvas? circleCanvas;
    private const int DIAMETER = 40;
    private const int CIRCLELINEWEIGHT = 3;
    private int circleDraggedID;
    private Point dragOffset;

    /// <summary>
    /// Collection of circles to draw on the canvas. key = circle ID, value = position of circle.
    /// </summary>
    public Dictionary<int, Point> Circles
    {
        get { return (Dictionary<int, Point>)GetValue(CirclesProperty); }
        set { SetValue(CirclesProperty, value); }
    }

    public static readonly DependencyProperty CirclesProperty =
        DependencyProperty.Register(nameof(Circles), typeof(Dictionary<int, Point>), typeof(CircleContainer), new PropertyMetadata(new Dictionary<int, Point>(), CirclesPropertyChanged));

    /// <summary>
    /// A command that will be called when a circle is dragged.
    /// </summary>
    public ICommand MoveCircleCommand
    {
        get { return (ICommand)GetValue(CircleMovedCommandProperty); }
        set { SetValue(CircleMovedCommandProperty, value); }
    }

    public static readonly DependencyProperty CircleMovedCommandProperty =
        DependencyProperty.Register(nameof(MoveCircleCommand), typeof(ICommand), typeof(CircleContainer), new PropertyMetadata(null));


    /// <summary>
    /// The height of the drawing area.
    /// </summary>
    public double MyHeight
    {
        get { return (double)GetValue(MyHeightProperty); }
        set { SetValue(MyHeightProperty, value); }
    }

    public static readonly DependencyProperty MyHeightProperty =
        DependencyProperty.Register(nameof(MyHeight), typeof(double), typeof(CircleContainer), new PropertyMetadata(0d));

    /// <summary>
    /// The width of the drawing area.
    /// </summary>
    public double MyWidth
    {
        get { return (double)GetValue(MyWidthProperty); }
        set { SetValue(MyWidthProperty, value); }
    }

    public static readonly DependencyProperty MyWidthProperty =
        DependencyProperty.Register(nameof(MyWidth), typeof(double), typeof(CircleContainer), new PropertyMetadata(0d));


    /// <summary>
    /// If true circles are given a random colour. If false circles are drawn as a simple black outline.
    /// </summary>
    public bool ColourCircles
    {
        get { return (bool)GetValue(ColourCirclesProperty); }
        set { SetValue(ColourCirclesProperty, value); }
    }

    public static readonly DependencyProperty ColourCirclesProperty =
        DependencyProperty.Register(nameof(ColourCircles), typeof(bool), typeof(CircleContainer), new PropertyMetadata(false));

    static CircleContainer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleContainer), new FrameworkPropertyMetadata(typeof(CircleContainer)));
    }

    public override void OnApplyTemplate()
    {
        circleCanvas = Template.FindName("PART_CircleCanvas", this) as Canvas;
        circleCanvas!.DragOver += CircleCanvas_Over;
        circleCanvas.Drop += CircleCanvas_Drop;
        circleCanvas.SizeChanged += CircleCanvas_SizeChanged;
        AddCircles();

        base.OnApplyTemplate();
    }

    private static void CirclesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CircleContainer? circleContainer = d as CircleContainer;

        if (circleContainer == null)
            return;

        circleContainer.AddCircles();
    }

    /// <summary>
    /// Draws all circles on the cavas based on the values in the Circles collection.
    /// </summary>
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

            if (ColourCircles)
                AddBackgroundToCircle(circle);

            circleCanvas.Children.Add(circle);

            Canvas.SetLeft(circle, Circles[circleKey].X);
            Canvas.SetTop(circle, Circles[circleKey].Y);
        }
    }

    /// <summary>
    /// Simple way to fill a circle with a random colour based on the tag (ID) of the circle. ID to colour
    /// mappings are always consistent. Picks from the predefined SolidColorBrushes in Brush.
    /// </summary>
    /// <param name="circle">The circle to add a background colour to.</param>
    private void AddBackgroundToCircle(Ellipse circle)
    {
        //Use reflection to get all SolidColorBrush properties of Brushes, then pick one at Random. Random takes
        //the circle Tag (the ID of the circle) as the seed so the same ID will always result in the same colour.

        PropertyInfo[] solidBrushProperties = typeof(Brushes).GetProperties()
            .Where(p => p.PropertyType.Equals(typeof(SolidColorBrush))).ToArray();

        Random random = new Random((int)circle.Tag);

        circle.Fill = solidBrushProperties[random.Next(solidBrushProperties.Length)].GetValue(null) as Brush;
    }

    private void CircleCanvas_Drop(object sender, DragEventArgs e)
    {
        if (MoveCircleCommand == null)
            return;

        Point dropPosition = e.GetPosition(circleCanvas);
        MoveCircleCommand.Execute((circleDraggedID, new Point(dropPosition.X - dragOffset.X, dropPosition.Y - dragOffset.Y)));
    }

    private void Circle_MouseMove(object sender, MouseEventArgs e)
    {
        Ellipse? circle = sender as Ellipse;

        if (circle == null)
            return;

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            dragOffset = e.GetPosition(circle);

            circleDraggedID = (int)circle.Tag;
            DragDrop.DoDragDrop(circle, new DataObject(DataFormats.Serializable, circle), DragDropEffects.Move);
        }
    }

    private void CircleCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        MyWidth = ActualWidth - DIAMETER;
        MyHeight = ActualHeight - DIAMETER;
    }

    private void CircleCanvas_Over(object sender, DragEventArgs e)
    {
        Ellipse? circle = e.Data.GetData(DataFormats.Serializable) as Ellipse;

        if (circle == null)
            return;

        Point position = e.GetPosition(circleCanvas);

        Canvas.SetLeft(circle, position.X - dragOffset.X);
        Canvas.SetTop(circle, position.Y - dragOffset.Y);
    }
}
