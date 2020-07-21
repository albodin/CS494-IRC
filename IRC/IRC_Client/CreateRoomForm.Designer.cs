namespace IRC_Client
{
    partial class CreateRoomForm
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
            this.CreateRoomButton = new System.Windows.Forms.Button();
            this.RoomLabel = new System.Windows.Forms.Label();
            this.RoomTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CreateRoomButton
            // 
            this.CreateRoomButton.Location = new System.Drawing.Point(12, 38);
            this.CreateRoomButton.Name = "CreateRoomButton";
            this.CreateRoomButton.Size = new System.Drawing.Size(204, 23);
            this.CreateRoomButton.TabIndex = 11;
            this.CreateRoomButton.Text = "Create It";
            this.CreateRoomButton.UseVisualStyleBackColor = true;
            this.CreateRoomButton.Click += new System.EventHandler(this.CreateRoomButton_Click);
            // 
            // RoomLabel
            // 
            this.RoomLabel.AutoSize = true;
            this.RoomLabel.Location = new System.Drawing.Point(12, 15);
            this.RoomLabel.Name = "RoomLabel";
            this.RoomLabel.Size = new System.Drawing.Size(69, 13);
            this.RoomLabel.TabIndex = 10;
            this.RoomLabel.Text = "Room Name:";
            // 
            // RoomTextBox
            // 
            this.RoomTextBox.Location = new System.Drawing.Point(87, 12);
            this.RoomTextBox.Name = "RoomTextBox";
            this.RoomTextBox.Size = new System.Drawing.Size(129, 20);
            this.RoomTextBox.TabIndex = 9;
            // 
            // CreateRoomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 71);
            this.Controls.Add(this.CreateRoomButton);
            this.Controls.Add(this.RoomLabel);
            this.Controls.Add(this.RoomTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CreateRoomForm";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Create Room";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.AcceptButton = CreateRoomButton;
        }

        #endregion

        private System.Windows.Forms.Button CreateRoomButton;
        private System.Windows.Forms.Label RoomLabel;
        private System.Windows.Forms.TextBox RoomTextBox;
    }
}