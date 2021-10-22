using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadeskabLibrary
{
    public interface IReader
    {
        public class ReaderEventArgs:EventArgs
        { public int idRead { get; set; } }
        event EventHandler<ReaderEventArgs> NewRead;
        void OnRead(int id);

    }

    public class RFIDReader : IReader
    {
        public int currentId { get; set; }
        public event EventHandler<IReader.ReaderEventArgs> NewRead;

        public void OnRead(int id)
        {
            currentId = id;
            NewRead?.Invoke(this, new IReader.ReaderEventArgs() { idRead = id });
        }
    }
}
