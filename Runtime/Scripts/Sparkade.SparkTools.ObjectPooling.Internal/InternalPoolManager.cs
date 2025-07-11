﻿namespace Sparkade.SparkTools.ObjectPooling.Internal
{
    using System;
    using System.Collections.Generic;
    using Sparkade.SparkTools.ObjectPooling;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages multiple object pools, automatically generating them as needed.
    /// </summary>
    internal class InternalPoolManager : IPoolManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalPoolManager"/> class.
        /// </summary>
        /// <param name="poolParent">GameObject all managed object pools are parented to.</param>
        public InternalPoolManager(GameObject poolParent)
        {
            this.PoolParent = poolParent;
        }

        /// <summary>
        /// Gets a dictionary of all object pools based on a prefab.
        /// </summary>
        public Dictionary<object, object> ObjectPools { get; } = new Dictionary<object, object>();

        /// <inheritdoc/>
        public GameObject PoolParent { get; }

        /// <inheritdoc/>
        public void CreatePool<T>(T prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (this.HasPool(prefab))
            {
                throw new InvalidOperationException("Pool already exists.");
            }

            ObjectPooling.ObjectPool<T> pool = new ObjectPooling.ObjectPool<T>((T)prefab, size, accessMode, loadingMode);
            pool.PoolParentCreated += (obj) =>
            {
                obj.transform.SetParent(this.PoolParent.transform);
            };

            this.ObjectPools.Add(prefab, pool);
        }

        /// <inheritdoc/>
        public bool DestroyPool<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return false;
            }

            ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).Clear();
            this.ObjectPools.Remove(prefab);
            return true;
        }

        /// <inheritdoc/>
        public T Pull<T>(T prefab)
             where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                this.CreatePool(prefab);
            }

            return ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).Pull();
        }

        /// <inheritdoc/>
        public void Push<T>(T prefab, T item)
            where T : ObjectPoolItem<T>
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

            ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).Push(item);
        }

        /// <inheritdoc/>
        public bool HasPool<T>(T prefab)
             where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            return this.ObjectPools.ContainsKey(prefab);
        }

        /// <inheritdoc/>
        public ObjectPooling.ObjectPool<T> GetPool<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (!this.HasPool(prefab))
            {
                return null;
            }

            return (ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab];
        }

        /// <inheritdoc/>
        public int GetCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).Count;
        }

        /// <inheritdoc/>
        public int GetFreeCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).FreeCount;
        }

        /// <inheritdoc/>
        public int GetInUseCount<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).InUseCount;
        }

        /// <inheritdoc/>
        public void RecallScene<T>(T prefab, Scene scene)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).RecallScene(scene);
        }

        /// <inheritdoc/>
        public void RecallScene(Scene scene)
        {
            foreach (KeyValuePair<object, object> entry in this.ObjectPools)
            {
                ((IUnityObjectPool)entry.Value).RecallScene(scene);
            }
        }

        /// <inheritdoc/>
        public void RecallAll<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).RecallAll();
        }

        /// <inheritdoc/>
        public void RecallAll()
        {
            foreach (KeyValuePair<object, object> entry in this.ObjectPools)
            {
                ((IUnityObjectPool)entry.Value).RecallAll();
            }
        }

        /// <inheritdoc/>
        public void Clear<T>(T prefab)
            where T : ObjectPoolItem<T>
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            ((ObjectPooling.ObjectPool<T>)this.ObjectPools[prefab]).Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (KeyValuePair<object, object> entry in this.ObjectPools)
            {
                ((IUnityObjectPool)entry.Value).Clear();
            }
        }
    }
}