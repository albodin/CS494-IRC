namespace IRC_Client
{
    partial class ConnectForm
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
            this.IpTextBox = new System.Windows.Forms.TextBox();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ServerIpLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.SslCheckBox = new System.Windows.Forms.CheckBox();
            this.HostNameLabel = new System.Windows.Forms.Label();
            this.HostNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // IpTextBox
            // 
            this.IpTextBox.Location = new System.Drawing.Point(72, 12);
            this.IpTextBox.Name = "IpTextBox";
            this.IpTextBox.Size = new System.Drawing.Size(100, 20);
            this.IpTextBox.TabIndex = 0;
            // 
            // PortTextBox
            // 
            this.PortTextBox.Location = new System.Drawing.Point(72, 38);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(100, 20);
            this.PortTextBox.TabIndex = 1;
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(72, 90);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(100, 20);
            this.UsernameTextBox.TabIndex = 2;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(6, 139);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(166, 23);
            this.ConnectButton.TabIndex = 3;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ServerIpLabel
            // 
            this.ServerIpLabel.AutoSize = true;
            this.ServerIpLabel.Location = new System.Drawing.Point(3, 15);
            this.ServerIpLabel.Name = "ServerIpLabel";
            this.ServerIpLabel.Size = new System.Drawing.Size(54, 13);
            this.ServerIpLabel.TabIndex = 4;
            this.ServerIpLabel.Text = "Server IP:";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(3, 41);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(63, 13);
            this.PortLabel.TabIndex = 5;
            this.PortLabel.Text = "Server Port:";
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(3, 93);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(58, 13);
            this.UsernameLabel.TabIndex = 6;
            this.UsernameLabel.Text = "Username:";
            // 
            // SslCheckBox
            // 
            this.SslCheckBox.AutoSize = true;
            this.SslCheckBox.Checked = true;
            this.SslCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SslCheckBox.Location = new System.Drawing.Point(6, 116);
            this.SslCheckBox.Name = "SslCheckBox";
            this.SslCheckBox.Size = new System.Drawing.Size(68, 17);
            this.SslCheckBox.TabIndex = 7;
            this.SslCheckBox.Text = "Use SSL";
            this.SslCheckBox.UseVisualStyleBackColor = true;
            // 
            // HostNameLabel
            // 
            this.HostNameLabel.AutoSize = true;
            this.HostNameLabel.Location = new System.Drawing.Point(3, 67);
            this.HostNameLabel.Name = "HostNameLabel";
            this.HostNameLabel.Size = new System.Drawing.Size(63, 13);
            this.HostNameLabel.TabIndex = 8;
            this.HostNameLabel.Text = "Host Name:";
            // 
            // HostNameTextBox
            // 
            this.HostNameTextBox.Location = new System.Drawing.Point(72, 64);
            this.HostNameTextBox.Name = "HostNameTextBox";
            this.HostNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.HostNameTextBox.TabIndex = 9;
            // 
            // ConnectForm
            // 
            this.AcceptButton = this.ConnectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 168);
            this.Controls.Add(this.HostNameTextBox);
            this.Controls.Add(this.HostNameLabel);
            this.Controls.Add(this.SslCheckBox);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.PortLabel);
            this.Controls.Add(this.ServerIpLabel);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.UsernameTextBox);
            this.Controls.Add(this.PortTextBox);
            this.Controls.Add(this.IpTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ConnectForm";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Connect";
            this.TopMost = true;
            this.VisibleChanged += new System.EventHandler(this.ConnectForm_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IpTextBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label ServerIpLabel;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.CheckBox SslCheckBox;
        private System.Windows.Forms.Label HostNameLabel;
        private System.Windows.Forms.TextBox HostNameTextBox;
    }
}