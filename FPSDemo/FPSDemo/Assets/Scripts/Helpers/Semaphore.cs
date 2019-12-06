using System.Collections.Generic;
namespace FYP
{
    /// <summary>
    /// The semaphore returns the value true if no object has it set and false otherwise
    /// </summary>
    public class Semaphore
    {
        private HashSet<object> objects = new HashSet<object>();
        /// <summary>
        /// Makes the semaphore set by the object passed.
        /// </summary>
        /// <param name="setter">The object which sets the semaphore</param>
        public void Set(object setter)
        {
            if (!objects.Contains(setter))
            {
                objects.Add(setter);
            }
        }
        /// <summary>
        /// Releases the semaphore for the object. If no other object has it set then the value of semaphore is true
        /// </summary>
        /// <param name="releaser">The object which will release the semaphore</param>
        public void Reset(object releaser)
        {
            if (objects.Contains(releaser))
            {
                objects.Remove(releaser);
            }
        }
        public void Clear()
        {
            objects.Clear();
        }
        public static implicit operator bool(Semaphore l)
        {
            return l.objects.Count == 0;
        }
    }
}