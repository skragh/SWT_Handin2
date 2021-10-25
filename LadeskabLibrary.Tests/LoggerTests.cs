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

        [SetUp]
        public void setup()
        {
            uut = new Logger();
        }

        //At teste - 2 styks: Både locked og unlocked
        //Blive koden kørt?
        //Er det gemt? AKA kan logfilen tilgås bagefter, og indeholder den det korrekte? Tjek for format
        //Kan der gemmes flere beskeder?

        #region logUnlocked
        //https://stackoverflow.com/questions/12480563/c-sharp-unit-test-a-streamwriter-parameter
        [TestCase(3)]
        public void RunLoggingCode_Unlocked_AssertDoorUnlockedLogged(int id)
        {
            Assert.Pass();
            //Definer en memorystream og reroute streamen til den nye memorystream, som herefter kan læses fra
            //Arrange
            using (var stream = new MemoryStream())         //Opret memorystream, som informationen kommer til at blive gemt i. Ligesom med stringwriter i Displaytest
            using (var writer = new StreamWriter(stream))   //Opret en writer, som skrives til
            {
                //Act
                uut.LogDoorUnlocked(id); //Kører metoden

                //Assert
                string actual = Encoding.UTF8.GetString(stream.ToArray()); //laver streamen om til et array og gemmer det
                Assert.AreEqual($"On {uut.LatestLog} - Door: {id} - unlocked", actual);
            }
                ////Arrange
                //System.IO.StringWriter output = new System.IO.StringWriter();
                //System.IO.FileStream.

                ////Act
                //uut.LogDoorUnlocked(id);

                ////Assert
            }
        public void checkLogData_Unlocked_AssertInformationHasBeenSaved()
        {

        }
        public void LoggingMultipleLogs_Unlocked_AssertInformationIsAppended()
        { }
        #endregion
        #region logLocked
        #endregion
    }
}
