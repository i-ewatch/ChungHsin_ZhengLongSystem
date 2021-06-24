using ChungHsin_ZhengLongSystem.Components;
using ChungHsin_ZhengLongSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Methods
{
    public class LogicMethod
    {
        private int chilerstatus = 4;
        /// <summary>
        /// 冰機時間控制
        /// </summary>
        public DateTime ChilerTime { get; set; }
        /// <summary>
        /// 冰機狀態
        /// </summary>
        public int ChilerStatus
        {
            get { return chilerstatus; }
            set
            {
                if (value != chilerstatus)
                {
                    ChilerTime = DateTime.Now;
                    chilerstatus = value;
                }
            }
        }
        /// <summary>
        /// 冰機狀態類型
        /// </summary>
        private ChilerStatusTypeEnum chilerStatusTypeEnum { get; set; }
        /// <summary>
        /// 通訊物件
        /// </summary>
        private TCPComponent TCPComponent { get; set; }
        public LogicMethod(TCPComponent component)
        {
            TCPComponent = component;
        }
        #region 冰機關機流程
        /// <summary>
        /// 冰機關機流程
        /// </summary>
        public void CH_Close()
        {
            chilerStatusTypeEnum = (ChilerStatusTypeEnum)ChilerStatus;
            switch (chilerStatusTypeEnum)
            {
                case ChilerStatusTypeEnum.CWP:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.DTime)
                        {
                            if (TCPComponent.CWP_State)
                            {
                                TCPComponent.CWP._State = true;
                                TCPComponent.CWP.State = false;
                            }
                            else
                            {
                                TCPComponent.CWP._State = false;
                                ChilerTime = DateTime.Now;
                                Console.WriteLine(ChilerTime + " 冷卻水泵關閉");
                                ChilerStatus = -1;
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CHP1:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.CTime)
                        {
                            if (TCPComponent.CHP_1_State)
                            {
                                TCPComponent.CHP_1._State = true;
                                TCPComponent.CHP_2._State = true;
                                TCPComponent.CHP_1.State = false;
                                TCPComponent.CHP_2.State = false;
                            }
                            else
                            {
                                TCPComponent.CHP_1._State = false;
                                TCPComponent.CHP_2._State = false;
                                ChilerTime = DateTime.Now;
                                Console.WriteLine(ChilerTime + " 冰水泵1關閉");
                                ChilerStatus = 0;
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CHP2:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.CTime)
                        {
                            if (TCPComponent.CHP_2_State)
                            {
                                TCPComponent.CHP_1._State = true;
                                TCPComponent.CHP_2._State = true;
                                TCPComponent.CHP_1.State = false;
                                TCPComponent.CHP_2.State = false;
                            }
                            else
                            {
                                TCPComponent.CHP_2._State = false;
                                TCPComponent.CHP_1._State = false;
                                Console.WriteLine(ChilerTime + " 冰水泵2關閉");
                                ChilerStatus = 0;
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CH:
                    {

                        if (TCPComponent.CH_State)
                        {
                            TCPComponent.SoftWare_Control._State = true;
                            TCPComponent.SoftWare_Control.State = false;
                        }
                        else
                        {
                            TCPComponent.SoftWare_Control._State = false;
                            ChilerTime = DateTime.Now;
                            Console.WriteLine(ChilerTime + " 冰機關閉");

                            if (TCPComponent.CHP_2_State)
                            {
                                ChilerStatus = 2;
                            }
                            else
                            {
                                if (TCPComponent.CHP_1_State)
                                {
                                    ChilerStatus = 1;
                                }
                                else
                                {
                                    ChilerStatus = 0;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        #endregion
        #region 冰機開機流程
        /// <summary>
        /// 冰機開機流程
        /// </summary>
        /// <param name="CHPOpen">冰水泵幾號機開機</param>
        public void CH_Open(int CHPOpen)
        {
            chilerStatusTypeEnum = (ChilerStatusTypeEnum)ChilerStatus;
            switch (chilerStatusTypeEnum)
            {
                case ChilerStatusTypeEnum.CWP:
                    {
                        if (!TCPComponent.CWP_State)
                        {
                            TCPComponent.CWP._State = false;
                            TCPComponent.CWP.State = true;
                        }
                        else
                        {
                            TCPComponent.CWP._State = true;
                            ChilerTime = DateTime.Now;
                            Console.WriteLine(ChilerTime + " 冷卻水泵開啟");
                            if (CHPOpen == 1)
                            {
                                if (!TCPComponent.CHP_1_State)
                                {
                                    ChilerStatus = CHPOpen;
                                }
                                else
                                {
                                    ChilerStatus = 3;
                                }
                            }
                            else if (CHPOpen == 2)
                            {
                                if (!TCPComponent.CHP_2_State)
                                {
                                    ChilerStatus = CHPOpen;
                                }
                                else
                                {
                                    ChilerStatus = 3;
                                }
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CHP1:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.ATime)
                        {
                            if (!TCPComponent.CHP_1_State && !TCPComponent.CHP_2_State)
                            {
                                TCPComponent.CHP_1._State = false;
                                TCPComponent.CHP_2._State = false;
                                TCPComponent.CHP_1.State = true;
                            }
                            else
                            {
                                TCPComponent.CHP_1._State = true;
                                TCPComponent.CHP_2._State = true;
                                ChilerTime = DateTime.Now;
                                Console.WriteLine(ChilerTime + " 冰水泵1開啟");
                                ChilerStatus = 3;
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CHP2:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.ATime)
                        {
                            if (!TCPComponent.CHP_1_State && !TCPComponent.CHP_2_State)
                            {
                                TCPComponent.CHP_1._State = false;
                                TCPComponent.CHP_2._State = false;
                                TCPComponent.CHP_2.State = true;
                            }
                            else
                            {
                                TCPComponent.CHP_1._State = true;
                                TCPComponent.CHP_2._State = true;
                                ChilerTime = DateTime.Now;
                                Console.WriteLine(ChilerTime + " 冰水泵2開啟");
                                ChilerStatus = 3;
                            }
                        }
                    }
                    break;
                case ChilerStatusTypeEnum.CH:
                    {
                        TimeSpan timeSpan = DateTime.Now.Subtract(ChilerTime);
                        if (timeSpan.TotalMilliseconds >= TCPComponent.Device.BTime)
                        {
                            if (!TCPComponent.CH_State)
                            {
                                TCPComponent.SoftWare_Control._State = false;
                                TCPComponent.SoftWare_Control.State = true;
                            }
                            else
                            {
                                TCPComponent.SoftWare_Control._State = true;
                                ChilerTime = DateTime.Now;
                                Console.WriteLine(ChilerTime + " 冰機開啟");
                                ChilerStatus = 4;
                            }
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
