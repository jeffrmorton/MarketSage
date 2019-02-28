using System;
using System.Collections;
using System.Text;

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
    public class AnnualReport
    {
        private IList _traders;
        private ArrayList _evalHistory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public AnnualReport(IList list)
        {
            _evalHistory = new ArrayList();
            _traders = list;
            pushEval();
        }

        private void pushEval()
        {
            double[] af = new double[_traders.Count];
            for (int i = 0; i < _traders.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traders[i];
                af[i] = abstracttrader.GetPnL();
            }
            _evalHistory.Insert(0, af);
        }

        public void yearUpdate()
        {
            pushEval();
        }

        public double getYearChange(int i)
        {
            double[] af = (double[])_evalHistory[0];
            double[] af1 = (double[])_evalHistory[1];
            return ((af[i] - af1[i]) * 100F) / af1[i];
        }
    }
}
