using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Planet.Performance.Pooling
{
    public static class WObjectPool
    {
        public class ExtendedObjectPool
        {
            public ExtendedObjectPool(GameObject PrefabRef, ObjectPool<GameObject> Pool, bool Valid, int MaxSize)
            {
                this.PrefabRef = PrefabRef;
                this.Pool = Pool;
                this.Valid = Valid;
                this.MaxSize = MaxSize;
            }

            private ObjectPool<GameObject> Pool;
            private GameObject PrefabRef;

            public bool Valid;
            public int MaxSize;
            public int CurrentSize;

            public GameObject Get() => GetItemFromPool(this);
            public void Release(GameObject obj) => DisposeToPool(this, obj);
            
            public void DisposePool()
            {
                RegisteredPools.Remove(PrefabRef);
                Pool.Dispose();
            }

            private static GameObject GetItemFromPool(ExtendedObjectPool EPool)
            {

                if (EPool == null || EPool.CurrentSize >= EPool.MaxSize)
                {
                    return null;
                }

                EPool.CurrentSize++;
                return EPool.Pool.Get();
            }
            private static void DisposeToPool(ExtendedObjectPool EPool, GameObject instance)
            {
                EPool.Pool.Release(instance);
                EPool.CurrentSize--;
            }
        }

        private static Dictionary<GameObject, ExtendedObjectPool> RegisteredPools = new Dictionary<GameObject, ExtendedObjectPool>();

        public static ExtendedObjectPool RegisterNewPool(GameObject Prefab, Func<GameObject> createFunc = null, Action<GameObject> actionOnGet = null, Action<GameObject> actionOnRelease = null,
                                                Action<GameObject> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            if(createFunc == null)
            {
                createFunc = () =>
                {
                    GameObject obj = GameObject.Instantiate(Prefab);
                    obj.SetActive(false);
                    return obj;
                };
            }
            if(actionOnRelease is null)
            {
                actionOnRelease = (instance) =>
                {
                    instance.SetActive(false);
                    instance.transform.position = Vector3.zero;
                    instance.transform.rotation = Quaternion.identity;
                };
            }
            if(actionOnDestroy == null)
            {
                actionOnDestroy = (obj) => GameObject.Destroy(obj);
            }


            ObjectPool<GameObject> Pool = new   (createFunc,
                                                actionOnGet,
                                                actionOnRelease,
                                                actionOnDestroy,
                                                collectionCheck,
                                                defaultCapacity,
                                                maxSize);

            ExtendedObjectPool Wrapper = new ExtendedObjectPool(Prefab, Pool, Pool != null, maxSize);
            RegisteredPools.Add(Prefab, Wrapper);
            return Wrapper;
        }

        
    }
}

