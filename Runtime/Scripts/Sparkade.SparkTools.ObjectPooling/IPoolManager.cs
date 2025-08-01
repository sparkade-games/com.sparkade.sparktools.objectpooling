namespace Sparkade.SparkTools.ObjectPooling
{
    using System.Collections.Generic;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Impliments managing of multiple object pools.
    /// </summary>
    internal interface IPoolManager
    {
        /// <summary>
        /// Creates an object pool.
        /// </summary>
        /// <param name="prefab">The prefab to be copied when creating a new item for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls items from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        void CreatePool(PoolableObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager);

        /// <summary>
        /// Destroys an object pool and all its managed items.
        /// </summary>
        /// <param name="prefab">The prefab to be copied when creating a new item for the pool.</param>
        /// <returns>True if the pool was destroyed, false if it did not exist.</returns>
        bool DestroyPool(PoolableObject prefab);

        /// <summary>
        /// Gets a reference to an object pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>The ObjectPool if one exists for the prefab, or null if it does not.</returns>
        ObjectPool GetPool(PoolableObject prefab);

        /// <summary>
        /// Checks whether or not a pool exists.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>True if the pool exists, otherwise false.</returns>
        bool HasPool(PoolableObject prefab);

        /// <summary>
        /// Gets the object pools managed by this manager.
        /// </summary>
        /// <returns>A collection object pools.</returns>
        IEnumerable<ObjectPool> GetObjectPools();

        /// <summary>
        /// Returns an item from a pool. If no pool exists, one will be created. If no items are currently inactive, one will be created.
        /// </summary>
        /// <typeparam name="T">The type of item to pull.</typeparam>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>Item from the pool.</returns>
        T Pull<T>(T prefab)
            where T : PoolableObject;

        /// <summary>
        /// Places an item back into a pool. If no pool exists, one will be created.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <param name="item">The item to be pushed into the pool.</param>
        void Push(PoolableObject prefab, PoolableObject item);

        /// <summary>
        /// Removes an item from a pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <param name="item">The item to be removed from the pool.</param>
        void Prune(PoolableObject prefab, PoolableObject item);

        /// <summary>
        /// Gets how many items a pool manages.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>The amount of items managed by the pool, or 0 if the pool does not exist..</returns>
        int GetCountAll(PoolableObject prefab);

        /// <summary>
        /// Gets how many items are currently available to pull from a pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>The amount of items free in the pool, or 0 if the pool does not exist.</returns>
        int GetCountInactive(PoolableObject prefab);

        /// <summary>
        /// Gets how many items are currently in use in a pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <returns>The amount of items in use in the pool, or 0 if the pool does not exist.</returns>
        int GetCountActive(PoolableObject prefab);

        /// <summary>
        /// Destroys all items in a pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        void Clear(PoolableObject prefab);

        /// <summary>
        /// Destroys all items in every pool.
        /// </summary>
        void Clear();

        /// <summary>
        /// Pushes all active items in a specific scene back into their respective pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        /// <param name="scene">The scene active items that should be pushed are in.</param>
        void RecallScene(PoolableObject prefab, Scene scene);

        /// <summary>
        /// Pushes all active items in a specific scene back into their respective pools.
        /// </summary>
        /// <param name="scene">The scene active items that should be pushed are in.</param>
        void RecallScene(Scene scene);

        /// <summary>
        /// Pushes all active items back into their respective pool.
        /// </summary>
        /// <param name="prefab">The prefab associated with the pool.</param>
        void RecallAll(PoolableObject prefab);

        /// <summary>
        /// Pushes all active items back into their respective pools.
        /// </summary>
        void RecallAll();
    }
}