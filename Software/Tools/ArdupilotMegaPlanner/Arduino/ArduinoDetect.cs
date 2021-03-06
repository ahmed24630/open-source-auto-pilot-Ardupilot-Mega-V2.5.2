using System;
using System.Reflection;
using System.Management;
using System.Windows.Forms;
using System.Threading;
using log4net;
using System.Globalization;
using ArdupilotMega.Comms;
using ArdupilotMega.Utilities;

namespace ArdupilotMega.Arduino
{
    public class ArduinoDetect
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// detects STK version 1 or 2
        /// </summary>
        /// <param name="port">comportname</param>
        /// <returns>string either (1280/2560) or "" for none</returns>
        public static string DetectVersion(string port)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = port;

            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 57600;
            serialPort.Open();

            serialPort.toggleDTR();

            Thread.Sleep(100);

            int a = 0;
            while (a < 20) // 20 * 50 = 1 sec
            {
                //Console.WriteLine("write " + DateTime.Now.Millisecond);
                serialPort.DiscardInBuffer();
                serialPort.Write(new byte[] { (byte)'0', (byte)' ' }, 0, 2);
                a++;
                Thread.Sleep(50);

                //Console.WriteLine("btr {0}", serialPort.BytesToRead);
                if (serialPort.BytesToRead >= 2)
                {
                    byte b1 = (byte)serialPort.ReadByte();
                    byte b2 = (byte)serialPort.ReadByte();
                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        serialPort.Close();
                        log.Info("is a 1280");
                        return "1280";
                    }
                }
            }

            serialPort.Close();

            log.Warn("Not a 1280");

            Thread.Sleep(500);

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 115200;
            serialPort.Open();

            serialPort.toggleDTR();

            Thread.Sleep(100);

            a = 0;
            while (a < 4)
            {
                byte[] temp = new byte[] { 0x6, 0, 0, 0, 0 };
                temp = ArduinoDetect.genstkv2packet(serialPort, temp);
                a++;
                Thread.Sleep(50);

                try
                {
                    if (temp[0] == 6 && temp[1] == 0 && temp.Length == 2)
                    {
                        serialPort.Close();
                        log.Info("is a 2560");
                        return "2560";

                    }
                }
                catch
                {
                }
            }

            serialPort.Close();
            log.Warn("Not a 2560");
            return "";
        }

        /// <summary>
        /// Detects APM board version
        /// </summary>
        /// <param name="port"></param>
        /// <returns> (1280/2560/2560-2)</returns>
        public static string DetectBoard(string port)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = port;

            if (serialPort.IsOpen)
                serialPort.Close();

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 57600;
            serialPort.Open();

            Thread.Sleep(100);

            int a = 0;
            while (a < 20) // 20 * 50 = 1 sec
            {
                //Console.WriteLine("write " + DateTime.Now.Millisecond);
                serialPort.DiscardInBuffer();
                serialPort.Write(new byte[] { (byte)'0', (byte)' ' }, 0, 2);
                a++;
                Thread.Sleep(50);

                //Console.WriteLine("btr {0}", serialPort.BytesToRead);
                if (serialPort.BytesToRead >= 2)
                {
                    byte b1 = (byte)serialPort.ReadByte();
                    byte b2 = (byte)serialPort.ReadByte();
                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        serialPort.Close();
                        log.Info("is a 1280");
                        return "1280";
                    }
                }
            }

            serialPort.Close();

            log.Warn("Not a 1280");

            Thread.Sleep(500);

            serialPort.DtrEnable = true;
            serialPort.BaudRate = 115200;
            serialPort.Open();

            Thread.Sleep(100);

            a = 0;
            while (a < 4)
            {
                byte[] temp = new byte[] { 0x6, 0, 0, 0, 0 };
                temp = ArduinoDetect.genstkv2packet(serialPort, temp);
                a++;
                Thread.Sleep(50);

                try
                {
                    if (temp[0] == 6 && temp[1] == 0 && temp.Length == 2)
                    {
                        serialPort.Close();
                        //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB\VID_2341&PID_0010\640333439373519060F0\Device Parameters
                        if (!MainV2.MONO && !Thread.CurrentThread.CurrentCulture.IsChildOf(CultureInfoEx.GetCultureInfo("zh-Hans")))
                        {
                            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort"); // Win32_USBControllerDevice
                            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                            foreach (ManagementObject obj2 in searcher.Get())
                            {
                                //Console.WriteLine("Dependant : " + obj2["Dependent"]);

                                // all apm 1-1.4 use a ftdi on the imu board.

                                obj2.Properties.ForEach(x =>
                                {
                                    try
                                    {
                                        log.Info(((PropertyData)x).Name.ToString() + " = " + ((PropertyData)x).Value.ToString());
                                    }
                                    catch { }
                                });

                                // check vid and pid
                                if (obj2.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_2341&PID_0010"))
                                {
                                    // check port name as well
                                    if (obj2.Properties["Name"].Value.ToString().ToUpper().Contains(serialPort.PortName.ToUpper()))
                                    {
                                        log.Info("is a 2560-2");
                                        return "2560-2";
                                    }
                                }
                            }

                            log.Info("is a 2560");
                            return "2560";
                        }
                        else
                        {
                            if (DialogResult.Yes == CustomMessageBox.Show("Is this a APM 2+?", "APM 2+", MessageBoxButtons.YesNo))
                            {
                                return "2560-2";
                            }
                            else
                            {
                                return "2560";
                            }
                        }

                    }
                }
                catch { }
            }

            serialPort.Close();
            log.Warn("Not a 2560");
            return "";
        }

        public enum ap_var_type
        {
            AP_PARAM_NONE = 0,
            AP_PARAM_INT8,
            AP_PARAM_INT16,
            AP_PARAM_INT32,
            AP_PARAM_FLOAT,
            AP_PARAM_VECTOR3F,
            AP_PARAM_VECTOR6F,
            AP_PARAM_MATRIX3F,
            AP_PARAM_GROUP
        };

       internal static string[] type_names = new string[] {
	"NONE", "INT8", "INT16", "INT32", "FLOAT", "VECTOR3F", "VECTOR6F","MATRIX6F", "GROUP"
};

      internal static byte type_size(ap_var_type type)
{
    switch (type) {
    case ap_var_type.AP_PARAM_NONE:
    case ap_var_type.AP_PARAM_GROUP:
        return 0;
    case ap_var_type.AP_PARAM_INT8:
        return 1;
    case ap_var_type.AP_PARAM_INT16:
        return 2;
    case ap_var_type.AP_PARAM_INT32:
        return 4;
    case ap_var_type.AP_PARAM_FLOAT:
        return 4;
    case ap_var_type.AP_PARAM_VECTOR3F:
        return 3*4;
    case ap_var_type.AP_PARAM_VECTOR6F:
        return 6*4;
    case ap_var_type.AP_PARAM_MATRIX3F:
        return 3*3*4;
    }
    return 0;
}

        /// <summary>
        /// return the software id from eeprom
        /// </summary>
        /// <param name="comport">Port</param>
        /// <param name="version">Board type</param>
        /// <returns></returns>
        public static int decodeApVar(string comport, string version)
        {
            ArduinoComms port = new ArduinoSTK();
            if (version == "1280")
            {
                port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else if (version == "2560" || version == "2560-2")
            {
                port = new ArduinoSTKv2();
                port.BaudRate = 115200;
            }
            else { return -1; }
            port.PortName = comport;
            port.DtrEnable = true;
            port.Open();
            port.connectAP();
            byte[] buffer = port.download(1024 * 4);
            port.Close();

            if (buffer[0] != 'A' && buffer[0] != 'P' || buffer[1] != 'P' && buffer[1] != 'A') // this is the apvar header
            {
                return -1;
            }
            else
            {
                if (buffer[0] == 'A' && buffer[1] == 'P' && buffer[2] == 2)
                { // apvar header and version
                    int pos = 4;
                    byte key = 0;
                    while (pos < (1024 * 4))
                    {
                        int size = buffer[pos] & 63;
                        pos++;
                        key = buffer[pos];
                        pos++;

                        log.InfoFormat("{0:X4}: key {1} size {2}\n ", pos - 2, key, size + 1);

                        if (key == 0xff)
                        {
                            log.InfoFormat("end sentinal at {0}", pos - 2);
                            break;
                        }

                        if (key == 0)
                        {
                            //Array.Reverse(buffer, pos, 2);
                            return BitConverter.ToUInt16(buffer, pos);
                        }


                        for (int i = 0; i <= size; i++)
                        {
                            Console.Write(" {0:X2}", buffer[pos]);
                            pos++;
                        }
                    }
                }

                if (buffer[0] == 'P' && buffer[1] == 'A' && buffer[2] == 5) // ap param
                {
                    int pos = 4;
                    byte key = 0;
                    while (pos < (1024 * 4))
                    {
                        key = buffer[pos];
                        pos++;
                        int group = buffer[pos];
                        pos++;
                        int type = buffer[pos];
                        pos++;

                        int size = type_size((ap_var_type)Enum.Parse(typeof(ap_var_type), type.ToString()));


                        Console.Write("{0:X4}: type {1} ({2}) key {3} group {4} size {5}\n ", pos - 2, type, type_names[type], key, group, size);

                        if (key == 0xff)
                        {
                            log.InfoFormat("end sentinal at {0}", pos - 2);
                            break;
                        }

                        if (key == 0)
                        {
                            //Array.Reverse(buffer, pos, 2);
                            return BitConverter.ToUInt16(buffer, pos);
                        }


                        for (int i = 0; i < size; i++)
                        {
                            Console.Write(" {0:X2}", buffer[pos]);
                            pos++;
                        }
                    }
                }

                if (buffer[0] == 'P' && buffer[1] == 'A' && buffer[2] == 6) // ap param
                {
                    int pos = 4;
                    byte key = 0;
                    while (pos < (1024 * 4))
                    {
                        key = buffer[pos];
                        pos++;

                        if (key == 0xff)
                        {
                            log.InfoFormat("end sentinal at {0}", pos - 1);
                            break;
                        }

                        int type = buffer[pos] & 0x3f; // 6 bits

                        uint group = BitConverter.ToUInt32(buffer, pos);//((byte)(buffer[pos]) >> 6) | ((byte)(buffer[pos + 1]) << 8) | ((byte)(buffer[pos + 2]) << 16); // 18 bits

                        group = (group >> 6) & 0x3ffff;
                        pos++;
                        pos++;
                        pos++;

                        int size = ArduinoDetect.type_size((ArduinoDetect.ap_var_type)Enum.Parse(typeof(ArduinoDetect.ap_var_type), type.ToString()));

                            Console.Write("{0:X4}: type {1} ({2}) key {3} group_element {4} size {5} value ", pos - 4, type, ArduinoDetect.type_names[type], key, group, size);

                        if (key == 0)
                        {
                            //Array.Reverse(buffer, pos, 2);
                             return BitConverter.ToUInt16(buffer, pos);
                        }


                        for (int i = 0; i < size; i++)
                        {
                            Console.Write(" {0:X2}", buffer[pos]);
                            pos++;
                        }
                        Console.WriteLine();
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// STK v2 generate packet
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        static byte[] genstkv2packet(SerialPort serialPort, byte[] message)
        {
            byte[] data = new byte[300];
            byte ck = 0;

            data[0] = 0x1b;
            ck ^= data[0];
            data[1] = 0x1;
            ck ^= data[1];
            data[2] = (byte)((message.Length >> 8) & 0xff);
            ck ^= data[2];
            data[3] = (byte)(message.Length & 0xff);
            ck ^= data[3];
            data[4] = 0xe;
            ck ^= data[4];

            int a = 5;
            foreach (byte let in message)
            {
                data[a] = let;
                ck ^= let;
                a++;
            }
            data[a] = ck;
            a++;

            serialPort.Write(data, 0, a);
            //Console.WriteLine("about to read packet");

            byte[] ret = ArduinoDetect.readpacket(serialPort);

            //if (ret[1] == 0x0)
            {
                //Console.WriteLine("received OK");
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPort"></param>
        /// <returns></returns>
        static byte[] readpacket(SerialPort serialPort)
        {
            byte[] temp = new byte[4000];
            byte[] mes = new byte[2] { 0x0, 0xC0 }; // fail
            int a = 7;
            int count = 0;

            serialPort.ReadTimeout = 1000;

            while (count < a)
            {
                //Console.WriteLine("count {0} a {1} mes leng {2}",count,a,mes.Length);
                try
                {
                    temp[count] = (byte)serialPort.ReadByte();
                }
                catch { break; }


                //Console.Write("{1}", temp[0], (char)temp[0]);

                if (temp[0] != 0x1b)
                {
                    count = 0;
                    continue;
                }

                if (count == 3)
                {
                    a = (temp[2] << 8) + temp[3];
                    mes = new byte[a];
                    a += 5;
                }

                if (count >= 5)
                {
                    mes[count - 5] = temp[count];
                }

                count++;
            }

            //Console.WriteLine("read ck");
            try
            {
                temp[count] = (byte)serialPort.ReadByte();
            }
            catch { }

            count++;

            Array.Resize<byte>(ref temp, count);

            //Console.WriteLine(this.BytesToRead);

            return mes;
        }
    }
}