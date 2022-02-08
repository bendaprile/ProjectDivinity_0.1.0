using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UtilityExtensions
{
    public static T[] GetComponentsOnlyInChildren<T>(this MonoBehaviour script) where T : class
    {
        List<T> group = new List<T>();

        //collect only if its an interface or a Component
        if (typeof(T).IsInterface
         || typeof(T).IsSubclassOf(typeof(Component))
         || typeof(T) == typeof(Component))
        {
            foreach (Transform child in script.transform)
            {
                group.AddRange(child.GetComponentsInChildren<T>());
            }
        }

        return group.ToArray();
    }
}
