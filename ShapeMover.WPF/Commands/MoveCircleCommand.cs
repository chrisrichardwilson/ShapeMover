using ShapeMover.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapeMover.WPF.Commands;

public class MoveCircleCommand : ICommand
{
    private CirclesViewModel circlesViewModel;

    public MoveCircleCommand(CirclesViewModel circlesViewModel)
    {
        this.circlesViewModel = circlesViewModel;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        (int, Point)? keyPosition = parameter as (int, Point)?;

        if (!keyPosition.HasValue)
            return;

        circlesViewModel.MoveCircle(keyPosition.Value.Item1, keyPosition.Value.Item2);
    }
}
