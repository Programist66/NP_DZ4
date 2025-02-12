using Microsoft.Win32;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using TcpLib;

namespace FileClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient server = new();
        private CancellationTokenSource cancel = new();

        public MainWindow()
        {
            InitializeComponent();
            _ = InitServerConnect();
            _ = ListenToServer(cancel.Token);
        }

        private async Task InitServerConnect() 
        {
            await server.ConnectAsync("192.168.100.5", 6596);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                if (!await server.SendFileFromPath(dialog.FileName))
                {
                    fileList.Items.Add($"файл {dialog.FileName} не отправился");
                }
            }
        }

        private async Task ListenToServer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                fileList.Items.Add(await server.ReceiveFileToPath());
            }
            server.Dispose();
        }
    }
}