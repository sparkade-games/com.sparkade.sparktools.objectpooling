namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// An object pool tracked by MonoBehaviour.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour stored in the pool.</typeparam>
    public class MonoBehaviourPool<T> : ObjectPool<T>
        where T : MonoBehaviour
    {
        private readonly string poolParentName;
        private GameObject poolParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoBehaviourPool{T}"/> class.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public MonoBehaviourPool(
            T monoBehaviour,
            int size = 0,
            PoolAccessMode accessMode = PoolAccessMode.LastIn,
            PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            : base(
                    (objectPool) =>
                    {
                        return GameObject.Instantiate(monoBehaviour.gameObject).GetComponent<T>();
                    },
                    size,
                    accessMode,
                    loadingMode)
        {
            if (monoBehaviour == null)
            {
                throw new ArgumentNullException("monoBehaviour");
            }

            this.poolParentName = monoBehaviour.name;
        }

        /// <summary>
        /// Gets the GameObject all currently free objects in the pool are parented to.
        /// </summary>
        public GameObject PoolParent
        {
            get
            {
                if (this.poolParent == null)
                {
                    this.poolParent = new GameObject($"{this.poolParentName} Pool");
                    this.poolParent.SetActive(false);
                    this.OnPoolParentCreated?.Invoke(this.poolParent);
                }

                return this.poolParent;
            }
        }

        /// <summary>
        /// Gets or sets a callback for when the pool's parent object is created.
        /// </summary>
        public Action<GameObject> OnPoolParentCreated { get; set; }

        /// <inheritdoc/>
        public override T Pull()
        {
            T item = this.PullWithoutCallback();
            (item as IPoolable)?.OnPull();
            this.OnPull?.Invoke(item);
            return item;
        }

        /// <inheritdoc/>
        public override void Push(T item)
        {
            this.PushWithoutCallback(item);
            (item as IPoolable)?.OnPush();
            this.OnPush?.Invoke(item);
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

        /// <summary>
        /// Destroys all objects in the pool.
        /// </summary>
        public void Clear()
        {
            T[] items = this.OwnedItems.ToArray();
            for (int i = 0; i < items.Length; i += 1)
            {
                GameObject.Destroy(items[i].gameObject);
            }

            this.ItemStore = this.CreateItemStore(this.AccessMode, this.Size);
        }

        /// <summary>
        /// Removes references to destroyed objects from the pool. This must be done in instances where objects managed by the pool are destroyed, rather than pushed back into it.
        /// </summary>
        public void Prune()
        {
            int prevStoredCount = this.StoredItems.Count;
            T[] destroyedItems = this.OwnedItems.Where(item => item == null).ToArray();
            for (int i = 0; i < destroyedItems.Length; i += 1)
            {
                this.OwnedItems.Remove(destroyedItems[i]);
                this.StoredItems.Remove(destroyedItems[i]);
            }

            if (this.StoredItems.Count != prevStoredCount)
            {
                T[] tempOwnedItems = this.OwnedItems.ToArray();
                T[] tempStoredItems = this.StoredItems.ToArray();

                this.ItemStore = this.CreateItemStore(this.AccessMode, this.Size);

                for (int i = 0; i < tempOwnedItems.Length; i += 1)
                {
                    this.OwnedItems.Add(tempOwnedItems[i]);
                }

                for (int i = 0; i < tempStoredItems.Length; i += 1)
                {
                    this.StoreItem(tempStoredItems[i]);
                }
            }
        }
    }
}