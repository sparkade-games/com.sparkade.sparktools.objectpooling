namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// An object pool tracked by GameObject.
    /// </summary>
    public class GameObjectPool : ObjectPool<GameObject>
    {
        private readonly string poolParentName;
        private GameObject poolParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectPool"/> class.
        /// </summary>
        /// <param name="gameObject">The GameObject to be copied when creating a new object for the pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls objects from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public GameObjectPool(
            GameObject gameObject,
            int size = 0,
            PoolAccessMode accessMode = PoolAccessMode.LastIn,
            PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            : base(
                    (objectPool) =>
                    {
                        return GameObject.Instantiate(gameObject);
                    },
                    size,
                    accessMode,
                    loadingMode)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("gameObject");
            }

            this.poolParentName = gameObject.name;
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
        public override GameObject Pull()
        {
            GameObject item = this.PullWithoutCallback();

            IPoolable[] poolableBehaviours = item.GetComponents<IPoolable>();
            for (int i = 0; i < poolableBehaviours.Length; i += 1)
            {
                poolableBehaviours[i].OnPull();
            }

            this.OnPull?.Invoke(item);

            return item;
        }

        /// <inheritdoc/>
        public override void Push(GameObject item)
        {
            this.PushWithoutCallback(item);

            IPoolable[] poolableBehaviours = item.GetComponents<IPoolable>();
            for (int i = 0; i < poolableBehaviours.Length; i += 1)
            {
                poolableBehaviours[i].OnPush();
            }

            this.OnPush?.Invoke(item);
        }

        /// <inheritdoc/>
        public override GameObject PullWithoutCallback()
        {
            GameObject item = base.PullWithoutCallback();
            item.transform.SetParent(null);
            if (item.scene != SceneManager.GetActiveScene())
            {
                SceneManager.MoveGameObjectToScene(item, SceneManager.GetActiveScene());
            }

            return item;
        }

        /// <inheritdoc/>
        public override void PushWithoutCallback(GameObject item)
        {
            base.PushWithoutCallback(item);
            item.transform.SetParent(this.PoolParent.transform);
        }

        /// <summary>
        /// Destroys all objects in the pool.
        /// </summary>
        public void Clear()
        {
            GameObject[] items = this.OwnedItems.ToArray();
            for (int i = 0; i < items.Length; i += 1)
            {
                GameObject.Destroy(items[i]);
            }

            this.ItemStore = this.CreateItemStore(this.AccessMode, this.Size);
        }

        /// <summary>
        /// Removes references to destroyed objects from the pool. This must be done in instances where objects managed by the pool are destroyed, rather than pushed back into it.
        /// </summary>
        public void Prune()
        {
            int prevStoredCount = this.StoredItems.Count;
            GameObject[] destroyedItems = this.OwnedItems.Where(item => item == null).ToArray();
            for (int i = 0; i < destroyedItems.Length; i += 1)
            {
                this.OwnedItems.Remove(destroyedItems[i]);
                this.StoredItems.Remove(destroyedItems[i]);
            }

            if (this.StoredItems.Count != prevStoredCount)
            {
                GameObject[] tempOwnedItems = this.OwnedItems.ToArray();
                GameObject[] tempStoredItems = this.StoredItems.ToArray();

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