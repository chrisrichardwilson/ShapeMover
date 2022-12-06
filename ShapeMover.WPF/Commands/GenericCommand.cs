using System;
using System.Windows.Input;

namespace ShapeMover.WPF.Commands;

/// <summary>
/// A generic <see cref="ICommand"/> class which takes an Action to execute and a Func<bool> to evaluate if execution is possible.
/// </summary>
public class GenericCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// The method to be executed when Execute is called.
    /// </summary>
    private Action methodToExecute;

    /// <summary>
    /// The method which evaluates the result of CanExecute.
    /// </summary>
    private Func<bool> canExecuteEvaluator;

    /// <summary>
    /// A generic <see cref="ICommand"/> class which takes an Action to execute and a Func<bool> to evaluate if execution is possible.
    /// </summary>
    /// <param name="methodToExecute">The method to be executed when Execute is called.</param>
    /// <param name="canExecuteEvaluator">Optional, The method which evaluates the result of CanExecute. Omit for CanExecute to always be true</param>
    public GenericCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
    {
        this.methodToExecute = methodToExecute;
        this.canExecuteEvaluator = canExecuteEvaluator;
    }

    /// <summary>
    /// A generic <see cref="ICommand"/> class which takes an Action to execute and a Func<bool> to evaluate if execution is possible.
    /// </summary>
    /// <param name="methodToExecute">The method to be executed when Execute is called.</param>
    public GenericCommand(Action methodToExecute)
        : this(methodToExecute, null)
    {
    }

    public bool CanExecute(object parameter)
    {
        if (this.canExecuteEvaluator == null)
        {
            return true;
        }

        bool result = this.canExecuteEvaluator.Invoke();
        return result;
    }

    public void Execute(object parameter)
    {
        this.methodToExecute.Invoke();
    }

    /// <summary>
    /// Causes evaluation of condition that determines if the command can be executed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
