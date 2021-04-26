using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Configuration
{
    public class GatewaySetting
    {
        /// <summary>
        /// 模擬IP 
        /// </summary>
        public string SlaveLocation { get; set; }
        /// <summary>
        /// 模擬Port
        /// </summary>
        public int SlaveRate { get; set; }
        /// <summary>
        /// 設備
        /// </summary>
        public List<Device> Devices { get; set; } = new List<Device>();
    }
    public class Device
    {
        /// <summary>
        /// 設備名稱
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 設備類型
        /// </summary>
        public int DeviceTypeEnum { get; set; }
        /// <summary>
        /// 監測IP
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Port號
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 設備ID
        /// </summary>
        public int DeviceID { get; set; }
        /// <summary>
        /// 模擬設備ID
        /// </summary>
        public int SlaveDeviceID { get; set; }
        /// <summary>
        /// A豪秒
        /// </summary>
        public int ATime { get; set; }
        /// <summary>
        /// B豪秒
        /// </summary>
        public int BTime { get; set; }
        /// <summary>
        /// C豪秒
        /// </summary>
        public int CTime { get; set; }
        /// <summary>
        /// D豪秒
        /// </summary>
        public int DTime { get; set; }
    }
}
