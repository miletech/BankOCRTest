using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BankOCR.Extensions
{
    public static class AccountEntryExtension
    {
        public static void WriteAccountInfoToFile(this List<AccountEntry> entries, string filePath)
        {
            using (StreamWriter writetext = new StreamWriter(filePath))
            {
                foreach (var entry in entries)
                {
                    writetext.WriteLine(entry.AccountStatus);
                }
            }
        }

        public static void WriteAccountPredictionsToFile(this List<AccountEntry> entries, string filePath)
        {
            using (StreamWriter writetext = new StreamWriter(filePath))
            {
                foreach(var entry in entries)
                {
                    writetext.WriteLine(entry.AccountPrediction);
                }
            }
        }
    }
}
