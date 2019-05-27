
using Landriesnidis.Serial.ESC_TK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SerialBackgroundWorker sbw;
        public Form1()
        {
            InitializeComponent();

            sbw = new SerialBackgroundWorker(tb);
            sbw.SerialDataReceivedEvent += Sbw_SerialDataReceivedEvent;
        }

        private void Sbw_SerialDataReceivedEvent(Landriesnidis.Serial.ESC_TK.SerialDataReceivedEventArgs e)
        {
            tb.AppendText(Bytes2HexString(e.Bytes));
                tb.AppendText(e.DataText);
        }

        private string String2Hex(string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = String.Format("{0:X} ", value);
                sb.Append(hexOutput);
            }
            return sb.ToString().Replace("D A ", "D A \r\n");
        }

        private string Bytes2HexString(byte[] bytes)
        {
            StringBuilder ret = new StringBuilder();
            foreach (byte b in bytes)
            {
                //{0:X2} 大写
                ret.AppendFormat("{0:X2} ", b);
            }
            return ret.ToString();
        }

        private string Hex2String(string input)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string[] hexValuesSplit = input.Split(' ');
                foreach (String hex in hexValuesSplit)
                {
                    if (hex == "") continue;
                    int value = Convert.ToInt32(hex, 16);
                    string stringValue = Char.ConvertFromUtf32(value);
                    //char charValue = (char)value;
                    sb.Append(stringValue);
                }
            }
            catch (FormatException ex)
            {
                return ex.Message;
            }
            return sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            sbw.SerialPort.BaudRate = 9600;
            sbw.SerialPort.PortName = "COM10";
            sbw.SerialPort.StopBits = StopBits.One;
            sbw.SerialPort.DataBits = 8;
            sbw.SerialPort.Parity   = Parity.None;
            sbw.SerialPort.ReadTimeout = -1;            // 设置超时读取时间
            sbw.SerialPort.RtsEnable = true;

            sbw.Mode = DataReceiveMode.STRING;
            sbw.Encode = Encoding.ASCII;
            sbw.EndMark = DataMark.LF;

            sbw.RunWorkerAsync();
        }
    }
}
