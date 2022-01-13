using ChungHsin_ZhengLongSystem.Configuration;
using ChungHsin_ZhengLongSystem.Methods;
using LineNotifyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Components
{
    public partial class LineNotifyComponent : Field4Component
    {
        private string Token { get; set; } = null;
        private List<LineNotifySetting> LineNotifySettings { get; set; }
        private List<Field4Component> Field4Components { get; set; } = new List<Field4Component>();
        private LineNotifyClass lineNotifyClass { get; set; }
        public LineNotifyComponent(List<Field4Component> field4Components)
        {
            InitializeComponent();
            Token = Convert.ToString(ConfigurationManager.AppSettings.Get("Token"));
            if (Token != null && Token != "Token")
            {
                lineNotifyClass = new LineNotifyClass(Token);
            }
            Field4Components = field4Components;
            LineNotifySettings = InitialMethod.LineNotify_Load();
            if (LineNotifySettings == null)
            {
                LineNotifySettings = new List<LineNotifySetting>();
                foreach (var item in Field4Components)
                {
                    LineNotifySetting setting = new LineNotifySetting()
                    {
                        Name = item.Device.DeviceName
                    };
                    LineNotifySettings.Add(setting);
                }
                InitialMethod.LineNotify_Save(LineNotifySettings);
            }
            else if (LineNotifySettings.Count != Field4Components.Count)
            {
                foreach (var item in Field4Components)
                {
                    LineNotifySetting line = LineNotifySettings.SingleOrDefault(g => g.Name == item.Device.DeviceName);
                    if (line == null)
                    {
                        LineNotifySetting setting = new LineNotifySetting()
                        {
                            Name = item.Device.DeviceName
                        };
                        LineNotifySettings.Add(setting);
                    }
                }
            }
        }

        public LineNotifyComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        int MinTime = -1;
        protected override void AfterMyWorkStateChanged(object sender, EventArgs e)
        {
            if (myWorkState)
            {
                ComponentThread = new Thread(ProtocolAnalysis);
                ComponentThread.Start();
            }
            else
            {
                if (ComponentThread != null)
                {
                    ComponentThread.Abort();
                }
            }
        }
        private void ProtocolAnalysis()
        {
            while (myWorkState)
            {
                if (DateTime.Now.Minute != MinTime)
                {
                    if (lineNotifyClass != null)
                    {
                        foreach (var item in LineNotifySettings)
                        {
                            TCPComponent component =(TCPComponent)Field4Components.SingleOrDefault(g => g.Device.DeviceName == item.Name);
                            if (component != null)
                            {
                                TimeSpan timeSpan = DateTime.Now.Subtract(component.ConnectionTime);
                                if (timeSpan.TotalMinutes >= 5 && component.CompleteFlag) //斷線超過五分鐘
                                {
                                    if (item.DisconnectFlag) //已發送
                                    {
                                        TimeSpan DisconnectionTimeSpan = DateTime.Now.Subtract(item.DisconnectTime);
                                        if (DisconnectionTimeSpan.TotalHours >= 24)//超過一天發送一次
                                        {
                                            lineNotifyClass.LineNotifyFunction($"{item.Name} \r\n 通訊斷線");
                                            item.DisconnectTime = DateTime.Now;
                                            InitialMethod.LineNotify_Save(LineNotifySettings);
                                        }
                                    }
                                    else//未發送
                                    {
                                        lineNotifyClass.LineNotifyFunction($"{item.Name} \r\n 通訊斷線");
                                        item.DisconnectFlag = true;
                                        item.DisconnectTime = DateTime.Now;
                                        InitialMethod.LineNotify_Save(LineNotifySettings);
                                    }
                                }
                                else if (item.DisconnectFlag)//通訊恢復
                                {
                                    lineNotifyClass.LineNotifyFunction($"{item.Name} \r\n 通訊恢復");
                                    item.DisconnectFlag = false;
                                    item.DisconnectTime = DateTime.Now;
                                    InitialMethod.LineNotify_Save(LineNotifySettings);
                                }
                            }
                            Thread.Sleep(80);
                        }
                    }
                    MinTime = DateTime.Now.Minute;
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
    }
}
