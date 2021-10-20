using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary.Tests
{
    class DisplayTest
    {
        private DisplayImplementation uut;

        [SetUp]
        public void setup()
        {
            uut = new DisplayImplementation();
        }

        [TestCase("Charging")]
        [TestCase("Fully Charged")]
        [TestCase("Disconnected")]
        [TestCase("Overload Error - Too much power being transferred - Disconnecting")]
        [TestCase("Error exception. Internal charging information is invalid")]
        [Test]
        public void DisplayValidChargeMessages_IgnoringStationMessage_AssertThatMessageSentToConsole(string chargeMessage)
        {
            //Arrange - setup stringwriter
            System.IO.StringWriter output = new System.IO.StringWriter(); //Oprettet en stringwriter
            System.Console.SetOut(output); //i stedet for hvad der normalt skrives til out, skrives der til stringWriterForTest

            //Act - Kør displaymessage
            uut.ChargeMessage = chargeMessage;
            uut.print(); //stringwriter -> stringbuilder -> hvad er der i
            //I stedet for at skrive til konsollen, som den normalt ville, skrives der til stringWriterForTest

            //Assert - hvad er der i stringbuilderen?
            //Nu er det der blev udskrevet skrevet til en string
            Assert.That(output.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty), Is.EqualTo(uut.ChargingMessageDeclaration.Trim() + chargeMessage.Trim() + uut.StationMessageDeclaration.Trim())); //Er det der kom ud, det samme som message?
        }

        [TestCase("Station initialized")]
        [TestCase("Not empty")]
        [TestCase("")]
        [TestCase("\r\n")]
        [TestCase("Tilslutningsfejl")]
        public void DisplayValidStationMessage_IgnoringChargingMessage_AssertThatMessageSentToConsole(string stationMessage)
        {
            //Arrange - setup stringwriter
            System.IO.StringWriter output = new System.IO.StringWriter(); //Oprettet en stringwriter
            System.Console.SetOut(output); //i stedet for hvad der normalt skrives til out, skrives der til stringWriterForTest

            //Act - Kør displaymessage
            uut.StationMessage = stationMessage;
            uut.print(); //stringwriter -> stringbuilder -> hvad er der i
            //I stedet for at skrive til konsollen, som den normalt ville, skrives der til stringWriterForTest

            //Assert - hvad er der i stringbuilderen?
            //Nu er det der blev udskrevet skrevet til en string
            Assert.That(output.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty), Is.EqualTo(uut.ChargingMessageDeclaration.Trim() + uut.StationMessageDeclaration.Trim() + uut.StationMessage.Trim())); //Er det der kom ud, det samme som message?
        }
    }
}
