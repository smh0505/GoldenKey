using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace 황금열쇠
{
    public struct Default
    {
        public string key;
        public List<string> values;

        public Default(string key, List<string> values)
        {
            this.key = key;
            this.values = values;
        }
    }

    public class GoldenKey
    {
        public string Payload { get; set; }
        public Default setting;
        private Form1 mainWindow;
        private delegate void GetReady();
        private delegate void AddOption(string option);

        public GoldenKey(Form1 window)
        {
            mainWindow = window;
        }

        public void CheckCode()
        {
            if (File.Exists("default.json"))
            {
                StreamReader r = new StreamReader("default.json");
                setting = JsonConvert.DeserializeObject<Default>(r.ReadToEnd());
                if (setting.key != null) mainWindow.Key = setting.key;
                foreach (var item in setting.values) mainWindow.AddOption(item);
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

                    StreamWriter w = new StreamWriter("default.json");
                    setting.key = key;
                    w.Write(JsonConvert.SerializeObject(setting));
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
                        if (rValue != "꽝")
                            mainWindow.BeginInvoke(new AddOption(mainWindow.ReadOption), rValue);
                    }
                });
                client.Start();
                exitEvent.WaitOne();
            }
        }
    }
}
