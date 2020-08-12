using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using StellaServer.Annotations;

namespace StellaServer
{
    /// <summary>
    /// I only use Console.WriteLine when writing to console. TODO add more functions of TextWriter
    /// </summary>
    public class ConsoleOutWriter : TextWriter
    {
        public event EventHandler<string> NewMessage;

        public override Encoding Encoding { get; }

        public override void WriteLine()
        {
            Console.Beep();
        }

        public override void WriteLine(string line)
        {
            NewMessage?.Invoke(this,$"{DateTime.Now} - {line}");
        }
    }
}
