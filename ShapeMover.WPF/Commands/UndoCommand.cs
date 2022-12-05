using ShapeMover.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShapeMover.WPF.Commands;

public class UndoCommand : ICommand
{
    private CirclesViewModel circlesViewModel;

    public UndoCommand(CirclesViewModel circlesViewModel)
    {
        this.circlesViewModel = circlesViewModel;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        circlesViewModel.Undo();
    }
}
