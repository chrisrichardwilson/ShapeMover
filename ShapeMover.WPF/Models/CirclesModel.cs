using System.Collections.Generic;
using System.Windows;

namespace ShapeMover.WPF.Models;

public class CirclesModel
{
    private int index = 0;
    public Dictionary<int, Point> Circles { get; set; } = new();

    public int AddCircle(Point circle)
    {
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
