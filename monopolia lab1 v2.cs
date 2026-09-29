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
        Properties = new List<Property>();

            // Сектор №1
            Properties.Add(new Property("СТАРТ", 0, 0));
            // Сектор №2-4
            Properties.Add(new Property("Hot Tea", 300, 50));
            Properties.Add(new Property("Ice Coffee", 400, 70));
            Properties.Add(new Property("Tasty Croissant", 500, 90));
            // Сектор №5-7
            Properties.Add(new Property("Ukr Poshta", 500, 90));
            Properties.Add(new Property("Nova Poshta", 600, 110));
            Properties.Add(new Property("Meest Poshta", 700, 130));
            // Сектор №8-10
            Properties.Add(new Property("Mercedes", 1300, 250));
            Properties.Add(new Property("BMW", 1400, 270));
            Properties.Add(new Property("Volkswagen", 1500, 290));
            // Сектор №11-13
            Properties.Add(new Property("Rozzetka", 900, 170));
            Properties.Add(new Property("Foxtrot", 1000, 190));
            Properties.Add(new Property("Eldorado", 1100, 210));
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

    while (true)
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

        if (property.Name == "СТАРТ")
        {
            Console.WriteLine();
            Console.WriteLine(" Ви знаходитеся на СТАРТІ!");
        }
        else if (property.Owner == null)
        {
            Console.WriteLine($"Ціна: ${property.Price}");
            Console.WriteLine($"Оренда: ${property.Rent}");

            if (player.Money >= property.Price)
            {
                Console.Write(
                    "Купити нерухомість? (так/ні): "
                );

                string answer = Console.ReadLine().ToLower();

                if (answer == "так")
                {
                    player.Money -= property.Price;
                    property.Owner = player;

                    Console.WriteLine();
                    Console.WriteLine(
                        $"Ви придбали {property.Name}!"
                    );

                    Console.WriteLine(
                        $"Залишок грошей: ${player.Money}"
                    );
                }
            }
            else
            {
                Console.WriteLine(
                    "У вас недостатньо грошей для покупки."
                );
            }
        }

        // ==========================
        // ЧУЖА НЕРУХОМІСТЬ
        // ==========================

        else if (property.Owner != player)
        {
            Console.WriteLine(
                $"Власник: {property.Owner.Name}"
            );

            player.Money -= property.Rent;
            property.Owner.Money += property.Rent;

            Console.WriteLine(
                $"Ви заплатили оренду ${property.Rent}."
            );

            Console.WriteLine(
                $"Ваш баланс: ${player.Money}"
            );
        }

        // ==========================
         // ВЛАСНА НЕРУХОМІСТЬ
        // ==========================

        else
        {
            Console.WriteLine(
                "Це ваша нерухомість."
             );
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
           