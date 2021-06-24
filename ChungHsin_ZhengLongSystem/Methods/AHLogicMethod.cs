using ChungHsin_ZhengLongSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Methods
{
    public class AHLogicMethod
    {
        /// <summary>
        /// 通訊物件
        /// </summary>
        private TCPComponent TCPComponent { get; set; }

        public AHLogicMethod(TCPComponent component)
        {
            TCPComponent = component;
        }
        /// <summary>
        /// 空調箱關閉流程
        /// </summary>
        public void AH_Close()
        {
            if (TCPComponent.AH_State)
            {
                TCPComponent.AH.State = false;
            }
            else
            {
                TCPComponent.AH._State = false;
            }
        }
        /// <summary>
        /// 空調箱開啟流程
        /// </summary>
        public void AH_Opent()
        {
            if (!TCPComponent.AH_State)
            {
                TCPComponent.AH.State = true;
            }
            else
            {
                TCPComponent.AH._State = true;
            }
        }
    }
}
