using NSubstitute;
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
        private IChargeControl chargeControlForTest;
        private IStationControl stationControlForTest;
        private IUsbCharger usbCharger;
        private ILogger logger;
        private IReader reader;
        private IDoor door;

        [SetUp]
        public void setup()
        {
            usbCharger = Substitute.For<UsbCharger>();
            logger = Substitute.For<Logger>("TestLog");
            reader = Substitute.For<RFIDReader>();
            door = Substitute.For<Door>(false, false);
            chargeControlForTest = Substitute.For<ChargeControl>(usbCharger);
            stationControlForTest = Substitute.For<StationControl>(door, chargeControlForTest, reader, logger);
            uut = new DisplayImplementation(chargeControlForTest, stationControlForTest);
        }
        #region ChargeMessage
        [TestCase("Charging")]
        [TestCase("Fully Charged")]
        [TestCase("Disconnected")]
        [TestCase("Overload Error - Too much power being transferred - Disconnecting")]
        [TestCase("Error exception. Internal charging information is invalid")]
        public void DisplayValidChargeMessages_IgnoringStationMessage_AssertThatMessageSentToConsole(string chargeMessage)
        {
            //Arrange - setup stringwriter
            System.IO.StringWriter output = new System.IO.StringWriter(); //Oprettet en stringwriter
            System.Console.SetOut(output); //i stedet for hvad der normalt skrives til out, skrives der til stringWriterForTest

            //Act - Kør displaymessage
            uut.ChargeMessage = chargeMessage;
            uut.Print(); //stringwriter -> stringbuilder -> hvad er der i
            //I stedet for at skrive til konsollen, som den normalt ville, skrives der til stringWriterForTest

            //Assert - hvad er der i stringbuilderen?
            //Nu er det der blev udskrevet skrevet til en string
            Assert.That(output.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty), Is.EqualTo(uut.ChargingMessageDeclaration.Trim() + chargeMessage.Trim() + uut.StationMessageDeclaration.Trim())); //Er det der kom ud, det samme som message?
        }
        #endregion
        #region StationMessage
        [TestCase("Station initialized")]
        [TestCase("Not empty")]
        [TestCase("")]
        [TestCase("\r\n")]
        [TestCase("Tilslutningsfejl")]
        public void SendValidStationMessageToConsole_IgnoringChargingMessage_AssertThatMessageSentToConsole(string stationMessage)
        {
            //Arrange - setup stringwriter
            System.IO.StringWriter output = new System.IO.StringWriter(); //Oprettet en stringwriter
            System.Console.SetOut(output); //i stedet for hvad der normalt skrives til out, skrives der til stringWriterForTest

            //Act - Kør displaymessage
            uut.StationMessage = stationMessage;
            uut.Print(); //stringwriter -> stringbuilder -> hvad er der i
            //I stedet for at skrive til konsollen, som den normalt ville, skrives der til stringWriterForTest

            //Assert - hvad er der i stringbuilderen?
            //Nu er det der blev udskrevet skrevet til en string
            Assert.That(output.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty), Is.EqualTo(uut.ChargingMessageDeclaration.Trim() + uut.StationMessageDeclaration.Trim() + uut.StationMessage.Trim())); //Er det der kom ud, det samme som message?
        }
        [TestCase("test")]
        public void DisplayValidStationMessage_IgnoringChargingMessage_AssertThatMessageSentToConsole(string message)
        {
            //Arrange
            /*IUsbCharger usbCharger = new UsbCharger(); IChargeControl chargeControl = new ChargeControl(usbCharger);*/
            //IDoor door = new Door(false, false); IReader reader = new RFIDReader(); ILogger logger = new Logger();
            //StationControl sender = new StationControl(door, chargeControlForTest, reader, logger);
            StationControl.StationMessageEventArgs e = new StationControl.StationMessageEventArgs();
            e.message = message;
            //Act
            uut.DisplayStationMessage(stationControlForTest, e);

            //Assert
            Assert.That(uut.StationMessage.Trim(), Is.EqualTo(message));
            //sub.Received().Print(); //This is not called?
        }
#endregion
        #region Incoming Enum
        //At teste
        //Enum - sat til det rigtige?
        [TestCase(ChargingState.CHARGING, "Charging")]
        [TestCase(ChargingState.IDLE, "Disconnected")]
        [TestCase(ChargingState.FULL, "Fully Charged")]
        [TestCase(ChargingState.OVERLOAD, "Overload Error - Too much power being transferred - Disconnecting")]
        [TestCase(4, "Error exception. Internal charging information is invalid")]
        public void EnumChangedReadCorrectlyForChargingMessage_AssertThatLocalMessageChanged(ChargingState state, string chargingMessage)
        {
            //Arrange
            //IUsbCharger usb = new UsbCharger();
            //IChargeControl chargeControlForTest = new ChargeControl(usb);
            ChargingStateEventArgs chargingEvent = new ChargingStateEventArgs();
            chargingEvent._chargingState = state;
            //Act
            uut.DisplayChargeMessage(chargeControlForTest, chargingEvent);

            //Assert
            Assert.That(uut.ChargeMessage, Is.EqualTo(chargingMessage));
        }
        #endregion
        #region Constructor
        [Test]
        public void ConstructorTest_AssertThatConstructorHasSetMessages()
        {
            //Arrange
            //Act
            //Assert
            Assert.That(uut.ChargingMessageDeclaration.Trim(), Is.EqualTo("CHARGING INFORMATION"));
            Assert.That(uut.StationMessageDeclaration.Trim(), Is.EqualTo("STATION INFORMATION"));
        }
        #endregion
    }
}
