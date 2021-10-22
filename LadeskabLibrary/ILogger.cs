using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public interface ILogger
    {
        void LogDoorLockeD(int id);
        void LogDoorUnlocked(int id);
    }
    class Logger : ILogger
    {
        public string logFile { get; set; }

        public void LogDoorLockeD(int id)
        {
        }

        public void LogDoorUnlocked(int id)
        {
        }
    }
}
