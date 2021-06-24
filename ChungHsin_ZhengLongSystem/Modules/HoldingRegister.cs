using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Modules
{
    public class HoldingRegister
    {
        /// <summary>
        /// 數值名稱
        /// </summary>
        public string valueName { get; set; }
        /// <summary>
        /// 數值欄位
        /// </summary>
        public byte ValueIndex { get; set; }
        /// <summary>
        /// 數值
        /// </summary>
        public ushort value { get; set; }
    }
}
