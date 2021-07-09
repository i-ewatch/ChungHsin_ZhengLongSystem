using NModbus;
using Serilog;
using System;
using ChungHsin_ZhengLongSystem.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChungHsin_ZhengLongSystem.Modules;
using RestSharp;
using Newtonsoft.Json;

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
                Updata_API();
                Connection = true;
            }
            catch (ThreadAbortException) { }
            catch (IOException) { }
            catch (Exception ex)
            {
                Connection = false;
                Log.Error(ex, $"分析失敗 IP : {Device.Location}, Port : {Device.Rate}");
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
        private void Updata_API()
        {
            DeviceTypeEnum DeviceTypeEnum = (DeviceTypeEnum)Device.DeviceTypeEnum;
            ReceiveData data = new ReceiveData();
            data.ttime = DateTime.Now.ToString("yyyyMMddHHmmss");
            data.CaseNo = CaseNo;
            data.RecNo = Device.RecNo;
            #region Fun1
            data.Ai[0] = MathClass.work2to10($"{Convert.ToInt32(Fun1[15])}{Convert.ToInt32(Fun1[14])}{Convert.ToInt32(Fun1[13])}{Convert.ToInt32(Fun1[12])}{Convert.ToInt32(Fun1[11])}{Convert.ToInt32(Fun1[10])}{Convert.ToInt32(Fun1[9])}{Convert.ToInt32(Fun1[8])}{Convert.ToInt32(Fun1[7])}{Convert.ToInt32(Fun1[6])}{Convert.ToInt32(Fun1[5])}{Convert.ToInt32(Fun1[4])}{Convert.ToInt32(Fun1[3])}{Convert.ToInt32(Fun1[2])}{Convert.ToInt32(Fun1[1])}{Convert.ToInt32(Fun1[0])}");
            data.Ai[1] = MathClass.work2to10($"000000000000{Convert.ToInt32(Fun1[19])}{Convert.ToInt32(Fun1[18])}{Convert.ToInt32(Fun1[17])}{Convert.ToInt32(Fun1[16])}");
            #endregion
            #region Fun2
            data.Ai[2] = MathClass.work2to10($"{Convert.ToInt32(Fun2[15])}{Convert.ToInt32(Fun2[14])}{Convert.ToInt32(Fun2[13])}{Convert.ToInt32(Fun2[12])}{Convert.ToInt32(Fun2[11])}{Convert.ToInt32(Fun2[10])}{Convert.ToInt32(Fun2[9])}{Convert.ToInt32(Fun2[8])}{Convert.ToInt32(Fun2[7])}{Convert.ToInt32(Fun2[6])}{Convert.ToInt32(Fun2[5])}{Convert.ToInt32(Fun2[4])}{Convert.ToInt32(Fun2[3])}{Convert.ToInt32(Fun2[2])}{Convert.ToInt32(Fun2[1])}{Convert.ToInt32(Fun2[0])}");
            data.Ai[3] = MathClass.work2to10($"{Convert.ToInt32(Fun2[31])}{Convert.ToInt32(Fun2[30])}{Convert.ToInt32(Fun2[29])}{Convert.ToInt32(Fun2[28])}{Convert.ToInt32(Fun2[27])}{Convert.ToInt32(Fun2[26])}{Convert.ToInt32(Fun2[25])}{Convert.ToInt32(Fun2[24])}{Convert.ToInt32(Fun2[23])}{Convert.ToInt32(Fun2[22])}{Convert.ToInt32(Fun2[21])}{Convert.ToInt32(Fun2[20])}{Convert.ToInt32(Fun2[19])}{Convert.ToInt32(Fun2[18])}{Convert.ToInt32(Fun2[17])}{Convert.ToInt32(Fun2[16])}");
            data.Ai[4] = MathClass.work2to10($"{Convert.ToInt32(Fun2[47])}{Convert.ToInt32(Fun2[46])}{Convert.ToInt32(Fun2[45])}{Convert.ToInt32(Fun2[44])}{Convert.ToInt32(Fun2[43])}{Convert.ToInt32(Fun2[42])}{Convert.ToInt32(Fun2[41])}{Convert.ToInt32(Fun2[40])}{Convert.ToInt32(Fun2[39])}{Convert.ToInt32(Fun2[38])}{Convert.ToInt32(Fun2[37])}{Convert.ToInt32(Fun2[36])}{Convert.ToInt32(Fun2[35])}{Convert.ToInt32(Fun2[34])}{Convert.ToInt32(Fun2[33])}{Convert.ToInt32(Fun2[32])}");
            data.Ai[5] = MathClass.work2to10($"{Convert.ToInt32(Fun2[63])}{Convert.ToInt32(Fun2[62])}{Convert.ToInt32(Fun2[61])}{Convert.ToInt32(Fun2[60])}{Convert.ToInt32(Fun2[59])}{Convert.ToInt32(Fun2[58])}{Convert.ToInt32(Fun2[57])}{Convert.ToInt32(Fun2[56])}{Convert.ToInt32(Fun2[55])}{Convert.ToInt32(Fun2[54])}{Convert.ToInt32(Fun2[53])}{Convert.ToInt32(Fun2[52])}{Convert.ToInt32(Fun2[51])}{Convert.ToInt32(Fun2[50])}{Convert.ToInt32(Fun2[49])}{Convert.ToInt32(Fun2[48])}");
            data.Ai[6] = MathClass.work2to10($"{Convert.ToInt32(Fun2[79])}{Convert.ToInt32(Fun2[78])}{Convert.ToInt32(Fun2[77])}{Convert.ToInt32(Fun2[76])}{Convert.ToInt32(Fun2[75])}{Convert.ToInt32(Fun2[74])}{Convert.ToInt32(Fun2[73])}{Convert.ToInt32(Fun2[72])}{Convert.ToInt32(Fun2[71])}{Convert.ToInt32(Fun2[70])}{Convert.ToInt32(Fun2[69])}{Convert.ToInt32(Fun2[68])}{Convert.ToInt32(Fun2[67])}{Convert.ToInt32(Fun2[66])}{Convert.ToInt32(Fun2[65])}{Convert.ToInt32(Fun2[64])}");
            data.Ai[7] = MathClass.work2to10($"{Convert.ToInt32(Fun2[95])}{Convert.ToInt32(Fun2[94])}{Convert.ToInt32(Fun2[93])}{Convert.ToInt32(Fun2[92])}{Convert.ToInt32(Fun2[91])}{Convert.ToInt32(Fun2[90])}{Convert.ToInt32(Fun2[89])}{Convert.ToInt32(Fun2[88])}{Convert.ToInt32(Fun2[87])}{Convert.ToInt32(Fun2[86])}{Convert.ToInt32(Fun2[85])}{Convert.ToInt32(Fun2[84])}{Convert.ToInt32(Fun2[83])}{Convert.ToInt32(Fun2[82])}{Convert.ToInt32(Fun2[81])}{Convert.ToInt32(Fun2[80])}");
            data.Ai[8] = MathClass.work2to10($"{Convert.ToInt32(Fun2[111])}{Convert.ToInt32(Fun2[110])}{Convert.ToInt32(Fun2[109])}{Convert.ToInt32(Fun2[108])}{Convert.ToInt32(Fun2[107])}{Convert.ToInt32(Fun2[106])}{Convert.ToInt32(Fun2[105])}{Convert.ToInt32(Fun2[104])}{Convert.ToInt32(Fun2[103])}{Convert.ToInt32(Fun2[102])}{Convert.ToInt32(Fun2[101])}{Convert.ToInt32(Fun2[100])}{Convert.ToInt32(Fun2[99])}{Convert.ToInt32(Fun2[98])}{Convert.ToInt32(Fun2[97])}{Convert.ToInt32(Fun2[96])}");
            data.Ai[9] = MathClass.work2to10($"{Convert.ToInt32(Fun2[127])}{Convert.ToInt32(Fun2[126])}{Convert.ToInt32(Fun2[125])}{Convert.ToInt32(Fun2[124])}{Convert.ToInt32(Fun2[123])}{Convert.ToInt32(Fun2[122])}{Convert.ToInt32(Fun2[121])}{Convert.ToInt32(Fun2[120])}{Convert.ToInt32(Fun2[119])}{Convert.ToInt32(Fun2[118])}{Convert.ToInt32(Fun2[117])}{Convert.ToInt32(Fun2[116])}{Convert.ToInt32(Fun2[115])}{Convert.ToInt32(Fun2[114])}{Convert.ToInt32(Fun2[113])}{Convert.ToInt32(Fun2[112])}");
            data.Ai[10] = MathClass.work2to10($"{Convert.ToInt32(Fun2[143])}{Convert.ToInt32(Fun2[142])}{Convert.ToInt32(Fun2[141])}{Convert.ToInt32(Fun2[140])}{Convert.ToInt32(Fun2[139])}{Convert.ToInt32(Fun2[138])}{Convert.ToInt32(Fun2[137])}{Convert.ToInt32(Fun2[136])}{Convert.ToInt32(Fun2[135])}{Convert.ToInt32(Fun2[134])}{Convert.ToInt32(Fun2[133])}{Convert.ToInt32(Fun2[132])}{Convert.ToInt32(Fun2[131])}{Convert.ToInt32(Fun2[130])}{Convert.ToInt32(Fun2[129])}{Convert.ToInt32(Fun2[128])}");
            data.Ai[11] = MathClass.work2to10($"{Convert.ToInt32(Fun2[159])}{Convert.ToInt32(Fun2[158])}{Convert.ToInt32(Fun2[157])}{Convert.ToInt32(Fun2[156])}{Convert.ToInt32(Fun2[155])}{Convert.ToInt32(Fun2[154])}{Convert.ToInt32(Fun2[153])}{Convert.ToInt32(Fun2[152])}{Convert.ToInt32(Fun2[151])}{Convert.ToInt32(Fun2[150])}{Convert.ToInt32(Fun2[149])}{Convert.ToInt32(Fun2[148])}{Convert.ToInt32(Fun2[147])}{Convert.ToInt32(Fun2[146])}{Convert.ToInt32(Fun2[145])}{Convert.ToInt32(Fun2[144])}");
            data.Ai[12] = MathClass.work2to10($"{Convert.ToInt32(Fun2[175])}{Convert.ToInt32(Fun2[174])}{Convert.ToInt32(Fun2[173])}{Convert.ToInt32(Fun2[172])}{Convert.ToInt32(Fun2[171])}{Convert.ToInt32(Fun2[170])}{Convert.ToInt32(Fun2[169])}{Convert.ToInt32(Fun2[168])}{Convert.ToInt32(Fun2[167])}{Convert.ToInt32(Fun2[166])}{Convert.ToInt32(Fun2[165])}{Convert.ToInt32(Fun2[164])}{Convert.ToInt32(Fun2[163])}{Convert.ToInt32(Fun2[162])}{Convert.ToInt32(Fun2[161])}{Convert.ToInt32(Fun2[160])}");
            data.Ai[13] = MathClass.work2to10($"00000{Convert.ToInt32(Fun2[186])}{Convert.ToInt32(Fun2[185])}{Convert.ToInt32(Fun2[184])}{Convert.ToInt32(Fun2[183])}{Convert.ToInt32(Fun2[182])}{Convert.ToInt32(Fun2[181])}{Convert.ToInt32(Fun2[180])}{Convert.ToInt32(Fun2[179])}{Convert.ToInt32(Fun2[178])}{Convert.ToInt32(Fun2[177])}{Convert.ToInt32(Fun2[176])}");
            #endregion
            #region Fun3
            switch (DeviceTypeEnum)
            {
                case Enums.DeviceTypeEnum.RT80:
                    {
                        data.Ai[14] = Fun3[103];
                        data.Ai[26] = Fun3[115];
                        data.Ai[27] = Fun3[116];
                        data.Ai[28] = Fun3[117];
                        data.Ai[29] = Fun3[118];
                    }
                    break;
                case Enums.DeviceTypeEnum.RT40_50_60:
                    {
                        data.Ai[14] = Fun3[3];
                    }
                    break;
            }
            #endregion
            #region Fun4
            for (int i = 0; i < Fun4.Length; i++)
            {
                data.Ai[30 + i] = Fun4[i];
            }
            #endregion
            var client = new RestClient($"http://chem.api.igrand.com.tw" + "/api/CHRecive");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusDescription != "OK")
            {
                Log.Error(response.Content);
            }
        }
    }
}
