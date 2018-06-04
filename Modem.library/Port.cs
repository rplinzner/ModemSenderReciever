using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modem.library
{
    public class Port
    {
        private SerialPort _port;
        public Port(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            _port = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits
            };
        }
    }
}
