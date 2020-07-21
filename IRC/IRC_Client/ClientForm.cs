using System;
using System.Windows.Forms;
using IRC_Server;

namespace IRC_Client
{
    public partial class ClientForm : Form
    {
        private ConnectForm _connectForm;// our connection form
        private SetUsernameForm _setUsernameForm;// form for changing usernames
        private CreateRoomForm _createRoomForm;// form for creating rooms
        private string _curSelectedRoom = "";// name of the currently active room

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enable and disable the chat text box
        /// </summary>
        /// <param name="enabled">State to set chat box to</param>
        public void SetChatBox(bool enabled)
        {
            BeginInvoke(new MethodInvoker(() => ChatBox.Enabled = enabled));
        }

        /// <summary>
        /// Scrolls the messages list box to the bottom
        /// </summary>
        private void MessagesScrollToBottom()
        {
            MessagesListBox.TopIndex = MessagesListBox.Items.Count - 1;
        }

        /// <summary>
        /// Adds a message to the messages list box and scrolls to the bottom if that was the previous state
        /// </summary>
        /// <param name="message">The message to add</param>
        private void AddMessageScroll(string message)
        {
            int maxMessages = MessagesListBox.Height / MessagesListBox.ItemHeight;// max messages visible on screen
            bool atBottom = (MessagesListBox.TopIndex + maxMessages) >= MessagesListBox.Items.Count - 1;// check if we are scrolled all the way to the bottom

            MessagesListBox.Items.Add(message);

            // Scroll to bottom if we were already at the bottom.
            if (atBottom)
            {
                MessagesScrollToBottom();
            }
        }

        /// <summary>
        /// Adds a message to the messages list box if the room is currently opened
        /// </summary>
        /// <param name="room">The room to add the message to</param>
        /// <param name="message">The message to add</param>
        public void AddMessage(string room, string message)
        {
            if (room == _curSelectedRoom)
            {
                BeginInvoke(new MethodInvoker(() =>  AddMessageScroll(message)));
            }
        }

        /// <summary>
        /// Adds a member to the members list box if the room is currently opened
        /// </summary>
        /// <param name="room">The room to add the member to</param>
        /// <param name="member">The member to add</param>
        public void AddMember(string room, string member)
        {
            if (room == _curSelectedRoom)
            {
                BeginInvoke(new MethodInvoker(() => MembersListBox.Items.Add(member)));
            }
        }

        /// <summary>
        /// Clears all members from the members list box if the room is open
        /// </summary>
        /// <param name="room">The room to wipe from</param>
        public void ClearMembers(string room)
        {
            if (room == _curSelectedRoom)
            {
                BeginInvoke(new MethodInvoker(() => MembersListBox.Items.Clear()));
            }
        }

        /// <summary>
        /// Reloads the members list box of the current room
        /// </summary>
        public void ReloadMembers()
        {
            if (_curSelectedRoom == "" || Globals.Client.GetRoom(_curSelectedRoom) == null)
            {
                // Don't do anything if no room selected or the room selected is a special room
                return;
            }

            BeginInvoke(new MethodInvoker(() => MembersListBox.Items.Clear()));// Clear the members list box

            // Add the members back into the list box
            foreach (var member in Globals.Client.GetRoom(_curSelectedRoom).MemberList())
            {
                BeginInvoke(new MethodInvoker(() => MembersListBox.Items.Add(member)));
            }
        }

        /// <summary>
        /// Clears all messages from the messages list box
        /// </summary>
        public void ClearMessages()
        {
            BeginInvoke(new MethodInvoker(() => MessagesListBox.Items.Clear()));
        }

        /// <summary>
        /// Clears all members from the members list box
        /// </summary>
        public void ClearMembers()
        {
            BeginInvoke(new MethodInvoker(() => MembersListBox.Items.Clear()));
        }

        /// <summary>
        /// Reloads all of the rooms
        /// </summary>
        public void ReloadRooms()
        {
            BeginInvoke(new MethodInvoker( () => RoomstreeView.Nodes.Clear()));// Clear the rooms tree view

            // Add in the special rooms
            BeginInvoke(new MethodInvoker(() => RoomstreeView.Nodes.Add("Rooms")));
            BeginInvoke(new MethodInvoker(() => RoomstreeView.Nodes.Add("DEBUG")));

            // Add the rooms back
            foreach (var room in Globals.Client.RoomList())
            {
                BeginInvoke(new MethodInvoker(() => RoomstreeView.Nodes.Add("Rooms", room.RoomName())));
            }
        }

