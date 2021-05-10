using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCollection<T> : ScriptableObject
{
    public List<T> Collection = new List<T>();
    public void Add(T t)
    {
        if (!Collection.Contains(t))
            Collection.Add(t);
    }

    public void Remove(T t)
    {
        if (Collection.Contains(t))
            Collection.Remove(t);
    }

    public bool Contains(T t)
    {
        return Collection.Contains(t);
    }

    public T GetIndex(int i)
    {
        if (i < Collection.Count && i > 0)
        {
            return Collection[i];
        }
        return default(T);
    }

    public T GetRandom()
    {
        return Collection[Random.Range(0, Collection.Count)];
    }

    public int Count()
    {
        return Collection.Count;
    }
}
