using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Protocols
{
    public abstract class CHData : AbsProtocol
    {
        public bool[] Fun1 { get; set; } = new bool[20];
        public bool[] Fun2 { get; set; } = new bool[187];
        public ushort[] Fun3 { get; set; } = new ushort[119];
        public ushort[] Fun4 { get; set; } = new ushort[89];
    }

}
