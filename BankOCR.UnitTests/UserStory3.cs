using BankOCR;
using NUnit.Framework;
using System;
using System.Linq;

namespace BankOcrKata
{
    public class UserStory3
    {
        [TestCase(@"
 _  _  _  _  _  _  _  _     
| || || || || || || ||_   |
|_||_||_||_||_||_||_| _|  |", "000000051")]
        [TestCase(@"
    _  _  _  _  _  _     _ 
|_||_|| || ||_   |  |  | _ 
  | _||_||_||_|  |  |  | _|", "49006771? ILL")]
        [TestCase(@"
    _  _     _  _  _  _  _ 
  | _| _||_| _ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _ ", "1234?678? ILL")]
        public void Tests(string input, string expectedResult)
        {
            var lines = input.Split(new[] { "\r\n" }, StringSplitOptions.None).Skip(1).ToArray();

            var newEntry = new AccountEntry(lines);

            Assert.AreEqual(newEntry.AccountStatus, expectedResult);
        }
    }
}