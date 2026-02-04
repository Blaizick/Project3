using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace BJect
{
    public class Resolver : MonoBehaviour
    {
        public DiContainer Container {get;set;}

        public void Init()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Container = new();
            Container.Bind<DiContainer>().FromInstance(Container).AsSingle();

            Resolve();

            foreach (var o in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                Container.InjectFields(o);
                Container.InjectMethods(o);
            } 

            UnityEngine.Debug.Log($"Resolved {Container.resolveCounter} dependencies in {sw.ElapsedMilliseconds} ms.");
        }

        public virtual void Resolve()
        {
            
        }
    }

    public class DiContainer
    {
        public Dictionary<BindSignature, List<object>> neededDic = new();
        public Dictionary<BindSignature, object> dic = new();

        public int resolveCounter = 0;

        public T Resolve<T>()
        {
            return (T)dic[new(typeof(T))];
        }
        public bool TryResolve(Type type, out object obj)
        {
            if (!dic.TryGetValue(new(){type = type}, out obj))
            {
                UnityEngine.Debug.LogError($"Instance of type {type.Name} not found!");
                obj = null;
                return false;
            }
            return true;
        }
        public bool TryResolve<T>(out T obj) where T : class
        {
            if (!TryResolve(typeof(T), out var o))
            {
                obj = null;
                return false;
            }
            obj = (T)o;
            return true;
        }

        public BindSignatureBuilder Bind<T>() where T : class
        {
            return new(this, typeof(T));
        }

        public object CreateInstance(BindSignature signature, List<object> data = null)
        {
            return CreateInstance(signature.type, data);
        }
        public object CreateInstance(Type type, List<object> data = null)
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
                        if (data == null)
                        {
                            success = false;
                            break;
                        }
                        int id = data.FindIndex(i => p.ParameterType.IsAssignableFrom(i.GetType()));
                        if (id < 0)
                        {
                            success = false;
                            break;
                        }
                        obj = data[id];
                    }
                    args.Add(obj);
                }    
                if (success)
                {
                    resolveCounter++;
                    return Activator.CreateInstance(type, args.ToArray());
                }
            }
            UnityEngine.Debug.LogError($"Couldn't find a constructor for {type.Name}!");
            return null;
        }
        public void Register(BindSignature signature, object obj)
        {
            InjectFields(obj);
            InjectMethods(obj);
            
            dic[signature] = obj;
            
            if (neededDic.TryGetValue(signature, out var l))
            {
                foreach (var o in l)
                {
                    InjectFields(o);
                }
                neededDic.Remove(signature);
            }
        }

        public void InjectFields(object obj, List<object> data = null)
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
                        AddNeeded(fieldSignature, obj);
                        continue;
                    }
                    int id = data.FindIndex(i => i.GetType() == f.FieldType);
                    if (id < 0)
                    {
                        AddNeeded(fieldSignature, obj);
                        continue;
                    }
                    o = data[id];
                }
                resolveCounter++;
                f.SetValue(obj, o);
            }
        }

        private void AddNeeded(BindSignature signature, object obj)
        {
            List<object> l;
            if (!neededDic.TryGetValue(signature, out l))
            {
                l = new();
                neededDic[signature] = l;
            }
            if (!l.Contains(obj))
            {
                l.Add(obj);
            }
        }
        public void InjectMethods(object obj, List<object> data = null)
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
                    resolveCounter++;
                    method.Invoke(obj, args.ToArray());
                    return;;
                }
            }
        }

        public GameObject Instantiate(GameObject pfb, Transform root = null, List<object> data = null)
        {
            var go = GameObject.Instantiate(pfb, root);
            foreach (var m in go.GetComponents<MonoBehaviour>())
            {
                InjectFields(m, data);
                InjectMethods(m, data);
            }
            return go;
        }
        public T Instantiate<T>(T pfb, Transform root = null, List<object> data = null) where T : UnityEngine.Object
        {
            var scr = GameObject.Instantiate<T>(pfb, root);
            foreach (var m in scr.GetComponents<MonoBehaviour>())
            {
                InjectFields(m, data);
                InjectMethods(m, data);
            }
            return scr;
        }

        public T Instantiate<T>(GameObject pfb, Transform root = null, List<object> data = null) where T : MonoBehaviour
        {
            return Instantiate(pfb, root, data).GetComponent<T>();
        }

        public T Create<T>(List<object> data = null)
        {
            return (T)Create(typeof(T), data);
        }
        public object Create(Type type, List<object> data)
        {
            var obj = CreateInstance(type, data);
            InjectFields(obj, data);
            InjectMethods(obj, data);
            return obj;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public string Tag {get;set;} = default;
    }

    public class BindSignatureBuilder
    {
        public DiContainer container;
        public BindSignature signature;

        public object obj;

        public BindSignatureBuilder(DiContainer container, Type type)
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
        public DiContainer container;

        public BaseFactory(DiContainer container)
        {
            this.container = container;
        }
    }

    public class TemplateGameObjectFactory<T> : BaseFactory where T : UnityEngine.Object
    {
        public T pfb;

        public TemplateGameObjectFactory(DiContainer container, T pfb) : base(container)
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
        public TemplateGameObjectFactory(DiContainer container, UnityEngine.Object pfb) : base(container, pfb)
        {
        }
    }
}