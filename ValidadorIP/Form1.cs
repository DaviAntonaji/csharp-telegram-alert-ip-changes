using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using Newtonsoft.Json;
using Telegram.Bot;

namespace ValidadorIP
{


  
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VerificaTelegram();
            Thread t = new Thread(unused => verificarIP());
            t.IsBackground = true;
            t.Start();
        }

        public void verificarIP()
        {
            while (true)
            {
               

                try
                {
                    string content = string.Empty;
                    string url = @"https://api.ipify.org?format=json";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }


                    var ip_body = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                    string ip = ip_body["ip"];

                    string current_ip = "0.0.0.0";
                    try
                    {
                        current_ip = File.ReadAllText("current_ip.txt");
                    }
                    catch (Exception)
                    {

                    }
                    current_ip = current_ip.Replace("\n", "").Replace("\r", "");
                    ip = ip.Replace("\n", "").Replace("\r", "");
                    if (ip != current_ip)
                    {
                        string text = File.ReadAllText("telegram.txt");
                        string[] c = text.Split('|');
                        string chat_id = c[0].Replace("\n", "").Replace("\r", "");
                        string token = c[1].Replace("\n", "").Replace("\r", "");
                        Console.WriteLine("chat_id: " + chat_id);
                        Console.WriteLine("token: " + token);
                        TelegramBotClient Client = new TelegramBotClient(token);
                        string message = "@-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-@\n" +
                            "\n" +
                            "IP ANTIGO:" + current_ip + "\n" +
                            "IP NOVO:" + ip + "\n" +
                            "\n" +
                            "@-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-@";
                        Client.SendTextMessageAsync(chat_id, message);
                      
                       
                        string[] lines =
                        {
                            ip
                        };

                        File.WriteAllLines("current_ip.txt", lines);

                    }



                }
                catch(Exception)
                {
                        
                }


                Thread.Sleep(60000);
            }
            
            
        }


        private bool VerificaTelegram()
        {
            try
            {


                string text = File.ReadAllText("telegram.txt");

                lb_mensagem.Text = "Conectado ao Telegram...";

                lb_mensagem.Visible = true;
                btnDesconectar.Visible = true;

                btnSalvar.Visible = false;

                btnSalvar.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                tb_chat_id.Visible = false;
                tb_token.Visible = false;
                return true;

            }
            catch (Exception)
            {
                btnDesconectar.Visible = false;
                lb_mensagem.Visible = false;

                btnSalvar.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                tb_chat_id.Visible = true;
                tb_token.Visible = true;
                return false;
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            string chat_id = tb_chat_id.Text;
            string token = tb_token.Text;
            if(chat_id != "" && token != "")
            {

                string[] lines =
                {
                    chat_id+"|"+token
                };

                File.WriteAllLines("telegram.txt", lines);
                VerificaTelegram();
            }
        }

        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            File.Delete("telegram.txt");
            VerificaTelegram();
        }
    }
}
