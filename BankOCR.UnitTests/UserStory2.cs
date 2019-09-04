using BankOCR;
using BankOCR.Helpers;
using NUnit.Framework;

namespace BankOcrKata
{
    [TestFixture]
    public class UserStory2
    {
        [TestCase("711111111", true)]
        [TestCase("123456789", true)]
        [TestCase("490867715", true)]
        [TestCase("888888888", false)]
        [TestCase("490067715", false)]
        [TestCase("012345678", false)]
        public void Tests(string accountNumber, bool isValid)
        {
            Assert.AreEqual(MathHelper.IsCheckSumValid(accountNumber), isValid);
        }
    }
}