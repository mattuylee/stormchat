namespace StormChat
{
	partial class LogForm
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.txtUser = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.btnLogin = new System.Windows.Forms.Button();
			this.chkRemerberPassword = new System.Windows.Forms.CheckBox();
			this.chkAutoLogin = new System.Windows.Forms.CheckBox();
			this.picPhoto = new System.Windows.Forms.PictureBox();
			this.lblBadge = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
			this.SuspendLayout();
			// 
			// txtUser
			// 
			this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtUser.Font = new System.Drawing.Font("DengXian", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtUser.Location = new System.Drawing.Point(207, 260);
			this.txtUser.Margin = new System.Windows.Forms.Padding(4);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(351, 30);
			this.txtUser.TabIndex = 1;
			this.txtUser.Text = "mattuy";
			// 
			// txtPassword
			// 
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtPassword.Font = new System.Drawing.Font("DengXian", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtPassword.Location = new System.Drawing.Point(207, 295);
			this.txtPassword.Margin = new System.Windows.Forms.Padding(133, 4, 4, 4);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(351, 32);
			this.txtPassword.TabIndex = 3;
			this.txtPassword.Text = "1233211234567";
			// 
			// btnLogin
			// 
			this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLogin.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnLogin.Location = new System.Drawing.Point(207, 362);
			this.btnLogin.Margin = new System.Windows.Forms.Padding(4);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(352, 48);
			this.btnLogin.TabIndex = 6;
			this.btnLogin.Text = "登录";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// chkRemerberPassword
			// 
			this.chkRemerberPassword.AutoSize = true;
			this.chkRemerberPassword.Font = new System.Drawing.Font("SimSun", 10F);
			this.chkRemerberPassword.Location = new System.Drawing.Point(208, 335);
			this.chkRemerberPassword.Margin = new System.Windows.Forms.Padding(4);
			this.chkRemerberPassword.Name = "chkRemerberPassword";
			this.chkRemerberPassword.Size = new System.Drawing.Size(98, 21);
			this.chkRemerberPassword.TabIndex = 4;
			this.chkRemerberPassword.Text = "记住密码";
			this.chkRemerberPassword.UseVisualStyleBackColor = true;
			// 
			// chkAutoLogin
			// 
			this.chkAutoLogin.AutoSize = true;
			this.chkAutoLogin.Font = new System.Drawing.Font("SimSun", 10F);
			this.chkAutoLogin.Location = new System.Drawing.Point(449, 335);
			this.chkAutoLogin.Margin = new System.Windows.Forms.Padding(4);
			this.chkAutoLogin.Name = "chkAutoLogin";
			this.chkAutoLogin.Size = new System.Drawing.Size(98, 21);
			this.chkAutoLogin.TabIndex = 5;
			this.chkAutoLogin.Text = "自动登录";
			this.chkAutoLogin.UseVisualStyleBackColor = true;
			// 
			// picPhoto
			// 
			this.picPhoto.Image = global::StormChat.Properties.Resources.DefaultPhoto;
			this.picPhoto.Location = new System.Drawing.Point(21, 260);
			this.picPhoto.Margin = new System.Windows.Forms.Padding(4);
			this.picPhoto.Name = "picPhoto";
			this.picPhoto.Size = new System.Drawing.Size(160, 150);
			this.picPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picPhoto.TabIndex = 4;
			this.picPhoto.TabStop = false;
			// 
			// lblBadge
			// 
			this.lblBadge.BackColor = System.Drawing.Color.Transparent;
			this.lblBadge.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblBadge.Font = new System.Drawing.Font("Microsoft Sans Serif", 50.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.lblBadge.ForeColor = System.Drawing.Color.Maroon;
			this.lblBadge.Image = global::StormChat.Properties.Resources.LogFormBackground;
			this.lblBadge.Location = new System.Drawing.Point(0, 0);
			this.lblBadge.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblBadge.Name = "lblBadge";
			this.lblBadge.Size = new System.Drawing.Size(579, 229);
			this.lblBadge.TabIndex = 2;
			this.lblBadge.Text = "StormChat";
			this.lblBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(579, 428);
			this.Controls.Add(this.chkAutoLogin);
			this.Controls.Add(this.chkRemerberPassword);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.picPhoto);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblBadge);
			this.Controls.Add(this.txtUser);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "LogForm";
			this.Text = "登录";
			this.Load += new System.EventHandler(this.LogForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.Label lblBadge;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.PictureBox picPhoto;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.CheckBox chkRemerberPassword;
		private System.Windows.Forms.CheckBox chkAutoLogin;
	}
}

