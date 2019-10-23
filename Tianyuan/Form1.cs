using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WebSocket4Net;
using System.Threading;
using System.Speech.Synthesis;
using SuperSocket.ClientEngine;

namespace Tianyuan
{
    public partial class Form1 : Form
    {
        WebSocket websocket;
        int flag = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!e.Message.Equals(" "))
                this.listBox1.Invoke(new EventHandler(ShowMessage), e.Message);

        }

        private void ShowMessage(object sender, EventArgs e)
        {
            this.listBox1.Items.Add(DateTime.Now.ToLocalTime().ToString() +"      " + sender.ToString());
            if (listBox1.Items.Count > 10)
            {
                listBox1.Items.RemoveAt(0);
            }
            SpeechSynthesizer speaker = new SpeechSynthesizer();
            speaker.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 2, System.Globalization.CultureInfo.CurrentCulture);
            speaker.Rate = 1;
            speaker.Volume = 100;
            speaker.SpeakAsync(sender.ToString());
        }


        void websocket_Error(object sender, ErrorEventArgs e)
        {
            if (flag == 0)
            {
                this.listBox1.Invoke(new EventHandler(ShowMessage), "网络异常");
                flag = 1;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            websocket = new WebSocket("wss://whpgl.ybty.com/websocket");
            websocket.MessageReceived += websocket_MessageReceived;
            websocket.Error += websocket_Error;
            while(websocket.State== WebSocketState.None)
                 websocket.Open();
            this.listBox1.Invoke(new EventHandler(ShowMessage), "服务器连接中");
            while (websocket.State== WebSocketState.Open)
                timer1.Enabled = true;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (websocket.State != WebSocketState.Open&& websocket.State != WebSocketState.Connecting)
            {
                websocket = new WebSocket("wss://whpgl.ybty.com/websocket");
                websocket.MessageReceived += websocket_MessageReceived;
                websocket.Error += websocket_Error;
                websocket.Open();
            }
            else
                flag = 0;
        }
    }
}
