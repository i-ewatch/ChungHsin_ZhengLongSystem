using ChungHsin_ZhengLongSystem.Configuration;
using ChungHsin_ZhengLongSystem.Enums;
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
        public TCPComponent(Device device, ModbusFactory factory, TcpListener SlaveTcpListener, IModbusSlaveNetwork Network)
        {
            InitializeComponent();
            Device = device;
            slaveTcpListener = SlaveTcpListener;
            network = Network;
            Factory = factory;
        }
        /// <summary>
        /// 冰機邏輯執行緒
        /// </summary>
        private LogicComponent logicComponent { get; set; }
        /// <summary>
        /// 空調箱邏輯執行緒
        /// </summary>
        private AHLogicComponent AHLogicComponent { get; set; }
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
                DeviceTypeEnum = (DeviceTypeEnum)Device.DeviceTypeEnum;
                switch (DeviceTypeEnum)
                {
                    case DeviceTypeEnum.RT80:
                        {
                            GeneralCHProtocol protocol = new GeneralCHProtocol() { ID = (byte)Device.DeviceID, Device = Device };
                            AbsProtocol = protocol;
                            SoftWare_Control = new Status() { StateName = "軟體遠控啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 0 };
                            Alarm_Reset = new Status() { StateName = "警報復歸", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 1 };
                            CHP_1 = new Status() { StateName = "冰水泵1啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 2 };
                            CHP_2 = new Status() { StateName = "冰水泵2啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 3 };
                            CWP = new Status() { StateName = "冷卻水泵啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 4 };
                            AH = new Status() { StateName = "空調箱啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 5 };
                            Output_Temp = new Value() { valueName = "冰水出水設定溫度", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, ValueIndex = 103 };
                        }
                        break;
                    case DeviceTypeEnum.RT40_50_60:
                        {
                            GeneralCHProtocol protocol = new GeneralCHProtocol() { ID = (byte)Device.DeviceID, Device = Device };
                            AbsProtocol = protocol;
                            SoftWare_Control = new Status() { StateName = "軟體遠控啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 0 };
                            Alarm_Reset = new Status() { StateName = "警報復歸", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 1 };
                            CHP_1 = new Status() { StateName = "冰水泵1啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 2 };
                            CHP_2 = new Status() { StateName = "冰水泵2啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 3 };
                            CWP = new Status() { StateName = "冷卻水泵啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 4 };
                            AH = new Status() { StateName = "空調箱啟動", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, StateIndex = 5 };
                            Output_Temp = new Value() { valueName = "出水設定溫度", Factory = Factory, AbsProtocol = AbsProtocol, master = master, Device = Device, ValueIndex = 3 };
                        }
                        break;
                }
                #endregion
                #region Slave
                slave = Factory.CreateSlave((byte)Device.SlaveDeviceID);//設定ID
                network.AddSlave(slave);//開啟通訊 (每個Function 都開到最大 65535 無法修改)

                #region 冰機邏輯
                logicComponent = new LogicComponent(this);
                logicComponent.MyWorkState = true;
                #endregion
                #region 空調箱邏輯
                AHLogicComponent = new AHLogicComponent(this);
                AHLogicComponent.MyWorkState = true;
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
                logicComponent.MyWorkState = false;
                AHLogicComponent.MyWorkState = false;
            }
        }
        private void ProtocolAnalysis()
        {
            while (myWorkState)
            {
                TimeSpan timeSpan = DateTime.Now.Subtract(ComponentTime);
                if (timeSpan.TotalMilliseconds >= 1000)
                {
                    #region Master
                    try
                    {
                        using (TcpClient client = new TcpClient(Device.Location, Device.Rate))
                        {
                            master = Factory.CreateMaster(client);//建立TCP通訊
                            AbsProtocol.Read_Data(master);
                        }
                    }
                    catch (ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Connect to device(IP : {Device.Location} 、 Port : {Device.Rate} ) failed.");
                    }
                    #endregion
                    CHData CHData = AbsProtocol as CHData;
                    #region 運轉狀態讀取
                    CH_State = CHData.Fun2[109];
                    CHP_1_State = CHData.Fun2[100];
                    CHP_2_State = CHData.Fun2[102];
                    CWP_State = CHData.Fun2[104];
                    AH_State = CHData.Fun2[106];
                    #endregion

                    #region Slave
                    if (RunFlag)
                    {
                        slave.DataStore.CoilInputs.WritePoints(0, CHData.Fun2);
                        slave.DataStore.InputRegisters.WritePoints(0, CHData.Fun4);
                        if (CHData.Fun4[0] == 1)
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
                        CHP_1._State = CHData.Fun1[CHP_1.StateIndex];
                        CHP_2._State = CHData.Fun1[CHP_2.StateIndex];
                        CWP._State = CHData.Fun1[CWP.StateIndex];
                        AH._State = CHData.Fun1[AH.StateIndex];
                        Output_Temp._value = CHData.Fun3[Output_Temp.ValueIndex];
                        RunFlag = true;
                    }
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
                    if (CHData.Fun4[0] == 2)
                    {
                        Manual_AutoFlag.control = slave.DataStore.CoilDiscretes.ReadPoints(20, 1)[0];//手自動
                        TimeFalg = slave.DataStore.CoilDiscretes.ReadPoints(21, 1)[0];//時控控制
                        AHManual_AutoFlag.control = slave.DataStore.CoilDiscretes.ReadPoints(22, 1)[0];
                        AHTimeFlag = slave.DataStore.CoilDiscretes.ReadPoints(23, 1)[0];
                       
                    }
                    #endregion
                    ComponentTime = DateTime.Now;
                    //Console.WriteLine("TCPComponent");
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
    }
}
