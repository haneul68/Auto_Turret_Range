using System.Collections.Generic;
using UnityEngine;

public class Local_Object_Pool<T> where T : Component
{
    private T prefab;
    private Transform parent;

    private Queue<T> pool = new Queue<T>();
    private HashSet<T> pooled_Set = new HashSet<T>();

    private int max_Count;

    public Local_Object_Pool(T prefab, Transform parent, int default_Count, int max_Count)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.max_Count = max_Count;

        for (int i = 0; i < default_Count; i++)
        {
            Create();
        }
    }

    private T Create()
    {
        T obj = Object.Instantiate(prefab, parent);
        Return(obj);
        return obj;
    }

    public T Get()
    {
        if (pool.Count <= 0)
        {
            Create();
        }

        T obj = pool.Dequeue();
        pooled_Set.Remove(obj);

        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null)
            return;

        if (pooled_Set.Contains(obj))
            return;

        if (pool.Count >= max_Count)
        {
            Object.Destroy(obj.gameObject);
            return;
        }

        obj.transform.SetParent(parent);
        obj.gameObject.SetActive(false);

        pool.Enqueue(obj);
        pooled_Set.Add(obj);
    }
}