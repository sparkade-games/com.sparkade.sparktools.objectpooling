namespace Sparkade.SparkTools.ObjectPooling
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Impliments a Unity specific object pool.
    /// </summary>
    public interface IUnityObjectPool
    {
        /// <summary>
        /// Gets the GameObject all currently free objects in the pool are parented to.
        /// </summary>
        public GameObject PoolParent { get; }

        /// <summary>
        /// Pushes all in use objects that are in a specific scene back into the pool.
        /// </summary>
        /// <param name="scene">The scene in use objects that should be pushed are in.</param>
        void RecallScene(Scene scene);

        /// <summary>
        /// Pushes all in use objects back into the pool.
        /// </summary>
        void RecallAll();

        /// <summary>
        /// Destroys all objects in the pool.
        /// </summary>
        void Clear();
    }
}