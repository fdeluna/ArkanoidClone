using UnityEngine;

public class PoollingPrefabManager : MonoBehaviour
{

    public static PoollingPrefabManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoollingPrefabManager>();

                if (_instance == null)
                {
                    string goName = typeof(PoollingPrefabManager).ToString();
                    GameObject go = GameObject.Find(goName);

                    if (go == null)
                    {
                        go = new GameObject();
                        go.name = goName;
                        _instance = go.AddComponent<PoollingPrefabManager>();
                    }

                }
            }
            return _instance;
        }
    }
    private static PoollingPrefabManager _instance;


    public void CreatePoolPrefab(GameObject prefab, int amount = 5)
    {
        string poolName = prefab.name + " - Pool";
        GameObject poolGO = GameObject.Find(poolName);
        if (poolGO == null)
        {
            poolGO = new GameObject();
            poolGO.name = poolName;
            poolGO.transform.parent = _instance.transform;

            for (int i = 0; i < amount; i++)
            {
                AddPrefabToPool(prefab, poolGO);
            }
        }
    }

    public GameObject GetPooledPrefab(GameObject prefab, Vector3 position)
    {
        string poolName = prefab.name + " - Pool";
        GameObject poolGO = GameObject.Find(poolName);
        GameObject pooledPrefab = null;

        if (poolGO == null)
        {
            CreatePoolPrefab(prefab);
            return GetPooledPrefab(prefab, position);
        }
        else
        {
            foreach (Transform t in poolGO.transform)
            {
                if (!t.gameObject.activeSelf)
                {
                    pooledPrefab = t.gameObject;
                }
            }

            if (pooledPrefab == null)
            {
                pooledPrefab = AddPrefabToPool(prefab, poolGO);                
            }

            pooledPrefab.transform.position = position;
            pooledPrefab.SetActive(true);
        }
        return pooledPrefab;
    }

    private GameObject AddPrefabToPool(GameObject prefab, GameObject prefabPool)
    {
        // If no one was found, create it and add it to the pool
        GameObject newPrefab = Instantiate(prefab) as GameObject;
        newPrefab.name = prefab.name;
        newPrefab.transform.SetParent(prefabPool.transform);
        newPrefab.transform.localPosition = Vector3.zero;
        newPrefab.SetActive(false);

        return newPrefab;
    }
}
