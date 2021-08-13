using ChungHsin_ZhengLongSystem.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChungHsin_ZhengLongSystem.Views
{
    public partial class ConnectionUserControl : UserControl
    {
        public ConnectionUserControl( TCPComponent component)
        {
            InitializeComponent();
            TCPComponent = component;
            Namelabel.Text = TCPComponent.Device.DeviceName;
        }
        
        private TCPComponent TCPComponent { get; set; }
        public void TextChange()
        {
            if (TCPComponent.AbsProtocol != null)
            {
                LastTimelabel.Text = TCPComponent.ConnectionTime.ToString("yyyy/MM/dd HH:mm:ss");
                if (!TCPComponent.AbsProtocol.Connection)
                {
                    Connectionlabel.ForeColor = Color.Red;
                    Connectionlabel.Text = "斷線";
                }
                else
                {
                    Connectionlabel.ForeColor = Color.Lime;
                    Connectionlabel.Text = "連線";
                }
            }
            else
            {
                LastTimelabel.Text = "-";
                Connectionlabel.ForeColor = Color.Red;
                Connectionlabel.Text = "斷線";
            }
        }
    }
}
