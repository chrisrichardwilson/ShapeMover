using ShapeMover.Helpers.Interfaces;

namespace ShapeMover.Helpers.Classes;

public class RandomGenerator : IRandomGenerator
{
    Random random = new Random();

    public int Generate(int max)
    {
        return random.Next(max);
    }
}
