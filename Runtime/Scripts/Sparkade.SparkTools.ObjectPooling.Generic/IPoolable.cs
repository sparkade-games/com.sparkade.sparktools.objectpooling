﻿namespace Sparkade.SparkTools.ObjectPooling.Generic
{
    using System;

    /// <summary>
    /// Provides several callbacks related to being an object in a pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Gets or sets a callback for when the object is pulled from a pool.
        /// </summary>
        Action Pulled { get; set; }

        /// <summary>
        /// Gets or sets a callback for when the object is pushed to a pool.
        /// </summary>
        Action Pushed { get; set; }

        /// <summary>
        /// Returns the object back to the object pool it belongs to.
        /// </summary>
        void Repool();
    }
}