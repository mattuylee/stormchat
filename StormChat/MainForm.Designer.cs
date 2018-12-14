namespace StormChat
{
	partial class MainForm
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
			this.picPhoto = new System.Windows.Forms.PictureBox();
			this.lblName = new System.Windows.Forms.Label();
			this.txtMotto = new System.Windows.Forms.TextBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
			this.SuspendLayout();
			// 
			// picPhoto
			// 
			this.picPhoto.Image = global::StormChat.Properties.Resources.DefaultPhoto;
			this.picPhoto.Location = new System.Drawing.Point(16, 20);
			this.picPhoto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.picPhoto.Name = "picPhoto";
			this.picPhoto.Size = new System.Drawing.Size(160, 150);
			this.picPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picPhoto.TabIndex = 5;
			this.picPhoto.TabStop = false;
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Font = new System.Drawing.Font("Microsoft YaHei", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblName.Location = new System.Drawing.Point(215, 20);
			this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(170, 52);
			this.lblName.TabIndex = 6;
			this.lblName.Text = "Mattuy";
			// 
			// txtMotto
			// 
			this.txtMotto.BackColor = System.Drawing.SystemColors.Control;
			this.txtMotto.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtMotto.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtMotto.Location = new System.Drawing.Point(215, 91);
			this.txtMotto.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtMotto.Name = "txtMotto";
			this.txtMotto.Size = new System.Drawing.Size(241, 23);
			this.txtMotto.TabIndex = 7;
			this.txtMotto.Text = "I am a programer.";
			// 
			// listBox1
			// 
			this.listBox1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 20;
			this.listBox1.Location = new System.Drawing.Point(16, 195);
			this.listBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(439, 564);
			this.listBox1.TabIndex = 8;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 789);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.txtMotto);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.picPhoto);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Opacity = 0D;
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picPhoto;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.TextBox txtMotto;
		private System.Windows.Forms.ListBox listBox1;
	}
}