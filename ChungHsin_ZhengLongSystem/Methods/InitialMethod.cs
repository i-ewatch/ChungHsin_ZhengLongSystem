using ChungHsin_ZhengLongSystem.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Methods
{
    public class InitialMethod
    {
        /// <summary>
        /// 路徑位址
        /// </summary>
        private static string MyWorkPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        public static GatewaySetting GatewaySettingLoad()
        {
            GatewaySetting setting = null;
            try
            {
                if (!Directory.Exists($"{MyWorkPath}\\stf"))
                    Directory.CreateDirectory($"{MyWorkPath}\\stf");
                string SettingPath = $"{MyWorkPath}\\stf\\Gateway.json";
                if (File.Exists(SettingPath))
                {
                    string json = File.ReadAllText(SettingPath, Encoding.UTF8);
                    setting = JsonConvert.DeserializeObject<GatewaySetting>(json);
                }
                else
                {
                    GatewaySetting Setting = new GatewaySetting()
                    {
                        SlaveLocation = "127.0.0.1",
                        SlaveRate = 503,
                        Devices =
                        {
                            new Device()
                            {
                                AutoFlag = false,
                                DeviceName = "設備1",
                                DeviceTypeEnum = 0,
                                Location = "127.0.0.1",
                                Rate = 502,
                                DeviceID = 1,
                                SlaveDeviceID = 1,
                                ATime = 1000,
                                BTime = 1000,
                                CTime = 1000,
                                DTime = 1000
                            }
                        }
                    };
                    setting = Setting;
                    string output = JsonConvert.SerializeObject(setting, Formatting.Indented, new JsonSerializerSettings());
                    File.WriteAllText(SettingPath, output);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "載入設備失敗");
            }
            return setting;
        }
    }
}
