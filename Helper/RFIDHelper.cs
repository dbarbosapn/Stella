using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Stella.Helper
{
    public class RFIDHelper
    {
        public static RFIDHelper Instance {
            get {
                if (_instance == null)
                    _instance = new RFIDHelper();
                return _instance;
            }
        }
        private static RFIDHelper _instance;

        private SerialPort reader = null;
        public Action<string> OnReaderDataReceived;

        public bool HasReader()
        {
            return reader != null && reader.IsOpen;
        }

        /// <summary>
        /// Tries to find the Stella Reader on the serial ports.
        /// </summary>
        /// <param name="OnReaderFound">Callback for when reader is found (can be null)</param>
        /// <param name="OnReaderNotFound">Callback for when reader is not found (can be null)</param>
        public async void ScanReader(Action OnReaderFound, Action OnReaderNotFound)
        {
            CloseReader();

            bool found = false;

            foreach (var port in SerialPort.GetPortNames())
            {
                SerialPort serial = new SerialPort(port, 9600);
                serial.Open();
                serial.Write("c");
                await Task.Delay(500);
                if(serial.BytesToRead > 0)
                {
                    try
                    {
                        string r = serial.ReadExisting();
                        if(r == "STELLA_READER")
                        {
                            reader = serial;
                            reader.DataReceived += new SerialDataReceivedEventHandler(OnReaderReceivedData);
                            found = true;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore exceptions
                    }
                }
            }

            if (found) OnReaderFound?.Invoke();
            else OnReaderNotFound?.Invoke();
        }

        /// <summary>
        /// Closes the Stella Reader
        /// </summary>
        private void CloseReader()
        {
            if(reader != null)
            {
                reader.Close();
                reader = null;
            }
        }

        /// <summary>
        /// Callback for when reader has data to read
        /// </summary>
        private void OnReaderReceivedData(object sender, SerialDataReceivedEventArgs e)
        {
            string data = reader.ReadExisting();
            OnReaderDataReceived?.Invoke(data);
        }

        /// <summary>
        /// Sends positive signal to the reader
        /// </summary>
        public void PositiveSignal()
        {
            SendSignal("1");
        }

        /// <summary>
        /// Sends negative signal to the reader
        /// </summary>
        public void NegativeSignal()
        {
            SendSignal("0");
        }

        private void SendSignal(string v)
        {
            if(HasReader())
            {
                reader.WriteLine(v);
            }
        }
    }
}
