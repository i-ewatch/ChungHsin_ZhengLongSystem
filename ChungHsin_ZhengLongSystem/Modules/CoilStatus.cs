using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Modules
{
    public class CoilStatus
    {
        /// <summary>
        /// 狀態名稱
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// 狀態欄位
        /// </summary>
        public byte StateIndex { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public bool value { get; set; }
    }
}
