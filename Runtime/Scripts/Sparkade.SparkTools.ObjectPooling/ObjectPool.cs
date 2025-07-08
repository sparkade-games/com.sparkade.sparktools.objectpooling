namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// A Unity implimentation of the generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of ObjectPoolItem to be stored in the pool.</typeparam>
    public class ObjectPool<T> : Generic.ObjectPool<T>, IUnityObjectPool
        where T : ObjectPoolItem<T>
    {
        private readonly string poolParentName;
        private GameObject poolParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="objectPoolItem">The ObjectPoolItem to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public ObjectPool(
            T objectPoolItem,
            int size = 0,
            PoolAccessMode accessMode = PoolAccessMode.LastIn,
            PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            : base(
                    (objectPool) =>
                    {
                        return GameObject.Instantiate(objectPoolItem.gameObject).GetComponent<T>();
                    },
                    size,
                    accessMode,
                    loadingMode)
        {
            if (objectPoolItem == null)
            {
                throw new ArgumentNullException("objectPoolItem");
            }

            this.poolParentName = objectPoolItem.gameObject.name;
        }

        /// <inheritdoc/>
        public GameObject PoolParent
        {
            get
            {
                if (this.poolParent == null)
                {
                    this.poolParent = new GameObject($"{this.poolParentName} Pool");
                    this.poolParent.SetActive(false);
                    this.PoolParentCreated?.Invoke(this.poolParent);
                }

                return this.poolParent;
            }
        }

        /// <summary>
        /// Gets or sets a callback for when the pool's parent object is created.
        /// </summary>
        public Action<GameObject> PoolParentCreated { get; set; }

        /// <inheritdoc/>
        public override T Pull()
        {
            T item = this.PullWithoutCallback();
            item.Pulled?.Invoke();
            this.Pulled?.Invoke(item);
            return item;
        }

        /// <inheritdoc/>
        public override void Push(T item)
        {
            this.PushWithoutCallback(item);
            item.Pushed?.Invoke();
            this.Pushed?.Invoke(item);
        }

        /// <inheritdoc/>
        public override T PullWithoutCallback()
        {
            T item = base.PullWithoutCallback();
            item.transform.SetParent(null);
            if (item.gameObject.scene != SceneManager.GetActiveScene())
            {
                SceneManager.MoveGameObjectToScene(item.gameObject, SceneManager.GetActiveScene());
            }

            return item;
        }

        /// <inheritdoc/>
        public override void PushWithoutCallback(T item)
        {
            base.PushWithoutCallback(item);
            item.transform.SetParent(this.PoolParent.transform);
        }

        /// <inheritdoc/>
        public void RecallScene(Scene scene)
        {
            T[] items = this.GetInUseItems();
            for (int i = 0; i < items.Length; i += 1)
            {
                if (items[i].gameObject.scene == scene)
                {
                    items[i].Repool();
                }
            }
        }

        /// <inheritdoc/>
        public void RecallAll()
        {
            T[] items = this.GetInUseItems();
            for (int i = 0; i < items.Length; i += 1)
            {
                items[i].Repool();
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (T item in this.OwnedItems)
            {
                GameObject.Destroy(item.gameObject);
            }

            this.OwnedItems.Clear();
            this.StoredItems.Clear();
            this.InitItemStore(this.AccessMode, this.Size);
            GameObject.Destroy(this.PoolParent);
        }

        /// <summary>
        /// Removes a specific object from the pool's ownership. Useful when an object is being destroyed.
        /// </summary>
        /// <param name="item">The item to be pruned.</param>
        internal void PruneItem(T item)
        {
            if (!this.OwnedItems.Remove(item))
            {
                return;
            }

            if (this.StoredItems.Remove(item))
            {
                this.InitItemStore(this.AccessMode, this.StoredItems);
            }
        }

        /// <inheritdoc/>
        protected override T CreateItem()
        {
            T item = base.CreateItem();
            item.ObjectPool = this;
            return item;
        }
    }
}