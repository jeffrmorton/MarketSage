// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import java.util.ArrayList;
import robotrader.trader.AbstractTrader;
import robotrader.trader.PropertyMetaData;

public class TurningPointTrader extends AbstractTrader
{

    public TurningPointTrader()
    {
        _data = new ArrayList();
        _maxloss = 0.1F;
        _minwin = 0.05F;
        _maxwin = 0.25F;
        _period = 50;
        _previous = -1D;
    }

    private void setMaxLoss(float f)
    {
        if(Math.abs(f) < 1.0F)
            _maxloss = Math.abs(f);
    }

    private void setMinWin(float f)
    {
        if(Math.abs(f) < 1.0F && f < _maxwin)
            _minwin = Math.abs(f);
    }

    private void setMaxWin(float f)
    {
        if(Math.abs(f) < 1.0F && f > _minwin)
            _maxwin = Math.abs(f);
    }

    public String getName()
    {
        return "TurningPoint (" + _period + "," + _minwin + "," + _maxwin + "," + _maxloss + ")";
    }

    public void setProperty(String s, String s1)
    {
        if(s.equals("MINWIN"))
            setMinWin(Float.parseFloat(s1));
        else
        if(s.equals("MAXLOSS"))
            setMaxLoss(Float.parseFloat(s1));
        else
        if(s.equals("PERIOD"))
            _period = Integer.parseInt(s1);
        else
        if(s.equals("MAXWIN"))
            setMaxWin(Float.parseFloat(s1));
    }

    public String getProperty(String s)
    {
        if(s.equals("MINWIN"))
            return Float.toString(_minwin);
        if(s.equals("MAXLOSS"))
            return Float.toString(_maxloss);
        if(s.equals("PERIOD"))
            return Integer.toString(_period);
        if(s.equals("MAXWIN"))
            return Float.toString(_maxwin);
        else
            return "1";
    }

    public void init()
    {
    }

    public void update()
    {
        String s = "";
        float f = getCash();
        float f1 = getPosition(s);
        float f2 = getPrice(s);
        _data.add(new Float(f2));
        if(_data.size() > _period)
        {
            _data.remove(0);
            float f3 = 3.402823E+038F;
            float f4 = 1.401298E-045F;
            for(int i = 0; i < _data.size(); i++)
            {
                Float float1 = (Float)_data.get(i);
                if(float1.floatValue() > f4)
                    f4 = float1.floatValue();
                if(float1.floatValue() < f3)
                    f3 = float1.floatValue();
            }

            float f5 = f3 + f3 * _minwin;
            float f6 = f4 - f4 * _maxloss;
            float f7 = f3 + f3 * _maxwin;
            if(f2 > f5 && f2 < f7 && f5 < f6 && (double)f2 > _previous && f > 0.0F)
                addAmountOrder(s, f);
            if(f2 < f6 && f5 > f6 && (double)f2 < _previous && f1 > 0.0F)
                addQuantityOrder(s, -f1);
            if(f2 > f7 && (double)f2 > _previous && f1 > 0.0F)
                addQuantityOrder(s, -f1);
            _data.remove(0);
        }
        _previous = f2;
    }

    public String toString()
    {
        return "TurningPointTrader";
    }

    public PropertyMetaData[] getPropertyMetaData()
    {
        return METADATA;
    }

    private static final PropertyMetaData METADATA[] = {
        new PropertyMetaData("MINWIN", 1), new PropertyMetaData("MAXLOSS", 1), new PropertyMetaData("MAXWIN", 1), new PropertyMetaData("PERIOD", 0)
    };
    private ArrayList _data;
    private float _maxloss;
    private float _minwin;
    private float _maxwin;
    private int _period;
    private double _previous;

}
