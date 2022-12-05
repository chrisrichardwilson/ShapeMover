using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeMover.Core.Models;

public class CirclesModel
{
    private int index = 0;
    private Dictionary<int, CircleModel> circles = new();
    public ReadOnlyCollection<CircleModel> Circles
    {
        get
        {
            return circles.Values.ToList().AsReadOnly();
        }
    }

    public int AddCircle(CircleModel circle)
    {
        //todo: maybe keep tryadding and incrementing index so we don't "lose" capacity when circles are added and removed
        circles.Add(index, circle);
        index++;
        return index;
    }

    public bool RemoveCircle(int key)
    {
        return circles.Remove(key);
    }

    public bool MoveCircle(int key, double x, double y)
    {
        if (!circles.ContainsKey(key))
            return false;

        circles[key].X = x;
        circles[key].Y = y;

        return true;
    }
}
