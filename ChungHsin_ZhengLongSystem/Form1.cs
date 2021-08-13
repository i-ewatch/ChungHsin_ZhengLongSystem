using ChungHsin_ZhengLongSystem.Components;
using ChungHsin_ZhengLongSystem.Configuration;
using ChungHsin_ZhengLongSystem.Methods;
using ChungHsin_ZhengLongSystem.Views;
using NModbus;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChungHsin_ZhengLongSystem
{
    public partial class Form1 : Form
    {

        #region Slave
        /// <summary>
        /// 通訊建置類別(通用)
        /// </summary>
        public ModbusFactory Factory { get; set; }
        /// <summary>
        /// 總Slave物件 (List類型，可以加入多個 IModbusSlave物件)
        /// </summary>
        public IModbusSlaveNetwork network;
        /// <summary>
        /// IP連線通訊
        /// </summary>
        public TcpListener slaveTcpListener;
        #endregion
        /// <summary>
        /// 軟體被開啟旗標
        /// </summary>
        private bool OpenFlag { get; set; }
        /// <summary>
        /// 通道資訊
        /// </summary>
        public GatewaySetting GatewaySetting { get; set; }
        /// <summary>
        /// 上傳物件
        /// </summary>
        public UpAPIComponent UpAPIComponent { get; set; }

        /// <summary>
        /// 通訊物件
        /// </summary>
        public List<Field4Component> Field4Components { get; set; } = new List<Field4Component>();
        /// <summary>
        /// 畫面物件
        /// </summary>
        public List<ConnectionUserControl> connectionUserControls { get; set; } = new List<ConnectionUserControl>();
        public Form1()
        {
            InitializeComponent();
            #region 禁止軟體重複開啟功能
            string ProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] p = Process.GetProcessesByName(ProcessName);
            if (p.Length > 1)
            {
                MessageBox.Show("軟體重複開啟");
                OpenFlag = true;
                Environment.Exit(1);
            }
            #endregion
            if (!OpenFlag)
            {
                #region Serilog initial
                Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}\\log\\log-.txt",
                                          rollingInterval: RollingInterval.Day,
                                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();        //宣告Serilog初始化
                #endregion
                GatewaySetting = InitialMethod.GatewaySettingLoad();
                #region Slave
                IPAddress address = new IPAddress(new byte[] { Convert.ToByte(GatewaySetting.SlaveLocation.Split('.')[0]), Convert.ToByte(GatewaySetting.SlaveLocation.Split('.')[1]), Convert.ToByte(GatewaySetting.SlaveLocation.Split('.')[2]), Convert.ToByte(GatewaySetting.SlaveLocation.Split('.')[3]) });
                // create and start the TCP slave
                slaveTcpListener = new TcpListener(address, GatewaySetting.SlaveRate);
                slaveTcpListener.Start();//通道打開
                Factory = new ModbusFactory();
                network = Factory.CreateSlaveNetwork(slaveTcpListener);
                network.ListenAsync();//開始側聽使用
                #endregion
                int Index = 0;
                foreach (var Deviceitem in GatewaySetting.Devices)
                {
                    TCPComponent component = new TCPComponent(Deviceitem, Factory, slaveTcpListener, network, GatewaySetting.CaseNo);
                    component.MyWorkState = true;
                    Field4Components.Add(component);
                    ConnectionUserControl connectionUserControl = new ConnectionUserControl(component) { Location = new Point(5 + 301 * (Index % 3), 10 + 90 * (Index / 3)) };//296, 84
                    Displaypanel.Controls.Add(connectionUserControl);
                    connectionUserControls.Add(connectionUserControl);
                    Index++;
                }
                UpAPIComponent = new UpAPIComponent(Field4Components);
                UpAPIComponent.MyWorkState = true;
                timer1.Interval = 1000;
                timer1.Enabled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in Field4Components)
            {
                item.MyWorkState = false;
            }
            UpAPIComponent.MyWorkState = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var item in connectionUserControls)
            {
                item.TextChange();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
            else
            {
                this.notifyIcon1.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_Resize(sender, e);
        }
    }
}
