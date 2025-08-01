namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// A Unity implimentation of the generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of ObjectPoolItem to be stored in the pool.</typeparam>
    public class ObjectPool : MonoBehaviour, IPool<PoolableObject>
    {
        [SerializeField]
        private PoolableObject prefab;

        [SerializeField]
        private int size = 0;

        [SerializeField]
        private PoolAccessMode accessMode = PoolAccessMode.LastIn;

        [SerializeField]
        private PoolLoadingMode loadingMode = PoolLoadingMode.Eager;

        private ObjectPool<PoolableObject> pool;
        private bool initialized;

        /// <inheritdoc/>
        public Action<PoolableObject> Pulled { get; set; }

        /// <inheritdoc/>
        public Action<PoolableObject> Pushed { get; set; }

        /// <inheritdoc/>
        public Action<PoolableObject> Pruned { get; set; }

        /// <inheritdoc/>
        public int Size => this.size;

        /// <inheritdoc/>
        public PoolAccessMode AccessMode => this.accessMode;

        /// <inheritdoc/>
        public PoolLoadingMode LoadingMode => this.loadingMode;

        /// <inheritdoc/>
        public int CountAll => this.initialized ? this.pool.CountAll : 0;

        /// <inheritdoc/>
        public int CountInactive => this.initialized ? this.pool.CountInactive : 0;

        /// <inheritdoc/>
        public int CountActive => this.initialized ? this.pool.CountActive : 0;

        /// <summary>
        /// Gets a value indicating whether the object pool has been initialized.
        /// </summary>
        public bool IsInitialized => this.initialized;

        /// <inheritdoc/>
        public PoolableObject Pull()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            PoolableObject item = this.pool.Pull();
            item.ObjectPool = this;
            item.transform.SetParent(null);
            if (item.gameObject.scene != SceneManager.GetActiveScene())
            {
                SceneManager.MoveGameObjectToScene(item.gameObject, SceneManager.GetActiveScene());
            }

            return item;
        }

        /// <summary>
        /// Returns an item cast as a type from the pool. If no items are currently inactive, one will be created.
        /// </summary>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An item pulled from the pool.</returns>
        public T Pull<T>()
            where T : PoolableObject
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            if (typeof(T) != this.prefab.GetType())
            {
                throw new ArgumentException($"The pool does not contain items of this type.", "T");
            }

            return (T)this.Pull();
        }

        /// <inheritdoc/>
        public void Push(PoolableObject item)
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            this.pool.Push(item);
            item.transform.SetParent(this.transform);
        }

        /// <inheritdoc/>
        public void Prune(PoolableObject item)
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            this.pool.Prune(item);
        }

        /// <inheritdoc/>
        public bool GetOwnsItem(PoolableObject item)
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            return this.pool.GetOwnsItem(item);
        }

        /// <inheritdoc/>
        public IEnumerable<PoolableObject> GetAllItems()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            return this.pool.GetAllItems();
        }

        /// <inheritdoc/>
        public IEnumerable<PoolableObject> GetInactiveItems()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            return this.pool.GetInactiveItems();
        }

        /// <inheritdoc/>
        public IEnumerable<PoolableObject> GetActiveItems()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The pool is not initialized.");
            }

            return this.pool.GetActiveItems();
        }

        /// <summary>
        /// Destroys all items in the pool.
        /// </summary>
        public void Clear()
        {
            PoolableObject[] items = this.GetAllItems().ToArray();
            for (int i = 0; i < items.Length; i += 1)
            {
                GameObject.Destroy(items[i].gameObject);
            }

            this.pool.Clear();
        }

        /// <summary>
        /// Pushes all in use items that are in a specific scene back into the pool.
        /// </summary>
        /// <param name="scene">The scene in use items that should be pushed are in.</param>
        public void RecallScene(Scene scene)
        {
            PoolableObject[] items = this.GetActiveItems().ToArray();
            for (int i = 0; i < items.Length; i += 1)
            {
                if (items[i].gameObject.scene == scene)
                {
                    this.Push(items[i]);
                }
            }
        }

        /// <summary>
        /// Pushes all in use items back into the pool.
        /// </summary>
        public void RecallAll()
        {
            PoolableObject[] items = this.GetActiveItems().ToArray();
            for (int i = 0; i < items.Length; i += 1)
            {
                this.Push(items[i]);
            }
        }

        /// <summary>
        /// Initializes the object pool. This is only required if the prefab was null on Awake.
        /// </summary>
        /// <param name="prefab">The pool item prefab.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls items from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public void Init(PoolableObject prefab, int size = 0, PoolAccessMode accessMode = PoolAccessMode.LastIn, PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException("prefab");
            }

            if (this.initialized)
            {
                throw new InvalidOperationException("The pool is already initialized.");
            }

            this.prefab = prefab;
            this.size = size;
            this.accessMode = accessMode;
            this.loadingMode = loadingMode;
            this.InitInternal();
        }

        private void InitInternal()
        {
            this.pool = new ObjectPool<PoolableObject>(
                (objectPool) =>
                {
                    return GameObject.Instantiate(this.prefab.gameObject).GetComponent<PoolableObject>();
                },
                this.size,
                this.accessMode,
                this.loadingMode);

            foreach (PoolableObject item in this.pool.GetInactiveItems())
            {
                item.transform.SetParent(this.transform);
                item.ObjectPool = this;
            }

            this.initialized = true;
        }

        private void Awake()
        {
            if (this.prefab != null)
            {
                this.InitInternal();
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

            if (this.initialized)
            {
                this.Clear();
            }
        }
    }
}