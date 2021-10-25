using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public interface ILogger
    {
        void LogDoorLocked(int id);
        void LogDoorUnlocked(int id);
    }
    public class Logger : ILogger
    {
        public Logger(string fileLocation = "LogFile")
        {
            LogFile = Environment.CurrentDirectory + $"{fileLocation}.txt";
            if (!System.IO.File.Exists(LogFile))
            {
                System.IO.File.WriteAllText(LogFile, "");
            }
        }
        public string LogFile { get; set; }
        //private const string LogFile = "C:/User/Bruger/Documents/SoftwareIngeniør/GIT/SWT_Handin2/LadeskabLibrary/LogFile.txt";
        //private const string LogFile = "C:/LogFile.txt";
        //private const string LogFile = "LogFile.txt";
        //private string LogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public DateTime LatestLog { get; set; }

        public void LogDoorLocked(int id)
        {
            LatestLog = DateTime.Now;
            string logInfo = $"On {LatestLog} - Door: {id} - locked\n";
            File.AppendAllText(LogFile, logInfo);
        }

        public void LogDoorUnlocked(int id)
        {
            LatestLog = DateTime.Now;
            string logInfo = $"On {LatestLog} - Door: {id} - unlocked\n";
            File.AppendAllText(LogFile, logInfo);
        }
    }
}
