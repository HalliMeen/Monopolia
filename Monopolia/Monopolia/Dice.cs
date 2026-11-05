namespace Monopolia
{
    using System;

    class Dice
    {
        private Random random = new Random();

        public int Roll()
        {
            return random.Next(1,7);
        }
    }
}