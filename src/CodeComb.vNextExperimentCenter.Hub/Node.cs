using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

namespace CodeComb.vNextExperimentCenter.Hub
{
    public enum NodeStatus
    {
        Free,
        Working,
        Busy,
        Lost
    }

    public class Node
    {
        public string Alias { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public int MaxThread { get; set; }
        public int CurrentThread { get; set; }
        public int LostConnectionCount { get; set; } = 0;
        public string PrivateKey { get; set; }
        public string OS { get; set; }
        public HttpClient client;
        public NodeStatus Status
        {
            get
            {
                if (LostConnectionCount > 0)
                    return NodeStatus.Lost;
                else if (CurrentThread == 0)
                    return NodeStatus.Free;
                else if (MaxThread > CurrentThread)
                    return NodeStatus.Working;
                else
                    return NodeStatus.Busy;
            }
        }

        public async Task RefreshNodeInfo()
        {
            var response = await client.GetAsync("/common/getnodeinfo");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.Error.WriteLine($"{Alias} 连接失败");
                return;
            }
            var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            MaxThread = result.MaxThread;
            CurrentThread = result.CurrentThread;
            OS = result.Platform;
            var timer = new Timer(x => { HeartBeat(); }, null, 0, 1000 * 15);
        }

        public async Task Init()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"http://{Server}:{Port}");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("private-key", PrivateKey);
            await RefreshNodeInfo();
        }

        public async Task HeartBeat()
        {
            var response = await client.GetAsync("/api/common/heartbeat");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                LostConnectionCount++;
                Console.Error.WriteLine($"{Alias} 心跳测试失败第{LostConnectionCount}次");
            }
            else
            {
                LostConnectionCount = 0;
                Console.WriteLine($"{Alias} 心跳测试 200 OK");
            }
        }

        public async Task<bool> SendJudgeTask(long id, byte[] user, byte[] problem, string nuget)
        {
            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString()))
            {
                content.Add(new StreamContent(new MemoryStream(user)), "user", "user.zip");
                content.Add(new StreamContent(new MemoryStream(problem)), "problem", "problem.zip");
                content.Add(new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("id", id.ToString()),
                    new KeyValuePair<string, string>("nuget", nuget)
                }));
                var result = await client.PostAsync("/api/judge/new", content);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
        }
    }
}
