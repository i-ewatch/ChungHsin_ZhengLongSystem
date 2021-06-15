using ChungHsin_ZhengLongSystem.Configuration;
using ChungHsin_ZhengLongSystem.Enums;
using ChungHsin_ZhengLongSystem.Protocols;
using NModbus;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Components
{
    public class Field4Component : Component
    {
        /// <summary>
        /// 初次旗標
        /// </summary>
        public bool RunFlag { get; set; }
        /// <summary>
        /// 通訊數值物件
        /// </summary>
        public AbsProtocol AbsProtocol { get; set; }
        /// <summary>
        /// 設備資訊
        /// </summary>
        public Device Device { get; set; }
        /// <summary>
        /// 設備類型
        /// </summary>
        public DeviceTypeEnum DeviceTypeEnum { get; set; }

        #region 控制物件
        /// <summary>
        /// 手/自動功能
        /// </summary>
        public Control Manual_AutoFlag { get; set; } = new Control();
        /// <summary>
        /// 時控控制
        /// </summary>
        public bool TimeFalg { get; set; }
        /// <summary>
        /// 空調箱手/自動功能
        /// </summary>
        public Control AHManual_AutoFlag { get; set; } = new Control();
        /// <summary>
        /// 空調箱時控控制
        /// </summary>
        public bool AHTimeFlag { get; set; }
        /// <summary>
        /// 軟體遠控啟動(冰機啟動)
        /// </summary>
        public Status SoftWare_Control { get; set; }
        /// <summary>
        /// 警報復歸
        /// </summary>
        public Status Alarm_Reset { get; set; }
        /// <summary>
        /// 冰水泵1啟動
        /// </summary>
        public Status CHP_1 { get; set; }
        /// <summary>
        /// 冰水泵2啟動
        /// </summary>
        public Status CHP_2 { get; set; }
        /// <summary>
        /// 冷卻水泵啟動
        /// </summary>
        public Status CWP { get; set; }
        /// <summary>
        /// 空調箱啟動
        /// </summary>
        public Status AH { get; set; }
        /// <summary>
        /// 出水設定溫度
        /// </summary>
        public Value Output_Temp { get; set; }
        /// <summary>
        /// 冰機運轉狀態
        /// </summary>
        public bool CH_State { get; set; }
        /// <summary>
        /// 冰水泵1運轉狀態
        /// </summary>
        public bool CHP_1_State { get; set; }
        /// <summary>
        /// 冰水泵2運轉狀態
        /// </summary>
        public bool CHP_2_State { get; set; }
        /// <summary>
        /// 冷卻水泵運轉狀態
        /// </summary>
        public bool CWP_State { get; set; }
        /// <summary>
        /// 空調箱運轉狀態
        /// </summary>
        public bool AH_State { get; set; }
        #region 手自動物件
        public class Control
        {
            /// <summary>
            /// 手/自動
            /// <para>False = 手動</para>
            /// <para>True = 自動</para>
            /// </summary>
            public bool _control { get; set; }
            public bool control
            {
                get { return _control; }
                set
                {
                    if (value != _control)
                    {
                        _control = value;
                    }
                }
            }
        }
        #endregion
        #region 狀態物件
        /// <summary>
        /// 狀態物件
        /// </summary>
        public class Status
        {
            public string StateName { get; set; }
            /// <summary>
            /// 通訊建置類別(通用)
            /// </summary>
            public ModbusFactory Factory { get; set; }
            /// <summary>
            /// 通訊物件
            /// </summary>
            public AbsProtocol AbsProtocol { get; set; }
            /// <summary>
            /// master通訊物件
            /// </summary>
            public IModbusMaster master { get; set; }
            /// <summary>
            /// 設備資訊
            /// </summary>
            public Device Device { get; set; }
            /// <summary>
            /// 狀態編號
            /// </summary>
            public byte StateIndex { get; set; }
            public bool Alarm { get; set; } = false;
            public bool _State { get; set; }
            public bool State
            {
                get { return _State; }
                set
                {
                    if (value != _State)
                    {
                        if (!Alarm)
                        {
                            try
                            {
                                using (TcpClient client = new TcpClient(Device.Location, Device.Rate))
                                {
                                    master = Factory.CreateMaster(client);//建立TCP通訊
                                    AbsProtocol.Write_State(master, StateIndex, value);
                                    Thread.Sleep(80);
                                }
                                _State = value;
                            }
                            catch (IOException) { }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"寫入狀態失敗 狀態名稱 : {StateName} Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                            }
                        }
                        else
                        {
                            try
                            {
                                using (TcpClient client = new TcpClient(Device.Location, Device.Rate))
                                {
                                    master = Factory.CreateMaster(client);//建立TCP通訊
                                    AbsProtocol.Write_State(master, StateIndex, false);
                                    Thread.Sleep(80);
                                }
                                _State = value;
                            }
                            catch (IOException) { }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"寫入狀態失敗 狀態名稱 : {StateName} Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region 數值物件
        /// <summary>
        /// 數值物件
        /// </summary>
        public class Value
        {
            public string valueName { get; set; }
            /// <summary>
            /// 通訊建置類別(通用)
            /// </summary>
            public ModbusFactory Factory { get; set; }
            /// <summary>
            /// 通訊物件
            /// </summary>
            public AbsProtocol AbsProtocol { get; set; }
            /// <summary>
            /// master通訊物件
            /// </summary>
            public IModbusMaster master { get; set; }
            /// <summary>
            /// 設備資訊
            /// </summary>
            public Device Device { get; set; }
            /// <summary>
            /// 寫入位址
            /// </summary>
            public byte ValueIndex { get; set; }
            public ushort _value { get; set; }
            public ushort value
            {
                get { return _value; }
                set
                {
                    if (value != _value)
                    {
                        try
                        {
                            using (TcpClient client = new TcpClient(Device.Location, Device.Rate))
                            {
                                master = Factory.CreateMaster(client);//建立TCP通訊
                                AbsProtocol.Write_Value(master, ValueIndex, value);
                                Thread.Sleep(80);
                            }
                            _value = value;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"寫入數值失敗 數值名稱 : {valueName} Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Nmodbus物件
        /// <summary>
        /// 通訊建置類別(通用)
        /// </summary>
        public ModbusFactory Factory { get; set; }

        #region Master
        /// <summary>
        /// 通訊物件
        /// </summary>
        public IModbusMaster master { get; set; }
        #endregion

        #region Slave
        /// <summary>
        /// Slave物件 (若要多個Slaver請不要加入在這Field4Component，請在SlaveComponent內加入)
        /// </summary>
        public IModbusSlave slave;
        /// <summary>
        /// 總Slave物件 (List類型，可以加入多個 IModbusSlave物件)
        /// </summary>
        public IModbusSlaveNetwork network;
        /// <summary>
        /// IP連線通訊
        /// </summary>
        public TcpListener slaveTcpListener;
        #endregion
        #endregion

        #region 初始功能
        /// <summary>
        /// 執行緒
        /// </summary>
        public Thread ComponentThread { get; set; }
        /// <summary>
        /// 時間
        /// </summary>
        public DateTime ComponentTime { get; set; }
        public Field4Component()
        {
            OnMyWorkStateChanged += new MyWorkStateChanged(AfterMyWorkStateChanged);
        }
        protected void WhenMyWorkStateChange()
        {
            OnMyWorkStateChanged?.Invoke(this, null);
        }
        public delegate void MyWorkStateChanged(object sender, EventArgs e);
        public event MyWorkStateChanged OnMyWorkStateChanged;
        /// <summary>
        /// 系統工作路徑
        /// </summary>
        protected readonly string WorkPath = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 通訊功能啟動判斷旗標
        /// </summary>
        protected bool myWorkState;
        /// <summary>
        /// 通訊功能啟動旗標
        /// </summary>
        public bool MyWorkState
        {
            get { return myWorkState; }
            set
            {
                if (value != myWorkState)
                {
                    myWorkState = value;
                    WhenMyWorkStateChange();
                }
            }
        }
        /// <summary>
        /// 執行續工作狀態改變觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void AfterMyWorkStateChanged(object sender, EventArgs e) { }
        #endregion
    }
}
