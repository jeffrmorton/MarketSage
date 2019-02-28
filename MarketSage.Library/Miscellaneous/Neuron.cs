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
    public class Neuron
    {
        public int NoOfInputs;
        Neuron[] inputs;
        public double[] weights;
        double[] lastWeightChange;
        public double bias;
        double lastBiasChange;
        public double BPerror;
        public double value;
        public double desiredValue;
        public int ID;
        public int LayerID;
        private Random _randomClass = new Random();

        public void Initialize(int neuronID, int Layer, int No_Of_Inputs)
        {
            ID = neuronID;
            LayerID = Layer;
            NoOfInputs = No_Of_Inputs;
            inputs = new Neuron[NoOfInputs];
            weights = new double[NoOfInputs];
            lastWeightChange = new double[NoOfInputs];
            initWeights();
            desiredValue = -1;
        }

        private double af(double x)
        {
            return x * (1 - x);
        }

        public void initWeights()
        {
            double min;
            double max;
            min = -0.1;
            max = 0.1;
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                weights[i] = min + (System.Math.Abs(_randomClass.NextDouble()) * (max - min));
                lastWeightChange[i] = 0;
            }
            bias = min + (System.Math.Abs(_randomClass.NextDouble()) * (max - min));
            lastBiasChange = 0;
        }

        public void InitializeWeights(double minVal, double maxVal)
        {
            double min;
            double max;
            min = minVal;
            max = maxVal;

            for (int i = 0; i < NoOfInputs; i++)
            {
                weights[i] = min + (System.Math.Abs(_randomClass.NextDouble()) * (max - min));
                lastWeightChange[i] = 0;
            }

            bias = min + (System.Math.Abs(_randomClass.NextDouble()) * (max - min));
            lastBiasChange = 0;
        }

        public void addConnection(int index, Neuron n)
        {
            inputs[index] = n;
        }

        public void feedForward(double randomness)
        {
            double adder;
            adder = bias;
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                adder = adder + (weights[i] * inputs[i].value);
            }
            if ((randomness > 0))
            {
                adder = ((1 - randomness) * adder) + (randomness * _randomClass.NextDouble());
            }
            value = function_sigmoid(adder);
        }

        public void Backprop()
        {
            Neuron n;
            double afact;
            if ((desiredValue > -1))
            {
                BPerror = desiredValue - value;
            }
            afact = af(value);
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                n = inputs[i];
                n.BPerror = n.BPerror + (BPerror * afact * weights[i]);
            }
        }

        public void learn(double learningRate)
        {
            double afact;
            double e;
            double gradient;
            e = learningRate / (1 + NoOfInputs);
            afact = af(value);
            gradient = afact * BPerror;
            lastBiasChange = e * (lastBiasChange + 1) * gradient;
            bias = bias + lastBiasChange;
            for (int i = 0; i <= (NoOfInputs - 1); i++)
            {
                lastWeightChange[i] = e * (lastWeightChange[i] + 1) * gradient * inputs[i].value;
                weights[i] = weights[i] + lastWeightChange[i];
            }
        }

        public double getWeight(int index)
        {
            return weights[index];

        }

        /*
        public void Save(ref int FileNumber)
        {
            int i;
            PrintLine(FileNumber, ID);
            PrintLine(FileNumber, LayerID);
            PrintLine(FileNumber, NoOfInputs);
            PrintLine(FileNumber, bias);
            PrintLine(FileNumber, BPerror);
            PrintLine(FileNumber, value);
            PrintLine(FileNumber, desiredValue);
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                PrintLine(FileNumber, weights(i));
            }
        }

        public void Load(ref int FileNumber)
        {
            int i;
            Input(FileNumber, ID);
            Input(FileNumber, LayerID);
            Input(FileNumber, NoOfInputs);
            Input(FileNumber, bias);
            Input(FileNumber, BPerror);
            Input(FileNumber, value);
            Input(FileNumber, desiredValue);
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                Input(FileNumber, weights(i));
            }
        }
        */
        public void FreeMem()
        {
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                inputs[i] = null;
            }
        }

        public double Acos(double x)
        {
            if ((System.Math.Abs(x) >= 1))
            {
                x = 0.9999 * (x / System.Math.Abs(x));
            }
            return System.Math.Atan(-x / System.Math.Sqrt(-x * x + 1)) + 2 * System.Math.Atan(1);
        }

        public double function_sigmoid(double x)
        {
            return 1 / (1 + System.Math.Exp(-x));
        }
    }
}
