using System;
using System.Collections;
using System.Text;
using MarketSage.Library;

namespace Technical
{
    /// <summary>
    /// Accelerator / Decelerator Oscillator
    /// 
    /// Acceleration/Deceleration Technical Indicator (AC) measures acceleration and
    /// deceleration of the current driving force. This indicator will change direction before
    /// any changes in the driving force, which, it its turn, will change its direction before
    /// the price. If you realize that Acceleration/Deceleration is a signal of an earlier
    /// warning, it gives you evident advantages.
    /// </summary>
    public class AC : IIndicator
    {
        private ArrayList _data;
        private ArrayList _ao;
        private ArrayList _ac;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        string myName = "Accelerator / Decelerator Oscillator";
        string myDescription = "Acceleration/Deceleration Technical Indicator (AC) measures acceleration and deceleration of the current driving force. This indicator will change direction before any changes in the driving force, which, it its turn, will change its direction before the price. If you realize that Acceleration/Deceleration is a signal of an earlier warning, it gives you evident advantages.";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>Creates new AC object of IIndicator interface</summary>
        public AC()
        {
            _period = 34;
            _name = "AC." + _period;
            _data = new ArrayList();
            _ao = new ArrayList();
            _ac = new ArrayList();
        }

        /// <summary>Creates new AC object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public AC(int period)
        {
            if (period < 34)
                _period = 34;
            else
                _period = period;
            _name = "AC." + _period;
            _data = new ArrayList();
            _ao = new ArrayList();
            _ac = new ArrayList();
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
        /// AC = AO-SMA(AO, 5) 
        /// </summary>
        private void GenerateSignal()
        {
            // Calculate indicator
            double sma1, sma2, ao, ac;
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
            sma2 = sma2 / 34;
            ao = sma1 - sma2;
            _ao.Add(ao);

            ac = 0;
            if (_ao.Count >= 5)
            {
                for (int i = (_ao.Count - 5); i < _ao.Count; i++)
                {
                    ac += (double)_ao[i];
                }
                ac = ac / 5;
                _ac.Add(ac);
            }

            // Generate signal
            _signal = 0;

            if (_ac.Count >= 2)
            {
                if ((((double)_ac[_ac.Count - 1]) > 0) && (((double)_ac[_ac.Count - 2]) > 0))
                {
                    if ((((double)_ac[_ac.Count - 1]) > ((double)_ac[_ac.Count - 2])))
                    {
                        _signal = 1;
                    }
                }
                if ((((double)_ac[_ac.Count - 1]) < 0) && (((double)_ac[_ac.Count - 2]) < 0))
                {
                    if ((((double)_ac[_ac.Count - 1]) < ((double)_ac[_ac.Count - 2])))
                    {
                        _signal = -1;
                    }
                }
            }
            if (_ac.Count >= 3)
            {
                if ((((double)_ac[_ac.Count - 1]) < 0) && (((double)_ac[_ac.Count - 2]) < 0) && (((double)_ac[_ac.Count - 3]) < 0))
                {
                    if ((((double)_ac[_ac.Count - 1]) > ((double)_ac[_ac.Count - 2])) && (((double)_ac[_ac.Count - 2]) > ((double)_ac[_ac.Count - 3])))
                    {
                        _signal = 1;
                    }
                }
                if ((((double)_ac[_ac.Count - 1]) > 0) && (((double)_ac[_ac.Count - 2]) > 0) && (((double)_ac[_ac.Count - 3]) > 0))
                {
                    if ((((double)_ac[_ac.Count - 1]) < ((double)_ac[_ac.Count - 2])) && (((double)_ac[_ac.Count - 2]) < ((double)_ac[_ac.Count - 3])))
                    {
                        _signal = -1;
                    }
                }
            }
        }
    }
}
