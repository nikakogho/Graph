using System.Collections.Generic;

public static class InsideMethods
{
    public static LogicManager.Function GetFunction(this Dictionary<string, LogicManager.Function> dictionary, string methodName)
    {
        bool contains = dictionary.ContainsKey(methodName);

        if (contains) return dictionary[methodName];

        return (x) => { return x; };
    }
}
