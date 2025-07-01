using System;
using System.Collections.Generic;

public class DescendingDateComparer : IComparer<DateTime>
{
    public int Compare(DateTime x, DateTime y)
    {
        return y.CompareTo(x);
    }
}
