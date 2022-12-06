using ShapeMover.Helpers.Interfaces;

namespace ShapeMover.Helpers.Classes;

/// <summary>
/// A class to generate a (pseudo) random number.
/// </summary>
public class RandomGenerator : IRandomGenerator
{
    Random random = new Random();

    /// <summary>
    /// Generate a (pseudo) random number.
    /// </summary>
    /// <param name="max">Upper limit of number to generate.</param>
    /// <returns>A random number.</returns>
    public int Generate(int max)
    {
        return random.Next(max);
    }
}
