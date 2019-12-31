using UnityEngine;

namespace Manager
{
    public class PoollingPrefabManager : MonoBehaviour
    {
        public static PoollingPrefabManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType<PoollingPrefabManager>();

                if (_instance != null) return _instance;
                
                var goName = typeof(PoollingPrefabManager).ToString();
                var go = GameObject.Find(goName);

                if (go != null) return _instance;

                go = new GameObject {name = goName};
                _instance = go.AddComponent<PoollingPrefabManager>();
                return _instance;
            }       
        }
        private static PoollingPrefabManager _instance;
        
        public void CreatePoolPrefab(GameObject prefab, int amount = 5)
        {
            var poolName = prefab.name + " - Pool";
            var poolGo = GameObject.Find(poolName);
            if (poolGo != null) return;
            poolGo = new GameObject {name = poolName};
            poolGo.transform.parent = _instance.transform;

            for (var i = 0; i < amount; i++)
            {
                AddPrefabToPool(prefab, poolGo);
            }
        }

        public GameObject GetPooledPrefab(GameObject prefab, Vector3 position)
        {
            var poolName = prefab.name + " - Pool";
            var poolGo = GameObject.Find(poolName);
            GameObject pooledPrefab = null;

            if (poolGo == null)
            {
                CreatePoolPrefab(prefab);
                return GetPooledPrefab(prefab, position);
            }
            else
            {
                foreach (Transform t in poolGo.transform)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        pooledPrefab = t.gameObject;
                    }
                }

                if (pooledPrefab == null)
                {
                    pooledPrefab = AddPrefabToPool(prefab, poolGo);                
                }

                pooledPrefab.transform.position = position;
                pooledPrefab.SetActive(true);
            }
            return pooledPrefab;
        }

        private GameObject AddPrefabToPool(GameObject prefab, GameObject prefabPool)
        {
            // If no one was found, create it and add it to the pool
            var newPrefab = Instantiate(prefab, prefabPool.transform, true) as GameObject;
            newPrefab.name = prefab.name;
            newPrefab.transform.localPosition = Vector3.zero;
            newPrefab.SetActive(false);
            return newPrefab;
        }
    }
}