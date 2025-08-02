namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// An object pool used internally by the object pool component.
    /// </summary>
    internal class InternalObjectPool : ObjectPool<PoolableObject>
    {
        private ObjectPool objectPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalObjectPool"/> class.
        /// </summary>
        /// <param name="factory">A method which takes in a pool and returns an item for that pool.</param>
        /// <param name="objectPool">The object pool utilizing this pool.</param>
        /// <param name="size">The target size of the pool. The pool will expand beyond this size if needed.</param>
        /// <param name="accessMode">Determines the order in which a pool pulls items from its store.</param>
        /// <param name="loadingMode">Determines how a pool reaches its size.</param>
        public InternalObjectPool(
            Func<ObjectPool<PoolableObject>, PoolableObject> factory,
            ObjectPool objectPool,
            int size = 0,
            PoolAccessMode accessMode = PoolAccessMode.LastIn,
            PoolLoadingMode loadingMode = PoolLoadingMode.Eager)
            : base(factory, size, accessMode, loadingMode)
        {
            this.objectPool = objectPool;
        }

        /// <inheritdoc/>
        public override PoolableObject Pull()
        {
            PoolableObject item = base.Pull();
            item.transform.SetParent(null);
            if (item.gameObject.scene != SceneManager.GetActiveScene())
            {
                SceneManager.MoveGameObjectToScene(item.gameObject, SceneManager.GetActiveScene());
            }

            return item;
        }

        /// <inheritdoc/>
        public override void Push(PoolableObject item)
        {
            base.Push(item);
            item.transform.SetParent(this.objectPool.transform);
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (PoolableObject item in this.OwnedItems)
            {
                GameObject.Destroy(item.gameObject);
            }

            base.Clear();
        }

        /// <summary>
        /// Removes an item from the pool.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void RemoveItem(PoolableObject item)
        {
            if (!this.OwnedItems.Remove(item))
            {
                return;
            }

            if (this.StoredItems.Remove(item))
            {
                this.InitItemStore(this.AccessMode, this.StoredItems);
            }

            item.ObjectPool = null;
        }

        /// <inheritdoc/>
        protected override PoolableObject CreateItem()
        {
            PoolableObject item = base.CreateItem();
            item.transform.SetParent(this.objectPool.transform);
            item.ObjectPool = this.objectPool;
            return item;
        }
    }
}