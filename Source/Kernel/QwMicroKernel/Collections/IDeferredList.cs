using System.Collections;
using System.Collections.Generic;

namespace QwMicroKernel.Collections
{
    public interface IDeferredList : IList, IDeferLoadable
    {
    }

    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IDeferredList<T> : IList<T>, IDeferredList
    {
    }
}