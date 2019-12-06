namespace FYP
{
    public interface IMotionAnimated
    {
        Semaphore moveAllowed { get; }
        float moveX { get; }
        float moveY { get; }
        float turning { get; }
    }
}