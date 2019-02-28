using System;
using System.Collections;
using System.Text;
using MarketSage.Library;

namespace MarketSage
{
    public class Gann
    {
        private ArrayList _data;
        private int _cycle;
        private double[] _forms;

        public Gann(ArrayList data, int cycle)
        {
            _data = data;
            _cycle = cycle;

            _forms = new double[_data.Count];
            for (int a = 0; a < _data.Count; a++)
                _forms[a] = 0;

            // Find bottoms
            int index = _cycle;
            while (index < _data.Count - _cycle)
            {
                if (bottomLower(index))
                    if (bottomHigher(index))
                        _forms[index] = -1;
                index++;
            }

            // Find tops
            index = _cycle;
            while (index < _data.Count - _cycle)
            {
                if (topLower(index))
                    if (topHigher(index))
                        _forms[index] = 1;
                index++;
            }
        }

        private bool bottomLower(int index)
        {
            if (index < _data.Count)
            {
                TechnicalData data2 = (TechnicalData)_data[index];
                for (int a = 1; a <= _cycle; a++)
                {
                    TechnicalData data1 = (TechnicalData)_data[index - a];
                    if (data2.AdjLow > data1.AdjLow)
                        return false;
                }
                return true;
            }
            return false;
        }

        private bool bottomHigher(int index)
        {
            if (index < _data.Count - _cycle)
            {
                TechnicalData data2 = (TechnicalData)_data[index];
                for (int a = 1; a <= _cycle; a++)
                {
                    TechnicalData data1 = (TechnicalData)_data[index + a];
                    if (data2.AdjLow > data1.AdjLow)
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        private bool topLower(int index)
        {
            if (index < _data.Count)
            {
                TechnicalData data2 = (TechnicalData)_data[index];
                for (int a = 1; a <= _cycle; a++)
                {
                    TechnicalData data1 = (TechnicalData)_data[index - a];
                    if (data2.AdjHigh < data1.AdjHigh)
                        return false;
                }
                return true;
            }
            return false;
        }

        private bool topHigher(int index)
        {
            if (index < _data.Count - _cycle)
            {
                TechnicalData data2 = (TechnicalData)_data[index];
                for (int a = 1; a <= _cycle; a++)
                {
                    TechnicalData data1 = (TechnicalData)_data[index + a];
                    if (data2.AdjHigh < data1.AdjHigh)
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        public double[] getForms()
        {
            return _forms;
        }
    }
}
