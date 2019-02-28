// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.trader.AbstractTrader;
import robotrader.trader.PropertyMetaData;

public class CautiousStreakFollower extends AbstractTrader
{

    public CautiousStreakFollower()
    {
        _maxrisk = 0.5F;
        _streaksize = 3;
        _direction = 0;
        _streakcount = 0;
    }

    public PropertyMetaData[] getPropertyMetaData()
    {
        return METADATA;
    }

    public void setMaxRisk(float f)
    {
        _maxrisk = f;
    }

    public String getName()
    {
        return "CautiousStreakFollower" + _streaksize + "r" + _maxrisk;
    }

    public void setStreak(int i)
    {
        _streaksize = i;
    }

    public void update()
    {
        String s = "";
        float f = getPrice(s);
        if(_previous > 0.0F)
        {
            float f1 = getCash();
            float f2 = getPosition(s);
            int i = getDirection(_previous, f);
            if(i == _direction)
            {
                _streakcount++;
                if(_streakcount >= _streaksize)
                {
                    _streakcount = 0;
                    if(_direction < 0)
                    {
                        if(f2 > 0.0F)
                            addQuantityOrder(s, -f2);
                    } else
                    if(_direction > 0 && f1 > 0.0F)
                    {
                        float f3 = f1 * _maxrisk;
                        addAmountOrder(s, f3);
                    }
                }
            } else
            {
                _streakcount = 0;
            }
            _direction = i;
        }
        _previous = f;
    }

    private int getDirection(float f, float f1)
    {
        if(f1 > f)
            return 1;
        return f1 >= f ? 0 : -1;
    }

    public void init()
    {
    }

    public void setProperty(String s, String s1)
    {
        if(s.equals("MAXRISK"))
            _maxrisk = Float.parseFloat(s1);
        else
        if(s.equals("STREAKSIZE"))
            _streaksize = Integer.parseInt(s1);
    }

    public String getProperty(String s)
    {
        if(s.equals("MAXRISK"))
            return Float.toString(_maxrisk);
        if(s.equals("STREAKSIZE"))
            return Integer.toString(_streaksize);
        else
            return "1";
    }

    public String toString()
    {
        return "CautiosStreakFollower";
    }

    private float _maxrisk;
    private int _streaksize;
    private float _previous;
    private int _direction;
    private int _streakcount;
    private static final PropertyMetaData METADATA[] = {
        new PropertyMetaData("MAXRISK", 1), new PropertyMetaData("STREAKSIZE", 0)
    };

}
