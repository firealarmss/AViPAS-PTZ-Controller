using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace PTZController
{
    public class PtzCommand
    {
        public SystemControl SysCtrl { get; set; }

        public PtzCommand(string ptzCmd, int value)
        {
            SysCtrl = new SystemControl
            {
                PtzCtrl = new PanTiltZoomControl
                {
                    Channel = 0,
                    Command = ptzCmd,
                    Value = value
                }
            };
        }

        public class SystemControl
        {
            public PanTiltZoomControl PtzCtrl { get; set; }
        }

        public class PanTiltZoomControl
        {
            public int Channel { get; set; }
            public string Command { get; set; }
            public int Value { get; set; }
        }
    }
}
