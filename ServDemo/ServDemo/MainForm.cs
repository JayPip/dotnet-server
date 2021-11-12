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

namespace ServDemo
{
    public partial class MainForm : Form
    {

        private int port_number;
        private IPAddress server_ip;
        private TcpListener listener;

        private Data itemData;

        public MainForm()
        {
            
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
          
            server_ip = IPAddress.Parse(txtIP.Text);
                
                
            listener.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

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
            if (File.Exists(path)) {
                StreamReader file = new StreamReader(path);
                XmlSerializer reader =
                new XmlSerializer(typeof(Data));
                itemData = (Data)reader.Deserialize(file);
                file.Close();
                //assign to textbox
                txtIP.Text = itemData.IpAdress;
                txtPort.Text = itemData.Port;
            }
            
        }
    }
}
