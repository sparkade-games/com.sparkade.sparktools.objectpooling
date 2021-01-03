namespace Sparkade.SparkTools.ObjectPooling
{
    using Sparkade.SparkTools.ObjectPooling.Internal;
    using Sparkade.SparkTools.Singletons;
    using UnityEngine;

    /// <summary>
    /// Manages multiple GameObject pools, automatically generating them as needed. Pools exist for the life of the entire game.
    /// </summary>
    public class GlobalObjectPoolManager : Singleton<GlobalObjectPoolManager>, IPoolManager
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
        public void CreatePool(GameObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            this.PoolManager.CreatePool(prefab, size, accessMode, loadingMode);
        }

        /// <inheritdoc/>
        public void CreatePool(MonoBehaviour prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            this.PoolManager.CreatePool(prefab, size, accessMode, loadingMode);
        }

        /// <inheritdoc/>
        public GameObjectPool GetPool(GameObject prefab)
        {
            return this.PoolManager.GetPool(prefab);
        }

        /// <inheritdoc/>
        public GameObjectPool GetPool(MonoBehaviour prefab)
        {
            return this.PoolManager.GetPool(prefab);
        }

        /// <inheritdoc/>
        public GameObject Pull(GameObject prefab)
        {
            return this.PoolManager.Pull(prefab);
        }

        /// <inheritdoc/>
        public T Pull<T>(T prefab)
            where T : MonoBehaviour
        {
            return this.PoolManager.Pull(prefab);
        }

        /// <inheritdoc/>
        public void Push(GameObject prefab, GameObject item)
        {
            this.PoolManager.Push(prefab, item);
        }

        /// <inheritdoc/>
        public void Push(MonoBehaviour prefab, MonoBehaviour item)
        {
            this.PoolManager.Push(prefab, item);
        }

        /// <inheritdoc/>
        public bool HasPool(GameObject prefab)
        {
            return this.PoolManager.HasPool(prefab);
        }

        /// <inheritdoc/>
        public bool HasPool(MonoBehaviour prefab)
        {
            return this.PoolManager.HasPool(prefab);
        }

        /// <inheritdoc/>
        public int GetCount(GameObject prefab)
        {
            return this.PoolManager.GetCount(prefab);
        }

        /// <inheritdoc/>
        public int GetCount(MonoBehaviour prefab)
        {
            return this.PoolManager.GetCount(prefab);
        }

        /// <inheritdoc/>
        public int GetFreeCount(GameObject prefab)
        {
            return this.PoolManager.GetFreeCount(prefab);
        }

        /// <inheritdoc/>
        public int GetFreeCount(MonoBehaviour prefab)
        {
            return this.PoolManager.GetFreeCount(prefab);
        }

        /// <inheritdoc/>
        public void ClearPool(GameObject prefab)
        {
            this.PoolManager.ClearPool(prefab);
        }

        /// <inheritdoc/>
        public void ClearPool(MonoBehaviour prefab)
        {
            this.PoolManager.ClearPool(prefab);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.PoolManager.Clear();
        }
    }
}