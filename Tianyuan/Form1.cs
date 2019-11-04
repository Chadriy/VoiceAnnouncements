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
using System.Net.NetworkInformation;

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
            {
                this.listBox1.Invoke(new EventHandler(ShowMessage), e.Message);
               
            }

        }

        private void ShowMessage(object sender, EventArgs e)
        {
                string content = DateTime.Now.ToLocalTime().ToString() + "      " + sender.ToString();
                this.listBox1.Items.Add(content);
                listBox1.SetSelected(listBox1.Items.Count - 1, true);
                if (listBox1.Items.Count > 100)
                {
                    listBox1.Items.RemoveAt(0);
                }
                SpeechSynthesizer speaker = new SpeechSynthesizer();
                speaker.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 2, System.Globalization.CultureInfo.CurrentCulture);
                speaker.Rate = 1;
                speaker.Volume = 100;
            if (!sender.ToString().Equals("服务器连接中"))
            {
                speaker.Speak(sender.ToString());
            }
            else
            {
                speaker.SpeakAsync(sender.ToString());
            }
        }


        void websocket_Error(object sender, ErrorEventArgs e)
        {
            this.listBox1.Invoke(new EventHandler(ShowMessage), "网络异常，请检查网络连接并重新启动");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            websocket.Send(" ");
            if (websocket.State==WebSocketState.Closed)
            {
                this.listBox1.Invoke(new EventHandler(ShowMessage), "网络异常，重新连接中");
                websocket = new WebSocket("wss://whpgl.ybty.com/websocket");
                websocket.MessageReceived += websocket_MessageReceived;
              //  websocket.Error += websocket_Error;
                while (websocket.State == WebSocketState.None)
                    websocket.Open();
            }
            else
                flag = 0;
        }
        public static bool PingIpOrDomainName(string strIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 500;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string sPath = "log.txt";

            System.IO.StreamWriter SaveFile = new System.IO.StreamWriter(sPath);
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.ToString();
            SaveFile.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            websocket = new WebSocket("wss://whpgl.ybty.com/websocket");
            websocket.MessageReceived += websocket_MessageReceived;
           // websocket.Error += websocket_Error;
            while (websocket.State == WebSocketState.None)
                websocket.Open();
            this.listBox1.Invoke(new EventHandler(ShowMessage), "服务器连接中");
            listBox1.SetSelected(listBox1.Items.Count - 1, true);
            timer1.Enabled = true;
        }
    }
}
