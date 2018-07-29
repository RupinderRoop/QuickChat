using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ChatApplication
{
    public partial class MainWindow : Form
    {
        Socket mySocket;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            MyIPTextBox.Text = GetIPAddress();
            RemoteIPTextBox.Text = GetIPAddress();
            UserGroupBox.Text = Data.Name;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if(MyPortTextBox.Text == String.Empty && RemotePortTextBox.Text ==String.Empty)
            {
                MessageBox.Show("Some Fields are missing for creating Connection","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            epLocal = new IPEndPoint(IPAddress.Parse(MyIPTextBox.Text),Convert.ToInt32(MyPortTextBox.Text));
            mySocket.Bind(epLocal);

            epRemote = new IPEndPoint(IPAddress.Parse(RemoteIPTextBox.Text), Convert.ToInt32(RemotePortTextBox.Text));
            mySocket.Connect(epRemote);

            buffer = new byte[1500];

            mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(GetMessages), buffer);
            ConnectButton.Text = "Connected";
            ConnectButton.Enabled = false;
        }

        private void GetMessages(IAsyncResult asyncResult)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            try
            {
                byte[] RecivedData = new byte[Data.DataLength];
                RecivedData = (byte[])asyncResult.AsyncState;

                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string RecivedMessage = aEncoding.GetString(RecivedData);
                string decMessage = Crypto.Decrypt(RecivedMessage, "123");

                MessageListBox.Items.Add(decMessage);
                buffer = new byte[1500];

                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(GetMessages), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured", ex.Message);
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (KeyTextBox.Text == String.Empty)
            {
                MessageBox.Show("Key is Required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string message = Crypto.Encrypt(UserGroupBox.Text + ": " + MessageTextBox.Text, KeyTextBox.Text);

                byte[] messageToSend = new byte[ASCIIEncoding.ASCII.GetByteCount(message)];

                ASCIIEncoding encoding = new ASCIIEncoding();
                messageToSend = encoding.GetBytes(message);

                mySocket.Send(messageToSend);
                MessageListBox.Items.Add(UserGroupBox.Text + ": " + MessageTextBox.Text);
                MessageTextBox.Text = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Occured", ex.Message);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private string GetIPAddress()
        {
            IPHostEntry hostEntry;
            hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            
            foreach (IPAddress address in hostEntry.AddressList)
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address.ToString();

            return "127.0.0.1";
        }
    }
}
