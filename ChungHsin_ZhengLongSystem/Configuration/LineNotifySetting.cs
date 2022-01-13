using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Configuration
{
    public class LineNotifySetting
    {
        /// <summary>
        /// 樓層名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 斷線發報時間點
        /// </summary>
        public DateTime DisconnectTime { get; set; } = Convert.ToDateTime("1990/01/01 00:00:00");
        /// <summary>
        /// 斷線發送旗標 True = 已發送 ，False = 未發送
        /// </summary>
        public bool DisconnectFlag { get; set; } = false;
    }
}
