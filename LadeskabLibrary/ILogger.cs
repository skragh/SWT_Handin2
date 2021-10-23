using System;
using System.Collections.Generic;
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
        public string logFile { get; set; }

        public void LogDoorLocked(int id)
        {
        }

        public void LogDoorUnlocked(int id)
        {
        }
    }
}
