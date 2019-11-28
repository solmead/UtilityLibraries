using System;
using System.Reflection;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using Utilities.Caching.Aspects.Naming;
using Extensions = Utilities.Caching.Aspects.Helpers.Extensions;

namespace Utilities.Caching.Aspects
{
    [PSerializable]
    [MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
    public class CacheFunction : OnMethodBoundaryAspect
    {
        private enum MethodReturnType { Void, Task, TaskOfT, Other }
        private MethodReturnType returnType;
        private Extensions.IHelper helper;

        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public ICacheEntryNamer Namer { get; set; }

        public CacheFunction()
        {
           // ApplyToStateMachine = false;
        }

        public ICacheEntryNamer GetNamer()
        {
            if (Namer == null)
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    Namer = new StringNamer(Name);
                }
                if (Namer == null)
                {
                    Namer = new BaseNamer();
                }
            }
            return Namer;
        }

        public override void CompileTimeInitialize(System.Reflection.MethodBase method, AspectInfo aspectInfo)
        {
            base.CompileTimeInitialize(method, aspectInfo);
            MethodInfo info = method as MethodInfo;
            if (info.ReturnType == typeof(void))
                returnType = MethodReturnType.Void;
            else if (info.ReturnType.IsGenericType && info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = MethodReturnType.TaskOfT;
                var taskTResult = info.ReturnType.GetGenericArguments()[0];
                helper = Activator.CreateInstance(typeof(Extensions.Helper<>).MakeGenericType(taskTResult)) as Extensions.IHelper;

            }
            else if (info.ReturnType == typeof(Task))
            {
                returnType = MethodReturnType.Task;
                helper = new Extensions.Helper<bool>();
            }
            else
                returnType = MethodReturnType.Other;
        }



        public override void OnEntry(MethodExecutionArgs args)
        {

            
            var name = GetNamer().GetName(BaseName, args);
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReturnType;
                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];
                    var ret = Cache.GetItem(CacheArea, name, retType, null, Tags);

                    if (ret != null)
                    {
                        if ((returnType == MethodReturnType.Task || returnType == MethodReturnType.TaskOfT))
                        {
                            args.ReturnValue = helper.GetReturnValue(ret);
                        }
                        args.FlowBehavior = FlowBehavior.Return;
                    }
                    
                }
                else
                {
                    var ret = Cache.GetItem(CacheArea, name, retType, null, Tags);
                    if (ret != null)
                    {
                        args.ReturnValue = ret;
                        args.FlowBehavior = FlowBehavior.Return;
                    }
                }
                
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            
            var name = GetNamer().GetName(BaseName, args);
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReturnType;


                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];

                    if ((returnType == MethodReturnType.Task || returnType == MethodReturnType.TaskOfT))
                    {
                        Task tsk = args.ReturnValue as Task;
                        args.ReturnValue = helper.GetReturnValue(tsk, t =>
                        {
                            Cache.SetItem(CacheArea, name, retType, t, Tags);
                        });
                    }
                    
                }
                else
                {
                    Cache.SetItem(CacheArea, name, retType, args.ReturnValue, Tags);
                }
            }


        }

        
    }



}
