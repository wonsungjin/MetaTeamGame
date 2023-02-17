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
        bool listCheck = table.TryGetValue(prefab.name.Replace("(Clone)", ""), out list);
        if (listCheck == false)
        {
            list = new List<GameObject>();
            table.Add(prefab.name.Replace("(Clone)", ""), list);
        }
        if (list.Count == 0)
        {
            instance = GameObject.Instantiate(prefab, position, rotation);
            Debug.Log(prefab.name.Replace("(Clone)", ""));
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
        if (Prefab.transform.GetChild(0) != null)
        {
            Prefab.transform.GetChild(0).TryGetComponent(out Card card);
            if (card != null)
                Prefab.GetComponentInChildren<Card>().SetMyInfo(prefabld);
        }
        Prefab.SetActive(false);
        list.Add(Prefab);
    }
}