using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary.Tests
{
    class LoggerTests
    {
        private Logger uut;
        private string filnavn = "LogFileTest";

        [SetUp]
        public void setup()
        {
            uut = new Logger(filnavn);
        }

        [TearDown]
        public void tearDown()
        {
            System.IO.File.Delete(uut.locationOfLogfile);
        }

        #region logUnlocked
        //https://stackoverflow.com/questions/12480563/c-sharp-unit-test-a-streamwriter-parameter
        [TestCase(3)]
        [TestCase(55)]
        [TestCase(int.MaxValue)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void RunLoggingCode_Unlocked_AssertDoorUnlockedLogged(int id)
        {
            //Arrange
            //Act
            uut.LogDoorUnlocked(id);

            //Assert
            Assert.That($"On {uut.LatestLog} - Door: {id} - unlocked\n", Is.EqualTo(System.IO.File.ReadAllText(uut.locationOfLogfile)));
        }

        [TestCase(1, 2)]
        [TestCase(1, 1)]
        [TestCase(int.MaxValue, int.MinValue)]
        [TestCase(-1, 1)]
        [TestCase(0, 0)]
        public void LoggingMultipleLogs_Unlocked_AssertAllInformationIsAppended(int id1, int id2)
        {
            //Arrange
            //Act
            uut.LogDoorUnlocked(id1);
            DateTime timeOfFirstLog = uut.LatestLog;
            uut.LogDoorUnlocked(id2);

            //Assert
            Assert.That($"On {timeOfFirstLog} - Door: {id1} - unlocked\nOn {uut.LatestLog} - Door: {id2} - unlocked\n", Is.EqualTo(System.IO.File.ReadAllText(uut.locationOfLogfile)));
        }
        #endregion
        #region logLocked
        [TestCase(3)]
        [TestCase(55)]
        [TestCase(int.MaxValue)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void RunLoggingCode_Locked_AssertDoorUnlockedLogged(int id)
        {
            //Arrange
            //Act
            uut.LogDoorLocked(id);

            //Assert
            Assert.That($"On {uut.LatestLog} - Door: {id} - locked\n", Is.EqualTo(System.IO.File.ReadAllText(uut.locationOfLogfile)));
        }

        [TestCase(1, 2)]
        [TestCase(1, 1)]
        [TestCase(int.MaxValue, int.MinValue)]
        [TestCase(-1, 1)]
        [TestCase(0, 0)]
        public void LoggingMultipleLogs_Locked_AssertAllInformationIsAppended(int id1, int id2)
        {
            //Arrange
            //Act
            uut.LogDoorLocked(id1);
            DateTime timeOfFirstLog = uut.LatestLog;
            uut.LogDoorLocked(id2);

            //Assert
            Assert.That($"On {timeOfFirstLog} - Door: {id1} - locked\nOn {uut.LatestLog} - Door: {id2} - locked\n", Is.EqualTo(System.IO.File.ReadAllText(uut.locationOfLogfile)));
        }
        #endregion
        #region
        [Test]
        public void LogFileLocation_AssertThatLocationIsCorrect()
        {
            //Arrange
            //Act
            //uut.locationOfLogfile = Environment.CurrentDirectory + $"{filnavn}.txt";
            string locationForTest = Environment.CurrentDirectory + $"{filnavn}.txt";
            //Assert
            Assert.That(locationForTest, Is.EqualTo(uut.locationOfLogfile));
        }
        #endregion
    }
}
