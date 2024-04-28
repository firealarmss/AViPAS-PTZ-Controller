using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PTZController
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static string ip = "192.168.0.100";
        public static int port = 80;

        public SettingsWindow()
        {
            InitializeComponent();

            txtCameraIP.Text = ip;
            txtPort.Text = port.ToString();
        }

        private void Save_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            SettingsWindow.ip = txtCameraIP.Text;
            SettingsWindow.port = Convert.ToInt32(txtPort.Text);
        }
    }
}
