﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZoomFake_TCP_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsConnected { get; set; }
        private bool IsScreenCasting = false;
        private GroupChat GroupChat;
        private FileChat FileChat;
        private ScreenCast ScreenCast;


        public MainWindow()
        {
            InitializeComponent();
            IsConnected = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsConnected)
                {
                    GroupChat = new GroupChat(IPAddress.Parse(Ip.Text));
                    FileChat = new FileChat(IPAddress.Parse(Ip.Text));

                    GroupChat.OnMessage += GroupChat_OnMessage;
                    FileChat.OnMessage += FileChat_OnMessage;

                    GroupChat.ReceiveAsync();
                    FileChat.ReceiveAsync();
                }
                else
                    CloseConnection();

                IsConnected = !IsConnected;
                ScreenCastGrid.IsEnabled = IsConnected;
                GridChats.IsEnabled = IsConnected;
                ChangeButtonState((Button)sender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseConnection()
        {
            GroupChat.StopChat();
            FileChat.StopChatting();

            ScreenCast?.Stop();
        }

        private void ScreenCast_OnFrameChange(System.Windows.Media.ImageBrush obj)
        {
            throw new NotImplementedException();
        }

        private void FileChat_OnMessage(Message obj)
        {
            GroupChat_OnMessage(obj);
        }

        private void GroupChat_OnMessage(Message obj)
        {
            Dispatcher.Invoke(() =>
            {
                ListViewChat.Items.Add(obj);
            });
        }

        private void ChangeButtonState(Button btn)
        {
            if (IsConnected)
            {
                btn.Content = "Disconnect";
            }
            else
            {
                btn.Content = "Connect";
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Enter:
                    GroupChat.Send(Encoding.UTF8.GetBytes(ChatTextBox.Text));
                    ChatTextBox.Text = "";
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GroupChat.Send(Encoding.UTF8.GetBytes(ChatTextBox.Text));
            ChatTextBox.Text = "";
        }

        private void ButtonBase_OnClick(object Sender, RoutedEventArgs E)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                string filename = ofd.FileName;
                FileChat.SendFile(new FileInfo(filename));
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ScreenCast?.Stop();
            ScreenCast = new ScreenCast(IPAddress.Parse(Ip.Text));
            ScreenCastWindow scw = new ScreenCastWindow();

            ScreenCast.OnFrameChange += (s) =>
            {
                scw.Dispatcher.Invoke(() =>
                {
                    scw.ScreenCast_OnFrameChange(s);
                });
            };
            scw.Show();
            ScreenCast.ReceiveAsync();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            IsScreenCasting = !IsScreenCasting;
            ScreenCast?.Stop();

            if (IsScreenCasting)
            {
                ScreenCast = new ScreenCast(IPAddress.Parse(Ip.Text));
                ScreenCast.SendAsync();
                SendButton.Content = "Stop";
                ReceiveButton.IsEnabled = false;
            }
            else
            {
                SendButton.Content = "Share";
                ReceiveButton.IsEnabled = true;
            }
        }

    }
}
