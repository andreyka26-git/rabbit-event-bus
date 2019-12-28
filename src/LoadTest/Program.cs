using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;

namespace LoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();

            var mainServiceTasks = new List<Task>();

            for (var i = 0; i < 20; i++)
            {
                var index = i;
                mainServiceTasks.Add(Task.Run(async () =>
                {
                    var request = new MainServiceModel
                    {
                        Content = $"content{index}",
                        Name = $"Name{index}"
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync("http://127.0.0.1:5000/main/publish-message", content);
                }));
            }

            var childServiceTasks = new List<Task>();

            for (var i = 0; i < 20; i++)
            {
                var index = i;
                childServiceTasks.Add(Task.Run(async () =>
                {
                    var request = new ChildModel
                    {
                        Value = $"value{index}"
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync("http://127.0.0.1:5001/child/publish-message", content);
                }));
            }

            mainServiceTasks.AddRange(childServiceTasks);
            Task.WhenAll(mainServiceTasks).GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }
    }
}
