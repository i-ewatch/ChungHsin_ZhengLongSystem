using ChungHsin_ZhengLongSystem.Configuration;
using MathLibrary;
using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Protocols
{
    public abstract class AbsProtocol
    {
        /// <summary>
        /// 數學公式
        /// </summary>
        public MathClass MathClass = new MathClass();
        /// <summary>
        /// 案場編號
        /// </summary>
        public string CaseNo { get; set; }
        /// <summary>
        /// 連線旗標
        /// </summary>
        public bool Connection { get; set; }
        /// <summary>
        /// 設備ID
        /// </summary>
        public byte ID { get; set; }
        /// <summary>
        /// 設備資訊
        /// </summary>
        public Device Device { get; set; }
        /// <summary>
        /// 讀取資料
        /// </summary>
        /// <param name="master"></param>
        public abstract void Read_Data(IModbusMaster master);
        /// <summary>
        /// 狀態寫入
        /// </summary>
        /// <param name="Master"></param>
        /// <param name="State"></param>

        public abstract void Write_State(IModbusMaster master, byte StateIndex, bool value);
        /// <summary>
        /// 數值寫入
        /// </summary>
        /// <param name="master"></param>
        public abstract void Write_Value(IModbusMaster master, byte Index, ushort value);
    }
}
