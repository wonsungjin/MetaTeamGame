using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{

    Dictionary<string, List<GameObject>> table = new Dictionary<string, List<GameObject>>();

    public GameObject CreatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        List<GameObject> list = null;
        GameObject instance = null;
        bool listCheck = table.TryGetValue(prefab.name, out list);
        if (listCheck == false)
        {
            list = new List<GameObject>();
            table.Add(prefab.name, list);
        }
        if (list.Count == 0)
        {
            instance = GameObject.Instantiate(prefab, position, rotation);
        }
        else if (list.Count > 0)
        {
            instance = list[0];
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            //instance.transform.parent = parent;
            list.RemoveAt(0);
        }

        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            return instance;
        }
        else { return null; }
    }
    public void DestroyPrefab(GameObject Prefab)
    {
        List<GameObject> list = null;
        string prefabld = Prefab.name.Replace("(Clone)", "");
        bool listCached = table.TryGetValue(prefabld, out list);
        if (listCached == false)
        {
            Debug.LogError("Not Found" + Prefab.name);
            return;
        }
        Prefab.gameObject.SetActive(false);
        list.Add(Prefab);
    }
}