namespace Monopolia
{
    using System;
    using System.IO;

    public static class Logger
    {
        private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "game_log.txt");

        // Ініціалізація нового логу при старті гри
        public static void StartNewLog()
        {
            File.WriteAllText(FilePath, $"=== Нова гра розпочата {DateTime.Now} ===\n");
        }

        // Виводить текст у консоль та дублює його у файл логування
        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
            LogToFile(message);
        }

        // Записує повідомлення лише у файл (можна використовувати для прихованих технічних логів)
        public static void LogToFile(string message)
        {
            try
            {
                File.AppendAllText(FilePath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
            }
            catch
            {
                // Ігноруємо помилки доступу до файлу, щоб гра не впала
            }
        }
    }
}