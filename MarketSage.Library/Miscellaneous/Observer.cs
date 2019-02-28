using System;
using System.Collections;

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
    public class Observer
    {
        private DateTime _dateStart;
        private DateTime _dateCurrent;
        private DateTime _dateEnd;
        private int _year;
        private Trader _traderAgent;
        private Instrument _instrument;
        private ArrayList _observers;
        private AnnualReport _reportAnnual;
        private TransactionsReport _reportTransactions;
        private Hashtable _traderstats;
        public bool _isInitialized = false;
        private double total;
        private double totalpnl;
        private double totaltransactions;
        private double totaldays;

        public Observer()
        {
            _observers = new ArrayList();
            _traderstats = new Hashtable();
            total = 0;
            totalpnl = 0;
            totaltransactions = 0;
            totaldays = 0;
        }

        public double TotalScore
        {
            get { return total; }
        }

        public double TotalPnL
        {
            get { return totalpnl; }
        }

        public double TotalTransactions
        {
            get { return totaltransactions; }
        }

        public double TotalDays
        {
            get { return totaldays; }
        }

        public DateTime GetCurrentDate()
        {
            return _dateCurrent;
        }

        public DateTime GetStartDate()
        {
            return _dateStart;
        }

        public DateTime GetEndDate()
        {
            return _dateEnd;
        }

        public void setMarket(Instrument market)
        {
            _instrument = market;
        }

        public void setTraderContainer(Trader itradercontainer)
        {
            _traderAgent = itradercontainer;
            _reportAnnual = new AnnualReport(_traderAgent.Agents);
            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                _traderstats.Add(_traderAgent.Agents[i], new Statistics());
            }
        }

        public void onStopped()
        {
            _dateCurrent = _instrument.Date;
            _dateEnd = _instrument.Date;
            _year = _dateCurrent.Year;
            _reportAnnual.yearUpdate();
            UpdateCumulative();
            for (int i = 0; i < _observers.Count; i++)
            {
                ((Observer)_observers[i]).onStopped();
            }
        }

        public void onStarted()
        {
            _dateStart = new DateTime();
            _dateCurrent = new DateTime();
            _dateEnd = new DateTime();
            _year = 0;
            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[i];
                ((Statistics)_traderstats[abstracttrader]).setLastEval(abstracttrader.GetValue());
            }
            for (int i = 0; i < _observers.Count; i++)
            {
                ((Observer)_observers[i]).onStarted();
            }

        }

        public void onUpdate(CalanderEvent evt)
        {
            if (_isInitialized == false)
            {
                _dateStart = _instrument.Date;
                _isInitialized = true;
            }
            _dateCurrent = _instrument.Date;
            _year = _dateCurrent.Year;
            UpdateStatistics();
            if (evt.YearEvent)
            {
                _reportAnnual.yearUpdate();
            }
            for (int i = 0; i < _observers.Count; i++)
            {
                ((Observer)_observers[i]).onUpdate(evt);
            }
        }

        public void addObserver(Observer observer)
        {
            _observers.Add(observer);
        }

        public Instrument getMarket()
        {
            return _instrument;
        }

        public ArrayList getTraders()
        {
            return _traderAgent.Agents;
        }

        public String[] getTitles()
        {
            ArrayList list = getTraders();
            String[] a = new String[list.Count];
            for (int i = 0; i < list.Count; i++)
                a[i] = ((AbstractStrategy)list[i]).EntryIndicatorName;
            return a;
        }

        public int getYear()
        {
            return _year;
        }

        public AnnualReport getYearReport()
        {
            return _reportAnnual;
        }

        public TransactionsReport getTransactionsReport()
        {
            if (_reportTransactions == null)
                _reportTransactions = new TransactionsReport(_traderAgent.Agents, _instrument);
            return _reportTransactions;
        }

        private void UpdateStatistics()
        {
            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[i];
                ((Statistics)_traderstats[abstracttrader]).Update(abstracttrader.GetPosition(""), abstracttrader.GetCash(), abstracttrader.GetValue());
            }
        }

        private void UpdateCumulative()
        {
            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[i];
                double score = (abstracttrader.GetTransactionCount() == 0) ? 0 : abstracttrader.GetPnL() / (abstracttrader.GetDaysHeld() / abstracttrader.GetTransactionCount());
                double transactions = abstracttrader.GetTransactionCount();
                double pnl = abstracttrader.GetPnL();
                //double days = abstracttrader.GetDaysHeld();
                total += score;
                totalpnl += pnl;
                totaltransactions += transactions;
                //totaldays += days;
            }
        }

        public void UpdateTotal(int count)
        {
            total = total / count;
        }

        public Statistics GetStatistics(Object obj)
        {
            return (Statistics)_traderstats[obj];
        }

        public Statistics GetStatistics()
        {
            AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[0];
            return (Statistics)_traderstats[abstracttrader];
        }

        public void Clear()
        {
            _dateCurrent = new DateTime();
            _dateStart = new DateTime();
            _dateEnd = new DateTime();
            _year = 0;
            _observers = new ArrayList();
            _traderstats = new Hashtable();
            _reportAnnual = null;
            _isInitialized = false;
            _traderstats.Clear();
            _reportTransactions = null;
            total = 0;
            totalpnl = 0;
            totaltransactions = 0;
            totaldays = 0;
        }

        public string GetReport()
        {
            string report = "";

            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[i];
                //report += "Instrument:\t\t\t\t\t" + abstracttrader._instrument.Name + "\r\n";
                report += "Date range:\t\t\t" + GetStartDate().ToShortDateString() + " - " + GetEndDate().ToShortDateString() + "\r\n";
                report += "Total bars:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getDays().ToString() + "\r\n";
                report += "Total transactions:\t\t\t" + TotalTransactions.ToString() + "\r\n";
                report += "Total bars held:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).TotalHeldBars.ToString() + "\r\n";
                report += "Total bars won:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getUpPosDays().ToString() + "\r\n";
                report += "Total bars even:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getEvenPosDays().ToString() + "\r\n";
                report += "Total bars lost:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getDownPosDays().ToString() + "\r\n";
                report += "Maximum consecutive wins:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveWinBars.ToString() + "\r\n";
                report += "Average consecutive wins:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveWinBars.ToString() + "\r\n";
                report += "Maximum consecutive even:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveEvenBars.ToString() + "\r\n";
                report += "Average consecutive even:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveEvenBars.ToString() + "\r\n";
                report += "Maximum consecutive losses:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveLoseBars.ToString() + "\r\n";
                report += "Average consecutive losses:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveLoseBars.ToString() + "\r\n";
                report += "Maximum bar profit:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMaximumPosWin().ToString() + "\r\n";
                report += "Maximum bar loss:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMaximumPosLoss().ToString() + "\r\n";
                report += "Total Profit/Loss:\t\t\t" + TotalPnL.ToString() + "\r\n";
                report += "Risk:\t\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMeanRisk().ToString() + "\r\n";
                //report += "Score:\t\t\t\t" + TotalScore.ToString();
            }

            return report;
        }

        public string GetTransactionReport()
        {
            string report = "";

            for (int i = 0; i < _traderAgent.Agents.Count; i++)
            {
                AbstractStrategy abstracttrader = (AbstractStrategy)_traderAgent.Agents[i];
                //report += "Instrument:\t\t\t\t\t" + abstracttrader._instrument.Name + "\r\n";
                report += "Date range:\t\t\t" + GetStartDate().ToShortDateString() + " - " + GetEndDate().ToShortDateString() + "\r\n";
                report += "Initial Balance:\t\t\t" + abstracttrader.Account.InitialBalance.ToString() + "\r\n";
                report += "Total bars:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getDays().ToString() + "\r\n";
                report += "Total transactions:\t\t" + TotalTransactions.ToString() + "\r\n";
                report += "Total bars held:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).TotalHeldBars.ToString() + "\r\n";
                report += "Total bars won:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getUpPosDays().ToString() + "\r\n";
                report += "Total bars even:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getEvenPosDays().ToString() + "\r\n";
                report += "Total bars lost:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getDownPosDays().ToString() + "\r\n";
                report += "Maximum consecutive wins:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveWinBars.ToString() + "\r\n";
                report += "Average consecutive wins:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveWinBars.ToString() + "\r\n";
                report += "Maximum consecutive even:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveEvenBars.ToString() + "\r\n";
                report += "Average consecutive even:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveEvenBars.ToString() + "\r\n";
                report += "Maximum consecutive losses:\t\t" + ((Statistics)_traderstats[abstracttrader]).MaximumConsectutiveLoseBars.ToString() + "\r\n";
                report += "Average consecutive losses:\t\t" + ((Statistics)_traderstats[abstracttrader]).AverageConsectutiveLoseBars.ToString() + "\r\n";
                report += "Maximum bar profit:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMaximumPosWin().ToString() + "\r\n";
                report += "Maximum bar loss:\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMaximumPosLoss().ToString() + "\r\n";
                report += "Total Profit/Loss:\t\t\t" + TotalPnL.ToString() + "\r\n";
                report += "Risk:\t\t\t\t" + ((Statistics)_traderstats[abstracttrader]).getMeanRisk().ToString() + "\r\n";
                //report += "Score:\t\t\t\t" + TotalScore.ToString() + "\r\n";
                report += "Transactions:\t\t\t" + ((Transaction)abstracttrader.Account.Transactions[0]).Date.ToShortDateString() + ", " + ((Transaction)abstracttrader.Account.Transactions[0]).Quantity.ToString() + ", " + ((Transaction)abstracttrader.Account.Transactions[0]).Amount.ToString() + ", " + (double)(Math.Abs(((Transaction)abstracttrader.Account.Transactions[0]).Amount)/Math.Abs(((Transaction)abstracttrader.Account.Transactions[0]).Quantity)) + "\r\n";
                for (int x = 1; x < abstracttrader.Account.Transactions.Count; x++)
                {
                    report += "\t\t\t\t" + ((Transaction)abstracttrader.Account.Transactions[x]).Date.ToShortDateString() + ", " + ((Transaction)abstracttrader.Account.Transactions[x]).Quantity.ToString() + ", " + ((Transaction)abstracttrader.Account.Transactions[x]).Amount.ToString() + ", " + (double)(Math.Abs(((Transaction)abstracttrader.Account.Transactions[x]).Amount)/Math.Abs(((Transaction)abstracttrader.Account.Transactions[x]).Quantity)) + "\r\n";
                }
            }
            return report;
        }
    }
}
