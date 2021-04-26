using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Components
{
    public partial class LogicComponent : Field4Component
    {
        public LogicComponent(TCPComponent component)
        {
            InitializeComponent();
            TCPComponent = component;
        }
        /// <summary>
        /// 寫入延遲
        /// </summary>
        public int WriteTime = 1000;
        private TCPComponent TCPComponent { get; set; }
        public LogicComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        protected override void AfterMyWorkStateChanged(object sender, EventArgs e)
        {
            if (myWorkState)
            {
                ComponentThread = new Thread(LogicAnalysis);
                ComponentThread.Start();
            }
            else
            {
                if (ComponentThread != null)
                {
                    ComponentThread.Abort();
                }
            }
        }
        protected void LogicAnalysis()
        {
            while (myWorkState)
            {
                TimeSpan timeSpan = DateTime.Now.Subtract(ComponentTime);
                if (timeSpan.TotalMilliseconds >= 1000)
                {
                    #region 冰機
                    if (TCPComponent.Manual_AutoFlag.control)//自動
                    {
                        #region 開機程序
                        if (TCPComponent.TimeFalg)//開機程序
                        {
                            if (TCPComponent.SoftWare_Control.Alarm)//冰機異常
                            {
                                CH_Close();
                            }
                            else//冰機正常
                            {
                                if (TCPComponent.CWP.Alarm)//CWP異常
                                {
                                    CH_Close();
                                }
                                else//CWP正常
                                {
                                    if (TCPComponent.CHP_1.Alarm)//CHP1異常
                                    {
                                        if (TCPComponent.CHP_2.Alarm)//CHP2異常
                                        {
                                            CH_Close();
                                        }
                                        else//CHP2正常
                                        {
                                            CH_Open(1);
                                        }
                                    }
                                    else//CHP1正常
                                    {
                                        CH_Open(0);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 關機程序
                        else//關機程序
                        {
                            CH_Close();
                        }
                        #endregion
                        if (TCPComponent.slave != null)
                        {
                            TCPComponent.AH.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.AH.StateIndex, 1)[0];
                            TCPComponent.slave.DataStore.CoilDiscretes.WritePoints(0, new bool[] { TCPComponent.CH_State, TCPComponent.Alarm_Reset.State, TCPComponent.CHP_1_State, TCPComponent.CHP_2_State, TCPComponent.CWP_State });
                        }
                    }
                    else//手動
                    {
                        if (TCPComponent.slave != null)
                        {
                            TCPComponent.SoftWare_Control.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.SoftWare_Control.StateIndex, 1)[0];
                            TCPComponent.Alarm_Reset.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.Alarm_Reset.StateIndex, 1)[0];
                            TCPComponent.CHP_1.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.CHP_1.StateIndex, 1)[0];
                            TCPComponent.CHP_2.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.CHP_2.StateIndex, 1)[0];
                            TCPComponent.CWP.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.CWP.StateIndex, 1)[0];
                            TCPComponent.Output_Temp.value = TCPComponent.slave.DataStore.HoldingRegisters.ReadPoints(TCPComponent.Output_Temp.ValueIndex, 1)[0];
                        }
                    }
                    #endregion
                    ComponentTime = DateTime.Now;
                    //Console.WriteLine("LogicComponent");
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
        #region 冰機關機流程
        /// <summary>
        /// 冰機關機流程
        /// </summary>
        public void CH_Close()
        {
            #region 冰機運轉
            if (TCPComponent.CH_State)//冰機運轉
            {
                while (TCPComponent.CH_State)
                {
                    TCPComponent.SoftWare_Control._State = true;
                    TCPComponent.SoftWare_Control.State = false;
                    Thread.Sleep(WriteTime);
                }
                Thread.Sleep(TCPComponent.Device.CTime);
                #region CHP1運轉
                if (TCPComponent.CHP_1_State)//CHP1運轉
                {
                    while (TCPComponent.CHP_1_State)
                    {
                        TCPComponent.CHP_1._State = true;
                        TCPComponent.CHP_1.State = false;
                        Thread.Sleep(WriteTime);
                    }
                    #region CHP2運轉
                    if (TCPComponent.CHP_2_State)//CHP2運轉
                    {
                        while (TCPComponent.CHP_2_State)
                        {
                            TCPComponent.CHP_2._State = true;
                            TCPComponent.CHP_2.State = false;
                            Thread.Sleep(WriteTime);
                        }
                        Thread.Sleep(TCPComponent.Device.DTime);
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                    #region CHP2停止
                    else//CHP2停止
                    {
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region CHP1停止
                else//CHP1停止
                {
                    #region CHP2運轉
                    if (TCPComponent.CHP_2_State)//CHP2運轉
                    {
                        while (TCPComponent.CHP_2_State)
                        {
                            TCPComponent.CHP_2._State = true;
                            TCPComponent.CHP_2.State = false;
                            Thread.Sleep(WriteTime);
                        }
                        Thread.Sleep(TCPComponent.Device.DTime);
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                    #region CHP2停止
                    else//CHP2停止
                    {
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            #region 冰機停止
            else//冰機停止
            {
                #region CHP1運轉
                if (TCPComponent.CHP_1_State)//CHP1運轉
                {
                    while (TCPComponent.CHP_1_State)
                    {
                        TCPComponent.CHP_1._State = true;
                        TCPComponent.CHP_1.State = false;
                        Thread.Sleep(WriteTime);
                    }
                    #region CHP2運轉
                    if (TCPComponent.CHP_2_State)//CHP2運轉
                    {
                        while (TCPComponent.CHP_2_State)
                        {
                            TCPComponent.CHP_2._State = true;
                            TCPComponent.CHP_2.State = false;
                            Thread.Sleep(WriteTime);
                        }
                        Thread.Sleep(TCPComponent.Device.DTime);
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                    #region CHP2停止
                    else//CHP2停止
                    {
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                }
                #endregion
                #region CHP1停止
                else//CHP1停止
                {
                    #region CHP2運轉
                    if (TCPComponent.CHP_2_State)//CHP2運轉
                    {
                        while (TCPComponent.CHP_2_State)
                        {
                            TCPComponent.CHP_2._State = true;
                            TCPComponent.CHP_2.State = false;
                            Thread.Sleep(WriteTime);
                        }
                        Thread.Sleep(TCPComponent.Device.DTime);
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                    #region CHP2停止
                    else//CHP2停止
                    {
                        if (TCPComponent.CWP_State)//CWP運轉
                        {
                            while (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                                Thread.Sleep(WriteTime);
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region 冰機開機流程
        /// <summary>
        /// 冰機開機流程
        /// </summary>
        /// <param name="CHPOpen"></param>
        public void CH_Open(int CHPOpen)
        {
            switch (CHPOpen)
            {
                case 0://CHP1
                    {
                        if (TCPComponent.CHP_2_State)
                        {
                            TCPComponent.CHP_2._State = true;
                            TCPComponent.CHP_2.State = false;
                        }                        
                        #region CWP停止
                        if (!TCPComponent.CWP_State)//CWP停止
                        {
                            while (!TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = false;
                                TCPComponent.CWP.State = true;
                                Thread.Sleep(WriteTime);
                            }
                            Thread.Sleep(TCPComponent.Device.ATime);
                            #region CHP1停止
                            if (!TCPComponent.CHP_1_State)//CHP1停止
                            {
                                while (!TCPComponent.CHP_1_State)
                                {
                                    TCPComponent.CHP_1._State = false;
                                    TCPComponent.CHP_1.State = true;
                                }
                                Thread.Sleep(TCPComponent.Device.BTime);
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion 
                            #region CHP1運轉
                            else//CHP1運轉
                            {
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                        #region CWP運轉
                        else//CWP運轉
                        {
                            #region CHP1停止
                            if (!TCPComponent.CHP_1_State)//CHP1停止
                            {
                                while (!TCPComponent.CHP_1_State)
                                {
                                    TCPComponent.CHP_1._State = false;
                                    TCPComponent.CHP_1.State = true;
                                }
                                Thread.Sleep(TCPComponent.Device.BTime);
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                            #region CHP1運轉
                            else//CHP1運轉
                            {
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    break;
                case 1://CHP2
                    {
                        if (TCPComponent.CHP_1_State)
                        {
                            TCPComponent.CHP_1._State = true;
                            TCPComponent.CHP_1.State = false;
                        }
                        #region CWP停止
                        if (!TCPComponent.CWP_State)//CWP停止
                        {
                            while (!TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = false;
                                TCPComponent.CWP.State = true;
                                Thread.Sleep(WriteTime);
                            }
                            Thread.Sleep(TCPComponent.Device.ATime);
                            #region CHP2停止
                            if (!TCPComponent.CHP_2_State)//CHP2停止
                            {
                                while (!TCPComponent.CHP_2_State)
                                {
                                    TCPComponent.CHP_2._State = false;
                                    TCPComponent.CHP_2.State = true;
                                }
                                Thread.Sleep(TCPComponent.Device.BTime);
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                            #region CHP2運轉
                            else//CHP2運轉
                            {
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                        #region CWP運轉
                        else//CWP運轉
                        {
                            #region CHP2停止
                            if (!TCPComponent.CHP_2_State)//CHP2停止
                            {
                                while (!TCPComponent.CHP_2_State)
                                {
                                    TCPComponent.CHP_2._State = false;
                                    TCPComponent.CHP_2.State = true;
                                }
                                Thread.Sleep(TCPComponent.Device.BTime);
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                            #region  CHP2運轉
                            else//CHP2運轉
                            {
                                if (!TCPComponent.CH_State)//冰機停止
                                {
                                    while (!TCPComponent.CH_State)
                                    {
                                        TCPComponent.SoftWare_Control._State = false;
                                        TCPComponent.SoftWare_Control.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    break;
            }
        }
        #endregion
    }
}