        /// <summary>
        /// Event when user presses a key inside the chat box
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Key press event info</param>
        private void ChatBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Don't do anything if the client is null or the user
            // didn't press return to send a message
            if (Globals.Client == null || e.KeyChar != (char)Keys.Return)
            {
                return;
            }

            Room curRoom = Globals.Client.GetRoom(_curSelectedRoom);
            if (curRoom == null)
            {
                return;// Room was removed and chat stayed open
            }

            string formattedMessage;
            if (_curSelectedRoom == "SERVER")
            {
                // Used for debugging purposes for manually formatted messages
                formattedMessage = ChatBox.Text;
            }
            else if (curRoom.PrivateRoom())
            {
                // Direct message
                formattedMessage = HelperMethods.FormatMessage(Headers.PrivMsg, curRoom.RoomName(), ChatBox.Text);
            }
            else
            {
                // General room message
                formattedMessage = HelperMethods.FormatMessage(Headers.RoomMsg, curRoom.RoomName(), ChatBox.Text);
            }

            Globals.Client.MessageClient(formattedMessage);// Send the message
            ChatBox.Clear();// Clear the chat box since the message has been sent
        }

        /// <summary>
        /// Occurs before the KeyDown event
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Previous key down event info</param>
        private void ChatBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // If the key is tab we add a tab character to the chat box
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
                ChatBox.AppendText("\t");
            }
        }

        /// <summary>
        /// Event after a user selects a new room to view
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void RoomsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text == "Rooms")
            {
                // Don't do anything since they selected a unused room
                return;
            }

            // Don't allow messaging in special rooms
            if (e.Node.Text == "SERVER" || e.Node.Text == "DEBUG")
            {
                ChatBox.Enabled = false;
            }
            else
            {
                ChatBox.Enabled = true;
            }

            Room curRoom;
            if (e.Node.Text == "DEBUG")
            {
                // Special case as the debug room is always open
                curRoom = Globals.Client.GetDebugRoom();
            }
            else
            {
                curRoom = Globals.Client.GetRoom(e.Node.Text);
            }

            // Clear members and messages and change
            // the currently selected room
            MembersListBox.Items.Clear();
            MessagesListBox.Items.Clear();
            _curSelectedRoom = e.Node.Text;

            if (curRoom == null)
            {
                // The room doesn't exist so we don't continue
                return;
            }

            // Disable chatbox in room that doesn't have client as a member
            if (!curRoom.ContainsMember(Globals.Client.GetUsername()))
            {
                ChatBox.Enabled = false;
            }

            // Add members in
            foreach (var member in curRoom.MemberList())
            {
                MembersListBox.Items.Add(member);
            }

            // Load in the message history
            foreach (var message in curRoom.MessageList())
            {
                MessagesListBox.Items.Add(message);
            }

            MessagesScrollToBottom();// Scroll to the bottom of the messages
        }

        /// <summary>
        /// Event after user presses to connect to server
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_connectForm.IsDisposed)
            {
                _connectForm = new ConnectForm();// user closed the form, so we have to initialize another one
            }
            _connectForm.Show();// connect was clicked, lets show the form
        }

        /// <summary>
        /// Event after user presses to disconnect from the server
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.Client != null)
            {
                Globals.Client.StopClient(false);
            }
        }

        /// <summary>
        /// Event when the form loads
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ClientForm_Load(object sender, EventArgs e)
        {
            Globals.ClientForm = this;

            // initialize our other forms
            _connectForm = new ConnectForm();
            _setUsernameForm = new SetUsernameForm();
            _createRoomForm = new CreateRoomForm();
        }

        /// <summary>
        /// Event when user clicks on a room in the list
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void RoomsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // If the user right clicked on one of the rooms
            if (e.Button == MouseButtons.Right)
            {
                // Select the room if a right click
                RoomstreeView.SelectedNode = e.Node;

                // Don't show option menu for these since it doesn't apply
                if (e.Node.Text == "Rooms" || Globals.Client.GetRoom(e.Node.Text) == null || Globals.Client.GetRoom(e.Node.Text).PrivateRoom())
                {
                    return;
                }

                // Client is in room, don't show join option, but show leave option
                if (Globals.Client.GetRoom(e.Node.Text).ContainsMember(Globals.Client.GetUsername()))
                {
                    joinToolStripMenuItem.Visible = false;
                    leaveToolStripMenuItem.Visible = true;
                }
                else// Not in room, do the opposite
                {
                    joinToolStripMenuItem.Visible = true;
                    leaveToolStripMenuItem.Visible = false;
                }

                // Only make the delete option visible if client is owner of the room
                if (Globals.Client.GetRoom(e.Node.Text).IsOwner())
                {
                    deleteToolStripMenuItem.Visible = true;
                }
                else
                {
                    deleteToolStripMenuItem.Visible = false;
                }
                e.Node.ContextMenuStrip = roomMenuStrip;
            }
        }

        /// <summary>
        /// Event when user clicks to leave a room
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void LeaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Can't leave these rooms
            if (RoomstreeView.SelectedNode.Text == "Rooms" || RoomstreeView.SelectedNode.Text == "SERVER" || RoomstreeView.SelectedNode.Text == "DEBUG")
            {
                return;
            }

            // Sent leave room message
            Globals.Client.MessageClient(HelperMethods.FormatMessage(Headers.LeaveRoom, RoomstreeView.SelectedNode.Text));
        }

        /// <summary>
        /// Event when the user clicks to delete a room
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Can't delete these rooms
            if (RoomstreeView.SelectedNode.Text == "Rooms" || RoomstreeView.SelectedNode.Text == "SERVER" || RoomstreeView.SelectedNode.Text == "DEBUG")
            {
                return;
            }

            // Send the remove room message
            Globals.Client.MessageClient(HelperMethods.FormatMessage(Headers.RemoveRoom, RoomstreeView.SelectedNode.Text));
        }

        /// <summary>
        /// Event when the user clicks to join a room
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void JoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Can't join these rooms
            if (RoomstreeView.SelectedNode.Text == "Rooms" || RoomstreeView.SelectedNode.Text == "SERVER" || RoomstreeView.SelectedNode.Text == "DEBUG" || Globals.Client.GetRoom(RoomstreeView.SelectedNode.Text) == null)
            {
                return;
            }

            // Send the join message if user isn't in the room
            if (!Globals.Client.GetRoom(RoomstreeView.SelectedNode.Text).ContainsMember(Globals.Client.GetUsername()))
            {
                Globals.Client.MessageClient(HelperMethods.FormatMessage(Headers.JoinRoom, RoomstreeView.SelectedNode.Text));
            }
        }

        /// <summary>
        /// Event when user clicks to change their username
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ChangeUsernameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_setUsernameForm.IsDisposed)
            {
                _setUsernameForm = new SetUsernameForm();// user closed the form, initialize a new one
            }
            _setUsernameForm.Show();
        }

        /// <summary>
        /// Event when user clicks to create a new room
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void CreateRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_createRoomForm.IsDisposed)
            {
                _createRoomForm = new CreateRoomForm();// user closed the form, initialize a new one
            }
            _createRoomForm.Show();
        }

        /// <summary>
        /// Event when user clicks somewhere in the members list box
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void MembersListBox_MouseDown(object sender, MouseEventArgs e)
        {
            // We only care about right clicks
            if (e.Button == MouseButtons.Right)
            {
                // Also do selection on right click
                MembersListBox.SelectedIndex = MembersListBox.IndexFromPoint(e.X, e.Y);

                // Only show up if we actually have something selected
                if (MembersListBox.SelectedIndex != -1 && MembersListBox.SelectedItem.ToString() != Globals.Client.GetUsername() && !Globals.Client.ContainsRoom(MembersListBox.SelectedItem.ToString()))
                {
                    pMToolStripMenuItem.Text = $@"PM {MembersListBox.SelectedItem}";
                    MembersListBox.ContextMenuStrip = memberMenuStrip;
                }
                else
                {
                    MembersListBox.ContextMenuStrip = null;
                }
            }
        }

        /// <summary>
        /// Event when user clicks to PM another user
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void PMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create a new private room with the user and other member and add it in
            Room newPrivateRoom = new Room(MembersListBox.SelectedItem.ToString(), true, false);
            newPrivateRoom.AddMember(Globals.Client.GetUsername());
            newPrivateRoom.AddMember(MembersListBox.SelectedItem.ToString());
            Globals.Client.AddRoom(newPrivateRoom);
        }

        /// <summary>
        /// Event when the main tool strip starts opening
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void SettingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // If the client is null or not connected we show the connect option only
            if (Globals.Client == null || !Globals.Client.Connected())
            {
                ConnectToolStripMenuItem.Visible = true;
                ChangeUsernameToolStripMenuItem.Visible = false;
                CreateRoomToolStripMenuItem.Visible = false;
                DisconnectToolStripMenuItem.Visible = false;
            }

            // If the client isn't null and is connected we show the other options
            else if (Globals.Client != null && Globals.Client.Connected())
            {
                ConnectToolStripMenuItem.Visible = false;
                ChangeUsernameToolStripMenuItem.Visible = true;
                CreateRoomToolStripMenuItem.Visible = true;
                DisconnectToolStripMenuItem.Visible = true;
            }
        }
    }
}
