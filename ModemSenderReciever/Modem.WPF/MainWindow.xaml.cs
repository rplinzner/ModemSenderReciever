﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Modem.Library;

namespace Modem.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> _list = new ObservableCollection<string>();
        private Port SelectedPort { get; set; }
        private Thread thread;
        
        public MainWindow()
        {
            InitializeComponent();
            BaudComboBox.SelectedIndex = 3;
            BitsComboBox.SelectedIndex = 4;
            ParityComboBox.SelectedIndex = 2;
            LoadGuiData();
        }

        private void LoadGuiData()
        {
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                _list.Add(port);
            }

            PortsComboBox.ItemsSource = _list;
        }


        private void OpnPrtBtn_OnClick(object sender, RoutedEventArgs e) //open port
        {
            if (PortsComboBox.Text == "")
            {
                MessageBox.Show("Please select Serial Port before opening port!", "Information", MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }

            Parity temp = Parity.None;
            if (ParityComboBox.SelectedIndex == 0) temp = Parity.Even;
            if (ParityComboBox.SelectedIndex == 1) temp = Parity.Odd;
            if (ParityComboBox.SelectedIndex == 2) temp = Parity.None;
            if (ParityComboBox.SelectedIndex == 3) temp = Parity.Mark;
            if (ParityComboBox.SelectedIndex == 4) temp = Parity.Space;

            SelectedPort = new Port(PortsComboBox.Text, Int32.Parse(BaudComboBox.Text), Int32.Parse(BitsComboBox.Text), temp);
            SelectedPort.Open();
            OpnPrtBtn.IsEnabled = false;
            ClsPrtBtn.IsEnabled = true;
            AnswerBtn.IsEnabled = true;
            CallBtn.IsEnabled = true;
            SendButton.IsEnabled = true;
            RecieveButton.IsEnabled = true;

        }

        private void ClsPrtBtn_Click(object sender, RoutedEventArgs e) //close port
        {
            if(thread.IsAlive) thread.Abort();
            RecieveButton.Content = "START RECIEVING";
            SelectedPort.Close();
            OpnPrtBtn.IsEnabled = true;
            ClsPrtBtn.IsEnabled = false;
            AnswerBtn.IsEnabled = false;
            CallBtn.IsEnabled = false;
            SendButton.IsEnabled = false;
            RecieveButton.IsEnabled = false;
        }

        private void CallBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NumberTextBox.Text == "")
            {
                MessageBox.Show("Please type number before calling!", "Information", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            Handler.Call(SelectedPort, NumberTextBox.Text);
            AnswerBtn.IsEnabled = false;
        }

        private void AnswerBtn_Click(object sender, RoutedEventArgs e)
        {
            CallBtn.IsEnabled = false;
            NumberTextBox.IsEnabled = false;
            Handler.Send(SelectedPort, "ATA");
        }


        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (Send_TextBox.Text == "")
            {
                MessageBox.Show("Please type something!", "Information", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            Handler.Send(SelectedPort, Send_TextBox.Text);
            Send_TextBox.Text = "";
        }

        //TODO: TEST TASK AND ADD CANCELATION TOKEN
        private void Recieve()
        {
            string up = "";
            while (true)
            {
                
                up += Handler.Recieve(SelectedPort);
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(delegate
                    {
                        Recieve_TextBox.Text = up;
                    }));
            }
        }

        private void RecieveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecieveButton.Content.ToString() == "STOP RECIEVING")
            {
                thread.Abort();
                RecieveButton.Content = "START RECIEVING";
                return;
            }

            if (RecieveButton.Content.ToString() == "START RECIEVING")
            {
                RecieveButton.Content = "STOP RECIEVING";
            }

            thread = new Thread(Recieve) {IsBackground = true};
            thread.Start();
        }


        //TExtbox with number will accept only numbers
        private void NumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            e.Handled = !Char.IsNumber(c);

            OnPreviewTextInput(e);
        }


    }
}
