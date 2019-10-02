using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class Extensions
{
    public static IEnumerable<Transform> GetChildren(this Transform t)
    {
        for(int i = 0; i < t.childCount; i++)
        {
            yield return t.GetChild(i);
        }
    }

    public static Vector3 GetForwardLocation(this Transform t)
    {
        return t.position + t.forward * 2f;
    }

    public static IEnumerable<Transform> GetThingsInFront(this MonoBehaviour go)
    {
        var t = go.transform;
        var p = go.GetComponentInParent<Grid>();
        var location = t.GetForwardLocation();
        return p
            .transform
            .GetChildren()
            .Where(c => Vector3.Distance(c.position, location) < 2);
    }

    public static IEnumerable<T> GetComponent<T>(this IEnumerable<Transform> gameObjects)
        where T: MonoBehaviour
    {
        return gameObjects.Select(go => go.GetComponent<T>()).Where(go => go != null);
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }

        return items;
    }
}

