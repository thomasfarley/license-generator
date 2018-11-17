using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Indygo.License
{
    public class Generator
    {
        public enum FileFormat { CSV, NewLine, CustomDelimeter, CustomDelimeterNewline, JSON, Excel }
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
        public List<string> Generate(int count, bool scrubDuplications = true)
        {
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
                    switch (format[j])
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
        public void Save(List<string> licenses, string filePath, FileFormat format = FileFormat.CSV, string customDelimeter = ",")
        {
            if (licenses.Count < 1) { return; }
            if (string.IsNullOrEmpty(filePath)) { return; }
            if (format == FileFormat.Excel) { throw new NotImplementedException(); }

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch
                {
                    throw;
                }
            }
            try
            {
                Write(ref licenses, filePath, format, customDelimeter);
            }
            catch
            {
                throw;
            }

        }
        private void Write(ref List<string> data, string filePath, FileFormat format, string delimeter)
        {
            switch (format)
            {
                case FileFormat.CSV:
                    delimeter = ",";
                    break;
                case FileFormat.NewLine:
                    delimeter = Environment.NewLine;
                    break;
                case FileFormat.CustomDelimeterNewline:
                    delimeter = delimeter + Environment.NewLine;
                    break;
                case FileFormat.JSON: //JSON array elements are separated by comma
                    delimeter = ",";
                    break;
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Append);
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    if (format == FileFormat.JSON)
                    {
                        writer.Write("[");
                        for (int i = 0; i < data.Count - 2; i++)
                        {
                            writer.Write("\"" + data[i] + "\"" + delimeter);
                        }
                        writer.Write("\"" + data[data.Count - 1] + "\"");
                        writer.Write("]");
                    }
                    else
                    {
                        for (int i = 0; i < data.Count - 1; i++)
                        {
                            writer.Write(data[i] + delimeter);
                        }
                    }
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
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

                if (letterOption == Letter.Lowercase) { c = char.ToLower(c); }
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