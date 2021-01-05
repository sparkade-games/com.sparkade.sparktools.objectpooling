namespace Sparkade.SparkTools.ObjectPooling
{
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using Sparkade.SparkTools.ObjectPooling.Internal;
    using Sparkade.SparkTools.Singletons;
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

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
        public void CreatePool<T>(T prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.CreatePool(prefab, size, accessMode, loadingMode);
        }

        /// <inheritdoc/>
        public bool DestroyPool<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            return this.PoolManager.DestroyPool(prefab);
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
        public void RecallScene<T>(T prefab, Scene scene)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.RecallScene(prefab, scene);
        }

        /// <inheritdoc/>
        public void RecallScene(Scene scene)
        {
            this.PoolManager.RecallScene(scene);
        }

        /// <inheritdoc/>
        public void RecallAll<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.RecallAll(prefab);
        }

        /// <inheritdoc/>
        public void RecallAll()
        {
            this.PoolManager.RecallAll();
        }

        /// <inheritdoc/>
        public void Clear<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            this.PoolManager.Clear(prefab);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.PoolManager.Clear();
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
            {
                return;
            }
#endif

            this.Clear();
        }
    }
}