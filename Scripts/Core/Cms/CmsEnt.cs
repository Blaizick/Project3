using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.SearchService;
using UnityEngine;

[CreateAssetMenu(fileName = "CmsEntity", menuName = "Cms Entity"), 
Serializable]
public class CmsEnt : ScriptableObject
{
    public string tag;
    [SerializeReference, SubclassSelector]
    public List<CmsComp> comps = new();

    public CmsComp Get(Type type)
    {
        return comps.Find(p => type.IsAssignableFrom(p.GetType()));
    }
    public T Get<T>() where T : CmsComp
    {
        return (T)Get(typeof(T));
    }

    public List<T> GetAll<T>() where T : CmsComp
    {
        var ret = comps.
            FindAll(p => typeof(T).IsAssignableFrom(p.GetType())).
            Cast<T>().
            ToList();
        return ret == null ? new() : ret;
    }

    public bool TryGet(Type type, out object obj)
    {
        int id = comps.FindIndex(p => type.IsAssignableFrom(p.GetType()));
        if (id <= 0)
        {
            obj = null;
            return false;
        }
        obj = comps[id];
        return true;
    }
    public bool TryGet<T>(out T obj) where T : CmsComp
    {
        object o;
        var s = TryGet(typeof(T), out o);
        obj = s ? (T)o : null;
        return s;
    }
}

[Serializable]
public class CmsComp
{
}

public static class Cms
{
    public static Dictionary<string, CmsEnt> entsDic = new();

    public static void Init()
    {
        entsDic.Clear();
        
        var ents = Resources.LoadAll<CmsEnt>(string.Empty).ToList();
        StringBuilder sb = new($"Loaded {ents.Count} Entitites: ");
        for (int i = 0; i < ents.Count; i++)
        {
            var ent = ents[i];
            entsDic[ent.tag] = ent;

            sb.Append(ent.tag);
            if (i + 1 < ents.Count)
            {
                sb.Append(", ");
            }
        }
        if (ents.Count > 0)
        {
            Debug.Log(sb);
        }
    }

    public static CmsEnt Get(string tag)
    {
        return entsDic[tag];
    }
}