using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracker object that notifies upon garbage collected.
/// Primarily used for detecting memory leaks
/// </summary>
public class DebugTracker
{
    private static Dictionary<string, int> counts = new Dictionary<string, int>();
    private static int count;

    public static int Count { get { return count; } }
    public int ThisCount { get { return counts[Name]; } }

    public string Name { get; private set; }

    public DebugTracker(string name)
    {
        Debug.Log($"Tracker \"{name}\"created");
        Name = name;
        ++count;
        if (!counts.ContainsKey(Name))
        {
            counts[Name] = 0;
        }
        ++counts[Name];
    }

    ~DebugTracker()
    {
        --count;
        --counts[Name];
        Debug.Log($"[DebugTracker]: Deleted \"{Name}\", {ThisCount} left");
    }
}

