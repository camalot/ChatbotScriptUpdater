namespace ChatbotScriptUpdater {
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
		protected override void Dispose ( bool disposing ) {
			if ( disposing && ( components != null ) ) {
				components.Dispose ( );
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( ) {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.statusLabel = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.updateNow = new System.Windows.Forms.Button();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.progressLabel = new System.Windows.Forms.Label();
			this.linkRepo = new System.Windows.Forms.LinkLabel();
			this.website = new System.Windows.Forms.LinkLabel();
			this.copyright = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// statusLabel
			// 
			this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.statusLabel.Location = new System.Drawing.Point(12, 9);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(527, 20);
			this.statusLabel.TabIndex = 0;
			this.statusLabel.Text = "[Status]";
			this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancel.Location = new System.Drawing.Point(423, 149);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(116, 37);
			this.cancel.TabIndex = 1;
			this.cancel.Text = "&Close";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// updateNow
			// 
			this.updateNow.Enabled = false;
			this.updateNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updateNow.Location = new System.Drawing.Point(12, 40);
			this.updateNow.Name = "updateNow";
			this.updateNow.Size = new System.Drawing.Size(527, 37);
			this.updateNow.TabIndex = 2;
			this.updateNow.Text = "Download && Update";
			this.updateNow.UseVisualStyleBackColor = true;
			this.updateNow.Click += new System.EventHandler(this.UpdateNow_Click);
			// 
			// progress
			// 
			this.progress.Location = new System.Drawing.Point(12, 109);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(527, 23);
			this.progress.Step = 1;
			this.progress.TabIndex = 3;
			// 
			// progressLabel
			// 
			this.progressLabel.AutoEllipsis = true;
			this.progressLabel.Location = new System.Drawing.Point(13, 80);
			this.progressLabel.Name = "progressLabel";
			this.progressLabel.Size = new System.Drawing.Size(526, 26);
			this.progressLabel.TabIndex = 4;
			this.progressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// linkRepo
			// 
			this.linkRepo.AutoSize = true;
			this.linkRepo.Location = new System.Drawing.Point(9, 149);
			this.linkRepo.Name = "linkRepo";
			this.linkRepo.Size = new System.Drawing.Size(86, 13);
			this.linkRepo.TabIndex = 5;
			this.linkRepo.TabStop = true;
			this.linkRepo.Text = "Open Repository";
			this.linkRepo.Click += new System.EventHandler(this.LinkRepo_Click);
			// 
			// website
			// 
			this.website.AutoSize = true;
			this.website.Location = new System.Drawing.Point(12, 173);
			this.website.Name = "website";
			this.website.Size = new System.Drawing.Size(52, 13);
			this.website.TabIndex = 6;
			this.website.TabStop = true;
			this.website.Text = "[Website]";
			this.website.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Website_LinkClicked);
			// 
			// copyright
			// 
			this.copyright.AutoSize = true;
			this.copyright.Location = new System.Drawing.Point(148, 198);
			this.copyright.Name = "copyright";
			this.copyright.Size = new System.Drawing.Size(257, 13);
			this.copyright.TabIndex = 7;
			this.copyright.TabStop = true;
			this.copyright.Text = "ApplicationUpdater - Copyright © Ryan Conrad 2020 ";
			this.copyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Copyright_LinkClicked);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(551, 220);
			this.Controls.Add(this.copyright);
			this.Controls.Add(this.website);
			this.Controls.Add(this.linkRepo);
			this.Controls.Add(this.progressLabel);
			this.Controls.Add(this.progress);
			this.Controls.Add(this.updateNow);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.statusLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Application Updater";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button updateNow;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.LinkLabel linkRepo;
		private System.Windows.Forms.LinkLabel website;
		private System.Windows.Forms.LinkLabel copyright;
	}
}

