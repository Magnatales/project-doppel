using System.Collections.Generic;

namespace Code.BehaviorTree;

public class TreeNode (string name, int priority = 0)
{
	public enum Status { Success, Failure, Running }
	
	public readonly string Name = name;
	public readonly int Priority = priority;
        
	public readonly List<TreeNode> Children = new();
	protected int CurrentChild;
        
	public void AddChild(TreeNode child) => Children.Add(child);
        
	public virtual Status Process(float delta) => Children[CurrentChild].Process(delta);

	public virtual void Reset() 
	{
		CurrentChild = 0;
		foreach (var child in Children) 
		{
			child.Reset();
		}
	}
}
