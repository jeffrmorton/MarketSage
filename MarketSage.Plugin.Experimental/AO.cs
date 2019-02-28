using System;
using System.Collections;
using System.Text;
using MarketSage.Library;

namespace Technical
{
    /// <summary>
    /// Awesome Oscillator
    /// 
    /// Awesome Oscillator Technical Indicator (AO) is a 34-period simple moving average,
    /// plotted through the middle points of the bars (H+L)/2, which is subtracted from
    /// the 5-period simple moving average, built across the central points of the bars
    /// (H+L)/2. It shows us quite clearly what’s happening to the market driving force
    /// at the present moment.
    /// </summary>
    public class AO : IIndicator
    {
        private ArrayList _data;
        private ArrayList _ao;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        string myName = "Awesome Oscillator";
        string myDescription = "Awesome Oscillator Technical Indicator (AO) is a 34-period simple moving average, plotted through the middle points of the bars (H+L)/2, which is subtracted from the 5-period simple moving average, built across the central points of the bars (H+L)/2. It shows us quite clearly what’s happening to the market driving force at the present moment.";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>Creates new AO object of IIndicator interface</summary>
        public AO()
        {
            _period = 34;
            _name = "AO." + _period;
            _data = new ArrayList();
            _ao = new ArrayList();
        }

        /// <summary>Creates new AO object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public AO(int period)
        {
            if (period < 34)
                _period = 34;
            else
                _period = period;
            _name = "AO." + _period;
            _data = new ArrayList();
            _ao = new ArrayList();
        }

        public string Name
        {
            get { return myName; }
        }
        public string Description
        {
            get { return myDescription; }
        }
        public string Author
        {
            get { return myAuthor; }
        }
        public string Version
        {
            get { return myVersion; }
        }

        /// <summary>
        /// Name of indicator
        /// </summary>
        /// <returns>Name</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>Returns direction</summary>
        /// <returns>int</returns>
        public int GetDirection()
        {
            return _signal;
        }

        /// <summary>Returns readyness of the indicator</summary>
        /// <returns>bool</returns>
        public bool IsReady()
        {
            return _ready;
        }

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _signal == 1;
        }

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _signal == 0;
        }

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _signal == -1;
        }

        /// <summary>Adds TechnicalData object to the indicator</summary>
        /// <param name="data">TechnicalData object to be added</param>
        public virtual void AddData(TechnicalData data)
        {
            if (!_ready)
            {
                _data.Add(data);
                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateSignal();
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(data);
                GenerateSignal();
            }
        }

        /// <summary>
        /// AO = SMA((HIGH+LOW)/2, 5) - SMA((HIGH+LOW)/2, 34)
        /// </summary>
        private void GenerateSignal()
        {
            // Calculate indicator
            double sma1, sma2, ao;
            sma1 = 0;
            for (int i = (_data.Count - 5 - 1); i < _data.Count; i++)
            {
                sma1 += (((TechnicalData)_data[i]).AdjHigh + ((TechnicalData)_data[i]).AdjLow) / 2;
            }
            sma1 = sma1 / 5;
            sma2 = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                sma2 += (((TechnicalData)_data[i]).AdjHigh + ((TechnicalData)_data[i]).AdjLow) / 2;
            }
            sma2 = sma2 / _period;
            ao = sma1 - sma2;
            _ao.Add(ao);
            // Generate signal
            _signal = 0;
            // Nought line crossing
            if (_ao.Count >= 2)
            {
                if ((((double)_ao[_ao.Count - 1]) > 0) && (((double)_ao[_ao.Count - 2]) < 0))
                {
                    _signal = 1;
                }
                if ((((double)_ao[_ao.Count - 1]) < 0) && (((double)_ao[_ao.Count - 2]) > 0))
                {
                    _signal = -1;
                }
            }
            // Saucer
            if (_ao.Count >= 3)
            {

                if ((((double)_ao[_ao.Count - 1]) > 0) && (((double)_ao[_ao.Count - 2]) > 0) && (((double)_ao[_ao.Count - 3]) > 0))
                {
                    if ((((double)_ao[_ao.Count - 1]) > ((double)_ao[_ao.Count - 2])) && (((double)_ao[_ao.Count - 2]) < ((double)_ao[_ao.Count - 3])))
                    {
                        _signal = 1;
                    }
                }
                if ((((double)_ao[_ao.Count - 1]) < 0) && (((double)_ao[_ao.Count - 2]) < 0) && (((double)_ao[_ao.Count - 3]) < 0))
                {
                    if ((((double)_ao[_ao.Count - 1]) < ((double)_ao[_ao.Count - 2])) && (((double)_ao[_ao.Count - 2]) > ((double)_ao[_ao.Count - 3])))
                    {
                        _signal = -1;
                    }
                }
            }
            // Two pikes
            if (_ao.Count >= 4)
            {
                if ((((double)_ao[_ao.Count - 1]) < 0))
                {
                    double pike1 = 0;
                    double pike2 = 0;
                    for (int x = _ao.Count - 3; pike2 != 0; x--)
                    {
                        if ((((double)_ao[x]) > ((double)_ao[x + 1])) && (((double)_ao[x + 1]) < ((double)_ao[x + 2])))
                        {
                            if (pike1 == 0)
                            {
                                pike1 = (double)_ao[x + 1];
                            }
                            else
                            {
                                pike2 = (double)_ao[x + 1];
                            }
                        }
                    }
                    if (pike2 < pike1)
                    {
                        _signal = 1;
                    }
                }
            }
            if ((((double)_ao.Count - 1) > 0))
            {
                double pike1 = 0;
                double pike2 = 0;
                for (int x = _ao.Count - 3; pike2 != 0; x--)
                {
                    if ((((double)_ao[x]) < ((double)_ao[x + 1])) && (((double)_ao[x + 1]) > ((double)_ao[x + 2])))
                    {
                        if (pike1 == 0)
                        {
                            pike1 = (double)_ao[x + 1];
                        }
                        else
                        {
                            pike2 = (double)_ao[x + 1];
                        }
                    }
                }
                if (pike2 > pike1)
                {
                    _signal = -1;
                }
            }
        }
    }
}