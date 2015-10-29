using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CodeComb.vNextExperimentCenter.Node
{
    public class Startup
    {
        public static IConfiguration Configuration;
        private static HttpClient client;
        public static HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                    client.BaseAddress = new Uri($"http://{Configuration["Server"]}:{Configuration["Port"]}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("private-key", Configuration["PrivateKey"]);
                }
                return client;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(out Configuration);
            services.AddMvc();
            services.AddDefaultCIRunner(Convert.ToInt32(Configuration["MaxThread"]));

            CI.Runner.Task.OnOutputReceived += Task_OnOutputReceived;
            CI.Runner.Task.OnBuiledFailed += Task_OnBuiledFailed;
            CI.Runner.Task.OnTimeLimitExceeded += Task_OnTimeLimitExceeded;
            CI.Runner.Task.OnBuildSuccessful += Task_OnBuildSuccessful;
        }

        private void Task_OnBuildSuccessful(object sender, CI.Runner.EventArgs.BuildSuccessfulArgs args)
        {
            var task = sender as CI.Runner.Task;
            client.PostAsync("/api/Judge/Successful", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("id", task.Identifier.ToString())
            })).Wait();
        }

        private void Task_OnTimeLimitExceeded(object sender, CI.Runner.EventArgs.TimeLimitExceededArgs args)
        {
            var task = sender as CI.Runner.Task;
            client.PostAsync("/api/Judge/TimeLimitExceeded", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("id", task.Identifier.ToString())
            })).Wait();
        }

        private void Task_OnBuiledFailed(object sender, CI.Runner.EventArgs.BuildFailedArgs args)
        {
            var task = sender as CI.Runner.Task;
            client.PostAsync("/api/Judge/Failed", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("id", task.Identifier.ToString())
            })).Wait();
        }

        private void Task_OnOutputReceived(object sender, CI.Runner.EventArgs.OutputReceivedEventArgs args)
        {
            var task = sender as CI.Runner.Task;
            client.PostAsync("/api/Judge/Output", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string> ("id", task.Identifier.ToString()),
                new KeyValuePair<string, string> ("text", args.Output),
            })).Wait();
            Console.WriteLine("向服务器反馈输出文本");
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Warning;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            
            app.UseMvc();
        }
    }
}
