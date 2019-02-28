// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.machinelearning;

import java.io.PrintStream;
import java.util.ArrayList;
import java.util.List;
import org.apache.log4j.Logger;
import org.joone.engine.*;
import org.joone.engine.learning.TeachingSynapse;
import org.joone.io.MemoryInputSynapse;
import org.joone.io.MemoryOutputSynapse;
import org.joone.net.NeuralNet;
import org.joone.util.*;
import robotrader.indicator.AbstractIndicator;
import robotrader.market.HistoricData;

public class JooneNeuralNetwork extends AbstractIndicator
    implements NeuralNetListener
{

    public JooneNeuralNetwork()
    {
        _data = new ArrayList();
        _training_rows = 150;
        _forecast_rows = 1;
        _epochs = 1000;
        _temporal_window = 10;
    }

    public JooneNeuralNetwork(int ai[], int i, int j, int k, float f)
    {
        _data = new ArrayList();
        _training_rows = 150;
        _forecast_rows = 1;
        _epochs = 1000;
        _temporal_window = 10;
        _rows = ai;
        _training_rows = i;
        _temporal_window = j;
        _epochs = k;
        _percentage_change = f;
        createNet();
    }

    public String getName()
    {
        return toString(_rows, _training_rows, _temporal_window, _percentage_change);
    }

    public void add(HistoricData historicdata)
    {
        _data.add(historicdata);
        if(!_ready)
        {
            if(_data.size() >= _training_rows + _forecast_rows)
            {
                double ad[][] = new double[_training_rows][2];
                double ad1[][] = new double[_training_rows][1];
                for(int j = 0; j < _training_rows; j++)
                {
                    HistoricData historicdata1 = (HistoricData)_data.get(j);
                    ad[j][0] = historicdata1.getAdjustedClose();
                    ad[j][1] = historicdata1.getVolume();
                    historicdata1 = (HistoricData)_data.get(j + _forecast_rows);
                    ad1[j][0] = historicdata1.getAdjustedClose();
                }

                MemoryInputSynapse memoryinputsynapse = getMemoryInputSynapse("1,1,1,2");
                memoryinputsynapse.addPlugIn(getNormalizerPlugin("1-4"));
                memoryinputsynapse.addPlugIn(getMovingAveragePlugIn("2,3", "10,50"));
                memoryinputsynapse.setInputArray(ad);
                memoryinputsynapse.setFirstRow(1);
                memoryinputsynapse.setLastRow(_training_rows);
                MemoryInputSynapse memoryinputsynapse2 = getMemoryInputSynapse("1");
                memoryinputsynapse2.addPlugIn(getNormalizerPlugin("1"));
                memoryinputsynapse2.addPlugIn(getMinMaxPlugIn("1", _percentage_change));
                memoryinputsynapse2.setInputArray(ad1);
                memoryinputsynapse2.setFirstRow(1);
                memoryinputsynapse2.setLastRow(_training_rows);
                _input_layer.addInputSynapse(memoryinputsynapse);
                trainer.setDesired(memoryinputsynapse2);
                train();
                _ready = true;
                _direction = 0;
                _data.clear();
            }
        } else
        {
            int i = _temporal_window;
            if(_data.size() >= i)
            {
                double ad2[][] = new double[i][2];
                for(int k = 0; k < i; k++)
                {
                    HistoricData historicdata2 = (HistoricData)_data.get(k);
                    ad2[k][0] = historicdata2.getAdjustedClose();
                    ad2[k][1] = historicdata2.getVolume();
                }

                _input_layer.removeAllInputs();
                MemoryInputSynapse memoryinputsynapse1 = getMemoryInputSynapse("1,1,1,2");
                NormalizerPlugIn normalizerplugin = getNormalizerPlugin("1-4");
                memoryinputsynapse1.addPlugIn(normalizerplugin);
                memoryinputsynapse1.addPlugIn(getMovingAveragePlugIn("2,3", "10,50"));
                memoryinputsynapse1.setFirstRow(1);
                memoryinputsynapse1.setLastRow(i);
                memoryinputsynapse1.setInputArray(ad2);
                _input_layer.addInputSynapse(memoryinputsynapse1);
                interrogate();
                _data.remove(0);
            }
        }
    }

    private void createNet()
    {
        nnet = new NeuralNet();
        _input_layer = new DelayLayer();
        _input_layer.setTaps(_temporal_window - 1);
        _input_layer.setRows(4);
        nnet.addLayer(_input_layer, 0);
        Object obj = _input_layer;
        for(int i = 0; i < _rows.length; i++)
        {
            SigmoidLayer sigmoidlayer = new SigmoidLayer();
            sigmoidlayer.setRows(_rows[i]);
            nnet.addLayer(sigmoidlayer, 1);
            connect(((Layer) (obj)), new FullSynapse(), sigmoidlayer);
            obj = sigmoidlayer;
        }

        output = new SigmoidLayer();
        output.setRows(1);
        connect(((Layer) (obj)), new FullSynapse(), output);
        nnet.addLayer(output, 2);
        trainer = new TeachingSynapse();
        output.addOutputSynapse(trainer);
        setAnnealingPlugin();
    }

    private void connect(Layer layer, Synapse synapse, Layer layer1)
    {
        layer.addOutputSynapse(synapse);
        layer1.addInputSynapse(synapse);
    }

    private void train()
    {
        Monitor monitor = nnet.getMonitor();
        monitor.setLearningRate(0.69999999999999996D);
        monitor.setMomentum(0.59999999999999998D);
        monitor.setTrainingPatterns(_training_rows);
        monitor.setTotCicles(_epochs);
        monitor.setPreLearning(_temporal_window);
        monitor.setLearning(true);
        monitor.addNeuralNetListener(this);
        nnet.start();
        monitor.Go();
        nnet.join();
    }

    public void cicleTerminated(NeuralNetEvent neuralnetevent)
    {
        Monitor monitor = (Monitor)neuralnetevent.getSource();
        int i = monitor.getTotCicles() - monitor.getCurrentCicle();
        if(i > 0 && i % 100 == 0)
            System.out.println("Epoch:" + i + " RMSE=" + monitor.getGlobalError());
    }

    public void errorChanged(NeuralNetEvent neuralnetevent)
    {
    }

    public void netStarted(NeuralNetEvent neuralnetevent)
    {
    }

    public void netStopped(NeuralNetEvent neuralnetevent)
    {
        Monitor monitor = (Monitor)neuralnetevent.getSource();
        if(monitor.isLearning())
        {
            int i = monitor.getTotCicles() - monitor.getCurrentCicle();
            trace.info("Epoch:" + i + " last RMSE=" + monitor.getGlobalError());
        }
    }

    public void netStoppedError(NeuralNetEvent neuralnetevent, String s)
    {
        trace.error("Error occurred");
    }

    private void interrogate()
    {
        Monitor monitor = nnet.getMonitor();
        output.removeAllOutputs();
        MemoryOutputSynapse memoryoutputsynapse = new MemoryOutputSynapse();
        output.addOutputSynapse(memoryoutputsynapse);
        monitor.setTotCicles(1);
        monitor.setLearning(false);
        nnet.start();
        monitor.Go();
        nnet.join();
        double ad[] = output.getLastOutputs();
        if(ad != null && ad.length > 0)
            if(ad[0] < 0.10000000000000001D)
                _direction = 1;
            else
            if(ad[0] > 0.90000000000000002D)
                _direction = -1;
            else
                _direction = 0;
    }

    public static String toString(int ai[], int i, int j, float f)
    {
        return "Joone " + ai.length + " " + i + " " + j + " " + f;
    }

    private NormalizerPlugIn getNormalizerPlugin(String s)
    {
        NormalizerPlugIn normalizerplugin = new NormalizerPlugIn();
        normalizerplugin.setAdvancedSerieSelector(s);
        normalizerplugin.setMax(1.0D);
        normalizerplugin.setMin(0.0D);
        normalizerplugin.setName("Normalizer");
        return normalizerplugin;
    }

    private MovingAveragePlugIn getMovingAveragePlugIn(String s, String s1)
    {
        MovingAveragePlugIn movingaverageplugin = new MovingAveragePlugIn();
        movingaverageplugin.setAdvancedSerieSelector(s);
        movingaverageplugin.setAdvancedMovAvgSpec(s1);
        movingaverageplugin.setName("MovingAverage");
        return movingaverageplugin;
    }

    private MinMaxExtractorPlugIn getMinMaxPlugIn(String s, float f)
    {
        MinMaxExtractorPlugIn minmaxextractorplugin = new MinMaxExtractorPlugIn();
        minmaxextractorplugin.setAdvancedSerieSelector(s);
        minmaxextractorplugin.setMinChangePercentage(f);
        minmaxextractorplugin.setName("MinMax");
        return minmaxextractorplugin;
    }

    private MemoryInputSynapse getMemoryInputSynapse(String s)
    {
        MemoryInputSynapse memoryinputsynapse = new MemoryInputSynapse();
        memoryinputsynapse.setAdvancedColumnSelector(s);
        return memoryinputsynapse;
    }

    private void setAnnealingPlugin()
    {
        DynamicAnnealing dynamicannealing = new DynamicAnnealing();
        dynamicannealing.setRate(5);
        dynamicannealing.setStep(15D);
        dynamicannealing.setNeuralNet(nnet);
    }

    private static final Logger trace;
    private List _data;
    private int _training_rows;
    private int _forecast_rows;
    private int _epochs;
    private int _temporal_window;
    private DelayLayer _input_layer;
    private int _rows[] = {
        20
    };
    private SigmoidLayer output;
    private TeachingSynapse trainer;
    private NeuralNet nnet;
    private float _percentage_change;

    static 
    {
        trace = Logger.getLogger(robotrader.machinelearning.JooneNeuralNetwork.class);
    }
}
