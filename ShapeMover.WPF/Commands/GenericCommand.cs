using System;
using System.Windows.Input;

namespace ShapeMover.WPF.Commands;

public class GenericCommand : ICommand
{
    public event EventHandler CanExecuteChanged;
    private Action methodToExecute;
    private Func<bool> canExecuteEvaluator;

    public GenericCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
    {
        this.methodToExecute = methodToExecute;
        this.canExecuteEvaluator = canExecuteEvaluator;
    }
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

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
