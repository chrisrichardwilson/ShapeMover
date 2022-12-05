using ShapeMover.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeMover.Core.ViewModels;

public class CirclesViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    CirclesModel circlesModel = new();

    //public Dictionary<int, Point>
}
