using System;
using System.Threading.Tasks;
using PostSharp.Serialization;

namespace Utilities.Caching.Aspects.Helpers
{
    class Extensions
    {



        [PSerializable]
        public class Helper<T> : IHelper
        {
            public object GetReturnValue(object returnValue)
            {
                return Task.FromResult<T>((T)returnValue);
            }
            public object GetReturnValue(Task task, Action<object> continueWith)
            {
                TaskCompletionSource<T> completion = new TaskCompletionSource<T>();

                task.ContinueWith(
                    t =>
                    {
                        //logAndNotify(t.Exception);
                        completion.SetResult(default(T));
                    }, TaskContinuationOptions.NotOnRanToCompletion);

                task.ContinueWith(
                    t =>
                    {
                        Task<T> completed = t as Task<T>;
                        T result = default(T);
                        if (completed != null)
                            result = completed.Result;

                        continueWith(result);
                        completion.SetResult(result);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                return completion.Task;
            }
        }
        public interface IHelper
        {
            object GetReturnValue(object returnValue);
            //object GetReturnValue(Task task, Action<Exception> logAndNotify);
            object GetReturnValue(Task task, Action<object> continueWith);
        }


    }
}
