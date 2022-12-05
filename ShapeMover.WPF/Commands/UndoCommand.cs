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
    //public event EventHandler? CanExecuteChanged;

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    private CirclesViewModel circlesViewModel;

    public UndoCommand(CirclesViewModel circlesViewModel)
    {
        this.circlesViewModel = circlesViewModel;
    }    

    public bool CanExecute(object? parameter)
    {
        return circlesViewModel.CanUndo();
    }

    public void Execute(object? parameter)
    {
        circlesViewModel.Undo();
    }
}
