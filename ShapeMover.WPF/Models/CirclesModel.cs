using System.Collections.Generic;
using System.Windows;

namespace ShapeMover.WPF.Models;

/// <summary>
/// Models a collection of circles in a dictionary with an ID and a Point to describe the X, Y location.
/// </summary>
public class CirclesModel
{
    /// <summary>
    /// Used to keep track of the circle IDs
    /// </summary>
    private int index = 0;

    /// <summary>
    /// Collection of circles. Key = ID, Value = location of circle.
    /// </summary>
    public Dictionary<int, Point> Circles { get; set; } = new();

    /// <summary>
    /// Adds a circle to the collection.
    /// </summary>
    /// <param name="circle">The location of the circle.</param>
    /// <returns>The ID of the circle added.</returns>
    public int AddCircle(Point circle)
    {
        Circles.Add(index, circle);
        index++;
        return index;
    }

    /// <summary>
    /// Removes a circle from the collection based on its ID.
    /// </summary>
    /// <param name="key">The ID of the circle to remove.</param>
    /// <returns>True if the circle is successfully found and removed, false otherwise.</returns>
    public bool RemoveCircle(int key)
    {
        return Circles.Remove(key);
    }

    /// <summary>
    /// Moves a circle in the collection.
    /// </summary>
    /// <param name="key">The ID of the circle.</param>
    /// <param name="location">The new location of the circle.</param>
    /// <returns>True if the circle is successfully found and moved, false otherwise.</returns>
    public bool MoveCircle(int key, Point location)
    {
        if (!Circles.ContainsKey(key))
            return false;

        Circles[key] = location;

        return true;
    }
}
