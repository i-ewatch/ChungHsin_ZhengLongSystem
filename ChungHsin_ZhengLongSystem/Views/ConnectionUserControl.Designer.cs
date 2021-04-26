
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
            this.Connectionlabel = new System.Windows.Forms.Label();
            this.Namelabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Connectionlabel);
            this.panel1.Controls.Add(this.Namelabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 28);
            this.panel1.TabIndex = 0;
            // 
            // Connectionlabel
            // 
            this.Connectionlabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Connectionlabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Connectionlabel.Font = new System.Drawing.Font("新細明體", 12F);
            this.Connectionlabel.ForeColor = System.Drawing.Color.Red;
            this.Connectionlabel.Location = new System.Drawing.Point(225, 0);
            this.Connectionlabel.Name = "Connectionlabel";
            this.Connectionlabel.Size = new System.Drawing.Size(69, 26);
            this.Connectionlabel.TabIndex = 3;
            this.Connectionlabel.Text = "斷線";
            this.Connectionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Namelabel
            // 
            this.Namelabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Namelabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.Namelabel.Font = new System.Drawing.Font("新細明體", 12F);
            this.Namelabel.Location = new System.Drawing.Point(0, 0);
            this.Namelabel.Name = "Namelabel";
            this.Namelabel.Size = new System.Drawing.Size(225, 26);
            this.Namelabel.TabIndex = 2;
            this.Namelabel.Text = "設備名稱";
            this.Namelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConnectionUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ConnectionUserControl";
            this.Size = new System.Drawing.Size(296, 28);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Connectionlabel;
        private System.Windows.Forms.Label Namelabel;
    }
}
