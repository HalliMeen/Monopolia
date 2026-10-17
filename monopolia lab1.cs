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
        SkipNextTurn = false;
    }

    // Метод 1: додавання грошей
    public void AddMoney(int amount)
    {
        Money += amount;
    }

    // Метод 2: віднімання грошей
    public void SpendMoney(int amount)
    {
        Money -= amount;
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

    // Метод 1: перевірка наявності власника
    public bool IsOwned()
    {
        return Owner != null;
    }

    // Метод 2: призначення власника
    public void SetOwner(Player player)
    {
        Owner = player;
    }
}

class Board
{
    public List<Property> Properties { get; set; }

    // Структура даних №1: Dictionary. Словник, дозволяє миттєво знаходити об'єкт за його назвам без перебору всього списку.
    public Dictionary<string, Property> PropertyByName { get; set; }

    // Структура даних №2: HashSet. Колекція, використовується для швидкої перевірки унікальності назв нерухомості.
    public HashSet<string> PropertyNames { get; set; }

    public Board()
    {
        Properties = new List<Property>
        {
            new Property("СТАРТ"),
            new Property("Hot Tea", 300, 50),
            new Property("Ice Coffee", 400, 70),
            new Property("Tasty Croissant", 500, 90),
            new Property("Ukr Poshta", 500, 90),
            new Property("Nova Poshta", 600, 110),
            new Property("Meest Poshta", 700, 130),
            new Property("Mercedes", 1300, 250),
            new Property("BMW", 1400, 270),
            new Property("Volkswagen", 1500, 290),
            new Property("Rozetka", 900, 170),
            new Property("Foxtrot", 1000, 190),
            new Property("Eldorado", 1100, 210),
            new Property("Comfy", 1200, 230),
            new Property("Moyo", 1300, 250),
            new Property("Citrus", 1400, 270)
        };

        PropertyByName = new Dictionary<string, Property>();
        PropertyNames = new HashSet<string>();

        foreach (Property property in Properties)
        {
            PropertyByName[property.Name] = property;
            PropertyNames.Add(property.Name);
        }
    }

    // Метод 1: пошук нерухомості за назвою
    public Property FindProperty(string name)
    {
        if (PropertyByName.ContainsKey(name))
        {
            return PropertyByName[name];
        }

        return null;
    }

    // Метод 2: перевірка унікальності назви
    public bool HasUniqueName(string name)
    {
        return !PropertyNames.Contains(name);
    }
}

class Dice
{
    private Random random = new Random();

    // Метод 1: кидок кубика від 1 до 6
    public int Roll()
    {
        return random.Next(1, 7);
    }


}

class Game
{
    // Структура даних №3: List
    public List<Player> Players { get; set; }

    public Board Board { get; set; }
    public Dice Dice { get; set; }

    // Структура даних №4: Queue. Черга, якщо гравець зробив хід то він стає в кінець черги і чекає свого ходу.
    public Queue<Player> TurnQueue { get; set; }

    private const int StartBonus = 200;

    public Game()
    {
        Players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };

        Board = new Board();
        Dice = new Dice();

        TurnQueue = new Queue<Player>();

        foreach (Player player in Players)
        {
            TurnQueue.Enqueue(player);
        }
    }

    // Метод 1: запуск гри
    public void Start()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("             МОНОПОЛІЯ           ");
        Console.WriteLine("=================================");

        while (TurnQueue.Count > 0)
        {
            Player player = TurnQueue.Dequeue();

            if (player.SkipNextTurn)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"{player.Name} пропускає наступний хід.");

                player.SkipNextTurn = false;
                TurnQueue.Enqueue(player);
                continue;
            }

            bool gameContinues = ProcessTurn(player);

            if (!gameContinues)
            {
                break;
            }

            TurnQueue.Enqueue(player);
        }
    }

    // Метод 2: виконання ходу гравця
    public bool ProcessTurn(Player player)
    {
        Console.WriteLine();
        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Хід: {player.Name}");
        Console.WriteLine($"Гроші: ${player.Money}");
        Console.WriteLine($"Сектор №{player.Position + 1}");
        Console.WriteLine(
            $"Зараз: {Board.Properties[player.Position].Name}");
        Console.WriteLine("---------------------------------");

        Console.WriteLine("Натисніть ENTER, щоб кинути кубик...");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break; 
            }
            else
            {
                    
                Console.WriteLine("Натисніть, будь ласка, Enter!");
            }
            
        }
        
        int diceResult = Dice.Roll();

        Console.WriteLine(
            $"{player.Name} викинув {diceResult}");

        MovePlayer(player, diceResult);

        Property property = Board.Properties[player.Position];

        Console.WriteLine();
        Console.WriteLine(
            $"Сектор №{player.Position + 1}: {property.Name}");

        if (property.Name == "СТАРТ")
        {
            Console.WriteLine("Ви знаходитеся на СТАРТІ!");
        }
        else if (!property.IsOwned())
        {
            Console.WriteLine("Власник: Нічий");
            Console.WriteLine($"Ціна: ${property.Price}");
            Console.WriteLine($"Оренда: ${property.Rent}");

            if (player.Money >= property.Price)
            {
                Console.Write("Купити нерухомість? (так/ні): ");
                string answer = Console.ReadLine()?.ToLower();

                if (answer == "так")
                {
                    player.SpendMoney(property.Price);
                    property.SetOwner(player);

                    Console.WriteLine(
                        $"Ви придбали {property.Name}!");
                    Console.WriteLine(
                        $"Залишок грошей: ${player.Money}");
                }
            }
            else
            {
                Console.WriteLine(
                    "Недостатньо грошей для покупки.");
            }
        }
        else if (property.Owner == player)
        {
            Console.WriteLine("Це ваша нерухомість.");
        }
        else
        {
            Console.WriteLine(
                $"Власник: {property.Owner.Name}");
            Console.WriteLine(
                $"Оренда становить ${property.Rent}");

            player.SpendMoney(property.Rent);
            property.Owner.AddMoney(property.Rent);

            Console.WriteLine(
                $"Ви сплатили ${property.Rent}.");
            Console.WriteLine(
                $"Ваш баланс: ${player.Money}");
        }

        return !CheckBankruptcy(player);
    }
    

    // Метод 3: переміщення гравця
    public void MovePlayer(Player player, int steps)
    {
        int oldPosition = player.Position;

        player.Position =
            (player.Position + steps) % Board.Properties.Count;

        if (oldPosition + steps >= Board.Properties.Count)
        {
            player.AddMoney(StartBonus);

            Console.WriteLine();
            Console.WriteLine($"{player.Name} пройшов СТАРТ!");
            Console.WriteLine($"Бонус: ${StartBonus}");
            Console.WriteLine($"Баланс: ${player.Money}");
        }
    }

    // Метод 4: перевірка банкрутства
    public bool CheckBankruptcy(Player player)
    {
        if (player.Money <= 0)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine(
                $"{player.Name} збанкрутував!");
            Console.WriteLine("ГРУ ЗАВЕРШЕНО.");
            Console.WriteLine("=================================");

            return true;
        }

        return false;
    
    }
}

class Program
{
    // Метод 1: налаштування консолі
    static void ConfigureConsole()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
    }

    // Метод 2: головний метод програми
    static void Main()
    {
        ConfigureConsole();

        Game game = new Game();
        game.Start();

        Console.WriteLine();
        Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}