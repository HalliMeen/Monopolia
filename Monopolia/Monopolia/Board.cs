namespace MonopoliaLab2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Board
    {
        private readonly BoardCell[] cells;

        private readonly Dictionary<string, List<StreetProperty>> colorGroupGraph;

        public int Size => cells.Length;

        public Board()
        {
            cells = new BoardCell[]
            {
                // 1. Старт
                new BonusCell("Старт", 200),                                   

                // 2. Бонус
                new BonusCell("Премія за старанність", 1000),                    

                // (Коричнева група)
                new StreetProperty("Проспект Перемоги", 600, "Brown", 20),
                new StreetProperty("Вулиця Дерибасівська", 600, "Brown", 40),
                new StreetProperty("Вулиця Пушкінська", 800, "Brown", 40),       

                // Податкова
                new TaxCell("Податок на доходи", 500),                         

                // Шанс
                new ChanceCell("Зона удачі"),                                  

                // (Блакитна група)
                new StreetProperty("Вулиця Сумська", 1000, "LightBlue", 60),
                new StreetProperty("Проспект Гагаріна", 1200, "LightBlue", 80),
                new StreetProperty("Вулиця Соборна", 1400, "LightBlue", 100),    

                // Бонус
                new BonusCell("Знайдено скарб", 1500),                          

                // Курорт
                new ResortCell("Курорт"),                        

                // (Рожева)
                new StreetProperty("Вулиця Лесі Українки", 1400, "Pink", 100),
                new StreetProperty("Вулиця Франка", 1600, "Pink", 120),
                new StreetProperty("Вулиця Городоцька", 1800, "Pink", 140),      

                // (Помаранчева)
                new StreetProperty("Вулиця Шевченка", 1800, "Orange", 140),
                new StreetProperty("Вулиця Свободи", 2000, "Orange", 160),
                new StreetProperty("Проспект Чорновола", 2200, "Orange", 180),   

                // Податкова
                new TaxCell("Податок на майно", 500),                          

                // Шанс
                new ChanceCell("Зона удачі"),                                 

                // (Червона група)
                new StreetProperty("Собор", 2200, "Red", 180),
                new StreetProperty("Хрещатик", 2400, "Red", 200),
                new StreetProperty("Вулиця Набережна", 2400, "Red", 200),        

                // В'язниця
                new PoliceCell("Тюрма"),                       

                // Шанс
                new ChanceCell("Зона удачі"),                                  

                // (Жовта група)
                new StreetProperty("Вулиця Зелена", 2600, "Yellow", 220),
                new StreetProperty("Вулиця Личаківська", 2800, "Yellow", 240),
                new StreetProperty("Приморський бульвар", 3000, "Yellow", 260),  

                // Бонус
                new BonusCell("Спадщина", 2000),                                

                // Шанс
                new ChanceCell("Зона удачі"),                                  

                // (Зелена)
                new StreetProperty("Французький бульвар", 3000, "Green", 260),
                new StreetProperty("Катерининська площа", 3200, "Green", 280),
                new StreetProperty("Вулиця Артема", 3500, "Green", 320),         

                // (Темно-синя)
                new StreetProperty("Андріївський узвіз", 3500, "DarkBlue", 500),
                new StreetProperty("Бульвар Шевченка", 3800, "DarkBlue", 400),
                new StreetProperty("Центральна площа", 4000, "DarkBlue", 450)
            };

            // Ініціалізація та побудова графа колірних груп при створенні дошки
            colorGroupGraph = new Dictionary<string, List<StreetProperty>>();
            BuildColorGroupGraph();
        }

        private void BuildColorGroupGraph()
        {
            foreach (var cell in cells.OfType<StreetProperty>())
            {
                if (!colorGroupGraph.ContainsKey(cell.ColorGroup))
                {
                    colorGroupGraph[cell.ColorGroup] = new List<StreetProperty>();
                }
                colorGroupGraph[cell.ColorGroup].Add(cell);
            }
        }

        public int GetNewPosition(int currentPosition, int steps)
        {
            return (currentPosition + steps) % Size;
        }

        public BoardCell GetCell(int position)
        {
            if (position >= 0 && position < Size)
            {
                return cells[position];
            }
            throw new ArgumentOutOfRangeException(nameof(position), "Такої клітинки не існує");
        }

        // Оптимізована перевірка монополії за допомогою побудованого графа
        public bool HasFullMonopoly(string colorGroup, Player player)
        {
            if (!colorGroupGraph.ContainsKey(colorGroup)) return false;

            var groupStreets = colorGroupGraph[colorGroup];

        
            return groupStreets.All(street => player.OwnedProperties.Contains(street));
        }

        // Додатковий метод графа: отримати всі сусіди (вулиці) певної колірної групи
        public IReadOnlyList<StreetProperty> GetStreetGroup(string colorGroup)
        {
            if (colorGroupGraph.ContainsKey(colorGroup))
            {
                return colorGroupGraph[colorGroup].AsReadOnly();
            }
            return new List<StreetProperty>().AsReadOnly();
        }
    }
}