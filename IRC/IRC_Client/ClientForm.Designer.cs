namespace IRC_Client
{
    partial class ClientForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Rooms");
            this.ChatBox = new System.Windows.Forms.TextBox();
            this.MembersListBox = new System.Windows.Forms.ListBox();
            this.MessagesListBox = new System.Windows.Forms.ListBox();
            this.RoomstreeView = new System.Windows.Forms.TreeView();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.SettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeUsernameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roomMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.joinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memberMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.roomMenuStrip.SuspendLayout();
            this.memberMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChatBox
            // 
            this.ChatBox.AcceptsTab = true;
            this.ChatBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ChatBox.Enabled = false;
            this.ChatBox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChatBox.Location = new System.Drawing.Point(0, 565);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(1005, 27);
            this.ChatBox.TabIndex = 3;
            this.ChatBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ChatBox_KeyPress);
            this.ChatBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ChatBox_PreviewKeyDown);
            // 
            // MembersListBox
            // 
            this.MembersListBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.MembersListBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MembersListBox.FormattingEnabled = true;
            this.MembersListBox.IntegralHeight = false;
            this.MembersListBox.ItemHeight = 16;
            this.MembersListBox.Location = new System.Drawing.Point(885, 24);
            this.MembersListBox.Name = "MembersListBox";
            this.MembersListBox.Size = new System.Drawing.Size(120, 541);
            this.MembersListBox.TabIndex = 5;
            this.MembersListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MembersListBox_MouseDown);
            // 
            // MessagesListBox
            // 
            this.MessagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagesListBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MessagesListBox.FormattingEnabled = true;
            this.MessagesListBox.HorizontalScrollbar = true;
            this.MessagesListBox.IntegralHeight = false;
            this.MessagesListBox.ItemHeight = 18;
            this.MessagesListBox.Location = new System.Drawing.Point(120, 24);
            this.MessagesListBox.Name = "MessagesListBox";
            this.MessagesListBox.Size = new System.Drawing.Size(765, 541);
            this.MessagesListBox.TabIndex = 6;
            // 
            // RoomstreeView
            // 
            this.RoomstreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.RoomstreeView.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RoomstreeView.Location = new System.Drawing.Point(0, 24);
            this.RoomstreeView.Name = "RoomstreeView";
            treeNode1.Name = "RoomsNode";
            treeNode1.Text = "Rooms";
            this.RoomstreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.RoomstreeView.Size = new System.Drawing.Size(120, 541);
            this.RoomstreeView.TabIndex = 7;
            this.RoomstreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RoomsTreeView_AfterSelect);
            this.RoomstreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RoomsTreeView_NodeMouseClick);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1005, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip1";
            // 
            // SettingsToolStripMenuItem
            // 
            this.SettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripMenuItem,
            this.DisconnectToolStripMenuItem,
            this.ChangeUsernameToolStripMenuItem,
            this.CreateRoomToolStripMenuItem});
            this.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem";
            this.SettingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.SettingsToolStripMenuItem.Text = "Settings";
            this.SettingsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.SettingsToolStripMenuItem_DropDownOpening);
            // 
            // ConnectToolStripMenuItem
            // 
            this.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem";
            this.ConnectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.ConnectToolStripMenuItem.Text = "Connect";
            this.ConnectToolStripMenuItem.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // DisconnectToolStripMenuItem
            // 
            this.DisconnectToolStripMenuItem.Name = "DisconnectToolStripMenuItem";
            this.DisconnectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.DisconnectToolStripMenuItem.Text = "Disconnect";
            this.DisconnectToolStripMenuItem.Click += new System.EventHandler(this.DisconnectToolStripMenuItem_Click);
            // 
            // ChangeUsernameToolStripMenuItem
            // 
            this.ChangeUsernameToolStripMenuItem.Name = "ChangeUsernameToolStripMenuItem";
            this.ChangeUsernameToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.ChangeUsernameToolStripMenuItem.Text = "Change Username";
            this.ChangeUsernameToolStripMenuItem.Click += new System.EventHandler(this.ChangeUsernameToolStripMenuItem_Click);
            // 
            // CreateRoomToolStripMenuItem
            // 
            this.CreateRoomToolStripMenuItem.Name = "CreateRoomToolStripMenuItem";
            this.CreateRoomToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.CreateRoomToolStripMenuItem.Text = "Create Room";
            this.CreateRoomToolStripMenuItem.Click += new System.EventHandler(this.CreateRoomToolStripMenuItem_Click);
            // 
            // roomMenuStrip
            // 
            this.roomMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinToolStripMenuItem,
            this.leaveToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.roomMenuStrip.Name = "roomMenuStrip";
            this.roomMenuStrip.Size = new System.Drawing.Size(108, 70);
            // 
            // joinToolStripMenuItem
            // 
            this.joinToolStripMenuItem.Name = "joinToolStripMenuItem";
            this.joinToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.joinToolStripMenuItem.Text = "Join";
            this.joinToolStripMenuItem.Click += new System.EventHandler(this.JoinToolStripMenuItem_Click);
            // 
            // leaveToolStripMenuItem
            // 
            this.leaveToolStripMenuItem.Name = "leaveToolStripMenuItem";
            this.leaveToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.leaveToolStripMenuItem.Text = "Leave";
            this.leaveToolStripMenuItem.Click += new System.EventHandler(this.LeaveToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // memberMenuStrip
            // 
            this.memberMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pMToolStripMenuItem});
            this.memberMenuStrip.Name = "memberMenuStrip";
            this.memberMenuStrip.Size = new System.Drawing.Size(93, 26);
            // 
            // pMToolStripMenuItem
            // 
            this.pMToolStripMenuItem.Name = "pMToolStripMenuItem";
            this.pMToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.pMToolStripMenuItem.Text = "PM";
            this.pMToolStripMenuItem.Click += new System.EventHandler(this.PMToolStripMenuItem_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 592);
            this.Controls.Add(this.MessagesListBox);
            this.Controls.Add(this.MembersListBox);
            this.Controls.Add(this.RoomstreeView);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.Text = "IRC Client";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.roomMenuStrip.ResumeLayout(false);
            this.memberMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox ChatBox;
        private System.Windows.Forms.ListBox MembersListBox;
        private System.Windows.Forms.ListBox MessagesListBox;
        private System.Windows.Forms.TreeView RoomstreeView;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem SettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeUsernameToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip roomMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem leaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateRoomToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip memberMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem pMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DisconnectToolStripMenuItem;
    }
}

