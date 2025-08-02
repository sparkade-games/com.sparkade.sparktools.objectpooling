namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;

    /// <summary>
    /// An object that can be pooled.
    /// </summary>
    public class PoolableObject : MonoBehaviour, IPoolable
    {
        /// <inheritdoc/>
        public Action Pulled { get; set; }

        /// <inheritdoc/>
        public Action Pushed { get; set; }

        /// <summary>
        /// Gets the object pool this item belongs to.
        /// </summary>
        public ObjectPool ObjectPool { get; internal set; }

        /// <inheritdoc/>
        public void Repool()
        {
            if (this.ObjectPool != null && this.ObjectPool.GetOwnsItem(this))
            {
                this.ObjectPool.Push(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Override this if you need to use Awake, but be sure to call the base first.
        /// </summary>
        protected virtual void Awake()
        {
            if (this.GetComponents<PoolableObject>().Length > 1)
            {
                Debug.LogWarning("There should never be more than one PoolableObject component on a GameObject.");
            }
        }

        /// <summary>
        /// Override this if you need to use OnDestroy, but be sure to call the base or pruning won't occur.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (this.ObjectPool != null)
            {
                this.ObjectPool.RemoveItem(this);
            }
        }
    }
}