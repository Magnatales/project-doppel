using System.Collections.Generic;

namespace Code.Blackboard;

public class Blackboard : IBlackBoard
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();
    
    public void Set<T>(string key, T value) => _data[key] = value;
    
    public T Get<T>(string key)
    {
        if (!_data.TryGetValue(key, out var value)) 
        {
            return default(T);
        }
        return (T)value;
    }

    public bool Contains(string key) => _data.ContainsKey(key);
    
    public void Remove(string key) => _data.Remove(key);
}