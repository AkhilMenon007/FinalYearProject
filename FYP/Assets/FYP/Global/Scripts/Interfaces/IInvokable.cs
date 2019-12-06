namespace FYP
{
    /// <summary>
    /// Classes implementing this can be assigned to as event listeners
    /// </summary>
    public interface IInvokable
    {
        void OnEventInvoked(object sender);
    }
}