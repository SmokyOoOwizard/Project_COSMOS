using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
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
