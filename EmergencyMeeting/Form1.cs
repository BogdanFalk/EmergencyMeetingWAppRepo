using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Runtime.InteropServices;

namespace EmergencyMeeting
{
    public partial class Form1 : Form
    {


        MqttClient client;
        string clientId;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public Form1()
        {
            InitializeComponent();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            BackColor = Color.Red;
            TransparencyKey = Color.Red;
            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Icon = new Icon(@"emergencyIco.ico");
            trayIcon.Visible = true;
            ShowInTaskbar = false;
            ContextMenu contextMenu1 = new System.Windows.Forms.ContextMenu();
            MenuItem menuItem1 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { menuItem1 });

            // Initialize menuItem1
            menuItem1.Index = 0;
            menuItem1.Text = "E&xit";
            menuItem1.Click += new System.EventHandler(menuItem1_Click);

            trayIcon.ContextMenu = contextMenu1;

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 2;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 2;
            this.Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            this.Size = new Size(w, h);
            pictureBox1.Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            pictureBox1.Size = new Size(w, h);


            string BrokerAddress = "broker.mqttdashboard.com";
            client = new MqttClient(BrokerAddress);
      
            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);

           
            // whole topic
            string Topic = "/emergencyVizuina/test";

            // subscribe to the topic with QoS 2
            client.Subscribe(new string[] { Topic }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
           
            
           
        }

        protected void menuItem1_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        protected override void OnShown(EventArgs e)
        {
            this.Hide();
            base.OnShown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            client.Disconnect();

            base.OnClosed(e);
         
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

            Invoke((MethodInvoker)(() => Visible = true));
            
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();        
            player.SoundLocation = @"sound.wav";
            player.Play();
            System.Threading.Thread.Sleep(4000);
            Invoke((MethodInvoker)(() => Visible = false));
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }


    }
}
