using System;
using System.Windows.Forms;
using IRC_Server;

namespace IRC_Client
{
    public partial class CreateRoomForm : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CreateRoomForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event when the user presses the create room button
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void CreateRoomButton_Click(object sender, EventArgs e)
        {
            if (Globals.Client == null)
            {
                // Don't do anything if client is null
                return;
            }

            // Send the create room message
            Globals.Client.MessageClient(HelperMethods.FormatMessage(Headers.MakeRoom, RoomTextBox.Text));

            this.Hide();// Hide this form
        }
    }
}
