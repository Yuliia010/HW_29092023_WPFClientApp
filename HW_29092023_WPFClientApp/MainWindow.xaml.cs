using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.Windows.Shapes;

namespace HW_29092023_WPFClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint endPoint;
        Socket socket;
        int isConnected = -1;

        string aswType = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            UpdateConnBtn();
        }

        private void UpdateConnBtn()
        {
            if (isConnected == 1)
            {
                btn_Connect.Background = new SolidColorBrush(Colors.LightGreen);
                btn_Connect.Content = "Connected";
            }
            else if (isConnected == 0 || isConnected == -1)
            {
                btn_Connect.Background = new SolidColorBrush(Colors.IndianRed);
                btn_Connect.Content = "Disconnected";
            }
        }

        private async Task ConnectAction()
        {
            string strport = tb_Port.Text;
            int port;
            if (int.TryParse(strport, out port))
            {
                string ip = tb_IP.Text;

                endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    await socket.ConnectAsync(endPoint);
                    MessageBox.Show("Connected to the server");
                    isConnected = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    isConnected = 0;
                }
            }
            UpdateConnBtn();
        }

        private async void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected == 0)
            {
                if (tb_IP.Text.Length == 0 || tb_Port.Text.Length == 0)
                {
                    MessageBox.Show("Input data to connect!");
                }
                else
                {
                    await ConnectAction();
                }
               
            }
            else if (isConnected == 1)
            {
                socket.Close();
                isConnected = 0;
            }
            else
            {
                if (tb_IP.Text.Length == 0 || tb_Port.Text.Length == 0)
                {
                    MessageBox.Show("Input data to connect!");
                }
                else
                {
                    await ConnectAction();
                }
            }
            UpdateConnBtn();
        }

        private async void btn_GetTime_Click(object sender, RoutedEventArgs e)
        {
            aswType = "time";
            await App();
        }

        private async void btn_GetDate_Click(object sender, RoutedEventArgs e)
        {
            aswType = "date";
            await App();
        }


        private async Task App()
        {
           if(isConnected == 0)
            {
                MessageBox.Show("Server is Not connected!");
            }
            else
            {
                if (aswType != string.Empty)
                {

                    byte[] data = Encoding.UTF8.GetBytes(aswType);
                    await socket.SendAsync(data, SocketFlags.None);


                    data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = await socket.ReceiveAsync(data, SocketFlags.None);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        tb_Answer.Text = $"The {aswType} was received from {endPoint.Address}: {builder.ToString()}";

                    } while (socket.Available > 0);

                    isConnected = 0;

                }
                UpdateConnBtn();
            }
           

        }
    }
}
