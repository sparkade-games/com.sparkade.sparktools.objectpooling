namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using System.Collections.Generic;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using Sparkade.SparkTools.Singletons;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages multiple GameObject pools, automatically generating them as needed. Pools exist for the life of the entire game.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>, IPoolManager
    {
        private Dictionary<PoolableObject, ObjectPool> objectPools = new Dictionary<PoolableObject, ObjectPool>();

        /// <inheritdoc/>
        public ObjectPool CreatePool(PoolableObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
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
            return pool;
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
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

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
            this.RemoveNullObjectPools();
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
                throw new InvalidOperationException("Item does not belong to any managed pool.");
            }

            this.objectPools[prefab].Push(item);
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
            this.RemoveNullObjectPools();
            foreach (ObjectPool objectPool in this.objectPools.Values)
            {
                objectPool.Clear();
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
            this.RemoveNullObjectPools();
            foreach (ObjectPool objectPool in this.objectPools.Values)
            {
                objectPool.RecallScene(scene);
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
            this.RemoveNullObjectPools();
            foreach (ObjectPool objectPool in this.objectPools.Values)
            {
                objectPool.RecallAll();
            }
        }

        private void RemoveNullObjectPools()
        {
            List<PoolableObject> keysToRemove = new List<PoolableObject>();
            foreach (KeyValuePair<PoolableObject, ObjectPool> entry in this.objectPools)
            {
                if (entry.Value == null)
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            for (int i = 0; i < keysToRemove.Count; i += 1)
            {
                this.objectPools.Remove(keysToRemove[i]);
            }
        }

        private void OnDestroy()
        {
            List<PoolableObject> poolableObjects = new List<PoolableObject>(this.objectPools.Keys);
            for (int i = 0; i < poolableObjects.Count; i += 1)
            {
                this.DestroyPool(poolableObjects[i]);
            }
        }
    }
}