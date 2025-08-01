namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using System.Collections.Generic;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using Sparkade.SparkTools.Singletons;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages multiple GameObject pools, automatically generating them as needed. Pools exist for the life of the scene they were created in.
    /// </summary>
    public class SceneObjectPoolManager : SceneSingleton<SceneObjectPoolManager>, IPoolManager
    {
        private Dictionary<PoolableObject, ObjectPool> objectPools = new Dictionary<PoolableObject, ObjectPool>();

        /// <inheritdoc/>
        public void CreatePool(PoolableObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (this.HasPool(prefab))
            {
                throw new InvalidOperationException("Pool already exists.");
            }

            GameObject poolGameObject = new GameObject(prefab.name);
            poolGameObject.transform.SetParent(this.transform);
            ObjectPool pool = poolGameObject.AddComponent<ObjectPool>();
            pool.Init(prefab);
            this.objectPools[prefab] = pool;
        }

        /// <inheritdoc/>
        public bool DestroyPool(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return false;
            }

            this.objectPools[prefab].Clear();
            this.objectPools.Remove(prefab);
            return true;
        }

        /// <inheritdoc/>
        public ObjectPool GetPool(PoolableObject prefab)
        {
            if (!this.HasPool(prefab))
            {
                return null;
            }

            return this.objectPools[prefab];
        }

        /// <inheritdoc/>
        public bool HasPool(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            return this.objectPools.ContainsKey(prefab) && this.objectPools[prefab] != null;
        }

        /// <inheritdoc/>
        public IEnumerable<ObjectPool> GetObjectPools()
        {
            foreach (KeyValuePair<PoolableObject, ObjectPool> entry in this.objectPools)
            {
                if (entry.Value == null)
                {
                    this.objectPools.Remove(entry.Key);
                }
            }

            return this.objectPools.Values;
        }

        /// <inheritdoc/>
        public T Pull<T>(T prefab)
            where T : PoolableObject
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                this.CreatePool(prefab);
            }

            return this.objectPools[prefab].Pull<T>();
        }

        /// <inheritdoc/>
        public void Push(PoolableObject prefab, PoolableObject item)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (!this.HasPool(prefab))
            {
                this.CreatePool(prefab);
            }

            this.objectPools[prefab].Push(item);
        }

        /// <inheritdoc/>
        public void Prune(PoolableObject prefab, PoolableObject item)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.objectPools[prefab].Prune(item);
        }

        /// <inheritdoc/>
        public int GetCountAll(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.objectPools[prefab].CountAll;
        }

        /// <inheritdoc/>
        public int GetCountInactive(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.objectPools[prefab].CountInactive;
        }

        /// <inheritdoc/>
        public int GetCountActive(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.objectPools[prefab].CountActive;
        }

        /// <inheritdoc/>
        public void Clear(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.objectPools[prefab].Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (ObjectPool pool in this.objectPools.Values)
            {
                pool.Clear();
            }
        }

        /// <inheritdoc/>
        public void RecallScene(PoolableObject prefab, Scene scene)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.objectPools[prefab].RecallScene(scene);
        }

        /// <inheritdoc/>
        public void RecallScene(Scene scene)
        {
            foreach (KeyValuePair<PoolableObject, ObjectPool> entry in this.objectPools)
            {
                if (entry.Value == null)
                {
                    this.objectPools.Remove(entry.Key);
                    continue;
                }

                entry.Value.RecallScene(scene);
            }
        }

        /// <inheritdoc/>
        public void RecallAll(PoolableObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.objectPools[prefab].RecallAll();
        }

        /// <inheritdoc/>
        public void RecallAll()
        {
            foreach (KeyValuePair<PoolableObject, ObjectPool> entry in this.objectPools)
            {
                if (entry.Value == null)
                {
                    this.objectPools.Remove(entry.Key);
                    continue;
                }

                entry.Value.RecallAll();
            }
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
#endif

            this.Clear();
        }
    }
}