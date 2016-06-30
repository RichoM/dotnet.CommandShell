using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichoM.CommandShell;
using System.Threading.Tasks;
using System.Threading;

namespace CommandShellTest
{
    [TestClass]
    public class CmdTest
    {
        [TestMethod]
        public void SyncExecute()
        {
            Cmd.Process("ping", "127.0.0.1").Execute((code, output) =>
            {
                Assert.AreEqual(0, code);
                string[] lines = output.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                Assert.AreEqual("Pinging 127.0.0.1 with 32 bytes of data:", lines[1]);
            }, false);
        }
    }
}
