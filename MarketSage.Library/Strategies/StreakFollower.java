// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.trader.AbstractTrader;
import robotrader.trader.PropertyMetaData;

public class StreakFollower extends AbstractTrader
{

    public StreakFollower()
    {
        _direction = 0;
        _streakcount = 0;
        _streakdownsize = 0;
        _streakupsize = 3;
    }

    public String getName()
    {
        return "StreakFollowerUp" + _streakupsize + "Down" + _streakdownsize;
    }

    public void setProperty(String s, String s1)
    {
        if(s.equals("STREAKUPSIZE"))
            _streakupsize = Integer.parseInt(s1);
        else
        if(s.equals("STREAKDOWNSIZE"))
            _streakdownsize = Integer.parseInt(s1);
    }

    public String getProperty(String s)
    {
        if(s.equals("STREAKUPSIZE"))
            return Integer.toString(_streakupsize);
        if(s.equals("STREAKDOWNSIZE"))
            return Integer.toString(_streakdownsize);
        else
            return "0";
    }

    public PropertyMetaData[] getPropertyMetaData()
    {
        return METADATA;
    }

    public void setStreakDown(int i)
    {
        _streakdownsize = i;
    }

    public void setStreakUp(int i)
    {
        _streakupsize = i;
    }

    public void init()
    {
    }

    public void update()
    {
        String s = "";
        double d = getPrice(s);
        if(_previous > 0.0D)
        {
            float f = getCash();
            float f1 = getPosition(s);
            int i = getDirection(_previous, d);
            if(i == _direction)
            {
                _streakcount++;
                if(_direction < 0 && _streakcount >= _streakdownsize)
                {
                    if(f1 > 0.0F)
                        addQuantityOrder(s, -f1);
                } else
                if(_direction > 0 && _streakcount >= _streakupsize && f > 0.0F)
                    addAmountOrder(s, f);
            } else
            {
                _streakcount = 0;
            }
            _direction = i;
        }
        _previous = d;
    }

    private int getDirection(double d, double d1)
    {
        if(d1 > d)
            return 1;
        return d1 >= d ? 0 : -1;
    }

    public String toString()
    {
        return "StreakFollower";
    }

    private static final PropertyMetaData METADATA[] = {
        new PropertyMetaData("STREAKUPSIZE", 0), new PropertyMetaData("STREAKDOWNSIZE", 0)
    };
    private double _previous;
    private int _direction;
    private int _streakcount;
    private int _streakdownsize;
    private int _streakupsize;

}
