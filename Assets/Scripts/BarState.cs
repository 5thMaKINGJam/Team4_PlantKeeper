
using UnityEngine;
using UnityEngine.UI;


public class BarState
{
    protected int value = 0;
    int maxValue;
    Image bar;


    public BarState(int initialValue, int maxValue, Image bar)
    {
        this.maxValue = maxValue;
        this.bar = bar;
        UpdateValue(initialValue);
    }


    public int GetValue()
    {
        return value;
    }

    public int UpdateValue(int dx)
    {
        value += dx;
        if (value < 0)
        {
            value = 0;
        }
        else if (value > maxValue)
        {
            value = maxValue;
        }
        bar.fillAmount = (float)value / maxValue;
        return value;
    }
}

public class BoundedBarState : BarState
{

    int minBound;
    int maxBound;

    public BoundedBarState(int initialValue, int maxValue, Image bar, int minBound, int maxBound) : base(initialValue, maxValue, bar)
    {
        this.minBound = minBound;
        this.maxBound = maxBound;
    }

    public bool isInBound()
    {
        return value >= minBound && value <= maxBound;
    }
}
