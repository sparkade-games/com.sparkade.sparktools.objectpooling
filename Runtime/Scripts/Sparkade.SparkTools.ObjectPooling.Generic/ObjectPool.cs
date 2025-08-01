namespace Sparkade.SparkTools.ObjectPooling.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A completely generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of item to be stored in the pool.</typeparam>
    public class ObjectPool<T> : IPool<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="factory">A method which takes in a pool and returns an item for that pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls items from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public ObjectPool(
            Func<ObjectPool<T>, T> factory,
            int size = 0,
            PoolAccessMode accessMode = PoolAccessMode.LastIn,
            PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "Argument 'size' cannot be negative.");
            }

            this.Factory = factory ?? throw new ArgumentNullException("factory");
            this.Size = size;
            this.AccessMode = accessMode;
            this.LoadingMode = loadingMode;
            this.OwnedItems = new HashSet<T>();
            this.StoredItems = new HashSet<T>();
            this.InitItemStore(this.AccessMode, this.Size);

            if (loadingMode == PoolLoadingMode.Eager)
            {
                this.PreloadItems();
            }
        }

        /// <summary>
        /// Impliments a generic store for items that can be added to and retrieved from.
        /// </summary>
        protected interface IItemStore
        {
            /// <summary>
            /// Removes an item from the store and returns it.
            /// </summary>
            /// <returns>The item removed form the store.</returns>
            T Fetch();

            /// <summary>
            /// Places an item into the store.
            /// </summary>
            /// <param name="item">The item to be stored.</param>
            void Store(T item);
        }

        /// <inheritdoc/>
        public Action<T> Pulled { get; set; }

        /// <inheritdoc/>
        public Action<T> Pushed { get; set; }

        /// <inheritdoc/>
        public Action<T> Pruned { get; set; }

        /// <inheritdoc/>
        public int Size { get; }

        /// <inheritdoc/>
        public PoolAccessMode AccessMode { get; }

        /// <inheritdoc/>
        public PoolLoadingMode LoadingMode { get; }

        /// <inheritdoc/>
        public int CountAll => this.OwnedItems.Count;

        /// <inheritdoc/>
        public int CountInactive => this.StoredItems.Count;

        /// <inheritdoc/>
        public int CountActive => this.CountAll - this.CountInactive;

        /// <summary>
        /// Gets or sets a method for creating an item for the pool.
        /// </summary>
        protected Func<ObjectPool<T>, T> Factory { get; set; }

        /// <summary>
        /// Gets or sets the generic item store for the pool.
        /// </summary>
        protected IItemStore ItemStore { get; set; }

        /// <summary>
        /// Gets or sets a collection of all items owned by the pool.
        /// </summary>
        protected HashSet<T> OwnedItems { get; set; }

        /// <summary>
        /// Gets or sets a collection of all items currently in the pool.
        /// </summary>
        protected HashSet<T> StoredItems { get; set; }

        /// <inheritdoc/>
        public virtual T Pull()
        {
            T item;
            if (this.CountInactive == 0 || (this.LoadingMode == PoolLoadingMode.Lazy && this.CountAll < this.Size))
            {
                item = this.CreateItem();
            }
            else
            {
                item = this.FetchItem();
            }

            (item as IPoolable)?.Pulled?.Invoke();
            this.Pulled?.Invoke(item);
            return item;
        }

        /// <inheritdoc/>
        public virtual void Push(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (!this.OwnedItems.Contains(item))
            {
                throw new ArgumentException("Item does not belong to the pool.", "item");
            }

            if (this.StoredItems.Contains(item))
            {
                throw new ArgumentException("Item is already in the pool.", "item");
            }

            this.StoreItem(item);
            (item as IPoolable)?.Pushed?.Invoke();
            this.Pushed?.Invoke(item);
        }

        /// <inheritdoc/>
        public virtual void Prune(T item)
        {
            if (!this.OwnedItems.Remove(item))
            {
                return;
            }

            if (this.StoredItems.Remove(item))
            {
                this.InitItemStore(this.AccessMode, this.StoredItems);
            }

            (item as IPoolable)?.Pruned?.Invoke();
            this.Pruned?.Invoke(item);
        }

        /// <inheritdoc/>
        public bool GetOwnsItem(T item)
        {
            return this.OwnedItems.Contains(item);
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetAllItems()
        {
            return this.OwnedItems;
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetInactiveItems()
        {
            return this.StoredItems;
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetActiveItems()
        {
            return this.OwnedItems.Except(this.StoredItems);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.OwnedItems.Clear();
            this.StoredItems.Clear();
            this.InitItemStore(this.AccessMode, this.Size);
        }

        /// <summary>
        /// Creates a new item and places it in the item store.
        /// </summary>
        /// <returns>The item created.</returns>
        protected virtual T CreateItem()
        {
            T item = this.Factory(this);
            this.OwnedItems.Add(item);
            return item;
        }

        /// <summary>
        /// Removes an item from the item store and returns it.
        /// </summary>
        /// <returns>The item removed.</returns>
        protected virtual T FetchItem()
        {
            T item = this.ItemStore.Fetch();
            this.StoredItems.Remove(item);
            return item;
        }

        /// <summary>
        /// Stores an item in the item store.
        /// </summary>
        /// <param name="item">The item to be stored.</param>
        protected virtual void StoreItem(T item)
        {
            this.ItemStore.Store(item);
            this.StoredItems.Add(item);
        }

        /// <summary>
        /// Initializes the item store for the pool.
        /// </summary>
        /// <param name="mode">Access mode of the pool.</param>
        /// <param name="size">Size of the pool.</param>
        protected virtual void InitItemStore(PoolAccessMode mode, int size = 0)
        {
            this.ItemStore = mode switch
            {
                PoolAccessMode.FirstIn => new QueueStore(size),
                PoolAccessMode.LastIn => new StackStore(size),
                _ => throw new ArgumentException($"No ItemStore exists for PoolAccessMode '{mode}'.", "mode"),
            };
        }

        /// <summary>
        /// Initializes the item store for the pool.
        /// </summary>
        /// <param name="mode">Access mode of the pool.</param>
        /// <param name="collection">Collection to add to the store.</param>
        protected virtual void InitItemStore(PoolAccessMode mode, IEnumerable<T> collection)
        {
            this.ItemStore = mode switch
            {
                PoolAccessMode.FirstIn => new QueueStore(collection),
                PoolAccessMode.LastIn => new StackStore(collection),
                _ => null,
            };
        }

        /// <summary>
        /// Creates enough items for the pool to reach its size and places them in the item store.
        /// </summary>
        protected virtual void PreloadItems()
        {
            for (int i = 0; i < this.Size; i += 1)
            {
                this.StoreItem(this.CreateItem());
            }
        }

        /// <summary>
        /// A type of item store that is first in first out.
        /// </summary>
        protected class QueueStore : Queue<T>, IItemStore
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="QueueStore"/> class.
            /// </summary>
            /// <param name="size">Initial size of the queue.</param>
            public QueueStore(int size = 0)
                : base(size)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="QueueStore"/> class.
            /// </summary>
            /// <param name="collection">Collection to add to the store.</param>
            public QueueStore(IEnumerable<T> collection)
                : base(collection)
            {
            }

            /// <inheritdoc/>
            public T Fetch()
            {
                return this.Dequeue();
            }

            /// <inheritdoc/>
            public void Store(T item)
            {
                this.Enqueue(item);
            }
        }

        /// <summary>
        /// A type of item store that is last in first out.
        /// </summary>
        protected class StackStore : Stack<T>, IItemStore
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StackStore"/> class.
            /// </summary>
            /// <param name="size">Initial size of the stack.</param>
            public StackStore(int size = 0)
                : base(size)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="StackStore"/> class.
            /// </summary>
            /// <param name="collection">Collection to add to the store.</param>
            public StackStore(IEnumerable<T> collection)
                : base(collection)
            {
            }

            /// <inheritdoc/>
            public T Fetch()
            {
                return this.Pop();
            }

            /// <inheritdoc/>
            public void Store(T item)
            {
                this.Push(item);
            }
        }
    }
}