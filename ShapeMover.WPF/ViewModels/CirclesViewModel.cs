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

    /*The undo/redo logic uses two stacks, they both store a number of states of the Circles collection. Each time a change
      to Circles is made (an add or a move) the previous state is pushed to the undo stack. Then undo can be performed by
      popping from the undo stack. When undo is performed the current state is pushed to the redo stack. Then redo can
      be performed by popping from the redo stack.

      When a new action is performed (an add or a move without undo/redo) the redo stack is cleared.

      This approach uses a memento pattern. An alternative approach would be the Command Pattern to only store the changes
      between states. This benefits from using less memory but is more complicated to implement. I went with the
      memento pattern since it is simpler and the state collection is unlikely to get big enough for the memory
      requirement to become a problem.
     */
    private Stack<Dictionary<int, Point>> undo = new();
    private Stack<Dictionary<int, Point>> redo = new();


    private CirclesModel circlesModel = new();
    private IRandomGenerator randomGenerator;

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
        updateHistoryWithNewAction();

        circlesModel.AddCircle(new Point(randomGenerator.Generate((int)Math.Floor(CanvasWidth)), randomGenerator.Generate((int)Math.Floor(CanvasHeight))));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));        

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Move circle in Circles collection.
    /// </summary>
    /// <param name="key">Key of circle.</param>
    /// <param name="location">New location.</param>
    public void MoveCircle(int key, Point location)
    {
        updateHistoryWithNewAction();

        circlesModel.MoveCircle(key, location);

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Undoes the last change to Circles collection.
    /// </summary>
    public void Undo()
    {
        if (undo.Count == 0)
            return;
;
        redo.Push(Circles);
        Circles = undo.Pop();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Redoes the last change to Circles collection.
    /// </summary>
    public void Redo()
    {
        if (redo.Count == 0)
            return;

        undo.Push(Circles);
        Circles = redo.Pop();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Circles)));

        ((GenericCommand)RedoCommand).RaiseCanExecuteChanged();
        ((GenericCommand)UndoCommand).RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Determines if Undo can be called. Undo isn't available if there are no states before the current state.
    /// </summary>
    /// <returns>True if Undo is possible, false otherwise.</returns>
    public bool CanUndo() => undo.Count > 0;

    /// <summary>
    /// Determines if Redo can be called. Redo isn't available if there are no states after the current state.
    /// </summary>
    /// <returns>True if Redo is possible, false otherwise.</returns>
    public bool CanRedo() => redo.Count > 0;

    /// <summary>
    /// Clear the redo stack and push the current state onto the undo stack.
    /// </summary>
    private void updateHistoryWithNewAction()
    {
        redo.Clear();
        undo.Push(Circles);
    }
}
