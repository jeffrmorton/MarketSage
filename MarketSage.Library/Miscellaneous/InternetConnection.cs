using System;
using System.Runtime;
using System.Runtime.InteropServices;

/* MarketSage
   Copyright © 2008, 2009 Jeffrey Morton
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

namespace MarketSage.Library
{
    public class InternetConnection
    {
        [Flags]
        enum ConnectionState : int { INTERNET_CONNECTION_MODEM = 0x1, INTERNET_CONNECTION_LAN = 0x2, INTERNET_CONNECTION_PROXY = 0x4, INTERNET_RAS_INSTALLED = 0x10, INTERNET_CONNECTION_OFFLINE = 0x20, INTERNET_CONNECTION_CONFIGURED = 0x40 }

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref ConnectionState lpdwFlags, int dwReserved);

        ConnectionState Description = 0;

        public bool Active()
        {
            return InternetGetConnectedState(ref Description, 0);
        }
    }
}
