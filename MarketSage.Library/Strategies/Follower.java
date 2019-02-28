// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.trader.AbstractTrader;

public class Follower extends AbstractTrader
{

    public Follower()
    {
    }

    public String getName()
    {
        return "Follower";
    }

    public void update()
    {
        String s = "";
        double d = getPrice(s);
        if(_previous > 0.0D)
        {
            float f = getCash();
            float f1 = getPosition(s);
            if(_previous > d)
            {
                if(f1 > 0.0F)
                    addQuantityOrder(s, -f1);
            } else
            if(_previous < d && f > 0.0F)
                addAmountOrder(s, f);
        }
        _previous = d;
    }

    public void init()
    {
    }

    public String toString()
    {
        return "Follower";
    }

    private double _previous;
}
