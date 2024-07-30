// See https://aka.ms/new-console-template for more information
using DiscordConsole;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

await StartJob();

Console.ReadLine();


async Task StartJob()
{
    Console.WriteLine("开始任务...");

    var channelId = "频道ID";
    var content = "发送的内容";
    var authorization = "你的Authorization";

    //var channelId = "1095065811586142221";
    //var content = "gm";

    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri($"https://discord.com/api/v9/channels/{channelId}/messages");
    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");
    httpClient.DefaultRequestHeaders.Add("Authorization", authorization);

    var lastExecuteTime = DateTime.MinValue;

    do
    {
        var nextExecuteTime = lastExecuteTime.AddHours(2);
        if (nextExecuteTime.AddSeconds(new Random().Next(10, 30)) > DateTime.Now)
        {
            var totalSeconds = (nextExecuteTime - DateTime.Now).TotalSeconds;
            Console.WriteLine($"上一次执行：{lastExecuteTime:MM-dd HH:mm:ss}还差{totalSeconds}秒到2小时");
            Thread.Sleep(1000);
            continue;
        }

        var message = new MessageModel()
        {
            content = content,
            nonce = $"126598031{DateTime.Now:MMddHHmmss}"
        };
        var stringContent = new StringContent(message.TrySerialize());
        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            Content = stringContent,
        };
        var httpResponse = await httpClient.SendAsync(httpRequest);
        var httpResponseString = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} {httpResponseString}");
        lastExecuteTime = DateTime.Now;
        Thread.Sleep(1000);

    } while (true);


}