namespace Sparkade.SparkTools.ObjectPooling
{
    using System;
    using Sparkade.SparkTools.ObjectPooling.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// An item to be tracked by the Unity implimentation of the generic object pool.
    /// </summary>
    /// <typeparam name="T">The type of object being inherited to.</typeparam>
    public abstract class ObjectPoolItem<T> : MonoBehaviour, IPoolable
        where T : ObjectPoolItem<T>
    {
        /// <inheritdoc/>
        public Action OnPull { get; set; }

        /// <inheritdoc/>
        public Action OnPush { get; set; }

        /// <summary>
        /// Gets or sets the object pool this item belongs to.
        /// </summary>
        internal ObjectPool<T> ObjectPool { get; set; }

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
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
            {
                return;
            }
#endif

            this.ObjectPool.PruneItem((T)this);
        }
    }
}