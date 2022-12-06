namespace ShapeMover.Helpers.Interfaces;

/// <summary>
/// An interface for generating a random number.
/// </summary>
public interface IRandomGenerator
{
    /// <summary>
    /// Generate a random number.
    /// </summary>
    /// <param name="max">Upper limit of number to generate.</param>
    /// <returns>A random number.</returns>
    int Generate(int max);
}
