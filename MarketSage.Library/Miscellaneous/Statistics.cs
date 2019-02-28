using System;
using System.Collections.Generic;
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
    public class Statistics
    {
        private double _positionOpen;
        //private double _positionClose;
        private int _totalBars;
        private int _totalHeldBars;
        private int _totalWinBars;
        private int _totalEvenBars;
        private int _totalLoseBars;
        private int _maximumConsecutiveWinBars;
        private int _maximumConsecutiveEvenBars;
        private int _maximumConsecutiveLoseBars;
        private int _averageConsecutiveWinBars;
        private int _averageConsecutiveEvenBars;
        private int _averageConsecutiveLoseBars;
        private int _countWin = 0;
        private int _countEven = 0;
        private int _countLose = 0;
        private int _countConsecutiveWin = 0;
        private int _countConsecutiveEven = 0;
        private int _countConsecutiveLose = 0;
        private double _maxloss;
        private double _maxwin;
        private int _winposday;
        private int _losposday;
        private double _lasteval;
        private double _cumulatedrisk;

        public Statistics()
        {
            _positionOpen = 0.0F;
            _maxloss = 0.0F;
            _maxwin = 0.0F;
            _winposday = 0;
            _losposday = 0;
            _totalBars = 0;
            _cumulatedrisk = 0.0F;
        }

        public void setLastEval(double f)
        {
            _lasteval = f;
        }

        public double getLastEval()
        {
            return (double)_lasteval;
        }

        public int getLosingPosDays()
        {
            return _losposday;
        }

        public double getMaximumPosLoss()
        {
            return (double)_maxloss;
        }

        public double getMaximumPosWin()
        {
            return (double)_maxwin;
        }

        public double getOpenPosition()
        {
            return (double)_positionOpen;
        }

        public int getWinningPosDays()
        {
            return _winposday;
        }

        public int getDownPosDays()
        {
            return _totalLoseBars;
        }

        public int getUpPosDays()
        {
            return _totalWinBars;
        }

        public int getEvenPosDays()
        {
            return _totalEvenBars;
        }

        public void IncrementWinBars()
        {
            _totalHeldBars++;
            _totalWinBars++;
            if (_countConsecutiveWin == 0)
            {
                if (_countConsecutiveEven > 0)
                {
                    _maximumConsecutiveEvenBars = Math.Max(_maximumConsecutiveEvenBars, _countConsecutiveEven);
                    _countEven++;
                    _averageConsecutiveEvenBars = _totalEvenBars / _countEven;
                    _countConsecutiveEven = 0;
                }
                if (_countConsecutiveLose > 0)
                {
                    _maximumConsecutiveLoseBars = Math.Max(_maximumConsecutiveLoseBars, _countConsecutiveLose);
                    _countLose++;
                    _averageConsecutiveLoseBars = _totalLoseBars / _countLose;
                    _countConsecutiveLose = 0;
                }
            }
            _countConsecutiveWin++;
        }

        public void IncrementEvenBars()
        {
            _totalHeldBars++;
            _totalEvenBars++;
            if (_countConsecutiveEven == 0)
            {
                if (_countConsecutiveWin > 0)
                {
                    _maximumConsecutiveWinBars = Math.Max(_maximumConsecutiveWinBars, _countConsecutiveWin);
                    _countWin++;
                    _averageConsecutiveWinBars = _totalWinBars / _countWin;
                    _countConsecutiveWin = 0;
                }
                if (_countConsecutiveLose > 0)
                {
                    _maximumConsecutiveLoseBars = Math.Max(_maximumConsecutiveLoseBars, _countConsecutiveLose);
                    _countLose++;
                    _averageConsecutiveLoseBars = _totalLoseBars / _countLose;
                    _countConsecutiveLose = 0;
                }
            }
            _countConsecutiveEven++;
        }

        public void IncrementLoseBars()
        {
            _totalHeldBars++;
            _totalLoseBars++;
            if (_countConsecutiveLose == 0)
            {
                if (_countConsecutiveWin > 0)
                {
                    _maximumConsecutiveWinBars = Math.Max(_maximumConsecutiveWinBars, _countConsecutiveWin);
                    _countWin++;
                    _averageConsecutiveWinBars = _totalWinBars / _countWin;
                    _countConsecutiveWin = 0;
                }
                if (_countConsecutiveEven > 0)
                {
                    _maximumConsecutiveEvenBars = Math.Max(_maximumConsecutiveEvenBars, _countConsecutiveEven);
                    _countEven++;
                    _averageConsecutiveEvenBars = _totalEvenBars / _countEven;
                    _countConsecutiveEven = 0;
                }
            }
            _countConsecutiveLose++;
        }

        public double MaximumConsectutiveWinBars
        {
            get { return _maximumConsecutiveWinBars; }
        }

        public double MaximumConsectutiveEvenBars
        {
            get { return _maximumConsecutiveEvenBars; }
        }

        public double MaximumConsectutiveLoseBars
        {
            get { return _maximumConsecutiveLoseBars; }
        }

        public double AverageConsectutiveWinBars
        {
            get { return _averageConsecutiveWinBars; }
        }

        public double AverageConsectutiveEvenBars
        {
            get { return _averageConsecutiveEvenBars; }
        }

        public double AverageConsectutiveLoseBars
        {
            get { return _averageConsecutiveLoseBars; }
        }

        public int TotalBarsPostiveVsNegative()
        {
            if (_totalWinBars > _totalLoseBars)
                return _totalWinBars - _totalLoseBars;
            else
                return 0;   
        }

        public int TotalHeldBars
        {
            get { return _totalHeldBars; }
        }


        public void IncrementTotalBars()
        {
            _totalBars++;
        }

        public void updtLoss(double f)
        {
            _maxloss = Math.Min(_maxloss, f);
        }

        public void updtWin(double f)
        {
            _maxwin = Math.Max(_maxwin, f);
        }

        public void setOpenPosition(double f)
        {
            _positionOpen = f;
        }

        public void incWinningPosDays()
        {
            _winposday++;
        }

        public int getDays()
        {
            return _totalBars;
        }

        public void addRisk(double f)
        {
            _cumulatedrisk += f;
        }

        public double getMeanRisk()
        {
            return _cumulatedrisk / (double)_totalBars;
        }

        public void Update(double position, double cash, double value)
        {
            addRisk(1.0 - cash / value);
            //if (position > 0 && position == getOpenPosition())
            if (position > 0)
            {
                if (value > getLastEval())
                {
                    updtWin(value - getLastEval());
                    IncrementWinBars();
                }
                else if (value < getLastEval())
                {
                    updtLoss(value - getLastEval());
                    IncrementLoseBars();
                }
                else
                {
                    IncrementEvenBars();
                }
            }
            setLastEval(value);
            setOpenPosition(position);
            IncrementTotalBars();
        }
    }
}
