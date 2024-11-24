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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;

namespace PTZController
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        private static IntPtr _hookID = IntPtr.Zero;
        private NativeMethods.LowLevelKeyboardProc _proc;

        private static readonly int KEY_LEFT = 37;
        private static readonly int KEY_UP = 38;
        private static readonly int KEY_RIGHT = 39;
        private static readonly int KEY_DOWN = 40;

        private static readonly int L_KEY = 76;

        private static readonly int KEY_PLUS = 107;
        private static readonly int KEY_MINUS = 109;

        public MainWindow()
        {
            InitializeComponent();
            InitializeHttpClient();
            _proc = HookCallback;
            _hookID = SetHook(_proc);

            btnLive.IsEnabled = false;
        }

        private void InitializeHttpClient()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task PostCommand(string ptzCmd, int value)
        {
            //Console.WriteLine($"{ptzCmd} sent with {value}");
            string ip = SettingsWindow.settings.ip.Trim();
            string port = SettingsWindow.settings.port.ToString().Trim();
            string url = $"http://{ip}:{port}/ajaxcom";

            var command = new
            {
                SysCtrl = new
                {
                    PtzCtrl = new
                    {
                        nChanel = 0,
                        szPtzCmd = ptzCmd,
                        byValue = value
                    }
                }
            };

            string serializedCommand = JsonConvert.SerializeObject(command);
            var data = new Dictionary<string, string>
            {
                { "szCmd", serializedCommand }
            };
            //Console.WriteLine(serializedCommand);
            var content = new FormUrlEncodedContent(data);
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Server response: {responseContent}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending command: {ex.Message}");
            }
        }

        private string currentPtzCommand = null;

        private async void Start_MoveCamera_Left(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("left");
            currentPtzCommand = "left_start";
            await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
        }

        private async void Start_MoveCamera_Right(object sender, MouseButtonEventArgs e)
        {
            currentPtzCommand = "right_start";
            await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
        }

        private async void Start_MoveCamera_Up(object sender, MouseButtonEventArgs e)
        {
            currentPtzCommand = "up_start";
            await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
        }

        private async void Start_MoveCamera_Down(object sender, MouseButtonEventArgs e)
        {
            currentPtzCommand = "down_start";
            await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
        }

        private async void Stop_MoveCamera(object sender, MouseButtonEventArgs e)
        {
            if (currentPtzCommand != null)
            {
                //Console.WriteLine($"start the stop {currentPtzCommand}");
                string stopCommand = currentPtzCommand.Replace("_start", "_stop");
                await PostCommand(stopCommand, 0);
                currentPtzCommand = null;
            }
        }

        private async void Start_ZoomIn(object sender, MouseButtonEventArgs e)
        {
            currentPtzCommand = "zoomadd_start";
            await PostCommand(currentPtzCommand, 0);
        }

        private async void Start_ZoomOut(object sender, MouseButtonEventArgs e)
        {
            currentPtzCommand = "zoomdec_start";
            await PostCommand(currentPtzCommand, 0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
/*            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyDown;
            window.KeyUp += HandleKeyUp;*/
        }

        private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine(vkCode);
                if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                {
                    OnGlobalKeyDown(vkCode);
                }
                else if (wParam == (IntPtr)NativeMethods.WM_KEYUP)
                {
                    OnGlobalKeyUp(vkCode);
                }
            }
            return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private async void OnGlobalKeyDown(int vkCode)
        {
            if (vkCode == L_KEY && SettingsWindow.settings.keysLocked)
            {
                SettingsWindow.settings.keysLocked = false;
            } 
            else if (vkCode == L_KEY && !SettingsWindow.settings.keysLocked)
            {
                SettingsWindow.settings.keysLocked = true;
            }

            if (SettingsWindow.settings.keysLocked)
                return;

            if (vkCode == KEY_UP && currentPtzCommand != "up_start")
            {
                currentPtzCommand = "up_start";
                await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
            }
            else if (vkCode == KEY_DOWN && currentPtzCommand != "down_start")
            {
                currentPtzCommand = "down_start";
                await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
            }
            else if (vkCode == KEY_LEFT && currentPtzCommand != "left_start")
            {
                currentPtzCommand = "left_start";
                await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
            }
            else if (vkCode == KEY_RIGHT && currentPtzCommand != "right_start")
            {
                currentPtzCommand = "right_start";
                await PostCommand(currentPtzCommand, SettingsWindow.settings.ptzSpeed);
            }
            else if (vkCode == KEY_PLUS && currentPtzCommand != "zoomadd_start")
            {
                currentPtzCommand = "zoomadd_start";
                await PostCommand(currentPtzCommand, 0);
            }
            else if (vkCode == KEY_MINUS && currentPtzCommand != "zoomdec_start")
            {
                currentPtzCommand = "zoomdec_start";
                await PostCommand(currentPtzCommand, 0);
            }
        }

        private async void OnGlobalKeyUp(int vkCode)
        {
            if (SettingsWindow.settings.keysLocked)
                return;

            if ((vkCode == KEY_DOWN || vkCode == KEY_UP || vkCode == KEY_LEFT || vkCode == KEY_RIGHT || vkCode == KEY_PLUS || vkCode == KEY_MINUS) && currentPtzCommand != null)
            {
                string stopCommand = currentPtzCommand.Replace("_start", "_stop");
                await PostCommand(stopCommand, 0);
                currentPtzCommand = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            NativeMethods.UnhookWindowsHookEx(_hookID);
            base.OnClosed(e);
        }

        private void Menu_Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.Show();
        }

        private void Menu_Live_Click(object sender, RoutedEventArgs e)
        {
            var liveView = new LiveView();
            liveView.Owner = this;
            liveView.Show();
        }
    }
}