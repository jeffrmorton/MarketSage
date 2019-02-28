using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
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
    public class TrainingSet
    {
        public int NoOfInstances;
        public ArrayList instances = new ArrayList();
        double[] MinValue;
        double[] MaxValue;

        public double getMinValue(int index)
        {
            return MinValue[index];
        }

        public double getMaxValue(int index)
        {
            return MaxValue[index];
        }

        public void addInstance(TrainingInstance newInstance)
        {
            instances.Add(newInstance);
            NoOfInstances = instances.Count;
        }

        public TrainingInstance getInstance(int index)
        {
            return (TrainingInstance)instances[index];
        }

        public void Clear()
        {
            instances.Clear();
            NoOfInstances = 0;
        }

        /*
        public void Save(ref string filename)
        {
            int i;
            classTrainingInstance inst;
            int FileNumber;
            FileNumber = FreeFile;
            FileOpen(FileNumber, filename, OpenMode.Output);
            PrintLine(FileNumber, "[Training Set]");
            PrintLine(FileNumber, instances.Count());
            for (int i = 0; i <= instances.Count() - 1; i++)
            {
                inst = getInstance(i);
                inst.Save(FileNumber);
            }
            FileClose(FileNumber);
        }

        public void Load(ref string filename)
        {
            int i;
            TrainingInstance inst;
            string dummy;
            int NoOfInstances;
            int FileNumber;
            Clear();
            FileNumber = FreeFile;
            FileOpen(FileNumber, filename, OpenMode.Input);
            Input(FileNumber, dummy);
            PrintLine(FileNumber, NoOfInstances);
            for (int i = 0; i <= NoOfInstances - 1; i++)
            {
                inst = new classTrainingInstance();
                inst.Load(FileNumber);
            }
            FileClose(FileNumber);
        }
        */
        /*
        public void Train(Network bp)
        {
            TrainingInstance inst;
            for (int i = 1; i <= instances.Count; i++)
            {
                inst = (TrainingInstance)instances[i];
                bp.loadTrainingInstance(inst);
                bp.update();
            }
        }
         */

        public void ImportTimeSeries(string filename, int Dimensions, int TimeDelay)
        {
            //int n;
            double dv;
            string[] dataArray = new string[7];
            Regex rex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            ArrayList _data = new ArrayList();

            TrainingInstance inst;

            double[] currentData = new double[Dimensions];
            double[,] prevHistory = new double[Dimensions, TimeDelay + 1];


            System.IO.StreamReader file_reader = null;
            System.IO.StreamReader buffered_reader = null;

            if ((filename != ""))
            {
                MinValue = new double[Dimensions];
                MaxValue = new double[Dimensions];
                for (int i = 0; i < Dimensions; i++)
                {
                    MinValue[i] = 999999999;
                    MaxValue[i] = -999999999;
                }

                Clear();

                if (File.Exists(filename))
                {
                    try
                    {
                        file_reader = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
                        buffered_reader = new System.IO.StreamReader(file_reader.BaseStream, file_reader.CurrentEncoding);
                        string line = null;
                        buffered_reader.ReadLine();
                        System.Collections.ArrayList _temp = new System.Collections.ArrayList();


                        while ((line = buffered_reader.ReadLine()) != null)
                        {
                            dataArray = rex.Split(line);

                            for (int i = 0; i < dataArray.Length - 1; i++)
                            {
                                if ((double.Parse(dataArray[i + 1]) < MinValue[i]))
                                {
                                    MinValue[i] = double.Parse(dataArray[i + 1]);
                                }

                                if ((double.Parse(dataArray[i + 1]) > MaxValue[i]))
                                {
                                    MaxValue[i] = double.Parse(dataArray[i + 1]);
                                }

                                dv = MaxValue[i] - MinValue[i];
                                if ((dv > 0))
                                {
                                    currentData[i] = System.Math.Abs(((double.Parse(dataArray[i + 1]) - MinValue[i]) * 0.6) / dv) + 0.2;
                                }
                                else
                                {
                                    currentData[i] = 0;
                                }

                                inst = new TrainingInstance();

                                inst.init(Dimensions * TimeDelay, Dimensions);

                                int n = 0;
                                for (int k = 0; k < TimeDelay; k++)
                                {
                                    for (int j = 0; j < Dimensions; j++)
                                    {
                                        inst.setInput(n, prevHistory[j, k]);
                                        n = n + 1;
                                    }
                                }

                                for (int j = 0; j < Dimensions; j++)
                                {
                                    inst.setOutput(j, currentData[j]);
                                }

                                addInstance(inst);

                                for (int j = 0; j < Dimensions; j++)
                                {
                                    for (int k = TimeDelay - 1; k > 0; k--)
                                    {
                                        prevHistory[j, k] = prevHistory[j, k - 1];
                                    }
                                }

                                for (int j = 0; j < Dimensions; j++)
                                {
                                    prevHistory[j, 0] = currentData[j];
                                }
                            }
                        }
                    }
                    finally
                    {
                        buffered_reader.Close();
                        file_reader.Close();
                    }
                }
            }
        }

        /*
        private double getNextValue(ref int FileNumber)
        {
            int i;
            string c;
            string dataStr;
            c = InputString(FileNumber, 1);
            while (((Asc(c) < 48) | (Asc(c) > 58)) & (!(EOF(FileNumber))))
            {
                c = InputString(FileNumber, 1);
            }
            if ((Asc(c) > 48) & (Asc(c) < 58))
            {
                dataStr = c;
            }
            while ((c != " ") & (c != ",") & (Asc(c) != 13) & (!(EOF(FileNumber))))
            {
                c = InputString(FileNumber, 1);
                dataStr = dataStr + c;
            }
            if ((dataStr != ""))
            {
                getNextValue = Val(dataStr);
            }
            else
            {
                getNextValue = -9999;
            }
        }
         */
    }
}
