using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace BJect
{
    public class Resolver : MonoBehaviour
    {
        public Container Container {get;set;}

        public void Init()
        {
            Container = new();
            Container.Bind<Container>().FromInstance(Container).AsSingle();

            Resolve();

            foreach (var o in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                Container.ResolveFields(o);
                Container.ResolveMethods(o);
            } 
        }

        public virtual void Resolve()
        {

        }
    }

    public class Container
    {
        public Dictionary<BindSignature, object> dic = new();

    // public T Resolve<T>()
    // {
    //     return (T)dic[new(){type = typeof(T)}];
    // }
    // public bool TryResolve(Type type, out object obj)
    // {
    //     if (!dic.TryGetValue(new(){type = type}, out obj))
    //     {
    //         Debug.LogError($"Instance of type {type.Name} not found!");
    //         obj = null;
    //         return false;
    //     }
    //     return true;
    // }
    // public bool TryResolve<T>(out T obj) where T : class
    // {
    //     if (!TryResolve(typeof(T), out var o))
    //     {
    //         obj = null;
    //         return false;
    //     }
    //     obj = (T)o;
    //     return true;
    // }

        public BindSignatureBuilder Bind<T>() where T : class
        {
            return new(this, typeof(T));
        }

        public object CreateInstance(BindSignature signature)
        {
            return CreateInstance(signature.type);
        }
        public object CreateInstance(Type type)
        {
            var constructors = type.GetConstructors().OrderBy(c => c.GetParameters().Count());
        
            foreach (var c in constructors)
            {
                bool success = true;
                List<object> args = new();
                foreach (var p in c.GetParameters())
                {
                    BindSignature parameterSignature = new(p.ParameterType);
                    if (p.HasAttribute(typeof(InjectAttribute)))
                    {
                        parameterSignature.tag = p.GetAttribute<InjectAttribute>().Tag;
                    }
                    object obj;
                    if (!dic.TryGetValue(parameterSignature, out obj))
                    {
                        success = false;
                        break;
                    }
                    args.Add(obj);
                }    
                if (success)
                {
                    return Activator.CreateInstance(type, args.ToArray());
                }
            }
            Debug.LogError($"Couldn't find a constructor for {type.Name}!");
            return null;
        }
        public void Register(BindSignature signature, object obj)
        {
            ResolveFields(obj);
            ResolveMethods(obj);
            dic[signature] = obj;
        }

        public void ResolveFields(object obj, List<object> data = null)
        {
            if (obj == null)
            {
                return;
            }
            foreach (var f in obj.GetType().GetFields())
            {
                if (!f.HasAttribute<InjectAttribute>())
                {
                    continue;
                }
                var attrib = f.GetCustomAttribute<InjectAttribute>();
                BindSignature fieldSignature = new(f.FieldType, attrib.Tag);
                object o;
                if (!dic.TryGetValue(fieldSignature, out o))
                {
                    if (data == null)
                    {
                        continue;
                    }
                    int id = data.FindIndex(i => i.GetType() == f.FieldType);
                    if (id < 0)
                    {
                        continue;
                    }
                    o = data[id];
                }
                f.SetValue(obj, o);
            }
        }

        public void ResolveMethods(object obj, List<object> data = null)
        {
            if (obj == null)
            {
                return;
            }
            foreach (var method in obj.GetType().GetMethods())
            {
                var attribs = method.GetCustomAttributes<InjectAttribute>().ToList();
                if (attribs.Count <= 0)
                {
                    continue;
                }
                
                var param = method.GetParameters().ToList();
                object[] args = new object[param.Count];

                bool success = true;
                for (int i = 0; i < param.Count; i++)
                {
                    var parameter = param[i];
                    BindSignature parameterSignature = new(parameter.ParameterType);
                    if (parameter.HasAttribute<InjectAttribute>())
                    {
                        parameterSignature.tag = parameter.GetAttribute<InjectAttribute>().Tag;
                    }

                    object o;
                    if (!dic.TryGetValue(parameterSignature, out o))
                    {
                        if (data == null)
                        {
                            success = false;
                            break;
                        }
                        int id = data.FindIndex(t => t.GetType() == parameter.ParameterType);
                        if (id < 0)
                        {
                            success = false;
                            break;
                        }
                        o = data[id];
                    }
                    args[i] = o;
                }

                if (success)
                {
                    method.Invoke(obj, args.ToArray());
                    break;
                }
            }
        }

        public GameObject Instantiate(GameObject pfb, Transform root = null, List<object> data = null)
        {
            var go = GameObject.Instantiate(pfb, root);
            foreach (var m in go.GetComponents<MonoBehaviour>())
            {
                ResolveFields(m, data);
                ResolveMethods(m, data);
            }
            return go;
        }
        public T Instantiate<T>(T pfb, Transform root = null, List<object> data = null) where T : UnityEngine.Object
        {
            var scr = GameObject.Instantiate<T>(pfb, root);
            foreach (var m in scr.GetComponents<MonoBehaviour>())
            {
                ResolveFields(m, data);
                ResolveMethods(m, data);
            }
            return scr;
        }

        public T Instantiate<T>(GameObject pfb, Transform root = null, List<object> data = null) where T : MonoBehaviour
        {
            return Instantiate(pfb, root, data).GetComponent<T>();
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }
        public object Create(Type type)
        {
            var obj = CreateInstance(type);
            ResolveFields(obj);
            ResolveMethods(obj);
            return obj;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public string Tag {get;set;} = null;
    }

    public class BindSignatureBuilder
    {
        public Container container;
        public BindSignature signature;

        public object obj;

        public BindSignatureBuilder(Container container, Type type)
        {
            this.container = container;
            signature = new()
            {
                type = type,
            };
        }

        public BindSignatureBuilder WithTag(string tag)
        {
            signature.tag = tag;
            return this;
        }

        public void AsSingle()
        {
            if (obj == null)
            {
                obj = container.CreateInstance(signature);
            }
            container.Register(signature, obj);
        }

        public BindSignatureBuilder FromInstance(object obj)
        {
            this.obj = obj;
            return this;
        }
    }

    public struct BindSignature
    {
        public string tag;
        public Type type;

        public BindSignature(Type type, string tag = default)
        {
            this.type = type;
            this.tag = tag;
        }
    }

    public class BaseFactory
    {
        public Container container;

        public BaseFactory(Container container)
        {
            this.container = container;
        }
    }

    public class TemplateGameObjectFactory<T> : BaseFactory where T : UnityEngine.Object
    {
        public T pfb;

        public TemplateGameObjectFactory(Container container, T pfb) : base(container)
        {
            this.pfb = pfb;
        }

        public T Use(Transform root = null)
        {
            return container.Instantiate(pfb, root);
        }
    }
    public class TemplateGameObjectFactory : TemplateGameObjectFactory<UnityEngine.Object>
    {
        public TemplateGameObjectFactory(Container container, UnityEngine.Object pfb) : base(container, pfb)
        {
        }
    }
}