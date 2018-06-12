using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modem.Library
{
    public class Port
    {
        private readonly SerialPort _port;

        public Port(string portName, int baudRate, int dataBits, Parity parity)
        {
            _port = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = StopBits.One,
                DtrEnable = true,
                RtsEnable = true,
                ReadTimeout = 500,
                WriteTimeout = 500

            };
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _port.Write(buffer, offset, count);
        }

        public void WriteString(string message)
        {
            _port.Write(message);
        }
        public void WriteCharacter(byte character)
        {
            var bytes = new byte[1]
            {
                character
            };
            Write(bytes, 0, 1);
        }

        public void Open()
        {
            _port.Open();
        }
        public void Close()
        {
            _port.Close();
        }

        public bool CheckBufferEmpty()
        {
            return _port.BytesToRead == 0;
        }
        public byte Read()
        {
            return (byte)_port.ReadByte();
        }


    }
}