using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeMover.WPF.Models;

public class CirclesModel
{
    private int index = 0;
    //private Dictionary<int, Point> circles = new();
    public Dictionary<int, Point> Circles { get; set; } = new();

    //private Dictionary<int, Point> circles = new();
    //public ReadOnlyDictionary<int, Point> Circles
    //{
    //    get
    //    {
    //        return circles.AsReadOnly();
    //    }
    //}

    public int AddCircle(Point circle)
    {
        //todo: maybe keep tryadding and incrementing index so we don't "lose" capacity when circles are added and removed
        Circles.Add(index, circle);
        index++;
        return index;
    }

    public bool RemoveCircle(int key)
    {
        return Circles.Remove(key);
    }

    public bool MoveCircle(int key, Point location)
    {
        if (!Circles.ContainsKey(key))
            return false;

        Circles[key] = location;

        return true;
    }
}
