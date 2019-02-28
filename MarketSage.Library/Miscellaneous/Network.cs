using System;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;

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
    public class Network
    {
        public string _name;
        public int _inputNeurons;
        public int _hiddenNeurons;
        public int _stateNeurons;
        public int _outputNeurons;
        //public string TrainingDataFilename;
        //public string TestDataFilename;
        public string outputFilename;
        bool[] showDimension;
        public Neuron[] _input;
        public Neuron[] _hidden;
        public Neuron[] _state;
        public Neuron[] _output;
        private double[] MinValue;
        private double[] MaxValue;
        public double BPerrorTotal;
        public double BPerror;
        public double learningRate;
        public double randomness;
        public int TrainingItterations;
        private bool _isInitialized;
        //private int outputGraphPosition;
        public double[,] outputGraph;
        public double[,] outputGraph2;
        string[] InputDimensionName;
        string[] OutputDimensionName;
        public DataTable tableGraphOutput = new DataTable();
        public DataTable tableGraphDesired = new DataTable();

        public Network(double rate, double noise)
        {
            learningRate = rate;
            randomness = noise;
        }

        public void setInputDimensionName(ref int inputIndex, ref string InputName)
        {
            InputDimensionName[inputIndex] = InputName;
        }

        public string getInputDimensionName(ref int inputIndex)
        {
            return InputDimensionName[inputIndex];
        }

        public void setOutputDimensionName(ref int OutputIndex, ref string OutputName)
        {
            OutputDimensionName[OutputIndex] = OutputName;
        }

        public string getOutputDimensionName(ref int OutputIndex)
        {
            return OutputDimensionName[OutputIndex];
        }

        public void storeOutputs()
        {
            if (tableGraphOutput.Rows.Count >= 450)
                tableGraphOutput.Rows.RemoveAt(0);
            DataRow rowGraphOutput = tableGraphOutput.NewRow();
            for (int i = 0; i < _outputNeurons; i++)
            {
                rowGraphOutput[i] = _output[i].value;
            }
            tableGraphOutput.Rows.Add(rowGraphOutput);

            if (tableGraphDesired.Rows.Count >= 450)
                tableGraphDesired.Rows.RemoveAt(0);
            DataRow rowGraphDesired = tableGraphDesired.NewRow();
            for (int i = 0; i < _outputNeurons; i++)
            {
                rowGraphDesired[i] = _output[i].desiredValue;
            }
            tableGraphDesired.Rows.Add(rowGraphDesired);

            /*
            if ((outputGraphPosition < 500))
            {
                for (int i = 0; i < _outputNeurons; i++)
                {
                    outputGraph[outputGraphPosition, i] = _output[i].value;
                    outputGraph2[outputGraphPosition, i] = _output[i].desiredValue;
                }
                outputGraphPosition = outputGraphPosition + 1;
            }
             */
        }

        public void ClearResultGraph()
        {
            tableGraphOutput.Clear();
            //outputGraphPosition = 0;
        }

        public bool getShowDimension(int dimension)
        {
            return showDimension[dimension];
        }

        public void show_Dimension(ref int dimension, ref bool state)
        {
            showDimension[dimension] = state;
        }

        public void Initialize(int inputs, int hiddens, int outputs, bool states)
        {
            if ((_isInitialized == true))
            {
                _isInitialized = false;
                FreeMem();
            }
            _inputNeurons = inputs;
            _hiddenNeurons = hiddens;
            _outputNeurons = outputs;

            _input = new Neuron[_inputNeurons];
            _hidden = new Neuron[_hiddenNeurons];
            _output = new Neuron[_outputNeurons];
            if (states == true)
            {
                _stateNeurons = _hiddenNeurons;
                _state = new Neuron[_stateNeurons];
            }

            showDimension = new bool[_inputNeurons];

            InputDimensionName = new string[_inputNeurons];
            OutputDimensionName = new string[_outputNeurons];

            for (int i = 0; i < _outputNeurons; i++)
            {
                tableGraphOutput.Columns.Add();
                tableGraphDesired.Columns.Add();
            }
            outputGraph = new double[500, _outputNeurons];
            outputGraph2 = new double[500, _outputNeurons];
            OutputDimensionName = new string[_outputNeurons];
            MinValue = new double[_outputNeurons];
            MaxValue = new double[_outputNeurons];

            for (int i = 0; i < _inputNeurons; i++)
            {
                _input[i] = new Neuron();
                _input[i].Initialize(i, 0, 0);
            }

            for (int i = 0; i < _hiddenNeurons; i++)
            {
                _hidden[i] = new Neuron();
                _hidden[i].Initialize(i, 1, _inputNeurons + _stateNeurons);

                for (int j = 0; j < _inputNeurons; j++)
                {
                    _hidden[i].addConnection(j, _input[j]);
                }
            }
            if (states == true)
            {

                for (int i = 0; i < _stateNeurons; i++)
                {
                    _state[i] = new Neuron();
                    _state[i].Initialize(i, 2, 0);

                    for (int j = 0; j < _hiddenNeurons; j++)
                    {
                        _hidden[j].addConnection(_inputNeurons + i, _state[i]);
                    }
                }
            }
            for (int i = 0; i < _outputNeurons; i++)
            {
                _output[i] = new Neuron();
                _output[i].Initialize(i, 3, _hiddenNeurons);

                for (int j = 0; j < _hiddenNeurons; j++)
                {
                    _output[i].addConnection(j, _hidden[j]);
                }
            }

            InitializeWeights();

            _isInitialized = true;
        }

        public void FreeMem()
        {
            for (int i = 0; i < _inputNeurons; i++)
            {
                _input[i].FreeMem();
                _input[i] = null;
            }
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                _hidden[i].FreeMem();
                _hidden[i] = null;
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                _state[i].FreeMem();
                _state[i] = null;
            }
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                _output[i].FreeMem();
                _output[i] = null;
            }
        }

        public void InitializeWeights()
        {
            double min;
            double max;
            Neuron n;
            min = -0.9;
            max = 0.9;

            for (int i = 0; i < _inputNeurons; i++)
            {
                n = _input[i];
                n.InitializeWeights(min, max);
            }

            for (int i = 0; i < _hiddenNeurons; i++)
            {
                n = _hidden[i];
                n.InitializeWeights(min, max);
            }

            for (int i = 0; i < _stateNeurons; i++)
            {
                n = _state[i];
                n.InitializeWeights(min, max);
            }

            for (int i = 0; i < _outputNeurons; i++)
            {
                n = _output[i];
                n.InitializeWeights(min, max);
            }
        }

        public void initWeights(double minVal, double maxVal)
        {
            double min;
            double max;
            Neuron n;
            min = minVal;
            max = maxVal;
            for (int i = 0; i <= _inputNeurons - 1; i++)
            {
                n = _input[i];
                n.InitializeWeights(min, max);
            }
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                n = _hidden[i];
                n.InitializeWeights(min, max);
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                n = _state[i];
                n.InitializeWeights(min, max);
            }
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                n = _output[i];
                n.InitializeWeights(min, max);
            }
        }

        public void feedForward()
        {
            Neuron n;
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                n = _hidden[i];
                n.feedForward(randomness);
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                n = _state[i];
                n.value = (n.value * 0.9) + ((1 - _hidden[i].value) * 0.1);
            }
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                n = _output[i];
                n.feedForward(randomness);
            }
        }

        public void Backprop()
        {
            Neuron n;
            for (int i = 0; i <= _inputNeurons - 1; i++)
            {
                n = _input[i];
                n.BPerror = 0;
            }
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                n = _hidden[i];
                n.BPerror = 0;
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                n = _state[i];
                n.BPerror = 0;
            }
            BPerrorTotal = 0;
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                n = _output[i];
                n.Backprop();
                BPerrorTotal = BPerrorTotal + n.BPerror;
            }
            BPerror = BPerrorTotal / _outputNeurons;
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                n = _hidden[i];
                n.Backprop();
                BPerrorTotal = BPerrorTotal + n.BPerror;
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                n = _state[i];
                n.Backprop();
                BPerrorTotal = BPerrorTotal + n.BPerror;
            }
            BPerrorTotal = BPerrorTotal / (_outputNeurons + _hiddenNeurons + _stateNeurons);
        }

        public void learn()
        {
            Neuron n;
            for (int i = 0; i <= _hiddenNeurons - 1; i++)
            {
                n = _hidden[i];
                n.learn(learningRate);
            }
            for (int i = 0; i <= _stateNeurons - 1; i++)
            {
                n = _state[i];
                n.learn(learningRate);
            }
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                n = _output[i];
                n.learn(learningRate);
            }
        }

        public void setInput(ref int index, ref double value)
        {
            Neuron n;
            n = _input[index];
            n.value = value;
        }

        public void setOutput(ref int index, ref double value)
        {
            Neuron n;
            n = _output[index];
            n.desiredValue = value;
        }

        public double getOutput(int index)
        {
            Neuron n;
            n = _output[index];
            return n.value;
        }

        public double getRealOutput(int index)
        {
            Neuron n;
            double dx;
            dx = MaxValue[index] - MinValue[index];
            if ((dx > 0))
            {
                n = _output[index];
                return (((n.value - 0.2) / 0.6) * dx) + MinValue[index];
            }
            else
            {
                return 0;
            }
        }

        public void setRealValues(int index, double min, double max)
        {
            MinValue[index] = min;
            MaxValue[index] = max;
        }

        public void setRealValuesFromTrainingSet(TrainingSet t)
        {
            for (int i = 0; i < _outputNeurons; i++)
            {
                MinValue[i] = t.getMinValue(i);
                MaxValue[i] = t.getMaxValue(i);
            }
        }

        public void setOutputsToGrid(ref DataTable table, int row)
        {
            DataRow newrow = table.NewRow();
            for (int i = 0; i < _outputNeurons + 1; i++)
            {
                if ((i > 0))
                {
                    newrow[i] = System.Convert.ToString((getRealOutput(i - 1) * 1000) / 1000);
                }
                else
                {
                    newrow[i] = System.Convert.ToString(row);
                }
            }
            table.Rows.Add(newrow);
        }

        public void initGrid(ref DataTable table)
        {
            for (int i = 0; i < _outputNeurons + 1; i++)
            {
                if (i > 0)
                    table.Columns.Add(OutputDimensionName[i - 1]);
                else
                    table.Columns.Add("step");
            }
            table.Clear();
        }

        public void setOutputsToFile(int FileNumber)
        {
            double v;
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                v = System.Convert.ToInt32(getRealOutput(i) * 1000) / 1000;
                //Print(FileNumber, v);
                // Print(FileNumber, " ");
            }
            //PrintLine(FileNumber, " ");
        }
        /*
        public void ShowGrid(int steps, ref DataGrid grid)
        {
            DataTable table = new DataTable();
            initGrid(ref table);
            for (int i = 0; i < steps; i++)
            {
                setOutputsToGrid(ref table, i + 1);
                feedForward();
                setInputsAsOutputs();
                storeOutputs();
            }
            grid.DataSource = table;
        }
         */

        public void loadTrainingInstance(TrainingInstance instance)
        {
            for (int i = 0; i < instance.NoOfInputs; i++)
            {
                _input[i].value = instance.getInput(i);
            }
            for (int i = 0; i <= instance.NoOfOutputs - 1; i++)
            {
                _output[i].desiredValue = instance.getOutput(i);
            }
        }

        /*
        public void setImage(ref object img)
        {
            int x;
            int y;
            i = 0;
            for (int x = 0; x <= img.Width - 1; x++)
            {
                for (int y = 0; y <= img.Height - 1; y++)
                {
                    inputs[i].value = img.getPoint(x, y) / 255;
                    i = i + 1;
                }
            }
        }
        */

        public bool Save(string filename)
        {
            XmlTextWriter tw = new XmlTextWriter(filename, null);
            tw.Formatting = Formatting.Indented;
            tw.WriteStartDocument(true);
            tw.WriteStartElement("network");
            tw.WriteAttributeString("name", _name);
            tw.WriteAttributeString("inputs", _inputNeurons.ToString());
            tw.WriteAttributeString("hiddens", _hiddenNeurons.ToString());
            tw.WriteAttributeString("states", _stateNeurons.ToString());
            tw.WriteAttributeString("outputs", _outputNeurons.ToString());
            tw.WriteAttributeString("learning_rate", learningRate.ToString());
            tw.WriteAttributeString("noise", randomness.ToString());
            tw.WriteAttributeString("iterations", TrainingItterations.ToString());
            tw.WriteAttributeString("error", BPerror.ToString());
            tw.WriteAttributeString("error_total", BPerrorTotal.ToString());
            tw.WriteStartElement("input");
            for (int i = 0; i < _inputNeurons; i++)
            {
                tw.WriteStartElement("neuron");
                tw.WriteAttributeString("unit", i.ToString());
                tw.WriteAttributeString("layer", _input[i].LayerID.ToString());
                tw.WriteAttributeString("inputs", _input[i].NoOfInputs.ToString());
                tw.WriteAttributeString("bias", _input[i].bias.ToString());
                tw.WriteAttributeString("error", _input[i].BPerror.ToString());
                tw.WriteAttributeString("value", _input[i].value.ToString());
                tw.WriteAttributeString("desired", _input[i].desiredValue.ToString());
                tw.WriteEndElement();
            }
            tw.WriteEndElement();
            tw.WriteStartElement("hidden");
            for (int i = 0; i < _hiddenNeurons; i++)
            {
                tw.WriteStartElement("neuron");
                tw.WriteAttributeString("unit", i.ToString());
                tw.WriteAttributeString("layer", _hidden[i].LayerID.ToString());
                tw.WriteAttributeString("inputs", _hidden[i].NoOfInputs.ToString());
                tw.WriteAttributeString("bias", _hidden[i].bias.ToString());
                tw.WriteAttributeString("error", _hidden[i].BPerror.ToString());
                tw.WriteAttributeString("value", _hidden[i].value.ToString());
                tw.WriteAttributeString("desired", _hidden[i].desiredValue.ToString());
                for (int j = 0; j < _hidden[i].NoOfInputs; j++)
                {
                    tw.WriteStartElement("weight");
                    tw.WriteAttributeString("unit", j.ToString());
                    tw.WriteAttributeString("value", _hidden[i].weights[j].ToString());
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();
            }
            tw.WriteEndElement();
            tw.WriteStartElement("state");
            for (int i = 0; i < _stateNeurons; i++)
            {
                tw.WriteStartElement("neuron");
                tw.WriteAttributeString("unit", i.ToString());
                tw.WriteAttributeString("layer", _state[i].LayerID.ToString());
                tw.WriteAttributeString("inputs", _state[i].NoOfInputs.ToString());
                tw.WriteAttributeString("bias", _state[i].bias.ToString());
                tw.WriteAttributeString("error", _state[i].BPerror.ToString());
                tw.WriteAttributeString("value", _state[i].value.ToString());
                tw.WriteAttributeString("desired", _state[i].desiredValue.ToString());
                for (int j = 0; j < _state[i].NoOfInputs; j++)
                {
                    tw.WriteStartElement("weight");
                    tw.WriteAttributeString("unit", j.ToString());
                    tw.WriteAttributeString("value", _state[i].weights[j].ToString());
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();
            }
            tw.WriteEndElement();
            tw.WriteStartElement("output");
            for (int i = 0; i < _outputNeurons; i++)
            {
                tw.WriteStartElement("neuron");
                tw.WriteAttributeString("unit", i.ToString());
                tw.WriteAttributeString("layer", _output[i].LayerID.ToString());
                tw.WriteAttributeString("inputs", _output[i].NoOfInputs.ToString());
                tw.WriteAttributeString("bias", _output[i].bias.ToString());
                tw.WriteAttributeString("error", _output[i].BPerror.ToString());
                tw.WriteAttributeString("value", _output[i].value.ToString());
                tw.WriteAttributeString("desired", _output[i].desiredValue.ToString());
                for (int j = 0; j < _output[i].NoOfInputs; j++)
                {
                    tw.WriteStartElement("weight");
                    tw.WriteAttributeString("unit", j.ToString());
                    tw.WriteAttributeString("value", _output[i].weights[j].ToString());
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();
            }
            tw.WriteEndElement();
            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Flush();
            tw.Close();
            return true;
        }

        public bool Load(string filename)
        {
            outputFilename = filename;
            if ((_isInitialized == true))
            {
                _isInitialized = false;
                //                FreeMem();
            }
            if (File.Exists(filename))
                try
                {
                    XmlTextReader tr = new XmlTextReader(filename);
                    int layer = 0;
                    int neuron = 0;
                    int weight = 0;
                    while (tr.Read())
                    {
                        switch (tr.NodeType)
                        {
                            case XmlNodeType.Element: // The node is an element.
                                if (tr.Name == "network")
                                {
                                    while (tr.MoveToNextAttribute()) // Read the attributes.
                                    {
                                        switch (tr.Name)
                                        {
                                            case "name":
                                                _name = tr.Value;
                                                break;
                                            case "inputs":
                                                _inputNeurons = int.Parse(tr.Value);
                                                break;
                                            case "hiddens":
                                                _hiddenNeurons = int.Parse(tr.Value);
                                                break;
                                            case "states":
                                                _stateNeurons = int.Parse(tr.Value);
                                                break;
                                            case "outputs":
                                                _outputNeurons = int.Parse(tr.Value);
                                                break;
                                            case "learning_rate":
                                                learningRate = double.Parse(tr.Value);
                                                break;
                                            case "noise":
                                                randomness = double.Parse(tr.Value);
                                                break;
                                            case "iterations":
                                                TrainingItterations = int.Parse(tr.Value);
                                                break;
                                            case "error":
                                                BPerror = double.Parse(tr.Value);
                                                break;
                                            case "error_total":
                                                BPerrorTotal = double.Parse(tr.Value);
                                                break;
                                        }
                                    }

                                    _input = new Neuron[_inputNeurons];
                                    _hidden = new Neuron[_hiddenNeurons];
                                    _output = new Neuron[_outputNeurons];
                                    _state = new Neuron[_stateNeurons];

                                    MinValue = new double[_outputNeurons];
                                    MaxValue = new double[_outputNeurons];

                                    for (int i = 0; i < _inputNeurons; i++)
                                    {
                                        _input[i] = new Neuron();
                                        _input[i].Initialize(i, 0, 0);
                                    }

                                    for (int i = 0; i < _hiddenNeurons; i++)
                                    {
                                        _hidden[i] = new Neuron();
                                        _hidden[i].Initialize(i, 1, _inputNeurons + _stateNeurons);

                                        for (int j = 0; j < _inputNeurons; j++)
                                        {
                                            _hidden[i].addConnection(j, _input[j]);
                                        }
                                    }
                                    for (int i = 0; i < _stateNeurons; i++)
                                    {
                                        _state[i] = new Neuron();
                                        _state[i].Initialize(i, 2, 0);

                                        for (int j = 0; j < _hiddenNeurons; j++)
                                        {
                                            _hidden[j].addConnection(_inputNeurons + i, _state[i]);
                                        }
                                    }
                                    for (int i = 0; i < _outputNeurons; i++)
                                    {
                                        _output[i] = new Neuron();
                                        _output[i].Initialize(i, 3, _hiddenNeurons);

                                        for (int j = 0; j < _hiddenNeurons; j++)
                                        {
                                            _output[i].addConnection(j, _hidden[j]);
                                        }
                                    }
                                }

                                if (tr.Name == "input")
                                {
                                    layer = 0;
                                }

                                if (tr.Name == "hidden")
                                {
                                    layer = 1;
                                }

                                if (tr.Name == "state")
                                {
                                    layer = 2;
                                }

                                if (tr.Name == "output")
                                {
                                    layer = 3;
                                }

                                if (tr.Name == "neuron")
                                {
                                    while (tr.MoveToNextAttribute()) // Read the attributes.
                                    {
                                        switch (tr.Name)
                                        {
                                            case "unit":
                                                neuron = int.Parse(tr.Value);
                                                break;
                                            case "layer":
                                                if (layer == 0)
                                                    _input[neuron].LayerID = int.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].LayerID = int.Parse(tr.Value);
                                                if (layer == 2)
                                                    _state[neuron].LayerID = int.Parse(tr.Value);
                                                if (layer == 3)
                                                    _output[neuron].LayerID = int.Parse(tr.Value);
                                                break;
                                            case "inputs":
                                                if (layer == 0)
                                                    _input[neuron].NoOfInputs = int.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].NoOfInputs = int.Parse(tr.Value);
                                                if (layer == 2)
                                                    _hidden[neuron].NoOfInputs = int.Parse(tr.Value);
                                                if (layer == 3)
                                                    _hidden[neuron].NoOfInputs = int.Parse(tr.Value);
                                                break;
                                            case "bias":
                                                if (layer == 0)
                                                    _input[neuron].bias = double.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].bias = double.Parse(tr.Value);
                                                if (layer == 2)
                                                    _hidden[neuron].bias = double.Parse(tr.Value);
                                                if (layer == 3)
                                                    _hidden[neuron].bias = double.Parse(tr.Value);
                                                break;
                                            case "error":
                                                if (layer == 0)
                                                    _input[neuron].BPerror = double.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].BPerror = double.Parse(tr.Value);
                                                if (layer == 2)
                                                    _hidden[neuron].BPerror = double.Parse(tr.Value);
                                                if (layer == 3)
                                                    _hidden[neuron].BPerror = double.Parse(tr.Value);
                                                break;
                                            case "value":
                                                if (layer == 0)
                                                    _input[neuron].value = double.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].value = double.Parse(tr.Value);
                                                if (layer == 2)
                                                    _hidden[neuron].value = double.Parse(tr.Value);
                                                if (layer == 3)
                                                    _hidden[neuron].value = double.Parse(tr.Value);
                                                break;
                                            case "desired":
                                                if (layer == 0)
                                                    _input[neuron].desiredValue = double.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].desiredValue = double.Parse(tr.Value);
                                                if (layer == 2)
                                                    _hidden[neuron].desiredValue = double.Parse(tr.Value);
                                                if (layer == 3)
                                                    _hidden[neuron].desiredValue = double.Parse(tr.Value);
                                                break;
                                        }
                                    }
                                }

                                if (tr.Name == "weight")
                                {
                                    while (tr.MoveToNextAttribute()) // Read the attributes.
                                    {
                                        switch (tr.Name)
                                        {
                                            case "unit":
                                                weight = int.Parse(tr.Value);
                                                break;
                                            case "value":
                                                if (layer == 0)
                                                    _input[neuron].weights[weight] = double.Parse(tr.Value);
                                                if (layer == 1)
                                                    _hidden[neuron].weights[weight] = double.Parse(tr.Value);
                                                if (layer == 2)
                                                    _state[neuron].weights[weight] = double.Parse(tr.Value);
                                                if (layer == 3)
                                                    _output[neuron].weights[weight] = double.Parse(tr.Value);
                                                break;
                                        }
                                    }
                                }
                                break;
                            case XmlNodeType.Text: //Display the text in each element.
                                break;
                            case XmlNodeType.EndElement: //Display the end of the element.
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            else
                return false;
            _isInitialized = true;
            return true;
        }

        /*
        public int getClassification()
        {
            double max;
            double value;
            max = -1;
            for (int i = 0; i <= NoOfOutputs - 1; i++)
            {
                value = outputs[i].value;
                if ((value > max))
                {
                    max = value;
                    return i;
                }
                else
                    return 0;
            }
        }

        public int setClassification(int classification)
        {
            for (int i = 0; i <= NoOfOutputs - 1; i++)
            {
                if ((i != classification))
                {
                    outputs[i].desiredValue = 0.1;
                }
                else
                {
                    outputs[i].desiredValue = 0.9;
                }
            }
        }
        /*
        public void showChart(ref object chart, ref int chartType)
        {
            int NoOfUnits;
            int i;
            int j;
            Neuron n;
            if (chartType == 0)
            {
                chart.chartType = 5;
                chart.RowCount = NoOfInputs;
                chart.ColumnCount = 1;
            }
            else if (chartType == 1)
            {
                chart.chartType = 5;
                chart.RowCount = NoOfHiddens;
                chart.ColumnCount = 1;
            }
            else if (chartType == 2)
            {
                chart.chartType = 5;
                chart.RowCount = noofStates;
                chart.ColumnCount = 1;
            }
            else if (chartType == 3)
            {
                chart.chartType = 5;
                chart.RowCount = NoOfOutputs;
                chart.ColumnCount = 1;
            }
            else if (chartType == 4)
            {
                chart.chartType = 4;
                chart.RowCount = NoOfInputs;
                chart.ColumnCount = NoOfHiddens;
            }
            else if (chartType == 5)
            {
                chart.chartType = 4;
                chart.RowCount = noofStates;
                chart.ColumnCount = NoOfHiddens;
            }
            else if (chartType == 6)
            {
                chart.chartType = 4;
                chart.RowCount = NoOfOutputs;
                chart.ColumnCount = NoOfHiddens;
            }
            for (int i = 0; i <= chart.RowCount - 1; i++)
            {
                chart.row = i + 1;
                if (chartType == 0)
                {
                    chart.Data = inputs[i].value;
                }
                else if (chartType == 1)
                {
                    chart.Data = Hiddens[i].value;
                }
                else if (chartType == 2)
                {
                    chart.Data = states[i].value;
                }
                else if (chartType == 3)
                {
                    chart.Data = outputs[i].value;
                }
                else if (chartType == 4)
                {
                    for (int j = 0; j <= chart.ColumnCount - 1; j++)
                    {
                        chart.Column = j + 1;
                        chart.Data = Hiddens[j].getWeight(i);
                    }
                }
                else if (chartType == 5)
                {
                    for (int j = 0; j <= chart.ColumnCount - 1; j++)
                    {
                        chart.Column = j + 1;
                        chart.Data = states[i].getWeight(j);
                    }
                }
                else if (chartType == 6)
                {
                    for (int j = 0; j <= chart.ColumnCount - 1; j++)
                    {
                        chart.Column = j + 1;
                        chart.Data = outputs[i].getWeight(j);
                    }
                }
            }
            chart.Refresh();
        }
        */

        public void ClearStateUnits()
        {
            for (int i = 0; i < _stateNeurons; i++)
            {
                _state[i].value = 0;
                _state[i].desiredValue = 0;
            }
        }

        public void update()
        {
            feedForward();
            Backprop();
            learn();
        }

        public void setInputsAsOutputs()
        {
            for (int i = 0; i <= _outputNeurons - 1; i++)
            {
                _input[i].value = _output[i].value;
            }
        }
    }
}
