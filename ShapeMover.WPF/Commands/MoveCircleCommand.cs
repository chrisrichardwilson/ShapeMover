using ShapeMover.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace ShapeMover.WPF.Commands;

/// <summary>
/// A <see cref="ICommand"/> for moving a circle.
/// </summary>
public class MoveCircleCommand : ICommand
{
    private Action<int, Point> moveAction;

    /// <summary>
    /// A <see cref="ICommand"/> for moving a circle.
    /// </summary>
    /// <param name="moveAction">The method to call to move a circle, param 1: circle key, param2: position.</param>
    public MoveCircleCommand(Action<int, Point> moveAction)
    {
        this.moveAction = moveAction; 
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        (int, Point)? keyPosition = parameter as (int, Point)?;

        if (!keyPosition.HasValue)
            return;

        moveAction.Invoke(keyPosition.Value.Item1, keyPosition.Value.Item2);
    }
}
