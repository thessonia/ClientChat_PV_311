﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAppChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint serverEndPoint;
        //const string serverAddress = "127.0.0.1";
        //const short serverPort = 4040;
        TcpClient client ;
        NetworkStream ns = null;
        StreamWriter writer = null;
        StreamReader reader = null;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();   
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            client = new TcpClient();
            string address = ConfigurationManager.AppSettings["ServerAddress"]!;
            short port =short.Parse( ConfigurationManager.AppSettings["ServerPort"]!);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(address), port);  
        }

        private void DisconnectBtnClick(object sender, RoutedEventArgs e)
        {
            ns.Close();
            client.Close();
        }

        private void ConnectBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect(serverEndPoint);
                ns = client.GetStream();
                
                writer = new StreamWriter(ns);
                reader = new StreamReader(ns);
                Listen();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {           
            string message = msgText.Text;
            writer.WriteLine(message);
            writer.Flush();
        }
   
        private async void Listen()
        {
    
            while (true)
            {
                string? message = await reader.ReadLineAsync();
                messages.Add(new MessageInfo(message, DateTime.Now));
            }
        }
    }
}