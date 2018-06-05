using System.Runtime.InteropServices;
using System.Text;

namespace Modem.Library
{
    public static class Handler
    {
        public static void Call(Port port, string number)
        {
            string command = "ATD" + " " + number;
            Send(port, command);
        }
        public static void Send(Port port, string message)
        {
            var SendThis = Encoding.Default.GetBytes(message);
            var n = SendThis.Length;
            port.Write(SendThis, 0, n); //check if you have to send each letter individually
            //TODO: Check working version
            /*foreach (var b in SendThis)
            {
                port.WriteCharacter(b);
            }*/
        }
        
    }
}