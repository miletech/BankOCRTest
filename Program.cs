using BankOCR.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BankOCR
{
    class Program
    {
        private const int BUFFER_SIZE = 27;

        static void Main(string[] args)
        {
            while (true)
            {
                List<string> fileList = new List<string>();

                Console.WriteLine("Introduce the list of input files. After this type execute to start.");

                string command = Console.ReadLine();

                do
                {
                    if (File.Exists(command))
                    {
                        fileList.Add(command);
                    }
                    else
                    {
                        Console.WriteLine("File does not exist.");
                    }

                    command = Console.ReadLine();

                } while (command.ToLower() != "execute");

                Console.WriteLine("Introduce the output directory and press enter.");

                string directory = Console.ReadLine();

                if (Directory.Exists(directory))
                {
                    foreach (var filePath in fileList)
                    {
                        List<AccountEntry> entries = new List<AccountEntry>();

                        using (var fileStream = File.OpenRead(filePath))
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BUFFER_SIZE))
                        {
                            string line;

                            while ((line = streamReader.ReadLine()) != null)
                            {
                                entries.Add(new AccountEntry(line, streamReader.ReadLine(), streamReader.ReadLine(), streamReader.ReadLine()));
                            }
                        }

                        entries.WriteAccountInfoToFile(Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(filePath)}_info.txt"));
                        entries.WriteAccountPredictionsToFile(Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(filePath)}_predictions.txt"));
                    }
                }
                else
                {
                    Console.Write("Incorrect directory.");
                }
            }
        }
    }
}
