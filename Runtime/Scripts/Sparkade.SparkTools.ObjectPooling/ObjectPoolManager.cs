namespace Sparkade.SparkTools.ObjectPooling
{
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using Sparkade.SparkTools.ObjectPooling.Internal;
    using Sparkade.SparkTools.Singletons;
    using UnityEngine;

    /// <summary>
    /// Manages multiple GameObject pools, automatically generating them as needed. Pools exist for the life of the scene they were created in.
    /// </summary>
    public class ObjectPoolManager : SceneSingleton<ObjectPoolManager>, IPoolManager
    {
        private InternalPoolManager poolManager;

        /// <inheritdoc/>
        public GameObject PoolParent => this.gameObject;

        private InternalPoolManager PoolManager
        {
            get
            {
                if (this.poolManager == null)
                {
                    this.poolManager = new InternalPoolManager(this.PoolParent);
                }

                return this.poolManager;
            }
        }

        /// <inheritdoc/>
        public void CreatePool<T>(T prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.CreatePool(prefab, size, accessMode, loadingMode);
        }

        /// <inheritdoc/>
        public T Pull<T>(T prefab)
             where T : ObjectPoolItem<T>
        {
            return this.PoolManager.Pull(prefab);
        }

        /// <inheritdoc/>
        public void Push<T>(T prefab, T item)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.Push(prefab, item);
        }

        /// <inheritdoc/>
        public bool HasPool<T>(T prefab)
             where T : ObjectPoolItem<T>
        {
            return this.PoolManager.HasPool(prefab);
        }

        /// <inheritdoc/>
        public ObjectPool<T> GetPool<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            return this.PoolManager.GetPool(prefab);
        }

        /// <inheritdoc/>
        public int GetCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            return this.PoolManager.GetCount(prefab);
        }

        /// <inheritdoc/>
        public int GetFreeCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            return this.PoolManager.GetFreeCount(prefab);
        }

        /// <inheritdoc/>
        public int GetInUseCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            return this.PoolManager.GetInUseCount(prefab);
        }

        /// <inheritdoc/>
        public void ClearPool<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.ClearPool(prefab);
        }
    }
}