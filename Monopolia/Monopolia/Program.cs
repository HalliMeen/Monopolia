namespace MonopoliaLab2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Монополія — Початок гри ===");

            int playerCount = 0;
            while (playerCount < 2 || playerCount > 4)
            {
                Console.Write("Введіть кількість гравців (2-4): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out playerCount) || playerCount < 2 || playerCount > 4)
                {
                    Console.WriteLine("Неправильне значення. Спробуйте ще раз.");
                    playerCount = 0;
                }
            }

            var availableTokens = Enum.GetValues(typeof(Player.Token)).Cast<Player.Token>().ToList();
            var players = new List<Player>();

            for (int i = 1; i <= playerCount; i++)
            {
                Console.Write($"Ім'я гравця #{i}: ");
                var name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = $"Гравець {i}";
                }

                Player.Token chosen = availableTokens[0];
                bool chosenOk = false;
                while (!chosenOk)
                {
                    Console.WriteLine("Виберіть фішку:");
                    for (int t = 0; t < availableTokens.Count; t++)
                    {
                        Console.WriteLine($"  {t + 1}. {availableTokens[t]}");
                    }
                    Console.Write("Введіть номер фішки: ");
                    var tokInput = Console.ReadLine();
                    if (int.TryParse(tokInput, out int tokIndex) && tokIndex >= 1 && tokIndex <= availableTokens.Count)
                    {
                        chosen = availableTokens[tokIndex - 1];
                        availableTokens.RemoveAt(tokIndex - 1);
                        chosenOk = true;
                    }
                    else
                    {
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    }
                }

                players.Add(new Player(name, chosen));
                Console.WriteLine($"Додано {name} з фішкою {chosen}\n");
            }

            var board = new Board();
            var dice = new Dice();

            Console.WriteLine("Гравці готові:");
            foreach (var p in players)
            {
                Console.WriteLine($"- {p.Name} (Фішка: {p.PlayerToken}) Баланс: ${p.Balance}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для початку гри...");
            Console.ReadKey(true);

            var activePlayers = players.Where(p => !p.IsBankrupt).ToList();
            int currentIndex = 0;

            while (activePlayers.Count > 1)
            {
                var current = activePlayers[currentIndex % activePlayers.Count];

                if (current.IsBankrupt)
                {
                    activePlayers.RemoveAt(currentIndex % activePlayers.Count);
                    if (activePlayers.Count <= 1) break;
                    continue;
                }

                // --- Обробка перебування на Курорті ---
                if (current.Curort)
                {
                    Console.WriteLine($"\n{current.Name} знаходиться на курорті і пропускає хід.");
                    current.JailTurnsCount--;

                    if (current.JailTurnsCount <= 0)
                    {
                        current.Curort = false;
                        current.JailTurnsCount = 0;
                        Console.WriteLine($"{current.Name} повернувся з курорту і може грати наступного разу.");
                    }

                    currentIndex++;
                    continue;
                }

                // --- Обробка перебування в Тюрмі ---
                if (current.Jail)
                {
                    Console.WriteLine($"\n{current.Name} знаходиться в тюрмі і пропускає хід. Залишилось пропустити: {current.JailTurnsCount}");
                    current.JailTurnsCount--;

                    if (current.JailTurnsCount <= 0)
                    {
                        current.Curort = false;
                        current.JailTurnsCount = 0;
                        Console.WriteLine($"{current.Name} вийшов з тюрми і може грати наступного разу.");
                    }

                    currentIndex++;
                    continue;
                }

                // --- Основний хід гравця ---
                Console.WriteLine($"\nХід гравця: {current.Name} (Баланс: ${current.Balance}) — натисніть клавішу для кидка кубика...");
                Console.ReadKey(true);

                var monopolies = board.GetMonopolies(current).ToList();
                if (monopolies.Count > 0)
                    Console.WriteLine($"Ваші монополії: {string.Join(", ", monopolies)}");

                int roll = dice.Roll();
                Console.WriteLine($"{current.Name} викинув: {roll}");

                int newPos = board.GetNewPosition(current.Position, roll);
                current.Position = newPos;
                var cell = board.GetCell(newPos);
                Console.WriteLine($"{current.Name} став на клітинку: {cell.Name}");

                if (cell is StreetProperty sp)
                {
                    if (sp.Owner == null)
                    {
                        Console.WriteLine($"Філія {sp.Name} доступна для купівлі за ${sp.PurchasePrice}. Ваш баланс: ${current.Balance}");
                        if (current.Balance >= sp.PurchasePrice)
                        {
                            int owned = board.CountOwnedInGroup(sp.ColorGroup, current);
                            int total = board.GetStreetGroup(sp.ColorGroup).Count;
                            Console.WriteLine($"У групі {sp.ColorGroup} у вас {owned} з {total}.");
                            if (board.MissingForMonopoly(sp.ColorGroup, current) == 1)
                            {
                                Console.WriteLine("Купівля цієї вулиці дасть вам монополію (оренда ×3)!");
                            }
                            Console.Write("Бажаєте купити? (y/n): ");
                            var ans = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(ans) && ans.ToLower().StartsWith("y"))
                            {
                                current.Balance -= sp.PurchasePrice;
                                sp.Owner = current;
                                current.OwnedProperties.Add(sp);
                                Console.WriteLine($"{current.Name} купив {sp.Name} за ${sp.PurchasePrice}. Новий баланс: ${current.Balance}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Недостатньо коштів для купівлі.");
                        }
                    }
                    else if (sp.Owner != current)
                    {
                        if (sp.IsMortgaged)
                        {
                            Console.WriteLine("Ця філія закладена — оренда не стягується.");
                        }
                        else
                        {
                            int rent = sp.GetCurrentRent();
                            Console.WriteLine($"Потрібно заплатити оренду ${rent} власнику {sp.Owner.Name}.");
                            current.Balance -= rent;
                            sp.Owner.Balance += rent;
                            Console.WriteLine($"Після оплати баланс {current.Name}: ${current.Balance}, баланс {sp.Owner.Name}: ${sp.Owner.Balance}");
                        }
                    }
                }
                else
                {
                    cell.OnPlayerLand(current);
                }

                // --- Перевірка банкрутства ---
                if (current.Balance <= 0)
                {
                    Console.WriteLine($"У {current.Name} баланс ${current.Balance}. Перевіряємо можливості продажу філій...");
                    TrySellPropertiesToCoverDebt(current, board);

                    if (current.Balance <= 0)
                    {
                        Console.WriteLine($"{current.Name} став банкрутом і вибуває з гри.");
                        foreach (var prop in current.OwnedProperties.ToList())
                        {
                            prop.Owner = null;
                        }
                        current.OwnedProperties.Clear();
                        current.IsBankrupt = true;

                        activePlayers = activePlayers.Where(p => !p.IsBankrupt).ToList();
                        if (currentIndex >= activePlayers.Count) currentIndex = 0;
                        continue;
                    }
                }

                currentIndex++;
                activePlayers = activePlayers.Where(p => !p.IsBankrupt).ToList();
            }

            var winner = players.FirstOrDefault(p => !p.IsBankrupt);
            if (winner != null)
            {
                Console.WriteLine($"\nГра завершена. Переможець: {winner.Name} з балансом ${winner.Balance}!");
            }
            else
            {
                Console.WriteLine("\nГра завершена. Переможців не знайдено.");
            }

            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey(true);
        }

        static void TrySellPropertiesToCoverDebt(Player player, Board board)
        {
            while (player.Balance <= 0 && player.OwnedProperties.Count > 0)
            {
                Console.WriteLine("У вас є філії, які можна продати за половину ціни:");
                for (int i = 0; i < player.OwnedProperties.Count; i++)
                {
                    var p = player.OwnedProperties[i];
                    string mark = board.HasFullMonopoly(p.ColorGroup, player) ? " [МОНОПОЛІЯ]" : "";
                    Console.WriteLine($"  {i + 1}. {p.Name}{mark} - продажна ціна ${p.PurchasePrice / 2}");
                }
                Console.Write("Введіть номер філії для продажу або 'n' щоб відмовитись: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input.Trim().ToLower() == "n") break;
                if (int.TryParse(input, out int idx) && idx >= 1 && idx <= player.OwnedProperties.Count)
                {
                    var prop = player.OwnedProperties[idx - 1];
                    int sale = prop.PurchasePrice / 2;
                    player.Balance += sale;
                    prop.Owner = null;
                    player.OwnedProperties.RemoveAt(idx - 1);
                    Console.WriteLine($"Продано {prop.Name} за ${sale}. Новий баланс: ${player.Balance}");
                }
                else
                {
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                }
            }
        }
    }
}