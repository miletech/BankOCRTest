using BankOCR.Enums;
using BankOCR.Extensions;
using System;
using System.Collections.Generic;

namespace BankOCR.Helpers
{
    public static class MathHelper
    {
        //Checksum calculation
        public static bool IsCheckSumValid(string accountNumber)
        {
            int checkSum = 0, number, multiplier = 1;

            for (int i = accountNumber.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(accountNumber[i].ToString(), out number))
                {
                    checkSum += multiplier * number;
                }
                else
                {
                    return false;
                }
                multiplier++;
            }

            return checkSum % 11 == 0;
        }

        //Converts an byte array which contains information of the segments which are on
        //Outputs:
        //string: Account number
        //bool: IsIlegible
        public static (string, bool) ConvertSevenSegmentToString(byte[] sevenSegmentArray)
        {
            string accountNumber = string.Empty;
            bool isIlegible = false;

            foreach (var number in sevenSegmentArray)
            {
                if (Enum.IsDefined(typeof(SevenSegments), (int)number))
                {
                    accountNumber += ((SevenSegments)number).GetDescription();
                }
                else
                {
                    accountNumber += "?";
                    isIlegible = true;
                }
            }

            return (accountNumber, isIlegible);
        }

        //Inverts all the possible bits and check that the new account number is valid and eligible
        public static List<string> ObtainPossibleValues(byte[] wrongAccountNumber)
        {
            List<string> possibleValues = new List<string>();

            for (int index = 0; index < wrongAccountNumber.Length; index++)
            {
                for (int mask = 1; mask <= 128; mask = mask << 1)
                {
                    if (Enum.IsDefined(typeof(SevenSegments), (int)wrongAccountNumber[index] ^ mask))
                    {
                        wrongAccountNumber[index] = (byte)((int)wrongAccountNumber[index] ^ mask);

                        var accountInfo = ConvertSevenSegmentToString(wrongAccountNumber);

                        if (!accountInfo.Item2)
                        {
                            if (IsCheckSumValid(accountInfo.Item1))
                            {
                                possibleValues.Add(accountInfo.Item1);
                            }
                        }

                        //Come back to the previous value
                        wrongAccountNumber[index] = (byte)((int)wrongAccountNumber[index] ^ mask);
                    }
                }
            }
            
            return possibleValues;
        }

    }
}
