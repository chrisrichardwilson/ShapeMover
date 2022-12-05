using ShapeMover.WPF.Models;
using ShapeMover.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShapeMover.WPF.Commands;

public class AddCircleCommand : ICommand
{
    private readonly CirclesViewModel circlesViewModel;

    //private Func<CircleModel, int> addCircle;

    public AddCircleCommand(CirclesViewModel circlesViewModel)
    {
        this.circlesViewModel = circlesViewModel;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        circlesViewModel.AddCircle();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
