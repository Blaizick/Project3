using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        if (comps == null)
            return null;
        var i = comps.Find(p => p != null && type.IsAssignableFrom(p.GetType()));
        if (i == null)
        {
            UnityEngine.Debug.LogError($"No component of type: {type.Name} found on entity with tag: {tag}!");
        }
        return i;
    }
    public T Get<T>() where T : CmsComp
    {
        return (T)Get(typeof(T));
    }

    public List<T> GetAll<T>() where T : CmsComp
    {
        if (comps == null)
            return new();
        var ret = comps.
            FindAll(p => p != null && typeof(T).IsAssignableFrom(p.GetType())).
            Cast<T>().
            ToList();
        return ret == null ? new() : ret;
    }

    public bool TryGet(Type type, out object obj)
    {
        obj = null;
        if (comps == null)
            return false;
        int id = comps.FindIndex(p => p != null && type.IsAssignableFrom(p.GetType()));
        if (id <= 0)
        {
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

    public bool Has(Type type)
    {
        if (comps == null)
            return false;
        return comps.FindIndex(p => p != null && type.IsAssignableFrom(p.GetType())) > -1;
    }
    public bool Has<T>() where T : CmsComp
    {
        return Has(typeof(T));
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
        Stopwatch sw = Stopwatch.StartNew();

        entsDic.Clear();
        
        var ents = Resources.LoadAll<CmsEnt>(string.Empty).ToList();
        StringBuilder sb = new();
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
            UnityEngine.Debug.Log($"Loaded {ents.Count} entities in {sw.ElapsedMilliseconds} ms.\n Entities: {sb}");
        }
    }

    public static CmsEnt Get(string tag)
    {
        return entsDic[tag];
    }
}