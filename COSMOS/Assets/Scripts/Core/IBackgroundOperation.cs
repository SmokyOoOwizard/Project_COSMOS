using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public class SuccessfulBackgroundOperation<T> : IBackgroundObjectOperation<T>
    {
        public T TObject { get; }

        public object Object => TObject;

        public float Percent { get; } = 1;

        public bool IsDone { get; } = true;

        public SuccessfulBackgroundOperation(T obj)
        {
            TObject = obj;
        }
    }
    public interface IBackgroundOperation
    {
        float Percent { get; }
        bool IsDone { get; }
    }
    public interface IBackgroundObjectOperation : IBackgroundOperation
    {
        object Object { get; }
    }
    public interface IBackgroundObjectOperation<T> : IBackgroundObjectOperation
    {
        T TObject { get; }
    }
}
