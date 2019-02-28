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
    public class TrainingInstance
    {
        public int NoOfInputs;
        public int NoOfOutputs;
        //public double TrainingError;
        double[] inputs;
        double[] outputs;
        public int imageWidth;
        public int imageHeight;

        public void init(int No_Of_Inputs, int No_Of_Outputs)
        {
            imageWidth = 0;
            imageHeight = 0;
            NoOfInputs = No_Of_Inputs;
            NoOfOutputs = No_Of_Outputs;
            inputs = new double[NoOfInputs];
            outputs = new double[NoOfOutputs];
        }

        //public void initImage(ref ImageProcessing img, int No_Of_Outputs)
        //{
        //    imageWidth = img.width;
        //    imageHeight = img.height;
        //    NoOfInputs = imageWidth * imageHeight;
        //    NoOfOutputs = No_Of_Outputs;
        //setImage(ref img);
        //}

        public void setInput(int index, double value)
        {
            inputs[index] = value;
        }

        public double getInput(int index)
        {
            return inputs[index];
        }

        public void setOutput(int index, double value)
        {
            outputs[index] = value;
        }

        public double getOutput(int index)
        {
            return outputs[index];
        }

        /*
        public void setImage(ref ImageProcessing img)
        {
            int i = 0;
            for (int x = 0; x <= img.width - 1; x++)
            {
                for (int y = 0; y <= img.height - 1; y++)
                {
                    inputs[i] = img.getPoint(x, y) / 255;
                    i = i + 1;
                }
            }
        }
        /*
        public void getImage(ref ImageProcessing img)
        {
            int i = 0;
            for (int x = 0; x <= imageWidth - 1; x++)
            {
                for (int y = 0; y <= imageHeight - 1; y++)
                {
                    img.setPoint(x, y, System.Convert.ToInt16(inputs[i] * 255));
                    i = i + 1;
                }
            }
        }
        */

        public void setClassification(int classificationID)
        {
            for (int i = 0; i <= NoOfOutputs; i++)
            {
                if ((i != classificationID))
                {
                    outputs[i] = 0.1;
                }
                else
                {
                    outputs[i] = 0.9;
                }
            }
        }

        /*
        public void Save(ref int FileNumber)
        {
            int i;
            int n;
            PrintLine(FileNumber, "[Training Instance]");
            PrintLine(FileNumber, NoOfInputs);
            PrintLine(FileNumber, NoOfOutputs);
            PrintLine(FileNumber, TrainingError);
            PrintLine(FileNumber, imageWidth);
            PrintLine(FileNumber, imageHeight);
            n = 0;
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                Print(FileNumber, inputs(i) + ", ");
                if ((n >= imageWidth))
                {
                    n = 0;
                    PrintLine(FileNumber, " ");
                }
                n = n + 1;
            }
            for (int i = 0; i <= NoOfOutputs - 1; i++)
            {
                PrintLine(FileNumber, outputs(i));
            }
        }

        public void Load(ref int FileNumber)
        {
            int i;
            string dummy;
            Input(FileNumber, dummy);
            Input(FileNumber, NoOfInputs);
            Input(FileNumber, NoOfOutputs);
            Input(FileNumber, TrainingError);
            Input(FileNumber, imageWidth);
            Input(FileNumber, imageHeight);
            init(NoOfInputs, NoOfOutputs);
            for (int i = 0; i <= NoOfInputs - 1; i++)
            {
                Input(FileNumber, inputs[i]);
            }
            for (int i = 0; i <= NoOfOutputs - 1; i++)
            {
                Input(FileNumber, outputs[i]);
            }
        }
         */
    }
}
