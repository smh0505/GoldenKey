using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Websocket.Client;

namespace 황금열쇠
{
    public class GoldenKey
    {
        public string Payload { get; set; }
        public int count = 0;
        private readonly string FileName = "setting.ini";
        private Form1 mainWindow;
        private delegate void GetReady();
        private delegate void AddOption(string option);

        public GoldenKey(Form1 window)
        {
            mainWindow = window;
        }

        public void CheckCode()
        {
            if (File.Exists(FileName))
            {
                StreamReader r = new StreamReader(FileName);
                var line = r.ReadLine();
                if (line.Contains("Key="))
                {
                    var key = line.Substring(4);
                    mainWindow.Key = key;
                }
                else MessageBox.Show("오류: 투네이션 비밀키를 찾을 수 없습니다.",
                        "황금열쇠");
                r.Close();
            }
        }

        public async Task LoadPayload(string key)
        {
            HttpClient client = new HttpClient();
            using (var response = await client.GetAsync("https://toon.at/widget/alertbox/" + key))
            {
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var line = Regex.Match(body, "\"payload\":\"[^\"]*\"").Value;
                    Payload = Regex.Match(line, @"[\w]{8,}").Value;

                    StreamWriter w = new StreamWriter(FileName);
                    w.Write("Key=" + key);
                    w.Close();
                }
            }
        }

        public void Connect()
        {
            Uri uri = new Uri("wss://toon.at:8071/" + Payload);
            var exitEvent = new ManualResetEvent(false);

            using (var client = new WebsocketClient(uri))
            {
                client.MessageReceived.Subscribe(msg =>
                {
                    if (msg.ToString().Contains("roulette"))
                    {
                        var roulette = Regex.Match(msg.ToString(), "\"message\":\"[^\"]* - [^\"]*\"").Value.Substring(10);
                        var rValue = roulette.Split('-')[1].Replace("\"", "").Substring(1);
                        if (!mainWindow.IsReady && count < 20)
                        {
                            mainWindow.BeginInvoke(new AddOption(Option), rValue);
                            count++;
                        }
                        if (!mainWindow.IsReady && count == 20) mainWindow.Invoke(new GetReady(SetReady));
                    }
                });
                client.Start();
                exitEvent.WaitOne();
            }
        }

        private void Option(string rValue)
        {
            mainWindow.Option = rValue;
        }

        private void SetReady()
        {
            mainWindow.IsReady = true;
        }
    }
}
