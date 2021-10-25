    using System;
    using System.Reflection.Metadata;
    using LadeskabLibrary;
    using UsbSimulator;

    class Program
    {
        static void Main(string[] args)
        {
				// Assemble your system here from all the classes
                IDoor door = new Door(false, false);
                IUsbCharger usbCharger = new UsbChargerSimulator(); //Simulated??
                IChargeControl chargeControl = new ChargeControl(usbCharger);
                ILogger logger = new Logger();
                IReader rfidReader = new RFIDReader();
                StationControl stationControl = new StationControl(door, chargeControl, rfidReader, logger);
                IDisplay display = new DisplayImplementation(chargeControl, stationControl);

        bool finish = false;
            do
            {
                string input;
                System.Console.WriteLine("Indtast E, O, C, R: ");
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'E':
                        finish = true;
                        break;

                    case 'O':
                        try
                        {
                            door.OnDoorOpen();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        
                        break;

                    case 'C':
                        try
                        {
                            door.OnDoorClose();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;

                    case 'R':
                        System.Console.WriteLine("Indtast RFID id: ");
                        string idString = System.Console.ReadLine();

                        int id = Convert.ToInt32(idString);
                        rfidReader.OnRead(id);
                        break;

                    default:
                        break;
                }

            } while (!finish);
        }
    }

