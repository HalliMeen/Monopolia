using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace Monopolia
{
    public class GameResult
    {
        public DateTime Date { get; set; }
        public List<string> Players { get; set; } = new List<string>();
        public string Winner { get; set; } = "";
    }

    public static class GameHistory
    {
        private static readonly string FilePath =
            Path.Combine(AppContext.BaseDirectory, "game_history.json");

        public static void Save(List<string> playerNames, string winnerName)
        {
            List<GameResult> history = new List<GameResult>();

            if (File.Exists(FilePath))
            {
                string oldJson = File.ReadAllText(FilePath);

                if (!string.IsNullOrWhiteSpace(oldJson))
                {
                    history = JsonSerializer.Deserialize<List<GameResult>>(oldJson)
                              ?? new List<GameResult>();
                }
            }

            history.Add(new GameResult
            {
                Date = DateTime.Now,
                Players = playerNames,
                Winner = winnerName
            });

            string json = JsonSerializer.Serialize(
                history,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(FilePath, json);
        }
    }
}