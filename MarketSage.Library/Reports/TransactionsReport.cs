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
    public class TransactionsReport
    {
        private ArrayList _traders;
        private Instrument _market;
        private int[] _winning;
        private int[] _losing;
        private double[] _winavg;
        private double[] _losavg;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="imarket"></param>
        public TransactionsReport(ArrayList list, Instrument imarket)
        {
            _traders = list;
            _market = imarket;
        }

        public void generate()
        {
            int i = _traders.Count;
            _winning = new int[i];
            _losing = new int[i];
            _winavg = new double[i];
            _losavg = new double[i];
            for (int j = 0; j < i; j++)
            {
                double d = 0.0D;
                double d1 = 0.0D;
                Transaction transaction = null;
                IList list = ((AbstractStrategy)_traders[j]).GetTransactions();
                for (int x = 0; x < list.Count; x++)
                {
                    transaction = (Transaction)list[x];
                    double d3 = d + (double)transaction.Quantity;
                    if (transaction.Amount < 0.0F && d3 < 0.0001D)
                    {
                        double d5 = d1 + (double)transaction.Amount;
                        if (d5 < 0.0D)
                        {
                            _winning[j]++;
                            _winavg[j] -= d5;
                        }
                        else
                        {
                            _losing[j]++;
                            _losavg[j] += d5;
                        }
                    }
                    d1 += transaction.Amount;
                    d += transaction.Quantity;
                }
                if (d > 0.0D)
                {
                    double d2 = _market.AdjClose;
                    double d4 = d1 - d * d2;
                    if (d4 < 0.0D)
                    {
                        _winning[j]++;
                        _winavg[j] -= d4;
                    }
                    else
                        if (d4 > 0.0D)
                        {
                            _losing[j]++;
                            _losavg[j] += d4;
                        }
                }
                if (_winning[j] > 0)
                    _winavg[j] = _winavg[j] / (double)_winning[j];
                if (_losing[j] > 0)
                    _losavg[j] = _losavg[j] / (double)_losing[j];
            }
        }

        public int getLosingCount(int i)
        {
            return _losing[i];
        }

        public int getWinningCount(int i)
        {
            return _winning[i];
        }

        public double getLosingAvg(int i)
        {
            return _losavg[i];
        }

        public double getWinningAvg(int i)
        {
            return _winavg[i];
        }
    }
}
