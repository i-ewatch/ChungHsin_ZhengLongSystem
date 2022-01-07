using ChungHsin_ZhengLongSystem.Components;
using ChungHsin_ZhengLongSystem.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Methods
{
    public class LogicMethod
    {
        /// <summary>
        /// 冰機狀態機
        /// </summary>
        private int chilerstatus = 4;
        /// <summary>
        /// 錯誤訊息狀態機
        /// </summary>
        private int alarmstaus = 0;
        /// <summary>
        /// 異常旗標
        /// </summary>
        public int AlarmStaus
        {
            get { return alarmstaus; }
            set
            {
                if (value != alarmstaus)
                {
                    alarmstaus = value;
                    switch (value)
                    {
                        case 1:
                            {
                                Log.Information($"{TCPComponent.Device.DeviceName} 冷卻水泵-異常 時間 : " + DateTime.Now.ToString());
                            }
                            break;
                        case 2:
                            {
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰水泵-異常 時間 : " + DateTime.Now.ToString());
                            }
                            break;
                        case 3:
                            {
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰機-異常 時間 : " + DateTime.Now.ToString());
                            }
                            break;
                    }
                }
            }
        }
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

        #region 狀態機開機確認
        /// <summary>
        /// 狀態機開機確認
        /// </summary>
        /// <returns></returns>
        public void Status_Start()
        {
            if (!TCPComponent.CWP_State && !TCPComponent.CWP.Alarm)
            {
                ChilerStatus = 0;
            }
            else if (!TCPComponent.CHP_1_State && !TCPComponent.CHP_1.Alarm)
            {
                ChilerStatus = 1;
            }
            else if (!TCPComponent.CHP_2_State && !TCPComponent.CHP_2.Alarm)
            {
                ChilerStatus = 2;
            }
            else if (!TCPComponent.CH_State && !TCPComponent.SoftWare_Control.Alarm)
            {
                ChilerStatus = 3;
            }
            else
            {
                ChilerStatus = 4;
            }
        }
        #endregion
        #region 狀態機關機確認
        /// <summary>
        /// 狀態機關機確認
        /// </summary>
        /// <returns></returns>
        public void Status_Stop()
        {
            if (TCPComponent.CH_State && !TCPComponent.SoftWare_Control.Alarm)
            {
                ChilerStatus = 3;
            }
            else if (TCPComponent.CHP_2_State && !TCPComponent.CHP_2.Alarm)
            {
                ChilerStatus = 2;
            }
            else if (TCPComponent.CHP_1_State && !TCPComponent.CHP_1.Alarm)
            {
                ChilerStatus = 1;
            }
            else if (TCPComponent.CWP_State && !TCPComponent.CWP.Alarm)
            {
                ChilerStatus = 0;
            }
            else
            {
                ChilerStatus = 0;
            }
        }
        #endregion

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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冷卻水泵關閉 時間 : " + ChilerTime);
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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰水泵1關閉 時間 : " + ChilerTime);
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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰水泵2關閉 時間 : " + ChilerTime);
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
                            Log.Information($"{TCPComponent.Device.DeviceName} 冰機關閉 時間 : " + ChilerTime);
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
                            Log.Information($"{TCPComponent.Device.DeviceName} 冷卻水泵開啟 時間 : " + ChilerTime);
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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰水泵1開啟 時間 : " + ChilerTime);
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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰水泵2開啟 時間 : " + ChilerTime);
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
                                Log.Information($"{TCPComponent.Device.DeviceName} 冰機開啟 時間 : " + ChilerTime);
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
