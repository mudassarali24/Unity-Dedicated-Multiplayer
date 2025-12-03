using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public string poolerID = "Pooler 1234";

    public GameObject objectToPool;
    public int initAmount = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        StartCoroutine(FillPool());
    }

    private IEnumerator FillPool()
    {
        for (int i = 0; i < initAmount; i++)
        {
            GameObject poolObj = Instantiate(objectToPool, transform);
            poolObj.name = $"{objectToPool.name}_{i + 1}";
            AddToPool(poolObj, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AddToPool(GameObject _object, bool init = false)
    {
        if (pool.Contains(_object)) return;
        pool.Enqueue(_object);
        if (!init) _object.SetActive(false);
        _object.transform.SetParent(transform);
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            GameObject poolObj = Instantiate(objectToPool, transform);
            AddToPool(poolObj, true);
        }
        GameObject objectToReturn = pool.Dequeue();
        objectToReturn.transform.parent = null;
        objectToReturn.SetActive(true);
        return objectToReturn;
    }
}