using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Indygo.License
{
    public class Generator
    {
        public enum FileFormat { CSV, NewLine, Delimeter, JSON, Excel }
        public enum Letter { Uppercase, Lowercase }

        public int Progress { get; }

        private readonly char[] validChars = { 'C', 'c', 'N' };
        private string format = "CNNCN-NNCNN-NNNCC-CCNNC"; // default format

        Random rnd;

        public Generator()
        {
            format = "";
            rnd = new Random();
        }
        public Tuple<bool, string> SetFormat(string format)
        {
            string message;
            var checkFormat = IsKeyValidFormat(format, out message);
            if (format != "") { this.format = format; }

            return new Tuple<bool, string>(checkFormat, message);
        }
        private bool IsKeyValidFormat(string format, out string errorMessage)
        {
            errorMessage = "";
            if (format.Length < 1)
            {
                errorMessage = "Format length is 0.";
                return false;
            }

            var randomChars = format.Where(c => validChars.Contains(c));
            
            if (randomChars.Count() < 1)
            {
                errorMessage = "At least one random character is required: 'C', 'c', 'N'";
                return false;
            }
            return true;
        }
        public object Generate(int count, FileFormat saveFormat = FileFormat.CSV, string fileDirectory, char delimeter = ',')
        {
            //TODO validate directory
            if (count <= 0) { throw new IllegalArgumentException("Parameter 'count' must contain an integer greater than 0."); }

            List<string> licenses = new List<string>(); // Temporary
            
            int lowercaseCount = format.Count(c => c == 'c');
            int uppercaseCount = format.Count(c => c == 'C');
            int numberCount    = format.Count(c => c == 'N');

            string generation = "";
            
            for (int i = 0; i < count; i++)
            {
                var lowercaseChars = GenerateRandomLetters(Letter.Lowercase, lowercaseCount);
                var uppercaseChars = GenerateRandomLetters(Letter.Uppercase, uppercaseCount);
                var numberChars    = GenerateRandomNumbers(numberCount);

                for (int j = 0; j < format.Length; j++)
                {
                    switch(format[j])
                    {
                        case 'N':
                            generation = generation + numberChars[rnd.Next(0, numberChars.Length - 1)].ToString(); 
                            break;
                        case 'C':
                            generation = generation + uppercaseChars[rnd.Next(0, uppercaseChars.Length - 1)].ToString();
                            break;
                        case 'c':
                            generation = generation + lowercaseChars[rnd.Next(0, lowercaseChars.Length - 1)].ToString();
                            break;
                        default:
                            generation = generation + format[j].ToString();
                            break;
                    }
                }
                licenses.Add(generation);
                generation = "";
            }
            return licenses;
        }
        private void Save(FileFormat format)
        {

        }
        
        private async Task<char[]> GenerateRandomLettersAsync(Letter letterOption, int count)
        {
            var task = Task.Run(() => GenerateRandomLetters(letterOption, count));
            char[] chars = await task;
            return chars;
        }
        private char[] GenerateRandomLetters(Letter letterOption, int count)
        {
            char[] chars = new char[count];
            for (int i = 0; i < count; i++)
            {
                int n = rnd.Next(0, 26);
                char c = (char)(n + 65);

                if (letterOption == Letter.Uppercase) { c = char.ToUpper(c); }
                chars[i] = c;
            }
            return chars;
        }
        private async Task<int[]> GenerateRandomNumbersAsync(int count)
        {
            var task = Task.Run(() => GenerateRandomNumbers(count));
            int[] numbers = await task;
            return numbers;
        }
        private int[] GenerateRandomNumbers(int count)
        {
            int[] numbers = new int[count];
            for (int i = 0; i < count; i++)
            {
                numbers[i] = rnd.Next(0, 9);
            }
            return numbers;
        }
    }
}