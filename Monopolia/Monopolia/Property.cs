namespace MonopoliaLab2
{
    using System;

    public abstract class BoardCell
    {
        public string Name { get; }

        protected BoardCell(string name)
        {
            Name = name;
        }
        public abstract void OnPlayerLand(Player visitor);

    }

    public class StreetProperty : BoardCell
    {
        public string ColorGroup { get; }
        public int BaseRent { get; }
        public int PurchasePrice { get; }
        public Player? Owner { get; set; } = null;
        public bool IsMortgaged { get; set; } = false;
        public Board? Board { get; set; }

        public StreetProperty(string name, int price, string color, int baseRent)
            : base(name)
        {
            ColorGroup = color;
            PurchasePrice = price;
            BaseRent = baseRent;
        }

        public int GetCurrentRent()
        {
            if (IsMortgaged || Owner == null) return 0;
            if (Board == null) return BaseRent;

            if (Board.HasFullMonopoly(ColorGroup, Owner)) return BaseRent * 3;
            if (Board.CountOwnedInGroup(ColorGroup, Owner) == 2) return BaseRent * 2;

            return BaseRent;
        }
        public override void OnPlayerLand(Player visitor)
        {
            if (Owner == null)
            {
                Console.WriteLine($"Нерухомість {Name} вільна. {visitor.Name} Може її купити за ${PurchasePrice}.");
                return;
            }

            if (Owner == visitor)
            {
                Console.WriteLine($"Нерухомість {visitor.Name} став на свою власну філію {Name}.");
                return;
            }

            if (IsMortgaged)
            {
                Console.WriteLine($"Нерухомість Філія {Name} закладена. Оренда не знімається.");
                return;
            }

            int rentAmount = GetCurrentRent();
            visitor.Balance -= rentAmount;
            Owner.Balance += rentAmount;

            Console.WriteLine($"Оренда {visitor.Name} заплатив ${rentAmount} оренду для {Owner.Name}!");
        }
    }

    //ШАНС
    public class ChanceCell : BoardCell
    {
        private Random random = new Random();
        public ChanceCell(string name) : base(name) { }
        public override void OnPlayerLand(Player visitor)
        {
            Console.WriteLine($"ШАНС {visitor.Name} випробовує удачу!");

            int eventid = random.Next(1, 10);
            if (eventid <= 3)
            {
                Console.WriteLine("Удача на вашому боці отримуєте 500");
                visitor.Balance += 500;
            }

            else if (eventid <= 6)
            {
                Console.WriteLine("Улюбленець удачі отримуєте 1000");
                visitor.Balance += 1000;
            }
            else
            {
                Console.WriteLine("ОГО та ти в ударі друже отримуй 2000");
                visitor.Balance += 2000;
            }
        }
    }

    public class BonusCell : BoardCell
    {
        public int BonusAmount { get; }
        private Random random = new Random();

        public BonusCell(string name, int bonusAmount) : base(name)
        {
            BonusAmount = bonusAmount;
        }

        public override void OnPlayerLand(Player visitor)
        {
            int eventid = random.Next(1, 3);
            if (eventid == 1)
            {
                visitor.Balance += 5000;
                Console.WriteLine("Бабина пенсія прийшла");
                Console.WriteLine($"Баланс {visitor.Balance}");
            }
            else
            {
                visitor.Balance += 5000;
                Console.WriteLine("Вкрали в циганки гроші");
                Console.WriteLine($"Баланс {visitor.Balance}");
            }
        }
    }
    public class RestCell : BoardCell
    {
        public RestCell(string name) : base(name) { }

        public override void OnPlayerLand(Player visitor)
        {
            visitor.JailTurnsCount = 1;
            PrintLandingMessage(visitor);
        }

        protected virtual void PrintLandingMessage(Player visitor)
        {
            Console.WriteLine($"{visitor.Name} потрапив(ла) на {Name}.");
        }
    }

    // Тюрма
    public class PoliceCell : RestCell
    {
        public PoliceCell(string v) : base("Тюрма") { }

        public override void OnPlayerLand(Player visitor)
        {
            visitor.Jail = true;
            base.OnPlayerLand(visitor);
        }

        protected override void PrintLandingMessage(Player visitor)
        {
            Console.WriteLine($"\n{visitor.Name} затримано поліцією і відправлено в тюрму!");
        }
    }

    // Курорт
    public class ResortCell : RestCell
    {
        public ResortCell(string v) : base("Курорт") { }

        public override void OnPlayerLand(Player visitor)
        {
            visitor.Curort = true;
            base.OnPlayerLand(visitor);
        }

        protected override void PrintLandingMessage(Player visitor)
        {
            Console.WriteLine($"\n{visitor.Name} відпочиває на курорті та пропускає наступний хід.");
        }
    }
    //Податок
    public class TaxCell : BoardCell
    {
        public int TaxPerProperty { get; }

        public TaxCell(string name, int taxPerProperty) : base(name)
        {
            TaxPerProperty = taxPerProperty;
        }

        public override void OnPlayerLand(Player visitor)
        {
            int totalOwnedProperties = visitor.OwnedProperties.Count;

            if (totalOwnedProperties == 0)
            {
                Console.WriteLine($"{visitor.Name} став на {Name} оскільки немає філій податок становить 0");
                return;
            }

            int totalTax = totalOwnedProperties * TaxPerProperty;

            visitor.Balance -= totalTax;

            Console.WriteLine($"{visitor.Name} має у власності {totalOwnedProperties}");
            Console.WriteLine($"Знято грошей {totalOwnedProperties} x ${TaxPerProperty} = ${totalTax}");
            Console.WriteLine($"Баланс {visitor.Name} становить ${visitor.Balance}");
        }
    }
}
