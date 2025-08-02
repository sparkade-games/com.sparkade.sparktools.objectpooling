namespace Sparkade.SparkTools.ObjectPooling.Generic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Determines the order in which a pool pulls items from its store.
    /// </summary>
    public enum PoolAccessMode
    {
        /// <summary>
        /// Use the oldest item in the pool.
        /// </summary>
        FirstIn,

        /// <summary>
        /// Use the newest item in the pool.
        /// </summary>
        LastIn,
    }

    /// <summary>
    /// Determines how a pool reaches its size.
    /// </summary>
    public enum PoolLoadingMode
    {
        /// <summary>
        /// Items are always created, rather than reused, with every pull until the the pool's size is reached.
        /// </summary>
        Lazy,

        /// <summary>
        /// The pool is given enough items upon construction to reach its size.
        /// </summary>
        Eager,
    }

    /// <summary>
    /// Impliments a generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of item the pool stores.</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets or sets a callback for when an item is pulled.
        /// </summary>
        Action<T> Pulled { get; set; }

        /// <summary>
        /// Gets or sets a callback for when an item is pushed.
        /// </summary>
        Action<T> Pushed { get; set; }

        /// <summary>
        /// Gets the target size of the pool. The pool will expand beyond this size if needed.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the order in which a pool pulls items from its store.
        /// </summary>
        PoolAccessMode AccessMode { get; }

        /// <summary>
        /// Gets how a pool reaches its size.
        /// </summary>
        PoolLoadingMode LoadingMode { get; }

        /// <summary>
        /// Gets how many items the pool manages.
        /// </summary>
        int CountAll { get; }

        /// <summary>
        /// Gets how many items are currently available to pull from the pool.
        /// </summary>
        int CountInactive { get; }

        /// <summary>
        /// Gets how many items owned by the pool that are currently in use.
        /// </summary>
        int CountActive { get; }

        /// <summary>
        /// Returns an item from the pool. If no items are currently inactive, one will be created.
        /// </summary>
        /// <returns>An item pulled from the pool.</returns>
        T Pull();

        /// <summary>
        /// Places an item back into the pool for later use. Only items that were pulled from the pool may be pushed back into it.
        /// </summary>
        /// <param name="item">The item to push into the pool.</param>
        void Push(T item);

        /// <summary>
        /// Gets whether or not an item is owned by this pool.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item belongs to this pool, false if it does not.</returns>
        bool GetOwnsItem(T item);

        /// <summary>
        /// Gets all items owned by the pool.
        /// </summary>
        /// <returns>A collection of all items owned by the pool.</returns>
        IEnumerable<T> GetAllItems();

        /// <summary>
        /// Gets all items currently stored in the pool.
        /// </summary>
        /// <returns>A collection of all items currently stored in the pool.</returns>
        IEnumerable<T> GetInactiveItems();

        /// <summary>
        /// Gets all items owned by the pool that are not currently stored in it.
        /// </summary>
        /// <returns>A collection of all items owned by the pool that are not currently stored in it.</returns>
        IEnumerable<T> GetActiveItems();

        /// <summary>
        /// Removes all items from the pool.
        /// </summary>
        void Clear();
    }
}