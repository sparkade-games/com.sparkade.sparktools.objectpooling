namespace Sparkade.SparkTools.ObjectPooling.Generic
{
    /// <summary>
    /// Determines the order in which a pool pulls objects from its store.
    /// </summary>
    public enum PoolAccessMode
    {
        /// <summary>
        /// Use the oldest object in the pool.
        /// </summary>
        FirstIn,

        /// <summary>
        /// Use the newest object in the pool.
        /// </summary>
        LastIn,
    }

    /// <summary>
    /// Determines how a pool reaches its size.
    /// </summary>
    public enum PoolLoadingMode
    {
        /// <summary>
        /// Objects are always created, rather than reused, with every pull until the the pool's size is reached.
        /// </summary>
        Lazy,

        /// <summary>
        /// The pool is given enough objects upon construction to reach its size.
        /// </summary>
        Eager,
    }

    /// <summary>
    /// Impliments a generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of object the pool stores.</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets how many objects the pool manages.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets how many objects are currently available to pull from the pool.
        /// </summary>
        int FreeCount { get; }

        /// <summary>
        /// Gets how many objects that are owned by the pool are currently in use.
        /// </summary>
        int InUseCount { get; }

        /// <summary>
        /// Gets the target size of the pool. The pool will expand beyond this size if needed.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the order in which a pool pulls objects from its store.
        /// </summary>
        PoolAccessMode AccessMode { get; }

        /// <summary>
        /// Gets how a pool reaches its size.
        /// </summary>
        PoolLoadingMode LoadingMode { get; }

        /// <summary>
        /// Returns an object from the pool. If no objects are currently free, one will be created.
        /// </summary>
        /// <returns>An object pulled from the pool.</returns>
        T Pull();

        /// <summary>
        /// Places an object back into the pool for later use. Only objects that were pulled from the pool may be pushed back into it.
        /// </summary>
        /// <param name="item">The object to pushed into the pool.</param>
        void Push(T item);
    }
}