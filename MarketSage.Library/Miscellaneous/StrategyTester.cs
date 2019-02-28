using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using ZedGraph;

namespace MarketSage.Library
{
    public class StrategyTester
    {
        private string _pathLogs;
        private string _instrumentModel;  // change this
        public ArrayList date;  // change this
        public ArrayList value;  // change this
        public Dictionary<int, GraphObj> graphObjs = new Dictionary<int, GraphObj>();


        public StrategyTester()
        {
            _pathLogs = "";
        }
        public StrategyTester(string pathLogs)
        {
            _pathLogs = pathLogs;
        }

        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, string log, Engine engine, Market market, ref string result, ref DataTable pool, string file, ref bool win)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            win = false;
            double i1 = 0;
            double i2 = market._instruments.Count;
            try
            {
                string report = "";
                string startTime = "";
                double score;
                double totalpnl;
                double totaltransactions;
                double totaldays;
                double totalbars;
                for (int j = 0; j < engine.Trader.Agents.Count; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        report = "";
                        startTime = DateTime.Now.ToString("yyyyMMddmmss");
                        AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                        report += "---------------------------------------------------------------------------\r\n";
                        report += strategy.EntryIndicatorName + "\r\n";
                        score = 0;
                        totalpnl = 0;
                        totaltransactions = 0;
                        totaldays = 0;
                        totalbars = 0;
                        for (int i = 0; i < market._instruments.Count; i++)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                string _instrument = market._instruments[i].ToString();
                                if (market.GetTechnicalData(_instrument).Count != 0)
                                {
                                    i1++;
                                    Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                                    engine.Market = m;
                                    engine.Start();
                                    while (engine._instrument.HasNext() && e.Cancel == false)
                                    {
                                        if (worker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                        }
                                        else
                                        {
                                            engine.Iterate();
                                        }
                                    }
                                    engine.Stop();

                                    // Debug shit
                                    //if (strategy.GetTransactionCount() != strategy.GetTransactions().Count / 2)
                                    //    MessageBox.Show(strategy.GetTransactionCount() + " : " + strategy.GetTransactions().Count / 2);
                                    // ----------

                                    double transactions = strategy.GetTransactionCount();
                                    double pnl = strategy.GetPnL();
                                    
                                    double days = engine.Observer.GetStatistics().getDays();

                                double bars = engine.Observer.GetStatistics().TotalHeldBars;
                                    double basis = strategy.Account.InitialBalance;
                                    double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                                    report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                    if (transactions > 0 && engine.Observer != null)
                                        report += engine.Observer.GetTransactionReport();
                                    score += fitness;
                                    totalpnl += pnl;
                                    totaltransactions += transactions;
                                    totaldays += days;
                                    totalbars += bars;
                                    int percentComplete = (int)((i1 / i2) * 100);
                                    worker.ReportProgress(percentComplete);
                                }
                            }
                        }
                        engine.Tally(market._instruments.Count);
                        report += "Cumulative Fitness: " + score + "\r\n";
                        result = DateTime.Now + " : " + strategy.EntryIndicatorName + " : " + strategy.ExitIndicatorName + " : " + score.ToString() + " @ " + totalbars + "\r\n";
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.IsNaN(score) || double.IsInfinity(score))
                            {
                                StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-error.log", FileMode.CreateNew, FileAccess.Write));
                                s2.Write(report);
                                s2.Close();
                            }
                            else if (totalpnl > 0 && totaltransactions > 0 && score > 0)  // Add winners to trader list
                            {
                                DataRow newrow = pool.NewRow();
                                newrow["fitness"] = score;
                                newrow["genome"] = strategy.Genome;
                                newrow["profit"] = totalpnl;
                                newrow["transactions"] = totaltransactions;
                                newrow["bars"] = totalbars;
                                newrow["inception"] = DateTime.Now;
                                pool.Rows.Add(newrow);
                                pool = SupportClass.FilterSortDataTable(pool, "", "fitness", 1);
                                FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool.WriteXml(fout);
                                fout.Close();
                                win = true;
                                //StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-winner.log", FileMode.CreateNew, FileAccess.Write));
                                //s2.Write(report);
                                //s2.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //System.Diagnostics.StackTrace trace = new StackTrace(System.Threading.Thread.CurrentThread, true);
                //System.Diagnostics.StackFrame frame = trace.GetFrame(0);
                //string fileName = frame.GetFileName();
                //int lineNumber = frame.GetFileLineNumber();
                //MessageBox.Show("File: " + fileName + "\r\nLine Number: " + lineNumber + "\r\nMessage: " + ex.Message);
            }
        }
        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, string log, Engine engine, Market market, ref string result, ref DataTable pool, string file, ref bool win, double baseline)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            win = false;
            double i1 = 0;
            double i2 = market._instruments.Count;
            try
            {
                string report = "";
                string startTime = "";
                double score;
                double totalpnl;
                double totaltransactions;
                double totaldays;
                double totalbars;
                for (int j = 0; j < engine.Trader.Agents.Count; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        report = "";
                        startTime = DateTime.Now.ToString("yyyyMMddmmss");
                        AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                        report += "---------------------------------------------------------------------------\r\n";
                        report += strategy.EntryIndicatorName + "\r\n";
                        score = 0;
                        totalpnl = 0;
                        totaltransactions = 0;
                        totaldays = 0;
                        totalbars = 0;
                        for (int i = 0; i < market._instruments.Count; i++)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                string _instrument = market._instruments[i].ToString();
                                if (market.GetTechnicalData(_instrument).Count != 0)
                                {
                                    i1++;
                                    Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                                    engine.Market = m;
                                    engine.Start();
                                    while (engine._instrument.HasNext() && e.Cancel == false)
                                    {
                                        if (worker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                        }
                                        else
                                        {
                                            engine.Iterate();
                                        }
                                    }
                                    engine.Stop();

                                    // Debug shit
                                    //if (strategy.GetTransactionCount() != strategy.GetTransactions().Count / 2)
                                    //MessageBox.Show(strategy.GetTransactionCount() + " : " + strategy.GetTransactions().Count / 2);
                                    // ----------

                                    double transactions = strategy.GetTransactionCount();
                                    double pnl = strategy.GetPnL();
                                    double days = engine.Observer.GetStatistics().getDays();
                                double bars = engine.Observer.GetStatistics().TotalHeldBars;
                                    double basis = strategy.Account.InitialBalance;
                                                                        double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                                    report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                    if (transactions > 0 && engine.Observer != null)
                                        report += engine.Observer.GetTransactionReport();
                                    score += fitness;
                                    totalpnl += pnl;
                                    totaltransactions += transactions;
                                    totaldays += days;
                                    totalbars += bars;
                                    int percentComplete = (int)((i1 / i2) * 100);
                                    worker.ReportProgress(percentComplete);
                                }
                            }
                        }
                        engine.Tally(market._instruments.Count);
                        report += "Cumulative Fitness: " + score + "\r\n";
                        result = DateTime.Now + " : " + strategy.EntryIndicatorName + " : " + strategy.ExitIndicatorName + " : " + score.ToString() + " @ " + totalbars + " : " + totalpnl.ToString() + "\r\n";
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.IsNaN(score) || double.IsInfinity(score))
                            {
                                StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-error.log", FileMode.CreateNew, FileAccess.Write));
                                s2.Write(report);
                                s2.Close();
                            }
                            else if (totalpnl > 0 && totalpnl >= baseline && totaltransactions > 0 && score > 0)  // Add winners to trader list
                            {
                                DataRow newrow = pool.NewRow();
                                newrow["fitness"] = score;
                                newrow["genome"] = strategy.Genome;
                                newrow["profit"] = totalpnl;
                                newrow["transactions"] = totaltransactions;
                                newrow["bars"] = totalbars;
                                newrow["inception"] = DateTime.Now;
                                pool.Rows.Add(newrow);
                                pool = SupportClass.FilterSortDataTable(pool, "", "fitness", 1);
                                FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool.WriteXml(fout);
                                fout.Close();
                                win = true;
                                //StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-winner.log", FileMode.CreateNew, FileAccess.Write));
                                //s2.Write(report);
                                //s2.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //System.Diagnostics.StackTrace trace = new StackTrace(System.Threading.Thread.CurrentThread, true);
                //System.Diagnostics.StackFrame frame = trace.GetFrame(0);
                //string fileName = frame.GetFileName();
                //int lineNumber = frame.GetFileLineNumber();
                //MessageBox.Show("File: " + fileName + "\r\nLine Number: " + lineNumber + "\r\nMessage: " + ex.Message);
            }
        }
        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, string log, Engine engine, Market market, ref string result, ref DataTable pool, string file, ref bool win, bool disposition)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            win = false;
            double i1 = 0;
            double i2 = market._instruments.Count;
            // start engine
            try
            {
                //FileStream fs = new FileStream(log, FileMode.Append, FileAccess.Write);
                //StreamWriter s = new StreamWriter(fs);
                string report = "";
                string startTime = "";
                double score;
                double totalpnl;
                double totaltransactions;
                double totaldays;
                double totalbars;
                for (int j = 0; j < engine.Trader.Agents.Count; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        report = "";
                        startTime = DateTime.Now.ToString("yyyyMMddmmss");
                        AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                        //s.Write("---------------------------------------------------------------------------\r\n");
                        report += "---------------------------------------------------------------------------\r\n";
                        //s.Write(DateTime.Now + " : " + strategy.EntryIndicatorName + "\r\n");
                        report += strategy.EntryIndicatorName + "\r\n";
                        score = 0;
                        totalpnl = 0;
                        totaltransactions = 0;
                        totaldays = 0;
                        totalbars = 0;
                        if (market.random == true)
                        {
                            Random _randomClass = new Random();
                            _instrumentModel = market._instruments[_randomClass.Next(0, market._instruments.Count - 1)].ToString();
                            if (market.GetTechnicalData(_instrumentModel).Count != 0)
                            {
                                Instrument m = new Instrument(market.GetTechnicalData(_instrumentModel), _instrumentModel);
                                engine.Market = m;
                                double count = 0;
                                double bars = m.TechnicalData.Count;
                                //technicalChart.GraphPane.CurveList.Clear();
                                date = new ArrayList();
                                value = new ArrayList();
                                /*
                                if (Next.Signal.IsShort && graphTrades)
                                {
                                    Chart.DrawArrow(Color.Red, 12.5f, Chart.ChartBars.CurrentBar, Ticks[0].Bid + 1, Chart.ChartBars.CurrentBar, Ticks[0].Bid);
                                    Chart.DrawText(trades.Count.ToString(), Color.Red, Chart.ChartBars.CurrentBar, Ticks[0].Bid + 10, Positioning.LowerLeft);
                                }
                                if (Next.Signal.IsLong && graphTrades)
                                {
                                    Chart.DrawArrow(Color.Green, 12.5f, Chart.ChartBars.CurrentBar, Ticks[0].Ask, Chart.ChartBars.CurrentBar, Ticks[0].Ask + 1);
                                    Chart.DrawText(trades.Count.ToString(), Color.Green, Chart.ChartBars.CurrentBar, Ticks[0].Ask - 10, Positioning.UpperLeft);
                                }
                                 **/
                                engine.Start();
                                while (engine._instrument.HasNext() && e.Cancel == false)
                                {
                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        engine.Iterate();
                                        count++;
                                        if (engine._instrument.Date > engine.StartDate)
                                        {
                                            XDate xDate = new XDate(engine._instrument.Date);
                                            date.Add(xDate.XLDate);
                                            value.Add(engine.Trader._account.AccountValue);
                                        }
                                        if (((AbstractStrategy)(engine.Trader._agents[0]))._pendingOrders.Count > 0)
                                        {
                                            
                                        }
                                        int percentComplete = (int)((count / bars) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                                engine.Stop();
                                double transactions = strategy.GetTransactionCount();
                                double pnl = strategy.GetPnL();
                                double days = engine.Observer.GetStatistics().getDays();
                                double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                double basis = strategy.Account.InitialBalance;
                                                                    double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                                //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                report += _instrumentModel + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                if (transactions > 0 && engine.Observer != null)
                                    report += engine.Observer.GetTransactionReport();
                                score += fitness;
                                totalpnl += pnl;
                                totaltransactions += transactions;
                                totaldays += days;
                                totalbars += tbars;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < market._instruments.Count; i++)
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                }
                                else
                                {
                                    string _instrument = market._instruments[i].ToString();
                                    if (market.GetTechnicalData(_instrument).Count != 0)
                                    {
                                        i1++;
                                        Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                                        engine.Market = m;
                                        engine.Start();
                                        while (engine._instrument.HasNext() && e.Cancel == false)
                                        {
                                            if (worker.CancellationPending)
                                            {
                                                e.Cancel = true;
                                            }
                                            else
                                            {
                                                engine.Iterate();
                                            }
                                        }
                                        engine.Stop();
                                        double transactions = strategy.GetTransactionCount();
                                        double pnl = strategy.GetPnL();
                                        double days = engine.Observer.GetStatistics().getDays();
                                        double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                        double basis = strategy.Account.InitialBalance;
                                                                            double fitness = CalculateFitness(pnl, transactions, days, tbars, i2);
                                        //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                        report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                        if (transactions > 0 && engine.Observer != null)
                                            report += engine.Observer.GetTransactionReport();
                                        score += fitness;
                                        totalpnl += pnl;
                                        totaltransactions += transactions;
                                        totaldays += days;
                                        totalbars += tbars;
                                        int percentComplete = (int)((i1 / i2) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                            }
                        }
                        engine.Tally(market._instruments.Count);
                        // Score was too low this way...
                        //if (market.random != true)
                        //{
                        //    score = score / (double)market._instruments.Count;
                        //}
                        //s.Write(DateTime.Now + " : Cumulative Fitness: " + score + "\r\n");
                        report += "Cumulative Fitness: " + score + "\r\n";
                        if (disposition == true)
                            result = DateTime.Now + " : " + strategy.EntryIndicatorName + " : " + score.ToString() + "\r\n";
                        else
                            result = DateTime.Now + " : " + strategy.ExitIndicatorName + " : " + score.ToString() + "\r\n";
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.IsNaN(score) || double.IsInfinity(score))
                            {
                                StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-error.log", FileMode.CreateNew, FileAccess.Write));
                                s2.Write(report);
                                s2.Close();
                            }
                            else if (totalpnl > 0 && totaltransactions > 0 && score > 0)  // Add winners to trader list
                            {
                                DataRow newrow = pool.NewRow();
                                newrow["fitness"] = score;
                                if (disposition == true)
                                    newrow["genome"] = strategy.EntryIndicatorName;
                                else
                                    newrow["genome"] = strategy.ExitIndicatorName;
                                newrow["profit"] = totalpnl;
                                newrow["transactions"] = totaltransactions;
                                newrow["bars"] = totalbars;
                                newrow["inception"] = DateTime.Now;
                                pool.Rows.Add(newrow);
                                pool = SupportClass.FilterSortDataTable(pool, "", "fitness", 1);
                                FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool.WriteXml(fout);
                                fout.Close();
                                win = true;
                                // winner log
                                if (market.random != true)
                                {
                                    StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-winner.log", FileMode.CreateNew, FileAccess.Write));
                                    s2.Write(report);
                                    s2.Close();
                                }
                            }
                        }
                    }
                    /*
                    for (int i = 0; i < _marketData.instruments.Count; i++)
                    {
                        string _instrument = _marketData.instruments[i].ToString();
                        s.Write(DateTime.Now + " : +++++ " + _instrument + " +++++\r\n");
                        ListMarketEngine m = new ListMarketEngine(_marketData.getQuotes(_instrument), _instrument);
                        _transactionEngine.MarketEngine = m;
                        _transactionEngine.started();
                        _transactionEngine.loop();
                        for (int j = 0; j < _transactionEngine.getTraderContainer.Traders.Count; j++)
                        {
                            AbstractTrader trader = (AbstractTrader)_transactionEngine.getTraderContainer.Traders[j];
                            double score = (trader.GetTransactionCount() == 0) ? 0 : (trader.GetPnL() / trader.GetTransactionCount());
                            s.Write(DateTime.Now + " : " + trader.GetName() + " : PnL/Transaction " + score + " : PnL " + trader.GetPnL() + " : Transactions " + trader.GetTransactionCount() + "\r\n");
                        }
                    }
                    */
                    //s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //System.Diagnostics.StackTrace trace = new StackTrace(System.Threading.Thread.CurrentThread, true);
                //System.Diagnostics.StackFrame frame = trace.GetFrame(0);
                //string fileName = frame.GetFileName();
                //int lineNumber = frame.GetFileLineNumber();
                //MessageBox.Show("File: " + fileName + "\r\nLine Number: " + lineNumber + "\r\nMessage: " + ex.Message);
            }
        }


        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, string log, Engine engine, Market market, ref string result, ref DataTable pool, ref DataTable pool2, string file, string file2, ref bool win, bool disposition)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            win = false;
            double i1 = 0;
            double i2 = market._instruments.Count;
            // start engine
            try
            {
                //FileStream fs = new FileStream(log, FileMode.Append, FileAccess.Write);
                //StreamWriter s = new StreamWriter(fs);
                string report = "";
                string startTime = "";
                double score;
                double totalpnl;
                double totaltransactions;
                double totaldays;
                double totalbars;
                for (int j = 0; j < engine.Trader.Agents.Count; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        report = "";
                        startTime = DateTime.Now.ToString("yyyyMMddmmss");
                        AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                        //s.Write("---------------------------------------------------------------------------\r\n");
                        report += "---------------------------------------------------------------------------\r\n";
                        //s.Write(DateTime.Now + " : " + strategy.EntryIndicatorName + "\r\n");
                        report += strategy.EntryIndicatorName + "\r\n";
                        score = 0;
                        totalpnl = 0;
                        totaltransactions = 0;
                        totaldays = 0;
                        totalbars = 0;
                        if (market.random == true)
                        {
                            Random _randomClass = new Random();
                            _instrumentModel = market._instruments[_randomClass.Next(0, market._instruments.Count - 1)].ToString();
                            if (market.GetTechnicalData(_instrumentModel).Count != 0)
                            {
                                Instrument m = new Instrument(market.GetTechnicalData(_instrumentModel), _instrumentModel);
                                engine.Market = m;
                                double count = 0;
                                double bars = m.TechnicalData.Count;
                                //technicalChart.GraphPane.CurveList.Clear();
                                date = new ArrayList();
                                value = new ArrayList();
                                engine.Start();
                                while (engine._instrument.HasNext() && e.Cancel == false)
                                {
                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        engine.Iterate();
                                        count++;
                                        XDate xDate = new XDate(engine._instrument.Date);
                                        date.Add(xDate.XLDate);
                                        value.Add(engine.Trader._account.AccountValue);
                                        int percentComplete = (int)((count / bars) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                                engine.Stop();
                                double transactions = strategy.GetTransactionCount();
                                double pnl = strategy.GetPnL();
                                double days = engine.Observer.GetStatistics().getDays();
                                double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                double basis = strategy.Account.InitialBalance;
                                                                    double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                                //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                report += _instrumentModel + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                if (transactions > 0 && engine.Observer != null)
                                    report += engine.Observer.GetTransactionReport();
                                score += fitness;
                                totalpnl += pnl;
                                totaltransactions += transactions;
                                totaldays += days;
                                totalbars += tbars;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < market._instruments.Count; i++)
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                }
                                else
                                {
                                    string _instrument = market._instruments[i].ToString();
                                    if (market.GetTechnicalData(_instrument).Count != 0)
                                    {
                                        i1++;
                                        Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                                        engine.Market = m;
                                        engine.Start();
                                        while (engine._instrument.HasNext() && e.Cancel == false)
                                        {
                                            if (worker.CancellationPending)
                                            {
                                                e.Cancel = true;
                                            }
                                            else
                                            {
                                                engine.Iterate();
                                            }
                                        }
                                        engine.Stop();
                                        double transactions = strategy.GetTransactionCount();
                                        double pnl = strategy.GetPnL();
                                        double days = engine.Observer.GetStatistics().getDays();
                                        double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                        double basis = strategy.Account.InitialBalance;
                                                                            double fitness = CalculateFitness(pnl, transactions, days, tbars, i2);
                                        //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                        report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                        if (transactions > 0 && engine.Observer != null)
                                            report += engine.Observer.GetTransactionReport();
                                        score += fitness;
                                        totalpnl += pnl;
                                        totaltransactions += transactions;
                                        totaldays += days;
                                        totalbars += tbars;
                                        int percentComplete = (int)((i1 / i2) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                            }
                        }
                        engine.Tally(market._instruments.Count);
                        // Score was too low this way...
                        //if (market.random != true)
                        //{
                        //    score = score / (double)market._instruments.Count;
                        //}
                        //s.Write(DateTime.Now + " : Cumulative Fitness: " + score + "\r\n");
                        report += "Cumulative Fitness: " + score + "\r\n";
                        result = DateTime.Now + " : " + strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName + " : " + score.ToString() + " : " + totalpnl.ToString() + "\r\n";
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.IsNaN(score) || double.IsInfinity(score))
                            {
                                StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-error.log", FileMode.CreateNew, FileAccess.Write));
                                s2.Write(report);
                                s2.Close();
                            }
                            else if (totalpnl > 0 && totaltransactions > 0 && score > 0)  // Add winners to trader list
                            {
                                DataRow newrow = pool.NewRow();
                                newrow["fitness"] = score;
                                if (strategy.EntryIndicatorName == "" || strategy.ExitIndicatorName == "")
                                {
                                    if (disposition == true)
                                    {
                                        newrow["genome"] = strategy.EntryIndicatorName;
                                    }
                                    else
                                        newrow["genome"] = strategy.ExitIndicatorName;
                                }
                                else
                                {
                                    newrow["genome"] = strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName;
                                }
                                newrow["profit"] = totalpnl;
                                newrow["transactions"] = totaltransactions;
                                newrow["bars"] = totalbars;
                                newrow["inception"] = DateTime.Now;
                                pool.Rows.Add(newrow);
                                pool = SupportClass.FilterSortDataTable(pool, "", "fitness", 1);
                                FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool.WriteXml(fout);
                                fout.Close();
                                DataRow newrow2 = pool2.NewRow();
                                newrow2["fitness"] = score;
                                if (strategy.EntryIndicatorName == "" || strategy.ExitIndicatorName == "")
                                {
                                    if (disposition == true)
                                        newrow2["genome"] = strategy.EntryIndicatorName;
                                    else
                                        newrow2["genome"] = strategy.ExitIndicatorName;
                                }
                                else
                                {
                                    newrow2["genome"] = strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName;
                                }
                                newrow2["profit"] = totalpnl;
                                newrow2["transactions"] = totaltransactions;
                                newrow2["bars"] = totalbars;
                                newrow2["inception"] = DateTime.Now;
                                pool2.Rows.Add(newrow2);
                                pool2 = SupportClass.FilterSortDataTable(pool2, "", "fitness", 1);
                                FileStream fout2 = new FileStream(file2, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool2.WriteXml(fout2);
                                fout2.Close();
                                win = true;
                                // winner log
                                if (market.random != true)
                                {
                                    StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-WINNER.log", FileMode.CreateNew, FileAccess.Write));
                                    s2.Write(report);
                                    s2.Close();
                                }
                            }
                        }
                    }
                    /*
                    for (int i = 0; i < _marketData.instruments.Count; i++)
                    {
                        string _instrument = _marketData.instruments[i].ToString();
                        s.Write(DateTime.Now + " : +++++ " + _instrument + " +++++\r\n");
                        ListMarketEngine m = new ListMarketEngine(_marketData.getQuotes(_instrument), _instrument);
                        _transactionEngine.MarketEngine = m;
                        _transactionEngine.started();
                        _transactionEngine.loop();
                        for (int j = 0; j < _transactionEngine.getTraderContainer.Traders.Count; j++)
                        {
                            AbstractTrader trader = (AbstractTrader)_transactionEngine.getTraderContainer.Traders[j];
                            double score = (trader.GetTransactionCount() == 0) ? 0 : (trader.GetPnL() / trader.GetTransactionCount());
                            s.Write(DateTime.Now + " : " + trader.GetName() + " : PnL/Transaction " + score + " : PnL " + trader.GetPnL() + " : Transactions " + trader.GetTransactionCount() + "\r\n");
                        }
                    }
                    */
                    //s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //System.Diagnostics.StackTrace trace = new StackTrace(System.Threading.Thread.CurrentThread, true);
                //System.Diagnostics.StackFrame frame = trace.GetFrame(0);
                //string fileName = frame.GetFileName();
                //int lineNumber = frame.GetFileLineNumber();
                //MessageBox.Show("File: " + fileName + "\r\nLine Number: " + lineNumber + "\r\nMessage: " + ex.Message);
            }
        }


        /*

        public int DrawArrow(Color color, float size, int bar1, double y1, int bar2, double y2)
        {
            ArrowObj arrow = CreateArrow(color, size, bar1, y1, bar2, y2);
            objectId++;
            graphObjs.Add(objectId, arrow);
            priceGraphPane.GraphObjList.Add(arrow);
            return objectId;
        }

        private ArrowObj CreateArrow(Color color, float size, int bar1, double y1, int bar2, double y2)
        {
            double x1 = barToXAxis(bar1);
            double x2 = barToXAxis(bar2);
            ArrowObj arrow = new ArrowObj(color, size, x1, y1, x2, y2);
            arrow.IsClippedToChartRect = true;
            arrow.Location.CoordinateFrame = CoordType.AxisXYScale;
            return arrow;
        }
         * 
         */


        public void BackTest(BackgroundWorker worker, DoWorkEventArgs e, string log, Engine engine, Market market, ref string result, ref DataTable pool, ref DataTable pool2, string file, string file2, ref bool win, bool disposition, double baseline)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            worker.ReportProgress(0);
            win = false;
            double i1 = 0;
            double i2 = market._instruments.Count;
            // start engine
            try
            {
                //FileStream fs = new FileStream(log, FileMode.Append, FileAccess.Write);
                //StreamWriter s = new StreamWriter(fs);
                string report = "";
                string startTime = "";
                double score;
                double totalpnl;
                double totaltransactions;
                double totaldays;
                double totalbars;
                for (int j = 0; j < engine.Trader.Agents.Count; j++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        report = "";
                        startTime = DateTime.Now.ToString("yyyyMMddmmss");
                        AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                        //s.Write("---------------------------------------------------------------------------\r\n");
                        report += "---------------------------------------------------------------------------\r\n";
                        //s.Write(DateTime.Now + " : " + strategy.EntryIndicatorName + "\r\n");
                        report += strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName + "\r\n";
                        score = 0;
                        totalpnl = 0;
                        totaltransactions = 0;
                        totaldays = 0;
                        totalbars = 0;
                        if (market.random == true)
                        {
                            Random _randomClass = new Random();
                            _instrumentModel = market._instruments[_randomClass.Next(0, market._instruments.Count - 1)].ToString();
                            if (market.GetTechnicalData(_instrumentModel).Count != 0)
                            {
                                Instrument m = new Instrument(market.GetTechnicalData(_instrumentModel), _instrumentModel);
                                engine.Market = m;
                                double count = 0;
                                double bars = m.TechnicalData.Count;
                                //technicalChart.GraphPane.CurveList.Clear();
                                date = new ArrayList();
                                value = new ArrayList();
                                engine.Start();
                                while (engine._instrument.HasNext() && e.Cancel == false)
                                {
                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                    }
                                    else
                                    {
                                        engine.Iterate();
                                        count++;
                                        XDate xDate = new XDate(engine._instrument.Date);
                                        date.Add(xDate.XLDate);
                                        value.Add(engine.Trader._account.AccountValue);
                                        int percentComplete = (int)((count / bars) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                                engine.Stop();
                                double transactions = strategy.GetTransactionCount();
                                double pnl = strategy.GetPnL();
                                double days = engine.Observer.GetStatistics().getDays();
                                double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                double basis = strategy.Account.InitialBalance;
                                double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                                //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                report += _instrumentModel + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                if (transactions > 0 && engine.Observer != null)
                                    report += engine.Observer.GetTransactionReport();
                                score += fitness;
                                totalpnl += pnl;
                                totaltransactions += transactions;
                                totaldays += days;
                                totalbars += tbars;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < market._instruments.Count; i++)
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                }
                                else
                                {
                                    string _instrument = market._instruments[i].ToString();
                                    if (market.GetTechnicalData(_instrument).Count != 0)
                                    {
                                        i1++;
                                        Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                                        engine.Market = m;
                                        engine.Start();
                                        while (engine._instrument.HasNext() && e.Cancel == false)
                                        {
                                            if (worker.CancellationPending)
                                            {
                                                e.Cancel = true;
                                            }
                                            else
                                            {
                                                engine.Iterate();
                                            }
                                        }
                                        engine.Stop();
                                        double transactions = strategy.GetTransactionCount();
                                        double pnl = strategy.GetPnL();
                                        double days = engine.Observer.GetStatistics().getDays();
                                        double tbars = engine.Observer.GetStatistics().TotalHeldBars;
                                        double basis = strategy.Account.InitialBalance;
                                                                            double fitness = CalculateFitness(pnl, transactions, days, tbars, i2);
                                        //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                                        report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                                        if (transactions > 0 && engine.Observer != null)
                                            report += engine.Observer.GetTransactionReport();
                                        score += fitness;
                                        totalpnl += pnl;
                                        totaltransactions += transactions;
                                        totaldays += days;
                                        totalbars += tbars;
                                        int percentComplete = (int)((i1 / i2) * 100);
                                        worker.ReportProgress(percentComplete);
                                    }
                                }
                            }
                        }
                        engine.Tally(market._instruments.Count);
                        // Score was too low this way...
                        //if (market.random != true)
                        //{
                        //    score = score / (double)market._instruments.Count;
                        //}
                        //s.Write(DateTime.Now + " : Cumulative Fitness: " + score + "\r\n");
                        report += "Cumulative Fitness: " + score + "\r\n";
                        result = DateTime.Now + " : " + strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName + " : " + score.ToString() + " : " + totalpnl.ToString() + "\r\n";
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.IsNaN(score) || double.IsInfinity(score))
                            {
                                StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-error.log", FileMode.CreateNew, FileAccess.Write));
                                s2.Write(report);
                                s2.Close();
                            }
                            else if (totalpnl >= baseline && totaltransactions > 0 && score > 0)  // Add winners to trader list
                            {
                                DataRow newrow = pool.NewRow();
                                newrow["fitness"] = score;
                                if (strategy.EntryIndicatorName == "" || strategy.ExitIndicatorName == "")
                                {
                                    if (disposition == true)
                                    {
                                        newrow["genome"] = strategy.EntryIndicatorName;
                                    }
                                    else
                                        newrow["genome"] = strategy.ExitIndicatorName;
                                }
                                else
                                {
                                    newrow["genome"] = strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName;
                                }
                                newrow["profit"] = totalpnl;
                                newrow["transactions"] = totaltransactions;
                                newrow["bars"] = totalbars;
                                newrow["inception"] = DateTime.Now;
                                ArrayList rowsToRemove = new ArrayList();
                                for (int x = 0; x < pool.Rows.Count; x++)
                                {
                                    if (pool.Rows[x].ItemArray[1].Equals(newrow.ItemArray[1]))
                                    {
                                        rowsToRemove.Add(pool.Rows[x]);
                                    }
                                }
                                for (int i = 0; i < rowsToRemove.Count; i++)
                                {
                                    pool.Rows.Remove((DataRow)rowsToRemove[i]);
                                }
                                pool.Rows.Add(newrow);
                                pool.AcceptChanges();
                                pool = SupportClass.FilterSortDataTable(pool, "", "fitness", 1);
                                FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool.WriteXml(fout);
                                fout.Close();


                                DataRow newrow2 = pool2.NewRow();
                                newrow2["fitness"] = score;
                                if (strategy.EntryIndicatorName == "" || strategy.ExitIndicatorName == "")
                                {
                                    if (disposition == true)
                                        newrow2["genome"] = strategy.EntryIndicatorName;
                                    else
                                        newrow2["genome"] = strategy.ExitIndicatorName;
                                }
                                else
                                {
                                    newrow2["genome"] = strategy.EntryIndicatorName + "&" + strategy.ExitIndicatorName;
                                }
                                newrow2["profit"] = totalpnl;
                                newrow2["transactions"] = totaltransactions;
                                newrow2["bars"] = totalbars;
                                newrow2["inception"] = DateTime.Now;
                                rowsToRemove = new ArrayList();
                                for (int y = 0; y < pool2.Rows.Count; y++)
                                {
                                    if (pool2.Rows[y].ItemArray[1].Equals(newrow2.ItemArray[1]))
                                    {
                                        rowsToRemove.Add(pool2.Rows[y]);
                                    }
                                }
                                for (int i = 0; i < rowsToRemove.Count; i++)
                                {
                                    pool2.Rows.Remove((DataRow)rowsToRemove[i]);
                                }
                                pool2.Rows.Add(newrow2);
                                pool2.AcceptChanges();
                                pool2 = SupportClass.FilterSortDataTable(pool2, "", "fitness", 1);
                                FileStream fout2 = new FileStream(file2, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                                pool2.WriteXml(fout2);
                                fout2.Close();
                                win = true;
                                // winner log
                                if (market.random != true)
                                {
                                    StreamWriter s2 = new StreamWriter(new FileStream(_pathLogs + startTime + "-WINNER.log", FileMode.CreateNew, FileAccess.Write));
                                    s2.Write(report);
                                    s2.Close();
                                }
                            }
                        }
                    }
                    /*
                    for (int i = 0; i < _marketData.instruments.Count; i++)
                    {
                        string _instrument = _marketData.instruments[i].ToString();
                        s.Write(DateTime.Now + " : +++++ " + _instrument + " +++++\r\n");
                        ListMarketEngine m = new ListMarketEngine(_marketData.getQuotes(_instrument), _instrument);
                        _transactionEngine.MarketEngine = m;
                        _transactionEngine.started();
                        _transactionEngine.loop();
                        for (int j = 0; j < _transactionEngine.getTraderContainer.Traders.Count; j++)
                        {
                            AbstractTrader trader = (AbstractTrader)_transactionEngine.getTraderContainer.Traders[j];
                            double score = (trader.GetTransactionCount() == 0) ? 0 : (trader.GetPnL() / trader.GetTransactionCount());
                            s.Write(DateTime.Now + " : " + trader.GetName() + " : PnL/Transaction " + score + " : PnL " + trader.GetPnL() + " : Transactions " + trader.GetTransactionCount() + "\r\n");
                        }
                    }
                    */
                    //s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //System.Diagnostics.StackTrace trace = new StackTrace(System.Threading.Thread.CurrentThread, true);
                //System.Diagnostics.StackFrame frame = trace.GetFrame(0);
                //string fileName = frame.GetFileName();
                //int lineNumber = frame.GetFileLineNumber();
                //MessageBox.Show("File: " + fileName + "\r\nLine Number: " + lineNumber + "\r\nMessage: " + ex.Message);
            }
        }
        public void BackTest(Engine engine, Market market, ref string instrument, ref DataTable pool)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            try
            {
                if (market.GetTechnicalData(instrument).Count != 0)
                {
                    Instrument m = new Instrument(market.GetTechnicalData(instrument), instrument);
                    engine.Market = m;
                    engine.Start();
                    engine.Run();
                    engine.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //MessageBox.Show(ex.Message);
            }
        }
        public void BackTest(Engine engine, Market market, ref string instrument)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            try
            {
                if (market.GetTechnicalData(instrument).Count != 0)
                {
                    Instrument m = new Instrument(market.GetTechnicalData(instrument), instrument);
                    engine.Market = m;
                    engine.Start();
                    engine.Run();
                    engine.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //MessageBox.Show(ex.Message);
            }
        }
        public double BackTest(Engine engine, Market market, bool disposition, ref SortedList list)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            double i1 = 0;
            double i2 = market._instruments.Count;
            // start engine
            //FileStream fs = new FileStream(log, FileMode.Append, FileAccess.Write);
            //StreamWriter s = new StreamWriter(fs);
            //string report = "";
            string startTime = "";
            double score;
            double totalpnl = 0;
            double totaltransactions;
            double totaldays;
            double totalbars;
            for (int j = 0; j < engine.Trader.Agents.Count; j++)
            {
                //report = "";
                startTime = DateTime.Now.ToString("yyyyMMddmmss");
                AbstractStrategy strategy = (AbstractStrategy)engine.Trader.Agents[j];
                //s.Write("---------------------------------------------------------------------------\r\n");
                //report += "---------------------------------------------------------------------------\r\n";
                //s.Write(DateTime.Now + " : " + strategy.EntryIndicatorName + "\r\n");
                //report += strategy.EntryIndicatorName + "\r\n";
                score = 0;
                totalpnl = 0;
                totaltransactions = 0;
                totaldays = 0;
                totalbars = 0;
                for (int i = 0; i < market._instruments.Count; i++)
                {
                    string _instrument = market._instruments[i].ToString();
                    if (market.GetTechnicalData(_instrument).Count != 0)
                    {
                        i1++;
                        Instrument m = new Instrument(market.GetTechnicalData(_instrument), _instrument);
                        engine.Market = m;
                        engine.Start();
                        while (engine._instrument.HasNext())
                        {
                            engine.Iterate();
                            if (engine._instrument.Date > engine.StartDate)
                            {
                                XDate xDate = new XDate(engine._instrument.Date);
                                if (list != null)
                                {
                                    if (list.ContainsKey((double)xDate))
                                    {
                                        list[(double)xDate] = (double)list[(double)xDate] + (engine.Trader._account.AccountValue - engine.Trader._account.InitialBalance);
                                    }
                                    else
                                    {
                                        list.Add((double)xDate, (engine.Trader._account.AccountValue - engine.Trader._account.InitialBalance));
                                    }
                                }
                                else
                                {
                                    list.Add((double)xDate, (engine.Trader._account.AccountValue - engine.Trader._account.InitialBalance));
                                }
                            }
                        }
                        engine.Stop();
                        double transactions = strategy.GetTransactionCount();
                        double pnl = strategy.GetPnL();
                        double days = engine.Observer.GetStatistics().getDays();
                        double bars = engine.Observer.GetStatistics().TotalHeldBars;
                        double basis = strategy.Account.InitialBalance;
                                                            double fitness = CalculateFitness(pnl, transactions, days, bars, i2);
                        //s.Write(DateTime.Now + " : " + _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n");
                        //report += _instrument + " : Fitness " + fitness + " : PnL " + pnl + " : Bars " + days + " : Transactions " + transactions + "\r\n";
                        //if (transactions > 0 && engine.Observer != null)
                        //    report += engine.Observer.GetTransactionReport();
                        score += fitness;
                        totalpnl += pnl;
                        totaltransactions += transactions;
                        totaldays += days;
                        totalbars += bars;
                        //int percentComplete = (int)((i1 / i2) * 100);
                    }
                }
            }
            engine.Tally(market._instruments.Count);
            return totalpnl;
        }


        /// <summary>
        /// Fitness Function
        /// </summary>
        /// <param name="total"></param>
        /// <param name="transactions"></param>
        /// <param name="bars"></param>
        /// <param name="basis"></param>
        /// <returns></returns>
        private double CalculateFitness(double total, double transactions, double days, double bars, double instruments)
        {
            // keep it simple stupid
            //return total;
            // or not
            //if (transactions > instruments && bars > 0 && total > 0)
                if (bars > 0 && total > 0)

            {
                double exposure = days / bars;
                return (total * transactions * exposure);
            }
            else
                return 0;
            /*
            if (transactions > 0 && bars > 0 && total > 0)
                    return (total * transactions / bars);
            else
                return 0;
             * */
        }
    }
}
