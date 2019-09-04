using BankOCR.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BankOCR
{
    public class AccountEntry
    {
        #region Private Properties

        private List<string> _possibleCorrectAccounts;

        private const byte SEGMENT_A = 0x01;
        private const byte SEGMENT_B = 0x02;
        private const byte SEGMENT_C = 0x04;
        private const byte SEGMENT_D = 0x08;
        private const byte SEGMENT_E = 0x10;
        private const byte SEGMENT_F = 0x20;
        private const byte SEGMENT_G = 0x40;

        #endregion

        #region Public Properties
        public bool IsIlegible { get; private set; }
        public bool IsValid { get; private set; }
        public byte[] AccountNumbers { get; } = new byte[9];
        public string AccountString { get; private set; }
        public string AccountStatus => $"{AccountString}{(IsValid ? "" : (IsIlegible ? " ILL" : " ERR")) }";

        public string AccountPrediction
        {
            get
            {
                if (_possibleCorrectAccounts == null || !_possibleCorrectAccounts.Any())
                {
                    return AccountStatus;
                }
                else if (_possibleCorrectAccounts.Count == 1)
                {
                    return _possibleCorrectAccounts.FirstOrDefault();
                }
                else
                {
                    return $"{AccountString} AMB [{string.Join(", ", _possibleCorrectAccounts.OrderBy(a => a).Select(a => $"'{a}'"))}]";
                }
            }
        }

        #endregion

        #region Constructors
        public AccountEntry(params string[] lines)
        {
            //Account numbers are stored following seven-segment display structure
            //In this way, each bit represents a segment (https://en.wikipedia.org/wiki/Seven-segment_display#/media/File:7_Segment_Display_with_Labeled_Segments.svg)
            //The next contains the weights of the different segments: A => 0x01, B => 0x02, C => 0x04, D => 0x20, E => 0x80, F => 0x04, G => 0x80

            if (lines.Length >= 3)
            {
                ProcessLine(lines[0], 0x00, SEGMENT_A, 0x00);
                ProcessLine(lines[1], SEGMENT_F, SEGMENT_G, SEGMENT_B);
                ProcessLine(lines[2], SEGMENT_E, SEGMENT_D, SEGMENT_C);

                Init();
            }
        }

        #endregion


        #region Private methods
        private void Init()
        {
            var accountInformation = MathHelper.ConvertSevenSegmentToString(AccountNumbers);

            AccountString = accountInformation.Item1;
            IsIlegible = accountInformation.Item2;

            if (!IsIlegible)
            {
                IsValid = MathHelper.IsCheckSumValid(AccountString);
            }

            if (IsIlegible || !IsValid)
            {
                _possibleCorrectAccounts = MathHelper.ObtainPossibleValues(AccountNumbers);
            }
        }

        //The segments will have different weights depending on the line
        private void ProcessLine(string line, byte weight1, byte weight2, byte weight3)
        {
            if (line != null)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    int index = i / 3;

                    if (i % 3 == 0 && line[i] == '|')
                    {
                        AccountNumbers[index] |= weight1;
                    }
                    else if (i % 3 == 1 && line[i] == '_')
                    {
                        AccountNumbers[index] |= weight2;
                    }
                    else if (i % 3 == 2 && line[i] == '|')
                    {
                        AccountNumbers[index] |= weight3;
                    }
                }
            }
        }

        #endregion
    }
}
