
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Monopolia
{
    public class BoardLoader
    {
        public static BoardCell[] Load(string filePath)
        {
            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "Файл игрового поля не найден: " + filePath);
            }

            // Читаем содержимое файла
            string json = File.ReadAllText(filePath);

            // Превращаем JSON в данные, понятные C#
            using JsonDocument document = JsonDocument.Parse(json);

            var cells = document.RootElement.EnumerateArray()
                .Select(cell =>
                {
                    string type = cell.GetProperty("type").GetString()!;
                    string name = cell.GetProperty("name").GetString()!;

                    return type switch
                    {
                        "Street" => (BoardCell)new StreetProperty(
                            name,
                            cell.GetProperty("price").GetInt32(),
                            cell.GetProperty("color").GetString()!,
                            cell.GetProperty("rent").GetInt32()),

                        "Bonus" => new BonusCell(
                            name,
                            cell.GetProperty("amount").GetInt32()),

                        "Tax" => new TaxCell(
                            name,
                            cell.GetProperty("amount").GetInt32()),

                        "Chance" => new ChanceCell(name),

                        "Police" => new PoliceCell(name),

                        "Resort" => new ResortCell(name),

                        _ => throw new InvalidDataException(
                            "Неизвестный тип клетки: " + type)
                    };
                })
                .ToArray();

            if (cells.Length != 36)
            {
                throw new InvalidDataException(
                    "На игровом поле должно быть ровно 36 клеток.");
            }

            return cells;
        }
    }
}
