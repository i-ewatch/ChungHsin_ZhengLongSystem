
namespace ChungHsin_ZhengLongSystem.Views
{
    partial class ConnectionUserControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Namelabel = new System.Windows.Forms.Label();
            this.Connectionlabel = new System.Windows.Forms.Label();
            this.LastTimelabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Connectionlabel);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 84);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.LastTimelabel);
            this.panel2.Controls.Add(this.Namelabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(225, 82);
            this.panel2.TabIndex = 0;
            // 
            // Namelabel
            // 
            this.Namelabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Namelabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Namelabel.Font = new System.Drawing.Font("新細明體", 12F);
            this.Namelabel.Location = new System.Drawing.Point(0, 0);
            this.Namelabel.Name = "Namelabel";
            this.Namelabel.Size = new System.Drawing.Size(225, 42);
            this.Namelabel.TabIndex = 4;
            this.Namelabel.Text = "設備名稱";
            this.Namelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Connectionlabel
            // 
            this.Connectionlabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Connectionlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Connectionlabel.Font = new System.Drawing.Font("新細明體", 12F);
            this.Connectionlabel.ForeColor = System.Drawing.Color.Red;
            this.Connectionlabel.Location = new System.Drawing.Point(225, 0);
            this.Connectionlabel.Name = "Connectionlabel";
            this.Connectionlabel.Size = new System.Drawing.Size(69, 82);
            this.Connectionlabel.TabIndex = 6;
            this.Connectionlabel.Text = "斷線";
            this.Connectionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LastTimelabel
            // 
            this.LastTimelabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LastTimelabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LastTimelabel.Font = new System.Drawing.Font("新細明體", 12F);
            this.LastTimelabel.Location = new System.Drawing.Point(0, 42);
            this.LastTimelabel.Name = "LastTimelabel";
            this.LastTimelabel.Size = new System.Drawing.Size(225, 42);
            this.LastTimelabel.TabIndex = 5;
            this.LastTimelabel.Text = "設備名稱";
            this.LastTimelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConnectionUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ConnectionUserControl";
            this.Size = new System.Drawing.Size(296, 84);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Connectionlabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label LastTimelabel;
        private System.Windows.Forms.Label Namelabel;
    }
}
