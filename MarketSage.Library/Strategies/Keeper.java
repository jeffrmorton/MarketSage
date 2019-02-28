// Decompiled by Jad v1.5.8e2. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://kpdus.tripod.com/jad.html
// Decompiler options: packimports(3) 

package robotrader.trader.example;

import robotrader.trader.AbstractTrader;

public class Keeper extends AbstractTrader
{

    public Keeper()
    {
    }

    public String getName()
    {
        return "Keeper";
    }

    public void init()
    {
    }

    public void update()
    {
        float f = getCash();
        if(f > 0.0F)
            addAmountOrder("", f);
    }

    public String toString()
    {
        return "Keeper";
    }
}
