using NModbus;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Protocols
{
    public class GeneralCHProtocol : CHData
    {
        public override void Read_Data(IModbusMaster master)
        {
            try
            {
                Fun1 = master.ReadCoils(ID, 0, 20);
                var fun2_1 = master.ReadInputs(ID, 0, 100);
                var fun2_2 = master.ReadInputs(ID, 100, 87);
                for (int i = 0; i < fun2_1.Length; i++)
                {
                    Fun2[i] = fun2_1[i];
                }
                for (int i = 0; i < fun2_2.Length; i++)
                {
                    Fun2[100 + i] = fun2_2[i];
                }
                Fun4 = master.ReadInputRegisters(ID, 0, 89);
                Fun3 = master.ReadHoldingRegisters(ID, 0, 119);
                Connection = true;
            }
            catch (ThreadAbortException) { }
            catch (IOException) { }
            catch (Exception ex)
            {
                Connection = false;
                Log.Error(ex, "分析失敗");
            }

        }
        public override void Write_State(IModbusMaster master, byte StateIndex, bool value)
        {
            master.WriteSingleCoil(ID, StateIndex, value);
        }
        public override void Write_Value(IModbusMaster master, byte Index, ushort value)
        {
            master.WriteSingleRegister(ID, Index, value);
        }
    }
}
