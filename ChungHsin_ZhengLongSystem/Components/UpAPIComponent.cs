using ChungHsin_ZhengLongSystem.Protocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChungHsin_ZhengLongSystem.Components
{
    public partial class UpAPIComponent : Field4Component
    {
        public UpAPIComponent(List<Field4Component> field4Components)
        {
            InitializeComponent();
            Field4Components = field4Components;
        }
        public List<Field4Component> Field4Components { get; set; } = new List<Field4Component>();
        public UpAPIComponent(IContainer container)
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
                    foreach (var item in Field4Components)
                    {
                        GeneralCHProtocol protocol = (GeneralCHProtocol)item.AbsProtocol;
                        if (protocol.Connection)
                        {
                            protocol.Updata_API();
                            MinTime = DateTime.Now.Minute;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(80);
                }
            }
        }
    }
}
