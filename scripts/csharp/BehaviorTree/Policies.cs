namespace Code.BehaviorTree;


public static class Policies 
{
    public interface IPolicy 
    {
        bool ShouldReturn(TreeNode.Status status);
    }
    
    public static readonly IPolicy RunForever = new RunForeverPolicy();
    public static readonly IPolicy RunUntilSuccess = new RunUntilSuccessPolicy();
    public static readonly IPolicy RunUntilFailure = new RunUntilFailurePolicy();

    private class RunForeverPolicy : IPolicy 
    {
        public bool ShouldReturn(TreeNode.Status status) => false;
    }

    private class RunUntilSuccessPolicy : IPolicy 
    {
        public bool ShouldReturn(TreeNode.Status status) => status == TreeNode.Status.Success;
    }

    private class RunUntilFailurePolicy : IPolicy 
    { 
        public bool ShouldReturn(TreeNode.Status status) => status == TreeNode.Status.Failure;
    }
}