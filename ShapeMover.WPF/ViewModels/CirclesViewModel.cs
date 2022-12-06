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

/// <summary>
/// View Model for Circles View.
/// </summary>
public class CirclesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /*The undo/redo logic uses a linked list to store the complete state of the Circles collection with every change.
      A current node points to the current state. Undo is then enabled by moving the current pointer to the previous
      node and restoring the Circles collection to that state. Likewise, redo is enabled by moving the current pointer
      to its next position and restoring the Circles collection to that state. Undo is unavailable if current.Previous
      is null. Likewise, redo is unavailable if current.Next is null.

      When a new action is performed (Add or move a circle, without using undo / redo) everything beyond the current
      node is discarded.

      This approach uses a memento pattern. An alternative approach would be the Command Pattern to only store the changes
      between states. This benefits from using less memory but is more complicated to implement. I went with the
      memento pattern since it is much simpler and the state collection is unlikely to get big enough for the memory
      requirement to become a problem.
     */
    private LinkedList<Dictionary<int, Point>> history = new();
    private LinkedListNode<Dictionary<int, Point>> current = new(new Dictionary<int, Point>());

    CirclesModel circlesModel = new();

    IRandomGenerator randomGenerator;

    /// <summary>
    /// Collection of circles to draw on the canvas. key = circle ID, value = position of circle.
    /// </summary>
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

    /// <summary>
    /// The height of the circle canvas, this comes from the view.
    /// </summary>
    public double CanvasHeight { get; set; }

    /// <summary>
    /// The width of the circle canvas, this comes from the view.
    /// </summary>
    public double CanvasWidth { get; set; }

    /// <summary>
    /// Command to add circle to the canvas.
    /// </summary>
    public ICommand AddCircleCommand { get; set; }

    /// <summary>
    /// Command called by the view when a circle is moved.
    /// </summary>
    public ICommand MoveCircleCommand { get; set; }

    /// <summary>
    /// Command to undo the last action.
    /// </summary>
    public ICommand UndoCommand { get; set; }

    /// <summary>
    /// Command to redo the last action.
    /// </summary>
    public ICommand RedoCommand { get; set; }

    /// <summary>
    /// View Model for Circles View.
    /// </summary>
    /// <param name="randomGenerator">The random number generator used to add circles in a random location.</param>
    public CirclesViewModel(IRandomGenerator randomGenerator)
    {
        this.randomGenerator = randomGenerator;
        MoveCircleCommand = new MoveCircleCommand(MoveCircle);

        AddCircleCommand = new GenericCommand(AddCircle);
        UndoCommand = new GenericCommand(Undo, CanUndo);
        RedoCommand = new GenericCommand(Redo, CanRedo);

        history.AddLast(current);
    }

    /// <summary>
    /// View Model for Circles View.
    /// </summary>
    public CirclesViewModel() : this(new RandomGenerator())
    { }

    /// <summary>
    /// Adds a circle to the Circles collection in a random position (based on CanvasWidth and CanvasHeight).
    /// </summary>
    public void AddCircle()
    {
        circlesModel.AddCircle(new Point(randomGenerator.Generate((int)Math.Floor(CanvasWidth)), randomGenerator.Generate((int)Math.Floor(CanvasHeight))));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        updateHistoryWithNewAction();

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Removes all history beyond the current state, then adds the current state, then sets current to the current state.
    /// </summary>
    private void updateHistoryWithNewAction()
    {
        while (history.Last != current)
            history.RemoveLast();

        history.AddLast(new LinkedListNode<Dictionary<int, Point>>(new(Circles)));
        current = history.Last;
    }

    /// <summary>
    /// Move circle in Circles collection.
    /// </summary>
    /// <param name="key">Key of circle.</param>
    /// <param name="location">New location.</param>
    public void MoveCircle(int key, Point location)
    {
        circlesModel.MoveCircle(key, location);

        updateHistoryWithNewAction();

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Undoes the last change to Circles collection.
    /// </summary>
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

    /// <summary>
    /// Redoes the last change to Circles collection.
    /// </summary>
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

    /// <summary>
    /// Determines if Undo can be called. Undo isn't available if there are no states before the current state.
    /// </summary>
    /// <returns>True if Undo is possible, false otherwise.</returns>
    public bool CanUndo() => current.Previous != null;

    /// <summary>
    /// Determines if Redo can be called. Redo isn't available if there are no states after the current state.
    /// </summary>
    /// <returns></returns>
    public bool CanRedo() => current.Next != null;
}
