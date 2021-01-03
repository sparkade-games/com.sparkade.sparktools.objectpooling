namespace Sparkade.SparkTools.ObjectPooling.Internal
{
    using UnityEngine;

    /// <summary>
    /// Impliments managing of multiple GameObject pools.
    /// </summary>
    internal interface IPoolManager
    {
        /// <summary>
        /// Gets the GameObject all managed object pools are parented to.
        /// </summary>
        public GameObject PoolParent { get; }

        /// <summary>
        /// Creates a GameObject pool.
        /// </summary>
        /// <param name="prefab">The GameObject to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public void CreatePool(GameObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager);

        /// <summary>
        /// Creates a GameObject pool.
        /// </summary>
        /// <param name="prefab">The MonoBehaviour to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public void CreatePool(MonoBehaviour prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager);

        /// <summary>
        /// Gets a reference to an object pool.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// <returns>The GameObjectPool if one exists for the prefab, or null if it does not.</returns>
        public GameObjectPool GetPool(GameObject prefab);

        /// <summary>
        /// Gets a reference to an object pool.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// <returns>The GameObjectPool if one exists for the prefab, or null if it does not.</returns>
        public GameObjectPool GetPool(MonoBehaviour prefab);

        /// <summary>
        /// Returns an object from a pool. If no pool exists, one will be created. If no objects are currently free, one will be created.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// <returns>GameObject from the pool.</returns>
        public GameObject Pull(GameObject prefab);

        /// <summary>
        /// Returns an object from a pool. If no pool exists, one will be created. If no objects are currently free, one will be created.
        /// </summary>
        /// <typeparam name="T">MonoBehaviour type.</typeparam>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// <returns>MonoBehaviour from the pool.</returns>
        public T Pull<T>(T prefab)
            where T : MonoBehaviour;

        /// <summary>
        /// Places an object back into a pool. If no pool exists, one will be created.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// /// <param name="item">The GameObject to pushed into the pool.</param>
        public void Push(GameObject prefab, GameObject item);

        /// <summary>
        /// Places an object back into a pool. If no pool exists, one will be created.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// /// <param name="item">The MonoBehaviour to pushed into the pool.</param>
        public void Push(MonoBehaviour prefab, MonoBehaviour item);

        /// <summary>
        /// Checks whether or not a pool exists.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// <returns>True if the pool exists, otherwise false.</returns>
        public bool HasPool(GameObject prefab);

        /// <summary>
        /// Checks whether or not a pool exists.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// <returns>True if the pool exists, otherwise false.</returns>
        public bool HasPool(MonoBehaviour prefab);

        /// <summary>
        /// Gets how many objects a pool manages.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// <returns>The amount of objects managed by the pool, or 0 if the pool does not exist..</returns>
        public int GetCount(GameObject prefab);

        /// <summary>
        /// Gets how many objects a pool manages.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// <returns>The amount of objects managed by the pool, or 0 if the pool does not exist..</returns>
        public int GetCount(MonoBehaviour prefab);

        /// <summary>
        /// Gets how many objects are currently available to pull from a pool.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        /// <returns>The amount of objects free in the pool, or 0 if the pool does not exist.</returns>
        public int GetFreeCount(GameObject prefab);

        /// <summary>
        /// Gets how many objects are currently available to pull from a pool.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        /// <returns>The amount of objects free in the pool, or 0 if the pool does not exist.</returns>
        public int GetFreeCount(MonoBehaviour prefab);

        /// <summary>
        /// Destroys all objects in a pool.
        /// </summary>
        /// <param name="prefab">GameObject prefab associated with the pool.</param>
        public void ClearPool(GameObject prefab);

        /// <summary>
        /// Destroys all objects in a pool.
        /// </summary>
        /// <param name="prefab">MonoBehaviour prefab associated with the pool.</param>
        public void ClearPool(MonoBehaviour prefab);

        /// <summary>
        /// Destroys all pools and objects within them.
        /// </summary>
        public void Clear();
    }
}