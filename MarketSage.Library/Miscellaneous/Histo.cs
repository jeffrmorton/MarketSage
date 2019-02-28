/////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2003 CenterSpace Software, LLC                            //
//                                                                         //
// This code is free software under the Artistic license.                  //
//                                                                         //
// CenterSpace Software                                                    //
// 2098 NW Myrtlewood Way                                                  //
// Corvallis, Oregon, 97330                                                //
// USA                                                                     //
// http://www.centerspace.net                                              //
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace MarketSage.Library
{

    /// <summary>
    /// Class Histo constructs and maintians a Histogram of input data.
    /// Input data is sorted into bins and a count is kept of how many data
    /// points fell into each bin. For example if the bin boundaries are
    /// 
    ///   b0, b1, b2,...,bn-1
    ///  
    ///  The n-1 bins are the intervals
    ///  
    ///  [b0,b1), [b1,b2), [b2,b3),...[bn-2,bn-1]
    /// </summary>
    public class Histo
    {
        #region Static Variables -------------------------------------------------

        private static string floatFormat_ = "E4";

        #endregion StaticVariables


        #region Instance Variables ----------------------------------------------

        private double[] binBoundaries_;
        private int[] counts_;
        private int numSmaller_ = 0;
        private int numLarger_ = 0;

        #endregion Instance Variables

        #region Constructors ----------------------------------------------------

        /// <summary>
        /// Constructs a Histogram with the specified number of bins and the 
        /// specified maximum and mininum values.
        /// </summary>
        /// <param name="numBins">The number of bins. Must be greated than zero</param>
        /// <param name="minValue">The maximum value for the Histogram.</param>
        /// <param name="maxValue">The minimum value for the Histogram.</param>
        /// <exception cref="ArgumentException">Thrown if the specified maximum
        /// and minimum values are the same. Or if the specified number of bins cannot
        /// be created.</exception>
        public Histo(int numBins, double minValue, double maxValue)
        {
            if (minValue == maxValue)
            {
                throw new ArgumentException("Maximum and minimum values are equal");
            }
            counts_ = new int[numBins];
            if (maxValue > minValue)
            {
                CalcBinBoundaries(numBins, minValue, maxValue);
            }
            else
            {
                CalcBinBoundaries(numBins, maxValue, minValue);
            }
            CheckBinBoundaries(this.binBoundaries_);
        }

        /// <summary>
        /// Create a Histogram with the specified bin boundaries.
        /// </summary>
        /// <param name="binBoundaries">Bin boundaries. Most be strictly
        /// monotonically increasing, i.e. <c>binBoundares[i]</c> is strictly
        /// less than <c>binBoundaries[i+1]</c> for each i.</param>
        public Histo(double[] binBoundaries)
        {
            CheckBinBoundaries(binBoundaries);
            binBoundaries_ = new double[binBoundaries.Length];
            Array.Copy(binBoundaries, binBoundaries_, binBoundaries.Length);
            counts_ = new int[binBoundaries_.Length - 1];
        }

        /// <summary>
        /// Construct a Histogram from the data in <c>data</c> with
        /// <c>numBins</c> bins. The bins are of eqaul size and scaled
        /// with the maximim and minimum data in <c>data</c>. The counts
        /// in the Histogram are initialized with the contents of <c>data</c>.
        /// </summary>
        /// <param name="numBins">Desired number of bins.</param>
        /// <param name="data">Vector of data to place in the Histogram.</param>
        public Histo(int numBins, double[] data)
        {
            double[] sortedData = new double[data.Length];
            Array.Copy(data, sortedData, data.Length);
            Array.Sort(sortedData);
            counts_ = new int[numBins];
            CalcBinBoundaries(numBins, sortedData[0], sortedData[data.Length - 1]);
            CheckBinBoundaries(this.binBoundaries_);
            AddSortedData(sortedData);
        }

        public Histo(int numBins, ArrayList data)
        {
            double[] sortedData = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                sortedData[i] = (double)data[i];
            }
            Array.Sort(sortedData);
            counts_ = new int[numBins];
            CalcBinBoundaries(numBins, sortedData[0], sortedData[data.Count - 1]);
            CheckBinBoundaries(this.binBoundaries_);
            AddSortedData(sortedData);
        }

        #endregion Constructors

        #region Properties ------------------------------------------------------

        /// <summary>
        /// Gets/sets the format string used to print the bin boundaries.
        /// </summary>
        public static string FloatFormat
        {
            get
            {
                return floatFormat_;
            }

            set
            {
                floatFormat_ = value;
            }
        }

        /// <summary>
        /// Gets the bin boundaries of the Histogram.
        /// </summary>
        public double[] BinBoundaries
        {
            get
            {
                return binBoundaries_;
            }
        }

        /// <summary>
        /// Gets the counts for each bin.
        /// </summary>
        public int[] Counts
        {
            get
            {
                return counts_;
            }
        }

        /// <summary>
        /// Gets the number of bins in the Histogram.
        /// </summary>
        public int NumBins
        {
            get
            {
                return counts_.Length;
            }
        }

        /// <summary>
        /// Gets the number of data points that were smaller
        /// than the smallest bin boundary.
        /// </summary>
        public int NumSmaller
        {
            get
            {
                return numSmaller_;
            }
        }

        /// <summary>
        /// Gets the number of data points that were larger than
        /// the larges bin boundary.
        /// </summary>
        public int NumLarger
        {
            get
            {
                return numLarger_;
            }
        }

        #endregion Properties

        #region Member Functions ------------------------------------------------

        /// <summary>
        /// Formats the contents of the Histogram into a string.
        /// </summary>
        /// <remarks>If the bin boundaries are b0, b1, b2,...,bn-1, and the 
        /// counts for these bins are c1, c2,...,cn, respectively,
        /// then the this method returns a string with the following 
        /// format:
        /// Number Smaller:   number smaller
        /// [b0,b1)  :   c1
        /// [b1,b2)  :   c2
        /// [b2,b3)  :   c3
        /// .
        /// .
        /// .
        /// [bn-2,bn-1]: cn
        /// Number larger : number larger</remarks>
        /// <returns>Fomatted string.</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            char closing;
            int i;
            if (numSmaller_ > 0)
            {
                buff.AppendFormat("Number smaller: {0}\r\n", numSmaller_);
            }
            for (i = 0; i < counts_.Length; ++i)
            {
                closing = (i == counts_.Length - 1 ? ']' : ')');
                buff.AppendFormat("[{0}, {1}{2}: {3}", binBoundaries_[i].ToString(floatFormat_), binBoundaries_[i + 1].ToString(FloatFormat), closing, counts_[i]);
                buff.Append("\r\n");
            }
            if (numLarger_ > 0)
            {
                buff.AppendFormat("Number larger: {0}\r\n", numLarger_);
            }
            return buff.ToString();
        }

        /// <summary>
        /// Formats the contents of the Histogram into a simple acsii stem-leaf
        /// diagram.
        /// </summary>
        /// <remarks>If the bin boundaries are b0, b1, b2,...,bn-1, and the 
        /// counts for these bins are c1, c2,...,cn, respectively,
        /// then the this method returns a string with the following 
        /// format:
        /// Number smaller:   ***number smaller
        /// [b0,b1):     *****c1
        /// [b1,b2):     **********c2
        /// [b2,b3):     ***************c3
        /// .
        /// .
        /// .
        /// [bn-2,bn-1]: *****cn
        /// Number larger : *****number larger.
        /// Where the number of '*'s is for a particular bin is equal to
        /// the count for that bin minus one.</remarks>
        /// <returns>Fomatted string.</returns>
        public string StemLeaf()
        {
            StringBuilder buff = new StringBuilder();
            char closing;
            int i, j;
            if (numSmaller_ > 0)
            {
                //buff.Append( string.Format("Number smaller: {0}\n", numSmaller_) );
                buff.AppendFormat("Number smaller: {0}\r\n", numSmaller_);
            }

            for (i = 0; i < counts_.Length; ++i)
            {
                closing = (i == counts_.Length - 1 ? ']' : ')');
                buff.AppendFormat("[{0}, {1}{2} ", binBoundaries_[i].ToString(floatFormat_),
                  binBoundaries_[i + 1].ToString(FloatFormat), closing);

                for (j = 0; j < counts_[i]; ++j)
                {
                    if (j < counts_[i] - 1)
                    {
                        buff.Append('*');
                    }
                    else
                    {
                        buff.Append(counts_[i]);
                    }
                }
                buff.Append('\r');
                buff.Append('\n');
            }

            if (numLarger_ > 0)
            {
                buff.AppendFormat("Number larger: {0}\r\n", numLarger_);
            }
            return buff.ToString();
        }

        /// <summary>
        /// Gets the count for the specifed bin.
        /// </summary>
        /// <param name="binNumber">Bin number to get the count for.
        /// <c>binNumber</c> must be between 0 and (number of bins - 1)</param>
        /// <returns>The count for the specified bin.</returns>
        public int Count(int binNumber)
        {
            return counts_[binNumber];
        }

        /// <summary>
        /// Updates the Histogram counts for the bin that the data
        /// point <c>d</c> falls into.
        /// </summary>
        /// <param name="d">The data point.</param>
        public void AddData(double d)
        {
            if (d < binBoundaries_[0])
            {
                ++numSmaller_;
            }
            else if (d > binBoundaries_[binBoundaries_.Length - 1])
            {
                ++numLarger_;
            }
            else
            {
                int bin = 1;
                while (bin < binBoundaries_.Length && d >= binBoundaries_[bin])
                {
                    ++bin;
                }
                if (bin == binBoundaries_.Length)
                {
                    ++counts_[counts_.Length - 1];
                }
                else
                {
                    ++counts_[bin - 1];
                }
            }
        }

        /// <summary>
        /// Updates the Histograms bin counts with the given data points.
        /// </summary>
        /// <param name="data">Data to update bin counts with.</param>
        public void AddData(double[] data)
        {
            double[] sortedData = (double[])data.Clone();
            Array.Sort(sortedData);
            AddSortedData(sortedData);
        }

        public void AddData(ArrayList data)
        {
            double[] sortedData = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                sortedData[i] = (double)data[i];
            }
            Array.Sort(sortedData);
            AddSortedData(sortedData);
        }

        /// <summary>
        /// Resets all counts to zero. The number of bins and bin boundaries 
        /// stay the same.
        /// </summary>
        public void Reset()
        {
            numSmaller_ = 0;
            numLarger_ = 0;
            for (int i = 0; i < counts_.Length; ++i)
            {
                counts_[i] = 0;
            }
        }

        private void AddSortedData(double[] data)
        {
            int i = 0;
            while (data[i] < binBoundaries_[0])
            {
                ++i;
                ++numSmaller_;
            }

            int binNumber;
            for (binNumber = 0; binNumber < counts_.Length; ++binNumber)
            {
                if (i >= data.Length)
                {
                    break;
                }
                for (; ; )
                {
                    if (i >= data.Length)
                    {
                        break;
                    }
                    if ((binNumber == counts_.Length - 1 && data[i] <= binBoundaries_[binNumber + 1]) ||
                      data[i] < binBoundaries_[binNumber + 1])
                    {
                        ++counts_[binNumber];
                        ++i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (i < data.Length)
            {
                numLarger_ += data.Length - i;
            }
        }

        /// <summary>
        /// Given the minimum and maximum data values and the number of bins
        /// this method fills in the boundaries vector.
        /// </summary>
        /// <param name="numBins">The number of bins desired.</param>
        /// <param name="minValue">The lower bin boundary.</param>
        /// <param name="maxValue">The upper bin boundary.</param>
        private void CalcBinBoundaries(int numBins, double minValue, double maxValue)
        {
            binBoundaries_ = new double[numBins + 1];
            binBoundaries_[0] = minValue;
            binBoundaries_[binBoundaries_.Length - 1] = maxValue;
            double binSize = (maxValue - minValue) / numBins;
            for (int i = 1; i < binBoundaries_.Length - 1; ++i)
            {
                binBoundaries_[i] = binBoundaries_[0] + i * binSize;
            }
        }

        /// <summary>
        /// Verifies that the given vector contains a strictly monotonically
        /// increasing sequence of numbers.
        /// </summary>
        /// <param name="boundaries">The boundaries to check.</param>
        /// <exception cref="ArgumentException">Thrown if the vector is not
        /// strictly monotonically increasing.</exception>
        private void CheckBinBoundaries(double[] boundaries)
        {
            for (int i = 0; i < boundaries.Length - 1; ++i)
            {
                if (boundaries[i] >= boundaries[i + 1])
                {
                    string msg = string.Format("Bin boundary {0} is >= bin boundary {1}",
                      boundaries[i], boundaries[i + 1]);
                    throw new ArgumentException(msg);
                }
            }
        }

        #endregion Member Functions
    }  // class

} // namespace
