using System;
using System.Collections.Generic;
using System.Text;

class Player
{
    public string Name { get; set; }
    public int Money { get; set; }
    public int Position { get; set; }
    public bool SkipNextTurn { get; set; }

    public Player(string name)
    {
        Name = name;
        Money = 1000;
        Position = 0;
    }
}

class Property
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int Rent { get; set; }
    public Player Owner { get; set; }

    public Property(string name, int price = 0, int rent = 0)
    {
        Name = name;
        Price = price;
        Rent = rent;
        Owner = null;
    }
}

class Board
{
    public List<Property> Properties { get; set; }

    public Board()
    {
        Properties = new List<Property>
        {
            new Property("Старт"),
            new Property("Hot Tea"),
            new Property("Aice Coffe"),
            new Property("Hova Poshta"),
            new Property("BMW"),
            new Property("AirPlain")
        };
    }
}

class Dice
{
    private Random random = new Random();

    public int Roll()
    {
        return random.Next(1, 7);
    }

    public int Roll(int sides)
    {
        return random.Next(1, sides + 1);
    }
}

class Game
{
    public List<Player> Players { get; set; }
    public Board Board { get; set; }
    public Dice Dice { get; set; }

    public Game()
    {
        Players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        Board = new Board();
        Dice = new Dice();
    }

    public void Start()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("             МОНОПОЛІЯ           ");
        Console.WriteLine("=================================");

        int currentPlayer = 0;

        {
            Player player = Players[currentPlayer];

            if (player.SkipNextTurn)
            {
                Console.WriteLine();
                Console.WriteLine($" {player.Name} пропускає наступний хід.");
                player.SkipNextTurn = false;

                currentPlayer = (currentPlayer + 1) % Players.Count;
                continue;
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Хід: {player.Name}");
            Console.WriteLine($"Гроші: ${player.Money}");

            int currentSector = player.Position + 1;

            Console.WriteLine($"Сектор: №{currentSector}");
            Console.WriteLine($"Зараз: {Board.Properties[player.Position].Name}");
            Console.WriteLine("---------------------------------");

            Console.WriteLine("Натисніть ENTER, щоб кинути кубик...");
            Console.ReadLine();

            int dice = Dice.Roll();

            Console.WriteLine($"{player.Name} викинув {dice}");

            player.Position = (player.Position + dice) % Board.Properties.Count;

            int sectorNumber = player.Position + 1;
            Property property = Board.Properties[player.Position];

            Console.WriteLine();
            Console.WriteLine($"Сектор №{sectorNumber}: {property.Name}");

            if (property.Name == "Старт")
            {
                Console.WriteLine();
                Console.WriteLine(" Ви знаходитеся на СТАРТІ!");
            }


            currentPlayer = (currentPlayer + 1) % Players.Count;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Game game = new Game();
        game.Start();

        Console.WriteLine();
        Console.WriteLine("Гру завершено.");
        Console.ReadKey();
    }
}