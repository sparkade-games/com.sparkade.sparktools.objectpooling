namespace Sparkade.SparkTools.ObjectPooling.Internal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Manages multiple GameObject pools, automatically generating them as needed.
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
        /// Gets a dictionary of all GameObjectPools, based on a prefab.
        /// </summary>
        public Dictionary<GameObject, GameObjectPool> GameObjectPools { get; } = new Dictionary<GameObject, GameObjectPool>();

        /// <summary>
        /// Gets a dictionary of all MonoBehaviourPools, based on a prefab.
        /// </summary>
        public Dictionary<MonoBehaviour, GameObjectPool> MonoBehaviourPools { get; } = new Dictionary<MonoBehaviour, GameObjectPool>();

        /// <inheritdoc/>
        public GameObject PoolParent { get; }

        /// <inheritdoc/>
        public void CreatePool(GameObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (this.HasPool(prefab))
            {
                throw new InvalidOperationException("Pool already exists.");
            }

            GameObjectPool pool = new GameObjectPool(prefab, size, accessMode, loadingMode);
            pool.OnPoolParentCreated += (obj) =>
            {
                obj.transform.SetParent(this.PoolParent.transform);
            };

            this.GameObjectPools.Add(prefab, pool);
        }

        /// <inheritdoc/>
        public void CreatePool(MonoBehaviour prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (this.HasPool(prefab))
            {
                throw new InvalidOperationException("Pool already exists.");
            }

            GameObjectPool pool = new GameObjectPool(prefab.gameObject, size, accessMode, loadingMode);
            pool.OnPoolParentCreated += (obj) =>
            {
                obj.name = $"{prefab.name} Pool";
                obj.transform.SetParent(this.PoolParent.transform);
            };

            this.MonoBehaviourPools.Add(prefab, pool);
        }

        /// <inheritdoc/>
        public GameObjectPool GetPool(GameObject prefab)
        {
            if (!this.HasPool(prefab))
            {
                return null;
            }

            return this.GameObjectPools[prefab];
        }

        /// <inheritdoc/>
        public GameObjectPool GetPool(MonoBehaviour prefab)
        {
            if (!this.HasPool(prefab))
            {
                return null;
            }

            return this.MonoBehaviourPools[prefab];
        }

        /// <inheritdoc/>
        public GameObject Pull(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                this.CreatePool(prefab);
            }

            return this.GameObjectPools[prefab].Pull();
        }

        /// <inheritdoc/>
        public T Pull<T>(T prefab)
            where T : MonoBehaviour
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                this.CreatePool(prefab);
            }

            return this.MonoBehaviourPools[prefab].Pull().GetComponent<T>();
        }

        /// <inheritdoc/>
        public void Push(GameObject prefab, GameObject item)
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

            this.GameObjectPools[prefab].Push(item);
        }

        /// <inheritdoc/>
        public void Push(MonoBehaviour prefab, MonoBehaviour item)
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

            this.MonoBehaviourPools[prefab].Push(item.gameObject);
        }

        /// <inheritdoc/>
        public bool HasPool(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            return this.GameObjectPools.ContainsKey(prefab);
        }

        /// <inheritdoc/>
        public bool HasPool(MonoBehaviour prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            return this.MonoBehaviourPools.ContainsKey(prefab);
        }

        /// <inheritdoc/>
        public int GetCount(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.GameObjectPools[prefab].Count;
        }

        /// <inheritdoc/>
        public int GetCount(MonoBehaviour prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.MonoBehaviourPools[prefab].Count;
        }

        /// <inheritdoc/>
        public int GetFreeCount(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.GameObjectPools[prefab].FreeCount;
        }

        /// <inheritdoc/>
        public int GetFreeCount(MonoBehaviour prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return 0;
            }

            return this.MonoBehaviourPools[prefab].FreeCount;
        }

        /// <inheritdoc/>
        public void ClearPool(GameObject prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.GameObjectPools[prefab].Clear();
        }

        /// <inheritdoc/>
        public void ClearPool(MonoBehaviour prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (!this.HasPool(prefab))
            {
                return;
            }

            this.MonoBehaviourPools[prefab].Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (KeyValuePair<GameObject, GameObjectPool> entry in this.GameObjectPools)
            {
                entry.Value.Clear();
            }

            this.GameObjectPools.Clear();

            foreach (KeyValuePair<MonoBehaviour, GameObjectPool> entry in this.MonoBehaviourPools)
            {
                entry.Value.Clear();
            }

            this.MonoBehaviourPools.Clear();
        }
    }
}