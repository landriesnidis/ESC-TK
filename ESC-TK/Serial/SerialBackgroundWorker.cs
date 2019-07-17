using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Landriesnidis.Serial.ESC_TK
{
    public class SerialBackgroundWorker : BackgroundWorker
    {
        // 串行端口
        public SerialPort SerialPort { get; set; } = new SerialPort();

        /// <summary>
        /// 定义委托：数据接收事件委托
        /// </summary>
        /// <param name="e"></param>
        public delegate void SerialDataReceivedEventHandler(SerialDataReceivedEventArgs e);

        /// <summary>
        /// 定义事件：数据接收事件
        /// </summary>
        public event SerialDataReceivedEventHandler SerialDataReceivedEvent;

        /// <summary>
        /// 终止符
        /// 当接收模式为“STRING”时有效
        /// </summary>
        public DataMark EndMark { get; set; } = DataMark.LF;

        /// <summary>
        /// 字符编码
        /// </summary>
        public Encoding Encode { get { return SerialPort.Encoding; } set { SerialPort.Encoding = value; } }

        /// <summary>
        /// 数据接收模式
        /// </summary>
        public DataReceiveMode Mode { get; set; } = DataReceiveMode.STRING;

        public Control ContentControl { get; set; }

        public SerialBackgroundWorker(Control control)
        {
            ContentControl = control;

            DoWork += SerialBackgroundWorker_DoWork;
            ProgressChanged += SerialBackgroundWorker_ProgressChanged;
            RunWorkerCompleted += SerialBackgroundWorker_RunWorkerCompleted;
            SerialPort.DataReceived += Serial_DataReceived;
        }

        private void Serial_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            
            int len1 = SerialPort.BytesToRead;
            int len2 = len1;

            // 延时接受数据
            while ((len2 = SerialPort.BytesToRead) > len1)
            {
                len1 = len2;
                System.Threading.Thread.Sleep(10);
            }
            

            SerialDataReceivedEventArgs args = new SerialDataReceivedEventArgs();
            args.SerialPort = SerialPort;
            args.DataReceiveMode = Mode;

            if (Mode == DataReceiveMode.STRING)
            {
                // 终止符字符串
                string strEndMark = string.Empty;
                switch (EndMark)
                {
                    case DataMark.CR:
                        strEndMark = "\r";
                        break;
                    case DataMark.LF:
                        strEndMark = "\n";
                        break;
                    case DataMark.CR_LF:
                        strEndMark = "\r\n";
                        break;
                    default:

                        break;
                }

                // 读取数据直到终止符
                string strData = SerialPort.ReadTo(strEndMark);
                args.DataText = strData;
                args.Bytes = Encode.GetBytes(strData);
                args.IsHexData = false;
            }
            else
            {
                byte[] ReceivedData = new byte[SerialPort.BytesToRead];
                SerialPort.Read(ReceivedData, 0, ReceivedData.Length);

                args.Bytes = ReceivedData;
                args.DataText = Encode.GetString(ReceivedData);
                args.IsHexData = true;
            }

            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                ContentControl.Invoke((EventHandler)delegate
                {
                    SerialDataReceivedEvent(args);
                });

            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }

            // 回调事件 
            // SerialDataReceivedEvent(args);
        }

        /// <summary>
        /// 异步操作完成或取消时执行的操作，当调用DoWork事件执行完成时触发。 
        /// 该事件的RunWorkerCompletedEventArgs参数包含三个常用的属性Error,Cancelled,Result。其中，Error表示在执行异步操作期间发生的错误；Cancelled用于判断用户是否取消了异步操作；Result属性接收来自DoWork事件的DoWorkEventArgs参数的Result属性值，可用于传递异步操作的执行结果。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 当调用BackgroundWorker.ReportProgress(int percentProgress)方式时触发该事件。 
        /// 该事件的ProgressChangedEventArgs.ProgressPercentage属性可以接收来自ReportProgress方法传递的percentProgress参数值,ProgressChangedEventArgs.UserState属性可以接收来自ReportProgress方法传递的userState参数。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 用于承载异步操作。当调用BackgroundWorker.RunWorkerAsync()时触发。 
        /// 需要注意的是，由于DoWork事件内部的代码运行在非UI线程之上，所以在DoWork事件内部应避免于用户界面交互，而于用户界面交互的操作应放置在ProgressChanged和RunWorkerCompleted事件中。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SerialPort.Open();
        }
    }

    /// <summary>
    /// 串口数据接收事件参数
    /// </summary>
    public class SerialDataReceivedEventArgs
    {
        public SerialPort SerialPort { get; set; }
        public bool IsHexData { get; set; }
        public byte[] Bytes { get; set; }
        public string DataText { get; set; }
        public DataReceiveMode DataReceiveMode { get; set; }
    }

    /// <summary>
    /// 标记符号
    /// </summary>
    public enum DataMark
    {
        CR,         // \r
        LF,         // \n
        CR_LF       // \r\n

    }

    /// <summary>
    /// 数据接收模式
    /// </summary>
    public enum DataReceiveMode
    {
        BYTES,
        STRING
    }
}
