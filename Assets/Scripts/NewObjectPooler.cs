using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewObjectPooler
{
    private GameObject prefab;
    private List<GameObject> pool;

    public NewObjectPooler(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new List<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool(CreateNewObject());
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = GameObject.Instantiate(prefab);
        newObj.SetActive(false);
        return newObj;
    }

    private void AddObjectToPool(GameObject obj)
    {
        pool.Add(obj);
    }

    public GameObject GetObjectFromPool()
    {
        GameObject obj = null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeSelf)
            {
                obj = pool[i];
                break;
            }
        }

        if (obj == null)
        {
            obj = CreateNewObject();
            AddObjectToPool(obj);
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
