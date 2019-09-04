using BankOCR.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BankOCR.Helpers
{
    public static class MathHelper
    {
        public static bool IsCheckSumValid(string accountNumber) =>
            accountNumber
                .Reverse()
                .Select((x, i) => (i + 1) * int.Parse(x.ToString()))
                .Sum() % 11 == 0;

        public static (string, bool) ConvertSevenSegmentToString(byte[] sevenSegmentArray)
        {
            var accountNumber = string.Empty;
            var isIllegible = false;

            foreach (var number in sevenSegmentArray)
            {
                if (Enum.IsDefined(typeof(SevenSegments), (int)number))
                {
                    var value = (SevenSegments)number;
                    var type = value.GetType();
                    var digit = type
                        .GetField(Enum.GetName(type, value))
                        .GetCustomAttribute<DescriptionAttribute>()
                        .Description;

                    accountNumber += digit;
                }
                else
                {
                    accountNumber += "?";
                    isIllegible = true;
                }
            }

            return (accountNumber, isIllegible);
        }

        public static List<string> ObtainPossibleValues(byte[] wrongAccountNumber)
        {
            var possibleValues = new List<string>();

            for (int index = 0; index < wrongAccountNumber.Length; index++)
            {
                for (int mask = 1; mask <= 128; mask <<= 1)
                {
                    if (Enum.IsDefined(typeof(SevenSegments), wrongAccountNumber[index] ^ mask))
                    {
                        wrongAccountNumber[index] = (byte)(wrongAccountNumber[index] ^ mask);

                        var (accountNumber, isIllegible) = ConvertSevenSegmentToString(wrongAccountNumber);

                        if (!isIllegible)
                        {
                            if (IsCheckSumValid(accountNumber))
                            {
                                possibleValues.Add(accountNumber);
                            }
                        }

                        //Come back to the previous value
                        wrongAccountNumber[index] = (byte)(wrongAccountNumber[index] ^ mask);
                    }
                }
            }
            return possibleValues;
        }
    }
}
