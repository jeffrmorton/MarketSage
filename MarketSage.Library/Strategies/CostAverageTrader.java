// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.engine.IAccount;
import robotrader.engine.IMarketEngine;
import robotrader.trader.AbstractTrader;
import robotrader.trader.PropertyMetaData;

public class CostAverageTrader extends AbstractTrader
{

    public CostAverageTrader()
    {
        _invest_period = 50;
        _invest_cash = 1.0F;
        _current_period = 0;
    }

    public String getName()
    {
        return "Cost Average " + _invest_period;
    }

    public void init()
    {
        int i = _market.getDataSize();
        float f = _account.getCash();
        int j = i / _invest_period;
        _invest_cash = f / (float)j;
    }

    public void update()
    {
        int i = ++_current_period % _invest_period;
        if(i == 0)
            addAmountOrder("", _invest_cash);
    }

    public void setProperty(String s, String s1)
    {
        if(s.equals("PERIOD"))
            _invest_period = Integer.parseInt(s1);
    }

    public String getProperty(String s)
    {
        if(s.equals("PERIOD"))
            return Integer.toString(_invest_period);
        else
            return "50";
    }

    public String toString()
    {
        return "Cost Average";
    }

    public PropertyMetaData[] getPropertyMetaData()
    {
        return METADATA;
    }

    private static final PropertyMetaData METADATA[] = {
        new PropertyMetaData("PERIOD", 0)
    };
    private int _invest_period;
    private float _invest_cash;
    private int _current_period;

}
