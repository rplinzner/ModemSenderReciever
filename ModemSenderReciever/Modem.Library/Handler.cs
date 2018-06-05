using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Modem.Library
{
    public static class Handler
    {
        private static bool busy;
        public static void Call(Port port, string number)
        {
            string command = "ATD" + number + '\r';
            Send(port, command);
        }
        public static void Send(Port port, string message)
        {
            /*var SendThis = Encoding.Default.GetBytes(message);
            var n = SendThis.Length;*/
            //port.Write(SendThis, 0, n); //check if you have to send each letter individually
            //TODO: Check working version
            /*foreach (var b in SendThis)
            {
                port.WriteCharacter(b);
            }*/
            busy = true;
            port.WriteString(message);
            busy = false;
        }

        public static char Recieve(Port port)
        {
            char Char;
            while (true)
            {
                if (!busy)
                {
                    while (port.CheckBufferEmpty())
                    {
                        Thread.Sleep(1000);
                    }

                    Char = Convert.ToChar(port.Read());
                    return Char;
                }
                Thread.Sleep(10);
            }
        }
        
    }
}