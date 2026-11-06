using System;

namespace Monopolia
{
    public class TwoDice : IDice
    {
        private readonly Random random = new Random();

        public int Roll()
        {
            return random.Next(1, 7) + random.Next(1, 7);
        }
    }
}