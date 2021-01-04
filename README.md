# SparkTools: Object Pooling
A generic object pooling solution for C#, along with a Unity specific application of it.

# Installation
It is recommened to install through the Unity Package Manager.

If you wish to manually install, clone the repository into the `Packages` folder of your project.

# How it works
Object pooling allows you to re-use objects instead of constantly destroying and creating them.

The `ObjectPool` and `ObjectPoolItem` classes can manage this recycling of objects, only creating new objects when needed.

The `ObjectPoolManager` and `GlobalObjectPoolManager` take this one step further and manage object pools, creating new object pools automatically when needed.

# How to Use

## Object Pool Types
- **Generic ObjectPool:** Generic object pool, can be used for any C# objects.

- **Unity ObjectPool:** A Unity specific implimentation of the generic object pool that allows you to push and pull GameObjectss based on a prefab containing an instance of the `ObjectPoolItem`.

## Object Pool Managers
Object Pool Managers streamline the use of Unity specific object pools. They are Singletons that you can use to push or pull `ObjectPoolItem`'s, and pools will be set up and expanded as needed. They come in two flavors:

- **ObjectPoolManager:** Manages object pools that exist for the life of the scene they were created in.

- **GlobalObjectPoolManager:** Manages object pools that exist for the life of the entire game.

Both of these provide options to self manage pools, allowing you to create or destroy them manually as well.

The `GlobalObjectPoolManager` will require a bit more manual managemnet, as any pooled items still in a scene as it unloads will be destroyed instead of being brought back to the pool.

# Example
Let's make a projectile:
```
public class Projectile : ObjectPoolItem<Projectile>
{
    private Vector2 direction;
    private float speed;

    private void Awake()
    {
        OnPull += ResetSpeed;
    }

    public void ResetSpeed()
    {
        speed = 0.0f;
    }

    void Update()
    {
        // move in direction
        // increase speed as we move
    }
}
```
Notice we inherited from `ObjectPoolItem`, which gives us the `OnPull` and `OnPush` callbacks. We use this to reset the projectile's speed every time it is pulled.

You may have also noticed the `<Projectile>` type passed to the `ObjectPoolItem`. This should always be the same type as the object it is inheriting from.

Now, we can use the ObjectPoolManager to spawn our projectiles:
```
public class Gun : MonoBehaviour
{
    public Projectile prefab;

    void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            Projectile proj = ObjectPoolManager.Instance.Pull(prefab);
            proj.direction = transform.right;
        }
    }
}
```
So when our shoot button is pressed projectiles are pulled from the Projectile object pool, or created when needed. Of course you'd also want the to be pushed back into the pool at some point, if they hit a wall for instance.