using System;

namespace CoursesDownloader.AdvancedIO.ConsoleHelpers
{
    public static class ConsoleUtils
    {
        #region With ConsoleColor

        public static string ReadLine(string message = null, ConsoleColor? messageColor = null, ConsoleColor? inputColor = null, bool isPassword = false)
        {
            Write(message, messageColor);

            var colorChanged = ChangeColor(inputColor);
            var input = isPassword ? ReadPassword() : Console.ReadLine();

            ResetColors(colorChanged);

            return input;
        }

        public static void WriteLine(string message = null, ConsoleColor? messageColor = null)
        {
            var colorChanged = ChangeColor(messageColor);
            Console.WriteLine(message);

            ResetColors(colorChanged);
        }

        public static void Write(string message = null, ConsoleColor? messageColor = null)
        {
            var colorChanged = ChangeColor(messageColor);
            Console.Write(message);

            ResetColors(colorChanged);
        }

        private static bool ChangeColor(ConsoleColor? color)
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            return color.HasValue;
        }

        #endregion

        #region With ConsoleIOType

        public static string ReadLine(string message, ConsoleIOType messageType, bool isPassword = false)
        {
            return ReadLine(
                message, 
                (ConsoleColor) messageType, 
                (ConsoleColor) ConsoleIOType.Input, 
                isPassword);
        }

        public static void WriteLine(string message, ConsoleIOType messageType)
        {
            WriteLine(
                message,
                (ConsoleColor)messageType
                );
        }

        public static void Write(string message, ConsoleIOType messageType)
        {
            Write(
                message,
                (ConsoleColor) messageType
            );
        }
        
        #endregion

        private static void ResetColors(bool shouldReset = true)
        {
            if (shouldReset)
            {
                Console.ResetColor();
            }
        }

        private static string ReadPassword()
        {
            var password = "";
            while (true)
            {
                var inputKey = Console.ReadKey(true);

                if (inputKey.Key != ConsoleKey.Backspace && inputKey.Key != ConsoleKey.Enter)
                {
                    password += inputKey.KeyChar;
                    Console.Write("*");
                }
                else if (inputKey.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
            }

            return password;
        }
    }
}