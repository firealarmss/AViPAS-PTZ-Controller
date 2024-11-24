/*
* PTZController
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
* 
* Copyright (C) 2024 Caleb, K4PHP
* 
*/

using System;
using System.Windows;

namespace PTZController
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static Settings settings = new Settings();

        public SettingsWindow()
        {
            InitializeComponent();

            if (settings.ip == null)
                settings.ip = "192.168.0.100";

            if (settings.port <= 0)
                settings.port = 80;

            if (settings.ptzSpeed <= 0)
                settings.ptzSpeed = 50;

            txtCameraIP.Text = settings.ip;
            txtPort.Text = settings.port.ToString();
            txtPtzSpeed.Text = settings.ptzSpeed.ToString();
            chkLockKeys.IsChecked = settings.keysLocked;
        }

        private void Save_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            settings.ip = txtCameraIP.Text;
            settings.port = Convert.ToInt32(txtPort.Text);
            settings.ptzSpeed = Convert.ToInt32(txtPtzSpeed.Text);
            settings.keysLocked = (bool)chkLockKeys.IsChecked;
        }
    }
}
