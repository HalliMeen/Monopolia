 namespace Monopolia
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Board
    {
        private readonly BoardCell[] cells;

        private readonly Dictionary<string, List<StreetProperty>> colorGroupGraph;

        public int Size => cells.Length;

        public Board()
        {
            cells = BoardLoader.Load(Path.Combine(AppContext.BaseDirectory, "board.json"));

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

                cell.Board = this; // щоб вулиця могла звертатися до графа
            }
        }

        // Скільки вулиць цього кольору має гравець
        public int CountOwnedInGroup(string colorGroup, Player player)
        {
            return GetStreetGroup(colorGroup).Count(s => s.Owner == player);
        }

        // Скількох вулиць не вистачає до монополії
        public int MissingForMonopoly(string colorGroup, Player player)
        {
            return GetStreetGroup(colorGroup).Count - CountOwnedInGroup(colorGroup, player);
        }

        // Усі кольори, де гравець має монополію
        public IEnumerable<string> GetMonopolies(Player player)
        {
            return colorGroupGraph.Keys.Where(color => HasFullMonopoly(color, player));
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