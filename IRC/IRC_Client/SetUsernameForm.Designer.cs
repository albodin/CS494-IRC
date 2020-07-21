namespace IRC_Client
{
    partial class SetUsernameForm
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
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.ChangeUsernameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(76, 12);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(129, 20);
            this.UsernameTextBox.TabIndex = 0;
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(12, 15);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(58, 13);
            this.UsernameLabel.TabIndex = 7;
            this.UsernameLabel.Text = "Username:";
            // 
            // ChangeUsernameButton
            // 
            this.ChangeUsernameButton.Location = new System.Drawing.Point(12, 38);
            this.ChangeUsernameButton.Name = "ChangeUsernameButton";
            this.ChangeUsernameButton.Size = new System.Drawing.Size(193, 23);
            this.ChangeUsernameButton.TabIndex = 8;
            this.ChangeUsernameButton.Text = "Change It";
            this.ChangeUsernameButton.UseVisualStyleBackColor = true;
            this.ChangeUsernameButton.Click += new System.EventHandler(this.ChangeUsernameButton_Click);
            // 
            // SetUsernameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 72);
            this.Controls.Add(this.ChangeUsernameButton);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.UsernameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetUsernameForm";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Change Username";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.AcceptButton = ChangeUsernameButton;
        }

        #endregion

        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Button ChangeUsernameButton;
    }
}