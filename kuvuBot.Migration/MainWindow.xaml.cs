using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace kuvuBot.Migration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if(!File.Exists("out.json"))
            {
                Logs.AppendText("Can't find out.json! Save rethinkdb first!");
                return;
            }

            var enviromentPath = Environment.GetEnvironmentVariable("PATH");
            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, "dotnet.exe"))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = exePath,
                    WorkingDirectory = Path.GetDirectoryName(kuvuBotPath.Text),
                    Arguments = $"kuvuBot.dll --migrate \"{Path.GetFullPath("out.json")}\"",
                    CreateNoWindow = false,
                    UseShellExecute = true,
                }
            };
            process.Start();

            process.Exited += (s, ee) => Dispatcher.Invoke(() => MysqlButton.IsEnabled = true);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            var host = Ip.Text.Split(':');

            var enviromentPath = Environment.GetEnvironmentVariable("PATH");
            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, "node.exe"))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();
            if (exePath == null)
            {
                Logs.AppendText("Can't find node.js! Ensure its in path!");
                return;
            }

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = exePath,
                    WorkingDirectory = ".",
                    Arguments = $"app.js {host[0]} {host[1]} {User.Text} {Password.Password} {Database.Text}",
                    CreateNoWindow = false,
                    UseShellExecute = true,
                }
            };
            process.Start();

            process.Exited += (s, ee) => Dispatcher.Invoke(() => button.IsEnabled = true);
        }
    }
}
