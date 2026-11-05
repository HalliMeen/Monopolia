namespace MonopoliaLab2
{
	using System.Collections.Generic;

    public class Player
	{
		public string Name { get; set; }
		public int Balance { get; set; }
		public int Position { get; set; }
		public bool Jail { get; set; }
        public bool Curort { get; set; }
        public int JailTurnsCount { get; set; }
		public bool IsBankrupt { get; set; }
        public List<StreetProperty> OwnedProperties { get; set; } = new List<StreetProperty>();
        public Token PlayerToken { get; set; }
        public enum Token
		{
			Car,
			Dog,
			Cat,
			Shark,
			Snake,
			Horse,

		}

		public Player(string name, Token token)
		{
			Name = name;
			Balance = 3000;
			Position = 0;
			IsBankrupt = false;
            Jail = false;
            Curort = false;
            PlayerToken = token;
		}

	}
}
