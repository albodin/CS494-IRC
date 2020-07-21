using System;
using System.Windows.Forms;
using IRC_Server;

namespace IRC_Client
{
    public partial class SetUsernameForm : Form
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public SetUsernameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event when the user presses the change username button
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ChangeUsernameButton_Click(object sender, EventArgs e)
        {
            if (Globals.Client == null)
            {
                // Don't do anything if the client is null
                return;
            }

            Globals.Client.MessageClient(HelperMethods.FormatMessage(Headers.Username, UsernameTextBox.Text));// Send the set username message
            Globals.Client.SetUsername(UsernameTextBox.Text);// Set the username

            this.Hide();// Hide this form
        }
    }
}
