using System.Collections.Generic;
using System.Linq;

public class LowPassFilter
{
    private int averageCounter = 20;

    private Queue<float> queue = new();
    private float initialValue;

    public LowPassFilter(int averageCounter = 20, float initialValue = 0)
    {
        this.averageCounter = averageCounter;
        this.initialValue = initialValue;
    }


    public float Append(float v)
    {

        queue.Enqueue(v);
        
        if (!IsSufficient())
        {
            return initialValue;
        }

        var avg = queue.Sum() / queue.Count;
        queue.Dequeue();
        
        return avg;
    }

    private bool IsSufficient()
    {
        return queue.Count > averageCounter;
    }

}
