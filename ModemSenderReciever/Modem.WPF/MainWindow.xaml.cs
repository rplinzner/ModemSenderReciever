using System;
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
        private Task _recieveTask;
        static CancellationTokenSource ts = new CancellationTokenSource();
        CancellationToken ct = ts.Token;
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
            ts.Cancel();
            SelectedPort.Close();
            OpnPrtBtn.IsEnabled = false;
            ClsPrtBtn.IsEnabled = false;
            AnswerBtn.IsEnabled = false;
            CallBtn.IsEnabled = false;
            SendButton.IsEnabled = false;
            RecieveButton.IsEnabled = false;
        }

        private void CallBtn_Click(object sender, RoutedEventArgs e)
        {
            if(NumberTextBox.Text == "")
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
            Handler.Send(SelectedPort, "ATH");
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
       /* private void Recieve()
        {
            while (true)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Recieve_TextBox.Text += SelectedPort.Read().ToString();
                }), DispatcherPriority.Background);

               
            }
        }*/

        private void RecieveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecieveButton.Content.ToString() == "STOP RECIEVING")
            {
                ts.Cancel();
                RecieveButton.IsEnabled = false;
            }

            if (RecieveButton.Content.ToString() == "START RECIEVING")
            {
                RecieveButton.Content = "STOP RECIEVING";
                
            }

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Recieve_TextBox.Text += "a";
                    }), DispatcherPriority.Background);
                    Thread.Sleep(100);
                    if (ct.IsCancellationRequested)
                    {
                        // another thread decided to cancel
                        Console.WriteLine("task canceled");
                        break;
                    }
                }
            }, ct);
        }

        //TExtbox with number will accept only numbers
        private void NumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

       
    }
}
