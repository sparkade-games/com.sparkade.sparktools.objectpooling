namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;

    /// <summary>
    /// An item to be tracked by the Unity implimentation of the generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of object being inherited to.</typeparam>
    public abstract class ObjectPoolItem<T> : MonoBehaviour, IPoolable
        where T : ObjectPoolItem<T>
    {
        /// <summary>
        /// Gets or sets callbacks for when the object is pulled from a pool.
        /// </summary>
        public Action OnPullCallback { get; set; }

        /// <summary>
        /// Gets or sets callbacks for when the object is pushed to a pool.
        /// </summary>
        public Action OnPushCallback { get; set; }

        /// <summary>
        /// Gets or sets the object pool this item belongs to.
        /// </summary>
        internal ObjectPool<T> ObjectPool { get; set; }

        /// <inheritdoc/>
        public void OnPull()
        {
            this.OnPullCallback?.Invoke();
        }

        /// <inheritdoc/>
        public void OnPush()
        {
            this.OnPushCallback?.Invoke();
        }

        /// <inheritdoc/>
        public void Repool()
        {
            this.ObjectPool.Push((T)this);
        }

        /// <summary>
        /// Override this if you need to use OnDestroy, but be sure to call the base or pruning won't occur.
        /// </summary>
        protected virtual void OnDestroy()
        {
            this.ObjectPool.PruneItem((T)this);
        }
    }
}