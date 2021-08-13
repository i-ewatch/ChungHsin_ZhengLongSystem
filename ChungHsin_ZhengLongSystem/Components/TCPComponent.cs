using ChungHsin_ZhengLongSystem.Configuration;
using ChungHsin_ZhengLongSystem.Enums;
using ChungHsin_ZhengLongSystem.Methods;
using ChungHsin_ZhengLongSystem.Modules;
using ChungHsin_ZhengLongSystem.Protocols;
using NModbus;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Components
{
    public partial class TCPComponent : Field4Component
    {
        public TCPComponent(Device device, ModbusFactory factory, TcpListener SlaveTcpListener, IModbusSlaveNetwork Network, string caseNo)
        {
            CaseNo = caseNo;
            InitializeComponent();
            Device = device;
            slaveTcpListener = SlaveTcpListener;
            network = Network;
            Factory = factory;
        }
        /// <summary>
        /// TCP連線
        /// </summary>
        private TcpClient client { get; set; }
        /// <summary>
        /// 冰機邏輯
        /// </summary>
        private LogicMethod logicMethod { get; set; }
        /// <summary>
        /// 空調箱邏輯
        /// </summary>
        private AHLogicMethod AHLogicMethod { get; set; }
        /// <summary>
        /// 斷線時間
        /// </summary>
        public DateTime ConnectionTime { get; set; } = DateTime.Now;
        public TCPComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        protected override void AfterMyWorkStateChanged(object sender, EventArgs e)
        {
            if (myWorkState)
            {
                #region Master
                try
                {
                    client = new TcpClient(Device.Location, Device.Rate);
                    master = Factory.CreateMaster(client);//建立TCP通訊
                    master.Transport.ReadTimeout = 2500;
                    master.Transport.WriteTimeout = 2500;
                    master.Transport.Retries = 0;
                }
                catch (Exception ex)
                {
                    client = null;
                    Log.Error(ex, $"連線失敗 IP : {Device.Location} , port : {Device.Rate}");
                }

                DeviceTypeEnum = (DeviceTypeEnum)Device.DeviceTypeEnum;
                switch (DeviceTypeEnum)
                {
                    case DeviceTypeEnum.RT80:
                        {
                            GeneralCHProtocol protocol = new GeneralCHProtocol() { ID = (byte)Device.DeviceID, Device = Device, CaseNo = CaseNo };
                            AbsProtocol = protocol;
                            SoftWare_Control = new Status() { StateName = "軟體遠控啟動", Device = Device, StateIndex = 0, TCPComponent = this };
                            Alarm_Reset = new Status() { StateName = "警報復歸", Device = Device, StateIndex = 1, TCPComponent = this };
                            CHP_1 = new Status() { StateName = "冰水泵1啟動", Device = Device, StateIndex = 2, TCPComponent = this };
                            CHP_2 = new Status() { StateName = "冰水泵2啟動", Device = Device, StateIndex = 3, TCPComponent = this };
                            CWP = new Status() { StateName = "冷卻水泵啟動", Device = Device, StateIndex = 4, TCPComponent = this };
                            AH = new Status() { StateName = "空調箱啟動", Device = Device, StateIndex = 5, TCPComponent = this };
                            Output_Temp = new Value() { valueName = "冰水出水設定溫度", Device = Device, ValueIndex = 103, TCPComponent = this };
                        }
                        break;
                    case DeviceTypeEnum.RT40_50_60:
                        {
                            GeneralCHProtocol protocol = new GeneralCHProtocol() { ID = (byte)Device.DeviceID, Device = Device, CaseNo = CaseNo };
                            AbsProtocol = protocol;
                            SoftWare_Control = new Status() { StateName = "軟體遠控啟動", Device = Device, StateIndex = 0, TCPComponent = this };
                            Alarm_Reset = new Status() { StateName = "警報復歸", Device = Device, StateIndex = 1, TCPComponent = this };
                            CHP_1 = new Status() { StateName = "冰水泵1啟動", Device = Device, StateIndex = 2, TCPComponent = this };
                            CHP_2 = new Status() { StateName = "冰水泵2啟動", Device = Device, StateIndex = 3, TCPComponent = this };
                            CWP = new Status() { StateName = "冷卻水泵啟動", Device = Device, StateIndex = 4, TCPComponent = this };
                            AH = new Status() { StateName = "空調箱啟動", Device = Device, StateIndex = 5, TCPComponent = this };
                            Output_Temp = new Value() { valueName = "出水設定溫度", Device = Device, ValueIndex = 3, TCPComponent = this };
                        }
                        break;
                }
                #endregion
                #region Slave
                slave = Factory.CreateSlave((byte)Device.SlaveDeviceID);//設定ID
                network.AddSlave(slave);//開啟通訊 (每個Function 都開到最大 65535 無法修改)

                #region 冰機邏輯
                logicMethod = new LogicMethod(this);
                #endregion
                #region 空調箱邏輯
                AHLogicMethod = new AHLogicMethod(this);
                #endregion

                #endregion
                ComponentThread = new Thread(ProtocolAnalysis);
                ComponentThread.Start();
            }
            else
            {
                if (ComponentThread != null)
                {
                    ComponentThread.Abort();
                }
                if (slaveTcpListener != null)
                {
                    slaveTcpListener.Stop();
                }
                if (network != null)
                {
                    network.Dispose();
                    network = null;
                }
                if (slave != null)
                {
                    slave = null;
                }
                client.Close();
            }
        }
        private void ProtocolAnalysis()
        {
            while (myWorkState)
            {
                TimeSpan timeSpan = DateTime.Now.Subtract(ComponentTime);
                if (timeSpan.TotalMilliseconds >= 2000)
                {
                    if (client != null)
                    {
                        #region Master
                        while (CoilStatuses.Count > 0)
                        {
                            CoilStatus coil = CoilStatuses.Dequeue();
                            try
                            {
                                AbsProtocol.Write_State(master, coil.StateIndex, coil.value);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"寫入狀態失敗 狀態名稱 : {coil.StateName} Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                            }
                        }
                        while (HoldingRegisters.Count > 0)
                        {
                            HoldingRegister holdingRegister = HoldingRegisters.Dequeue();
                            try
                            {
                                AbsProtocol.Write_Value(master, holdingRegister.ValueIndex, holdingRegister.value);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"寫入數值失敗 數值名稱 : {holdingRegister.valueName} Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                            }
                        }
                        try
                        {
                            AbsProtocol.Read_Data(master);
                        }
                        catch (ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                        }
                        #endregion
                        CHData CHData = AbsProtocol as CHData;
                        if (CHData.Connection)//連線
                        {
                            ConnectionTime = DateTime.Now;
                            slave.DataStore.CoilDiscretes.WritePoints(24, new bool[] { true });
                            #region 運轉狀態讀取
                            CH_State = CHData.Fun2[109];
                            CHP_1_State = CHData.Fun2[100];
                            CHP_2_State = CHData.Fun2[102];
                            CWP_State = CHData.Fun2[104];
                            AH_State = CHData.Fun2[106];
                            #endregion
                            #region 回授狀態讀取
                            CHP_1.ResPonse = CHData.Fun1[CHP_1.StateIndex];
                            CHP_2.ResPonse = CHData.Fun1[CHP_2.StateIndex];
                            CWP.ResPonse = CHData.Fun1[CWP.StateIndex];
                            AH.ResPonse = CHData.Fun1[AH.StateIndex];
                            #endregion
                            #region 警報
                            switch (DeviceTypeEnum)
                            {
                                case DeviceTypeEnum.RT80:
                                    {
                                        SoftWare_Control.Alarm = CHData.Fun2[92];
                                    }
                                    break;
                                case DeviceTypeEnum.RT40_50_60:
                                    {
                                        SoftWare_Control.Alarm = CHData.Fun2[94];
                                    }
                                    break;
                            }
                            CHP_1.Alarm = CHData.Fun2[101];
                            CHP_2.Alarm = CHData.Fun2[103];
                            CWP.Alarm = CHData.Fun2[105];
                            AH.Alarm = CHData.Fun2[107];
                            #endregion
                            #region Slave
                            if (RunFlag)
                            {
                                slave.DataStore.CoilInputs.WritePoints(0, CHData.Fun2);
                                slave.DataStore.InputRegisters.WritePoints(0, CHData.Fun4);
                                if (CHData.Fun4[0] != 2)
                                {
                                    slave.DataStore.CoilDiscretes.WritePoints(0, CHData.Fun1);
                                    slave.DataStore.HoldingRegisters.WritePoints(0, CHData.Fun3);
                                }
                            }
                            else//初次讀取
                            {
                                slave.DataStore.CoilDiscretes.WritePoints(0, CHData.Fun1);
                                slave.DataStore.CoilInputs.WritePoints(0, CHData.Fun2);
                                slave.DataStore.HoldingRegisters.WritePoints(0, CHData.Fun3);
                                slave.DataStore.InputRegisters.WritePoints(0, CHData.Fun4);
                                SoftWare_Control._State = CHData.Fun1[SoftWare_Control.StateIndex];
                                Alarm_Reset._State = CHData.Fun1[Alarm_Reset.StateIndex];
                                if (CHData.Fun1[CHP_1.StateIndex])
                                {
                                    CHP_1._State = CHData.Fun1[CHP_1.StateIndex];
                                    CHP_2._State = CHData.Fun1[CHP_1.StateIndex];
                                }
                                else if (CHData.Fun1[CHP_2.StateIndex])
                                {
                                    CHP_1._State = CHData.Fun1[CHP_2.StateIndex];
                                    CHP_2._State = CHData.Fun1[CHP_2.StateIndex];
                                }
                                CWP._State = CHData.Fun1[CWP.StateIndex];
                                AH._State = CHData.Fun1[AH.StateIndex];

                                SoftWare_Control.ResPonse = CHData.Fun1[SoftWare_Control.StateIndex];
                                CHP_1.ResPonse = CHData.Fun1[CHP_1.StateIndex];
                                CHP_2.ResPonse = CHData.Fun1[CHP_2.StateIndex];
                                CWP.ResPonse = CHData.Fun1[CWP.StateIndex];
                                AH.ResPonse = CHData.Fun1[AH.StateIndex];
                                Output_Temp.ResPonse = CHData.Fun3[Output_Temp.ValueIndex];
                                RunFlag = true;
                            }
                            #endregion
                            #region 遠控功能
                            if (CHData.Fun4[0] == 2)
                            {
                                SoftWare_Control.ResPonse = CHData.Fun1[SoftWare_Control.StateIndex];
                                Manual_AutoFlag.control = slave.DataStore.CoilDiscretes.ReadPoints(20, 1)[0];//冰機手自動
                                TimeFlag = slave.DataStore.CoilDiscretes.ReadPoints(21, 1)[0];//冰機時控控制
                                AHManual_AutoFlag.control = slave.DataStore.CoilDiscretes.ReadPoints(22, 1)[0];//空調箱手自動
                                AHTimeFlag = slave.DataStore.CoilDiscretes.ReadPoints(23, 1)[0];//空調箱時控控制
                                #region 冰機邏輯
                                if (Manual_AutoFlag.control)//冰機自動
                                {
                                    if (TimeFlag)//啟動
                                    {
                                        #region 開機程序

                                        if (SoftWare_Control.Alarm)//冰機異常
                                        {
                                            if (logicMethod.ChilerStatus == 4)
                                            {
                                                logicMethod.ChilerStatus = 3;
                                            }
                                            logicMethod.CH_Close();
                                        }
                                        else//冰機正常
                                        {
                                            if (CWP.Alarm)//CWP異常
                                            {
                                                if (logicMethod.ChilerStatus == 4)
                                                {
                                                    logicMethod.ChilerStatus = 3;
                                                }
                                                logicMethod.CH_Close();
                                            }
                                            else//CWP正常
                                            {
                                                if (CHP_1.Alarm)//CHP1異常
                                                {
                                                    if (CHP_2.Alarm)//CHP2異常
                                                    {
                                                        if (logicMethod.ChilerStatus == 4)
                                                        {
                                                            logicMethod.ChilerStatus = 3;
                                                        }
                                                        logicMethod.CH_Close();
                                                    }
                                                    else//CHP2正常
                                                    {
                                                        if (logicMethod.ChilerStatus == -1)
                                                        {
                                                            logicMethod.ChilerStatus = 0;
                                                        }
                                                        logicMethod.CH_Open(2);
                                                    }
                                                }
                                                else//CHP1正常
                                                {
                                                    if (logicMethod.ChilerStatus == -1)
                                                    {
                                                        logicMethod.ChilerStatus = 0;
                                                    }
                                                    logicMethod.CH_Open(1);
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else//關閉
                                    {
                                        if (logicMethod.ChilerStatus == 4)
                                        {
                                            logicMethod.ChilerStatus = 3;
                                        }
                                        logicMethod.CH_Close();
                                    }
                                    slave.DataStore.CoilDiscretes.WritePoints(0, new bool[] { CH_State, Alarm_Reset.State, CHP_1_State, CHP_2_State, CWP_State });
                                }
                                else//冰機手動
                                {
                                    SoftWare_Control.State = slave.DataStore.CoilDiscretes.ReadPoints(SoftWare_Control.StateIndex, 1)[0];
                                    Alarm_Reset.State = slave.DataStore.CoilDiscretes.ReadPoints(Alarm_Reset.StateIndex, 1)[0];
                                    CHP_1.State = slave.DataStore.CoilDiscretes.ReadPoints(CHP_1.StateIndex, 1)[0];
                                    CHP_2.State = slave.DataStore.CoilDiscretes.ReadPoints(CHP_2.StateIndex, 1)[0];
                                    CWP.State = slave.DataStore.CoilDiscretes.ReadPoints(CWP.StateIndex, 1)[0];
                                    Output_Temp.value = slave.DataStore.HoldingRegisters.ReadPoints(Output_Temp.ValueIndex, 1)[0];
                                }
                                #endregion
                                #region 空調箱邏輯
                                if (AHManual_AutoFlag.control)//空調箱自動
                                {
                                    if (AHTimeFlag)
                                    {
                                        AHLogicMethod.AH_Opent();
                                    }
                                    else
                                    {
                                        AHLogicMethod.AH_Close();
                                    }
                                    AH._State = AH_State;
                                    slave.DataStore.CoilDiscretes.WritePoints(5, new bool[] { AH_State });
                                }
                                else//空調箱手動
                                {
                                    AH.State = slave.DataStore.CoilDiscretes.ReadPoints(AH.StateIndex, 1)[0];
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else//斷線
                        {
                            client = null;
                            TimeSpan ConnectiontimeSpan = DateTime.Now.Subtract(ConnectionTime);
                            if (ConnectiontimeSpan.TotalSeconds >= 300)
                            {
                                slave.DataStore.CoilDiscretes.WritePoints(24, new bool[] { false });
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            client = new TcpClient(Device.Location, Device.Rate);
                            master = Factory.CreateMaster(client);//建立TCP通訊
                            master.Transport.ReadTimeout = 2500;
                            master.Transport.WriteTimeout = 2500;
                            master.Transport.Retries = 0;
                        }
                        catch (Exception ex)
                        {
                            client = null;
                            TimeSpan ConnectiontimeSpan = DateTime.Now.Subtract(ConnectionTime);
                            if (ConnectiontimeSpan.TotalSeconds >= 300)
                            {
                                slave.DataStore.CoilDiscretes.WritePoints(24, new bool[] { false });
                            }
                            Log.Error(ex, $"連線失敗 IP : {Device.Location} , port : {Device.Rate}");
                        }
                    }
                    ComponentTime = DateTime.Now;
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
    }
}
