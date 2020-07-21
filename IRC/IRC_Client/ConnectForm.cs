using System;
using System.Windows.Forms;

namespace IRC_Client
{
    public partial class ConnectForm : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConnectForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event when the connect button is clicked
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // Create new client if it's null
            if (Globals.Client == null)
            {
                Globals.Client = new ClientInfo(IpTextBox.Text, Convert.ToInt32(PortTextBox.Text), HostNameTextBox.Text, UsernameTextBox.Text, SslCheckBox.Checked);
            }
            else
            {
                // Set all the client info
                Globals.Client.SetServerIp(IpTextBox.Text);
                Globals.Client.SetServerPort(Convert.ToInt32(PortTextBox.Text));
                Globals.Client.SetUsername(UsernameTextBox.Text);
                Globals.Client.SetHostName(HostNameTextBox.Text);
            }

            Globals.Client.StartClient();// Start the client
            this.Hide();// Hide this form
        }

        /// <summary>
        /// Event when the visibility changes
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Event info</param>
        private void ConnectForm_VisibleChanged(object sender, EventArgs e)
        {
            // Set all the controls to the current settings
            IpTextBox.Text = Globals.Settings.ServerIp;
            PortTextBox.Text = Globals.Settings.ServerPort.ToString();
            HostNameTextBox.Text = Globals.Settings.HostName;
            UsernameTextBox.Text = Globals.Settings.Username;
            SslCheckBox.Checked = Globals.Settings.UseSsl;
        }
    }
}
