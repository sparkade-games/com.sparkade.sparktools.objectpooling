namespace Sparkade.SparkTools.ObjectPooling.Internal
{
    using Sparkade.SparkTools.ObjectPooling;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;

    /// <summary>
    /// Impliments managing of multiple Unity object pools.
    /// </summary>
    internal interface IPoolManager
    {
        /// <summary>
        /// Gets the GameObject all managed object pools are parented to.
        /// </summary>
        public GameObject PoolParent { get; }

        /// <summary>
        /// Creates an ObjectPool.
        /// </summary>
        /// <param name="prefab">The ObjectPoolItem to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        public void CreatePool<T>(T prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Destroys an ObjectPool and all its managed objects.
        /// </summary>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <param name="prefab">The ObjectPoolItem to be copied when creating a new object for the pool.</param>
        public void DestroyPool<T>(T prefab)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Returns an object from a pool. If no pool exists, one will be created. If no objects are currently free, one will be created.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>ObjectPoolItem from the pool.</returns>
        public T Pull<T>(T prefab)
             where T : ObjectPoolItem<T>;

        /// <summary>
        /// Places an object back into a pool. If no pool exists, one will be created.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// /// <param name="item">The ObjectPoolItem to pushed into the pool.</param>
        public void Push<T>(T prefab, T item)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Checks whether or not a pool exists.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>True if the pool exists, otherwise false.</returns>
        public bool HasPool<T>(T prefab)
             where T : ObjectPoolItem<T>;

        /// <summary>
        /// Gets a reference to an object pool.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>The ObjectPool if one exists for the prefab, or null if it does not.</returns>
        public ObjectPooling.ObjectPool<T> GetPool<T>(T prefab)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Gets how many objects a pool manages.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>The amount of objects managed by the pool, or 0 if the pool does not exist..</returns>
        public int GetCount<T>(T prefab)
           where T : ObjectPoolItem<T>;

        /// <summary>
        /// Gets how many objects are currently available to pull from a pool.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>The amount of objects free in the pool, or 0 if the pool does not exist.</returns>
        public int GetFreeCount<T>(T prefab)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Gets how many objects are currently in use in a pool.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        /// <returns>The amount of objects in use in the pool, or 0 if the pool does not exist.</returns>
        public int GetInUseCount<T>(T prefab)
            where T : ObjectPoolItem<T>;

        /// <summary>
        /// Destroys all objects in a pool.
        /// </summary>
        /// <param name="prefab">ObjectPoolItem prefab associated with the pool.</param>
        /// <typeparam name="T">Type of ObjectPoolItem.</typeparam>
        public void ClearPool<T>(T prefab)
            where T : ObjectPoolItem<T>;
    }
}