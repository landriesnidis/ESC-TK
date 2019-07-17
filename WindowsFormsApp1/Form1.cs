
using Karfans.VirtualCar.Xml;
using Landriesnidis.ESC_TK.Utils;
using Landriesnidis.Serial.ESC_TK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

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
            //tb.AppendText(Bytes2HexString(e.Bytes));
            tb.AppendText(e.DataText + "\n");
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

        private void button2_Click(object sender, EventArgs e)
        {
            XmlContent xc = new XmlContent();

            FileStream fs = new FileStream("./aaa.xml", FileMode.Create);
            XmlSerializer xs = new XmlSerializer(typeof(XmlContent));
            xs.Serialize(fs, xc);
            fs.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string json = JsonConvert.SerializeObject(sbw.SerialPort);
            tb.Text = json;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            byte[] bytes = Translator.HexString2Bytes(textBox1.Text);
            textBox2.Text = CheckSum(bytes);
        }

        private string CheckSum(byte[] bytes,bool isUseNegation = true)
        {
            int checksum = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                checksum += (int)bytes[i];
            }
            checksum = ((isUseNegation?~checksum: checksum) & 0xFF);
            return Convert.ToString(checksum, 16).ToUpper();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tb.AppendText(new Random().Next(15).ToString("X"));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "A5 A5 00 0E 33 89 F1 10 C1 12 87 98 34 54 56 22 12 95 9B ";
            byte[] bytes = Translator.HexString2Bytes(textBox1.Text);
            for (int i=0;i<bytes.Length-1;i++)
            {
                for (int j = i + 1; j< bytes.Length;j++)
                {
                    byte[] ab = bytes.Skip(i).Take(j - i).ToArray();
                    string s1 = Translator.Bytes2HexString(ab);
                    string s2 = CheckSum(ab,false);
                    tb.AppendText($"[{s2}] <= {s1}\n");

                    if (s2 == "8E")
                    {
                        Text += "发现！";
                        return;
                    }
                }
            }
        }
    }
}
