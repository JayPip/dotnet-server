using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace ServDemo
{
    public partial class MainForm : Form
    {

        private int port_number;
        private IPAddress server_ip;
        private TcpListener server;
        private Data itemData;

        Byte[] bytes ;
        String data ;
        Thread backgroundListener;

        public MainForm()
        {
            
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
          
            server_ip = IPAddress.Parse(txtIP.Text);
            port_number = int.Parse(txtPort.Text);
            server = new TcpListener(server_ip, port_number);
                
            server.Start();
            bytes = new byte[256];
            data = null;
            backgroundListener = new Thread(Listener);
            backgroundListener.IsBackground = true;
            backgroundListener.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //stopping listening for a client
            //checking if istener is bound to a port which means its active
            if (server.Server.IsBound)
            {
                backgroundListener.Abort();
                AppendTextBox("Server closed..");
                AppendTextBox(Environment.NewLine);
                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            itemData.IpAdress = txtIP.Text;
            itemData.Port = txtPort.Text;


            XmlSerializer writer =
            new XmlSerializer(typeof(Data));
            string fileName = "data.xml";
            string path = Path.Combine(Environment.CurrentDirectory, @"data\", fileName);

            FileStream file = File.Create(path);
            writer.Serialize(file, itemData);
            file.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"data\", "data.xml");
            if (File.Exists(path))
            {
                StreamReader file = new StreamReader(path);
                XmlSerializer reader =
                new XmlSerializer(typeof(Data));
                itemData = (Data)reader.Deserialize(file);
                file.Close();
                //assign to textbox
                txtIP.Text = itemData.IpAdress;
                txtPort.Text = itemData.Port;
            }
            else itemData = new Data();
            
        }

        async private void Listener() {
            try
            {
                while (true)
                {
                    AppendTextBox("waiting for connection...");
                    AppendTextBox(Environment.NewLine);

                    await AcceptClient(await server.AcceptTcpClientAsync());

                }
            }
            finally {
                server.Stop();
            }
            
        }

        async Task AcceptClient(TcpClient client) {
            await Task.Yield();

            AppendTextBox("Connected");
            AppendTextBox(Environment.NewLine);

            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }

            // Shutdown and end connection
            stream.Close();
            client.Close();
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            txtLog.Text += value;
        }
    }
}
