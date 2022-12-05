using ShapeMover.WPF.Commands;
using ShapeMover.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapeMover.WPF.ViewModels;

public class CirclesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private LinkedList<Dictionary<int, Point>> history = new();
    private LinkedListNode<Dictionary<int, Point>> current = new(new Dictionary<int, Point>());

    CirclesModel circlesModel = new();

    Random random = new Random();

    //Func<CircleModel, int> AddCircleAction => circlesModel.AddCircle;

    public int Test { get; set; }

    //public ObservableCollection<Point> Circles
    //{
    //    get
    //    {

    //    }
    //}

    public Dictionary<int, Point> Circles
    {
        get
        {
            return new Dictionary<int, Point>(circlesModel.Circles);
        }
        set
        {
            circlesModel.Circles = value;
        }
    }

    public double CanvasHeight { get; set; }
    public double CanvasWidth { get; set; }

    public ICommand AddCircleCommand { get; set; }
    public ICommand MoveCircleCommand { get; set; }
    public ICommand UndoCommand { get; set; }
    public ICommand RedoCommand { get; set; }

    public CirclesViewModel()
    {
        Test = 0;

        AddCircleCommand = new AddCircleCommand(this);
        MoveCircleCommand = new MoveCircleCommand(this);
        UndoCommand = new UndoCommand(this);
        RedoCommand = new RedoCommand(this);

        history.AddLast(current);

        //test code
        //circlesModel.AddCircle(new Point(10, 10));
        //circlesModel.AddCircle(new Point(50, 50));
    }

    public void AddCircle()
    {
        //todo: don't hardcode height width, get from the control size
        circlesModel.AddCircle(new Point(random.Next(500), random.Next(500)));
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        while (history.Last != current)
            history.RemoveLast();

        history.AddLast(new LinkedListNode<Dictionary<int, Point>>(new(Circles)));
        current = history.Last;
    }

    public void MoveCircle(int key, Point location)
    {
        circlesModel.MoveCircle(key, location);

        //todo remove dup
        while (history.Last != current)
            history.RemoveLast();

        history.AddLast(new LinkedListNode<Dictionary<int, Point>>(new(Circles)));
        current = history.Last;

        Debug.WriteLine("Move");
    }

    public void Undo()
    {
        if (current.Previous == null)
            return;

        current = current.Previous;
        Circles = new(current.Value);
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));
    }

    public void Redo()
    {
        if (current.Next == null)
            return;

        current = current.Next;
        Circles = new(current.Value);
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));
    }
}
