namespace Code.Blackboard;

public interface IBlackBoard
{
    T Get<T>(string key);
    void Set<T>(string key, T value);
}