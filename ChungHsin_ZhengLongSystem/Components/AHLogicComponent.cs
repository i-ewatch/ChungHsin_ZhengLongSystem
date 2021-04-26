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
    public partial class AHLogicComponent : Field4Component
    {
        public AHLogicComponent(TCPComponent component)
        {
            InitializeComponent();
            TCPComponent = component;
        }
        /// <summary>
        /// 寫入延遲
        /// </summary>
        public int WriteTime = 1000;
        private TCPComponent TCPComponent { get; set; }
        public AHLogicComponent(IContainer container)
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
                    #region 空調
                    if (TCPComponent.AHManual_AutoFlag.control)//自動
                    {
                        #region 空調箱開機程序
                        if (TCPComponent.AHTimeFlag)//空調箱開機程序
                        {
                            if (TCPComponent.AH.Alarm)//空調箱異常
                            {
                                if (TCPComponent.AH_State)
                                {
                                    while (TCPComponent.AH_State)
                                    {
                                        TCPComponent.AH._State = true;
                                        TCPComponent.AH.State = false;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                            else//空調箱正常
                            {
                                if (!TCPComponent.AH_State)
                                {
                                    while (!TCPComponent.AH_State)
                                    {
                                        TCPComponent.AH._State = false;
                                        TCPComponent.AH.State = true;
                                        Thread.Sleep(WriteTime);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 空調箱關機程序
                        else//空調箱關機程序
                        {
                            if (TCPComponent.AH_State)
                            {
                                while (TCPComponent.AH_State)
                                {
                                    TCPComponent.AH._State = true;
                                    TCPComponent.AH.State = false;
                                    Thread.Sleep(WriteTime);
                                }
                            }
                        }
                        #endregion
                        if (TCPComponent.slave != null)
                        {
                            TCPComponent.slave.DataStore.CoilDiscretes.WritePoints(5, new bool[] { TCPComponent.AH_State });
                        }
                    }
                    else//手動
                    {
                        if (TCPComponent.slave != null)
                        {
                            TCPComponent.AH.State = TCPComponent.slave.DataStore.CoilDiscretes.ReadPoints(TCPComponent.AH.StateIndex, 1)[0];
                        }
                    }
                    #endregion
                    ComponentTime = DateTime.Now;
                    //Console.WriteLine("AHLogicComponent");
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
    }
}
