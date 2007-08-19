namespace TestHarness
{
    partial class Harness
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
            this.cboRepos = new System.Windows.Forms.ComboBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtCommands = new System.Windows.Forms.TextBox();
            this.txtRepoDir = new System.Windows.Forms.TextBox();
            this.btnRoot = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCommands = new System.Windows.Forms.Button();
            this.btnRepos = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.fbdLoad = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // cboRepos
            // 
            this.cboRepos.Enabled = false;
            this.cboRepos.FormattingEnabled = true;
            this.cboRepos.Location = new System.Drawing.Point(12, 212);
            this.cboRepos.Name = "cboRepos";
            this.cboRepos.Size = new System.Drawing.Size(243, 21);
            this.cboRepos.TabIndex = 0;
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(13, 28);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(374, 20);
            this.txtServer.TabIndex = 1;
            this.txtServer.Leave += new System.EventHandler(this.txtServer_Leave);
            // 
            // txtCommands
            // 
            this.txtCommands.Location = new System.Drawing.Point(12, 65);
            this.txtCommands.Name = "txtCommands";
            this.txtCommands.Size = new System.Drawing.Size(374, 20);
            this.txtCommands.TabIndex = 2;
            this.txtCommands.Leave += new System.EventHandler(this.txtCommands_Leave);
            // 
            // txtRepoDir
            // 
            this.txtRepoDir.Location = new System.Drawing.Point(13, 109);
            this.txtRepoDir.Name = "txtRepoDir";
            this.txtRepoDir.Size = new System.Drawing.Size(374, 20);
            this.txtRepoDir.TabIndex = 3;
            this.txtRepoDir.Leave += new System.EventHandler(this.txtRepoDir_Leave);
            this.txtRepoDir.TextChanged += new System.EventHandler(this.txtRepoDir_TextChanged);
            // 
            // btnRoot
            // 
            this.btnRoot.Location = new System.Drawing.Point(403, 24);
            this.btnRoot.Name = "btnRoot";
            this.btnRoot.Size = new System.Drawing.Size(45, 23);
            this.btnRoot.TabIndex = 4;
            this.btnRoot.Text = "...";
            this.btnRoot.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRoot.UseVisualStyleBackColor = true;
            this.btnRoot.Click += new System.EventHandler(this.btnRoot_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Directory where the Subversion server was installed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(272, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Directory where the Subversion server commands reside";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(242, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Directory where the Subversion repositories reside";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Available Repositories";
            // 
            // btnCommands
            // 
            this.btnCommands.Location = new System.Drawing.Point(403, 64);
            this.btnCommands.Name = "btnCommands";
            this.btnCommands.Size = new System.Drawing.Size(45, 23);
            this.btnCommands.TabIndex = 9;
            this.btnCommands.Text = "...";
            this.btnCommands.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCommands.UseVisualStyleBackColor = true;
            this.btnCommands.Click += new System.EventHandler(this.btnCommands_Click);
            // 
            // btnRepos
            // 
            this.btnRepos.Location = new System.Drawing.Point(403, 108);
            this.btnRepos.Name = "btnRepos";
            this.btnRepos.Size = new System.Drawing.Size(45, 23);
            this.btnRepos.TabIndex = 10;
            this.btnRepos.Text = "...";
            this.btnRepos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRepos.UseVisualStyleBackColor = true;
            this.btnRepos.Click += new System.EventHandler(this.btnRepos_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Enabled = false;
            this.btnLoad.Location = new System.Drawing.Point(167, 158);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(134, 23);
            this.btnLoad.TabIndex = 11;
            this.btnLoad.Text = "Load Server Objects";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // Harness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 339);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnRepos);
            this.Controls.Add(this.btnCommands);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRoot);
            this.Controls.Add(this.txtRepoDir);
            this.Controls.Add(this.txtCommands);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.cboRepos);
            this.Name = "Harness";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SVNManagerLib Test Harness";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboRepos;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtCommands;
        private System.Windows.Forms.TextBox txtRepoDir;
        private System.Windows.Forms.Button btnRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCommands;
        private System.Windows.Forms.Button btnRepos;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.FolderBrowserDialog fbdLoad;
    }
}