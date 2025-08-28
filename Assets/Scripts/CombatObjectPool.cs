using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolItem
    {
        public PoolType type;      
        public GameObject prefab;   
        public int size = 10;       
    }
    
    [SerializeField] private List<PoolItem> poolItems;
    [SerializeField] private Canvas canvas;
    

    private Dictionary<PoolType, Queue<GameObject>> _pools = new Dictionary<PoolType, Queue<GameObject>>();

    private void Awake()
    {
        foreach (var item in poolItems)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < item.size; i++)
            {
                GameObject obj = Instantiate(item.prefab, this.transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            _pools[item.type] = queue;
        }
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
    public GameObject Get(PoolType type)
    {
        return InternalGet(type, this.transform);
    }
    
    public GameObject GetUI(PoolType type, Transform uiCanvas)
    {
        return InternalGet(type, uiCanvas);
    }

    private GameObject InternalGet(PoolType type, Transform parent)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogError($"{type} 풀 없음");
            return null;
        }

        Queue<GameObject> pool = _pools[type];
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            PoolItem item = poolItems.Find(x => x.type == type);
            if (item == null)
            {
                Debug.LogError($"{type} 프리팹 없음");
                return null;
            }
            obj = Instantiate(item.prefab, parent);
        }
        
        obj.transform.SetParent(parent, false);
        return obj;
    }


    public void Return(PoolType type, GameObject obj)
    {
        if (!_pools.ContainsKey(type))
        {
            Debug.LogError($"{type} 풀 없음.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _pools[type].Enqueue(obj);
    }
}
