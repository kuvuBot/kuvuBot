using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace kuvuBot.Panel.Pages
{
    public class Statistics
    {
        public string Uptime { get; internal set; } = "🔄";
        public string Memory { get; internal set; } = "🔄";
        public string Cpu { get; internal set; } = "🔄";
    }

    public class StatisticsModel : PageModel
    {
        public static Statistics Statistics { get; } = new Statistics();

        static StatisticsModel()
        {
            var uptime = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            uptime.Elapsed += (source, e) => { Statistics.Uptime = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"hh\:mm\:ss"); };
            uptime.Start();

            var cpu = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            cpu.Elapsed += async (source, e) =>
            {
                var startTime = DateTime.UtcNow;
                var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
                await Task.Delay(500);

                var endTime = DateTime.UtcNow;
                var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                Statistics.Memory = (Process.GetCurrentProcess().WorkingSet64 / 1000000d).ToString("F1") + "MB";
                Statistics.Cpu = (cpuUsageTotal * 100).ToString("F1") + "%";
            };
            cpu.Start();
        }

        public async Task OnGetAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                while (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Statistics)),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}