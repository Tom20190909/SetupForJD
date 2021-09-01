
using System;
using System.Configuration;

namespace SleckEc2KdInterface
{
    partial class wflogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.uiCheckBox1 = new Sunny.UI.UICheckBox();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uitxtLoginIPwd = new Sunny.UI.UITextBox();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uitxtLoginId = new Sunny.UI.UITextBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.uiCheckBox1);
            this.uiGroupBox1.Controls.Add(this.uiButton1);
            this.uiGroupBox1.Controls.Add(this.uitxtLoginIPwd);
            this.uiGroupBox1.Controls.Add(this.uiLabel2);
            this.uiGroupBox1.Controls.Add(this.uitxtLoginId);
            this.uiGroupBox1.Controls.Add(this.uiLabel1);
            this.uiGroupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox1.Location = new System.Drawing.Point(21, 40);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.Size = new System.Drawing.Size(469, 286);
            this.uiGroupBox1.TabIndex = 0;
            this.uiGroupBox1.Text = "登陆信息";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uiCheckBox1
            // 
            this.uiCheckBox1.Checked = true;
            this.uiCheckBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiCheckBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiCheckBox1.Location = new System.Drawing.Point(206, 178);
            this.uiCheckBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBox1.Name = "uiCheckBox1";
            this.uiCheckBox1.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.uiCheckBox1.Size = new System.Drawing.Size(175, 29);
            this.uiCheckBox1.TabIndex = 15;
            this.uiCheckBox1.Text = "记住用户名和密码";
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(206, 208);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(66, 31);
            this.uiButton1.TabIndex = 14;
            this.uiButton1.Text = "登陆";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // uitxtLoginIPwd
            // 
            this.uitxtLoginIPwd.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uitxtLoginIPwd.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uitxtLoginIPwd.Location = new System.Drawing.Point(206, 140);
            this.uitxtLoginIPwd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uitxtLoginIPwd.MinimumSize = new System.Drawing.Size(1, 16);
            this.uitxtLoginIPwd.Name = "uitxtLoginIPwd";
            this.uitxtLoginIPwd.PasswordChar = '*';
            this.uitxtLoginIPwd.ShowText = false;
            this.uitxtLoginIPwd.Size = new System.Drawing.Size(162, 29);
            this.uitxtLoginIPwd.TabIndex = 9;
            this.uitxtLoginIPwd.Text = "uvwxyz";
            this.uitxtLoginIPwd.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uitxtLoginIPwd.Watermark = "";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(92, 146);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(89, 23);
            this.uiLabel2.TabIndex = 8;
            this.uiLabel2.Text = "密  码:";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uitxtLoginId
            // 
            this.uitxtLoginId.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uitxtLoginId.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uitxtLoginId.Location = new System.Drawing.Point(206, 77);
            this.uitxtLoginId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uitxtLoginId.MinimumSize = new System.Drawing.Size(1, 16);
            this.uitxtLoginId.Name = "uitxtLoginId";
            this.uitxtLoginId.ShowText = false;
            this.uitxtLoginId.Size = new System.Drawing.Size(162, 29);
            this.uitxtLoginId.TabIndex = 7;
            this.uitxtLoginId.Text = "CUST1010";
            this.uitxtLoginId.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uitxtLoginId.Watermark = "";
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(92, 83);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(89, 23);
            this.uiLabel1.TabIndex = 1;
            this.uiLabel1.Text = "用户名:";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // wflogin
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(514, 349);
            this.Controls.Add(this.uiGroupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wflogin";
            this.ShowIcon = false;
            this.Text = "登陆";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 514, 349);
            this.Load += new System.EventHandler(this.wflogin_Load);
            this.uiGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void wflogin_Load(object sender, EventArgs e)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            uitxtLoginId.Text = config.AppSettings.Settings["userName"].Value;

            uitxtLoginIPwd.Text=config.AppSettings.Settings["userPwd"].Value;
        }

        #endregion

        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UITextBox uitxtLoginIPwd;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UITextBox uitxtLoginId;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UICheckBox uiCheckBox1;
    }
}