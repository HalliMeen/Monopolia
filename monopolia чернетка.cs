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
        Money = 2000;
        Position = 0;
        SkipNextTurn = false;
    }
}

class Property
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int Rent { get; set; }
    public Player Owner { get; set; }

    public Property(string name, int price, int rent)
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

        // Сектор №2
        Properties.Add(new Property("Нагорода", 500, 0));

        // Сектори №3-5
        Properties.Add(new Property("Смачна Кава", 300, 50));
        Properties.Add(new Property("Легкий Круасан", 400, 70));
        Properties.Add(new Property("Золотий Чай", 500, 90));

        // Сектори №6-7
        Properties.Add(new Property("Нагорода", 500, 0));
        Properties.Add(new Property("Штраф", 700, 0));

        // Сектори №8-10
        Properties.Add(new Property("УКР Пошта", 500, 90));
        Properties.Add(new Property("Нова Пошта", 600, 110));
        Properties.Add(new Property("Meest Пошта", 700, 130));

        // Сектор №11
        Properties.Add(new Property("Шанс", 0, 0));

        // Сектор №12
        Properties.Add(new Property("Покарання", 0, 0));

        // Сектори №13-15
        Properties.Add(new Property("ГрандКіно", 700, 130));
        Properties.Add(new Property("ФлайПарк", 800, 150));
        Properties.Add(new Property("АкваСвіт", 900, 170));

        // Сектори №16-18
        Properties.Add(new Property("Rozzetka", 900, 170));
        Properties.Add(new Property("Foxtrot", 1000, 190));
        Properties.Add(new Property("Eldorado", 1100, 210));

        // Сектор №19
        Properties.Add(new Property("Покарання", 0, 0));

        // Сектори №20-22
        Properties.Add(new Property("Zara", 1100, 210));
        Properties.Add(new Property("Tommy Hilfiger", 1200, 230));
        Properties.Add(new Property("H&M", 1300, 250));

        // Сектори №23-24
        Properties.Add(new Property("Нагорода", 500, 0));
        Properties.Add(new Property("Штраф", 700, 0));

        // Сектор №25
        Properties.Add(new Property("Гіпер Шанс", 0, 0));

        // Сектори №26-28
        Properties.Add(new Property("Mercedes", 1300, 250));
        Properties.Add(new Property("Audi", 1400, 270));
        Properties.Add(new Property("Volkswagen", 1500, 290));

        // Сектор №29
        Properties.Add(new Property("Шанс", 0, 0));

        // Сектор №30
        Properties.Add(new Property("Відпочинок", 0, 0));

        // Сектори №31-33
        Properties.Add(new Property("Ryanair", 1500, 310));
        Properties.Add(new Property("Wizz Air", 1600, 330));
        Properties.Add(new Property("Lufthansa", 1700, 350));

        // Сектори №34-36
        Properties.Add(new Property("HSBC Bank", 1700, 370));
        Properties.Add(new Property("ICBC Bank", 1800, 390));
        Properties.Add(new Property("GOLD Bank", 1900, 410));
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
        Players = new List<Player>();
        Board = new Board();
        Dice = new Dice();

        Players.Add(new Player("Гравець 1"));
        Players.Add(new Player("Гравець 2"));
    }

    public void Start()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("        МІНІ-МОНОПОЛІЯ");
        Console.WriteLine("=================================");

        int currentPlayer = 0;

        while (true)
        {
            Player player = Players[currentPlayer];

            // Перевірка пропуску ходу
            if (player.SkipNextTurn)
            {
                Console.WriteLine();
                Console.WriteLine($"⏭️ {player.Name} пропускає наступний хід.");

                player.SkipNextTurn = false;

                currentPlayer++;

                if (currentPlayer >= Players.Count)
                {
                    currentPlayer = 0;
                }

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

            Console.WriteLine($"{player.Name} викинув: {dice}");

            // Запам'ятовуємо стару позицію
            int oldPosition = player.Position;

            // Рух гравця
            player.Position += dice;

            // Перевіряємо, чи гравець пройшов СТАРТ
            if (player.Position >= Board.Properties.Count)
            {
                player.Position -= Board.Properties.Count;

                // Нарахування $200 за проходження старту
                player.Money += 200;

                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("        🎯 ПРОХІД СТАРТУ!");
                Console.WriteLine("=================================");
                Console.WriteLine("💰 Ви отримали $200!");
                Console.WriteLine($"Ваш баланс: ${player.Money}");
            }

            // Номер сектора
            int sectorNumber = player.Position + 1;

            // Поточний сектор
            Property property = Board.Properties[player.Position];

            Console.WriteLine();
            Console.WriteLine(
                $"📍 Сектор №{sectorNumber}: {property.Name}"
            );

            // ==========================
            // СТАРТ
            // ==========================

            if (property.Name == "СТАРТ")
            {
                Console.WriteLine();
                Console.WriteLine("🏁 Ви знаходитеся на СТАРТІ!");
            }

            // ==========================
            // НАГОРОДА
            // ==========================

            else if (property.Name == "Нагорода")
            {
                player.Money += 500;

                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("        🎉 НАГОРОДА!");
                Console.WriteLine("=================================");
                Console.WriteLine("💰 Ви отримали $500!");
                Console.WriteLine($"Ваш баланс: ${player.Money}");
            }

            // ==========================
            // ШТРАФ
            // ==========================

            else if (property.Name == "Штраф")
            {
                player.Money -= 700;

                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("        🚨 ШТРАФ!");
                Console.WriteLine("=================================");
                Console.WriteLine("💰 Ви заплатили штраф $700!");
                Console.WriteLine($"Ваш баланс: ${player.Money}");
            }

            // ==========================
            // ШАНС
            // ==========================

            else if (property.Name == "Шанс")
            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("          🎲 СЕКТОР ШАНСУ");
                Console.WriteLine("=================================");

                Console.WriteLine(
                    "Натисніть ENTER, щоб кинути кубик шансу..."
                );

                Console.ReadLine();

                int chance = Dice.Roll(10);

                Console.WriteLine($"Вам випало число: {chance}");

                if (chance <= 3)
                {
                    player.Money -= 300;

                    Console.WriteLine(
                        "🚨 Ви отримали штраф $300."
                    );
                }
                else if (chance <= 6)
                {
                    player.Money += 500;

                    Console.WriteLine(
                        "💰 Ви отримали нагороду $500!"
                    );
                }
                else if (chance <= 9)
                {
                    player.Money += 1000;

                    Console.WriteLine(
                        "🎉 Ви отримали велику нагороду $1000!"
                    );
                }
                else
                {
                    player.Position = 0;

                    Console.WriteLine(
                        "🏁 Ви відправляєтеся на сектор СТАРТ!"
                    );
                }

                Console.WriteLine(
                    $"Ваш баланс: ${player.Money}"
                );
            }

            // ==========================
            // ПОКАРАННЯ
            // ==========================

            else if (property.Name == "Покарання")
            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("          ПОКАРАННЯ");
                Console.WriteLine("=================================");

                Console.WriteLine(
                    "Натисніть ENTER, щоб кинути кубик шансу..."
                );

                Console.ReadLine();

                int chance = Dice.Roll(10);

                Console.WriteLine(
                    $"Вам випало число: {chance}"
                );

                if (chance <= 3)
                {
                    player.Money -= 500;

                    Console.WriteLine(
                        "🚨 Ви отримали штраф $500."
                    );
                }
                else if (chance <= 6)
                {
                    player.Money -= 700;

                    Console.WriteLine(
                        "🚨 Ви отримали штраф $700."
                    );
                }
                else if (chance <= 9)
                {
                    player.Money -= 1000;

                    Console.WriteLine(
                        "🚨 Ви отримали штраф $1000."
                    );
                }
                else
                {
                    player.SkipNextTurn = true;

                    Console.WriteLine(
                        "⏭️ Удача на вашому боці!"
                    );

                    Console.WriteLine(
                        "Замість штрафу ви пропускаєте наступний хід."
                    );
                }

                Console.WriteLine(
                    $"Ваш баланс: ${player.Money}"
                );
            }

            // ==========================
            // ГІПЕР ШАНС
            // ==========================

            else if (property.Name == "Гіпер Шанс")
            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("          🎲 ГІПЕР ШАНС");
                Console.WriteLine("=================================");

                Console.WriteLine(
                    "Ви потрапили на сектор «Гіпер Шанс»!"
                );

                Console.WriteLine();
                Console.WriteLine("Бажаєте кинути кубик?");
                Console.WriteLine("1 - Так");
                Console.WriteLine("2 - Ні");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Натисніть ENTER, щоб кинути кубик..."
                    );

                    Console.ReadLine();

                    // Кубик має 25 граней
                    int chance = Dice.Roll(25);

                    Console.WriteLine();
                    Console.WriteLine(
                        $"🎲 Вам випало число: {chance}"
                    );

                    // Випала 1 — ГРАВЕЦЬ ПЕРЕМАГАЄ
                    if (chance == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("=================================");
                        Console.WriteLine("          🎉 ПЕРЕМОГА! 🎉");
                        Console.WriteLine("=================================");
                        Console.WriteLine(
                            $"🏆 {player.Name} викинув 1!"
                        );
                        Console.WriteLine();
                        Console.WriteLine(
                            $"🏆 {player.Name} ПЕРЕМАГАЄ!"
                        );
                        Console.WriteLine();
                        Console.WriteLine(
                            "🎮 ГРА ЗАКІНЧЕНА!"
                        );
                        Console.WriteLine("=================================");

                        // Завершення гри
                        break;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "❌ Вам не випала 1."
                        );

                        Console.WriteLine(
                            "Гра продовжується."
                        );
                    }
                }
                else if (choice == "2")
                {
                    // Відмова від ризику
                    player.Money += 1000;

                    Console.WriteLine();
                    Console.WriteLine("=================================");
                    Console.WriteLine("       💰 БЕЗПЕЧНИЙ ВИБІР");
                    Console.WriteLine("=================================");

                    Console.WriteLine(
                        "Ви вирішили не кидати кубик."
                    );

                    Console.WriteLine(
                        "💰 Ви отримуєте $1000!"
                    );

                    Console.WriteLine(
                        $"Ваш баланс: ${player.Money}"
                    );

                    Console.WriteLine("=================================");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "❌ Неправильний вибір."
                    );

                    Console.WriteLine(
                        "Гра продовжується."
                    );
                }
            }

            // ==========================
            // ВІДПОЧИНОК
            // ==========================

            else if (property.Name == "Відпочинок")
            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("          ВІДПОЧИНОК");
                Console.WriteLine("=================================");

                player.SkipNextTurn = true;

                Console.WriteLine(
                    "Ви відпочиваєте на цьому секторі!"
                );

                Console.WriteLine(
                    "⏭️ Ви пропускаєте наступний хід."
                );
            }

            // ==========================
            // ЗВИЧАЙНА НЕРУХОМІСТЬ
            // ==========================

            else if (property.Owner == null)
            {
                Console.WriteLine(
                    $"Ціна: ${property.Price}"
                );

                Console.WriteLine(
                    $"Оренда: ${property.Rent}"
                );

                if (player.Money >= property.Price)
                {
                    Console.Write(
                        "Купити нерухомість? (так/ні): "
                    );

                    string answer =
                        Console.ReadLine().ToLower();

                    if (answer == "так")
                    {
                        player.Money -= property.Price;

                        property.Owner = player;

                        Console.WriteLine();
                        Console.WriteLine(
                            $"🏠 Ви придбали {property.Name}!"
                        );

                        Console.WriteLine(
                            $"💰 Залишок грошей: ${player.Money}"
                        );
                    }
                }
                else
                {
                    Console.WriteLine(
                        "❌ У вас недостатньо грошей для покупки."
                    );
                }
            }

            // ==========================
            // ЧУЖА НЕРУХОМІСТЬ
            // ==========================

            else if (property.Owner != player)
            {
                Console.WriteLine(
                    $"👤 Власник: {property.Owner.Name}"
                );

                player.Money -= property.Rent;

                property.Owner.Money += property.Rent;

                Console.WriteLine(
                    $"💸 Ви заплатили оренду ${property.Rent}."
                );

                Console.WriteLine(
                    $"💰 Ваш баланс: ${player.Money}"
                );
            }

            // ==========================
            // ВЛАСНА НЕРУХОМІСТЬ
            // ==========================

            else
            {
                Console.WriteLine(
                    "🏠 Це ваша нерухомість."
                );
            }

            // ==========================
            // ПЕРЕВІРКА ГРАВЦЯ
            // ==========================

            if (player.Money <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("=================================");

                Console.WriteLine(
                    $"💀 {player.Name} втратив усі гроші!"
                );

                Console.WriteLine(
                    $"🏆 ПЕРЕМІГ {Players[1 - currentPlayer].Name}!"
                );

                Console.WriteLine("=================================");

                break;
            }

            Console.WriteLine();
            Console.WriteLine(
                $"💰 Залишок грошей: ${player.Money}"
            );

            // Наступний гравець
            currentPlayer++;

            if (currentPlayer >= Players.Count)
            {
                currentPlayer = 0;
            }
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