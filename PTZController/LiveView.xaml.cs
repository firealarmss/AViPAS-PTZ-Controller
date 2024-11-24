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
using System.Windows.Threading;
using Accord.Video.FFMPEG;
using System.Drawing;


namespace PTZController
{
    /// <summary>
    /// Interaction logic for LiveView.xaml
    /// </summary>
    public partial class LiveView : Window
    {
        private VideoFileReader reader = new VideoFileReader();
        private DispatcherTimer timer;

        public LiveView()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Open the video file NOTE: this does NOT work properly at this time
            reader.Open("rtsp://192.168.0.100:554/live/av0");  // Use your actual RTSP URL here

            // Set up a timer to fetch new frames
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33); // ~30 fps
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (reader.FrameCount > 0 && !reader.IsOpen)
            {
                reader.Close();
                timer.Stop();
                return;
            }

            Bitmap frame = reader.ReadVideoFrame();
            if (frame != null)
            {
                videoImage.Source = BitmapToBitmapSource(frame);
                frame.Dispose();
            }
        }

        private BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            reader.Close();
            base.OnClosed(e);
        }
    }
}