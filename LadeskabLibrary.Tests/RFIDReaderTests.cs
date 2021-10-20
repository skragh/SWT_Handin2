using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace LadeskabLibrary.Tests
{
    class RFIDReaderTests
    {
        public RFIDReader uut { get; set; }
        private IReader.ReaderEventArgs msgArgs;
        [SetUp]
        public void Setup()
        {
            msgArgs = null;
            uut = new RFIDReader();
            uut.NewRead += (o, args) => { msgArgs = args; };
        }

        [TestCase (1)]
        [TestCase (0)]
        [TestCase (-1)]
        public void OnRead_IdPassed_AssertCurrentId(int id)
        {
            uut.OnRead(id);
            Assert.That(uut.currentId, Is.EqualTo(id));
        }
        [Test]
        public void OnRead_IdPassed_AssertEventCalled()
        {
            uut.OnRead(5);
            Assert.That(msgArgs, Is.Not.Null);
        }
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        public void OnRead_IdPassed_AssertCorrectIdSent(int id)
        {
            uut.OnRead(id);
            Assert.That(msgArgs.idRead, Is.EqualTo(id));
        }



        //public int currentId { get; set; }
        //public event EventHandler<IReader.ReaderEventArgs> NewRead;

        //public void OnRead(int id)
        //{
        //    currentId = id;
        //    NewRead?.Invoke(this, new IReader.ReaderEventArgs() { idRead = id });
        //}
    }
}
