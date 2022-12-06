using ShapeMover.Helpers.Classes;
using ShapeMover.Helpers.Interfaces;
using ShapeMover.WPF.Commands;
using ShapeMover.WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ShapeMover.WPF.ViewModels;

public class CirclesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private LinkedList<Dictionary<int, Point>> history = new();
    private LinkedListNode<Dictionary<int, Point>> current = new(new Dictionary<int, Point>());

    CirclesModel circlesModel = new();

    IRandomGenerator randomGenerator;

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

    public CirclesViewModel(IRandomGenerator randomGenerator)
    {
        this.randomGenerator = randomGenerator;
        MoveCircleCommand = new MoveCircleCommand(this);

        AddCircleCommand = new GenericCommand(AddCircle);
        UndoCommand = new GenericCommand(Undo, CanUndo);
        RedoCommand = new GenericCommand(Redo, CanRedo);

        history.AddLast(current);
    }

    public CirclesViewModel() : this(new RandomGenerator())
    { }

    public void AddCircle()
    {
        circlesModel.AddCircle(new Point(randomGenerator.Generate((int)Math.Floor(CanvasWidth)), randomGenerator.Generate((int)Math.Floor(CanvasHeight))));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        updateHistoryWithNewAction();
    }

    private void updateHistoryWithNewAction()
    {
        while (history.Last != current)
            history.RemoveLast();

        history.AddLast(new LinkedListNode<Dictionary<int, Point>>(new(Circles)));
        current = history.Last;

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    public void MoveCircle(int key, Point location)
    {
        circlesModel.MoveCircle(key, location);

        updateHistoryWithNewAction();
    }

    public void Undo()
    {
        if (current.Previous == null)
            return;

        current = current.Previous;
        Circles = new(current.Value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    public void Redo()
    {
        if (current.Next == null)
            return;

        current = current.Next;
        Circles = new(current.Value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    public bool CanUndo() => current.Previous != null;

    public bool CanRedo() => current.Next != null;
}
