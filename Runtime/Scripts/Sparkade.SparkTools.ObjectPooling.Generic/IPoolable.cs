namespace Sparkade.SparkTools.ObjectPooling.Generic
{
    /// <summary>
    /// Provides several callbacks related to being an object in a pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Returns the object back to the object pool it belongs to.
        /// </summary>
        void Repool();

        /// <summary>
        /// Called when the object is pulled from a pool.
        /// </summary>
        /// <param name="pool">The pool the object was pulled from.</param>
        void OnPull();

        /// <summary>
        /// Called when the object is pushed to a pool.
        /// </summary>
        /// <param name="pool">The pool the object was pushed to.</param>
        void OnPush();
    }
}