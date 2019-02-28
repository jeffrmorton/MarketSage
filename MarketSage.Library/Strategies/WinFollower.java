// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.trader.AbstractTrader;
import robotrader.trader.PropertyMetaData;

public class WinFollower extends AbstractTrader
{

    public WinFollower()
    {
        _maxloss = 0.05F;
        _maxwin = 0.1F;
        _value = 0.0F;
        _max_value = 0.0F;
    }

    public void setMaxLoss(float f)
    {
        if(Math.abs(f) < 1.0F)
            if(f > 0.0F)
                _maxloss = -f;
            else
            if(f < 0.0F)
                _maxloss = f;
    }

    public void setMaxWin(float f)
    {
        if(f > 0.0F)
            _maxwin = f;
    }

    public String getName()
    {
        return "WinFollowerW" + _maxwin + "L" + _maxloss;
    }

    public void setProperty(String s, String s1)
    {
        if(s.equals("MAXWIN"))
            setMaxWin(Float.parseFloat(s1));
        else
        if(s.equals("MAXLOSS"))
            setMaxLoss(Float.parseFloat(s1));
    }

    public String getProperty(String s)
    {
        if(s.equals("MAXWIN"))
            return Float.toString(_maxwin);
        if(s.equals("MAXLOSS"))
            return Float.toString(_maxloss);
        else
            return "0";
    }

    public PropertyMetaData[] getPropertyMetaData()
    {
        return METADATA;
    }

    public void init()
    {
    }

    public void update()
    {
        String s = "";
        float f = getPrice(s);
        float f1 = getCash();
        float f2 = getPosition(s);
        float f3 = getPrice(s);
        if(f2 > 0.0F)
        {
            float f4 = f2 * f3;
            float f5 = (f4 - _value) / _value;
            float f6 = (f4 - _max_value) / _max_value;
            if(f5 >= _maxwin)
                addQuantityOrder(s, -f2);
            else
            if(f6 <= _maxloss)
                addQuantityOrder(s, -f2);
            if(f4 > _value)
                _max_value = f4;
        } else
        if(_previous > 0.0F && _previous < f && f1 > 0.0F)
        {
            addAmountOrder(s, f1);
            _value = f1;
            _max_value = f1;
        }
        _previous = f;
    }

    public String toString()
    {
        return "WinFollower";
    }

    private static final PropertyMetaData METADATA[] = {
        new PropertyMetaData("MAXWIN", 1), new PropertyMetaData("MAXLOSS", 1)
    };
    private float _maxloss;
    private float _maxwin;
    private float _previous;
    private float _value;
    private float _max_value;

}
