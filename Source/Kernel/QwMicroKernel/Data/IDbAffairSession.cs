using System;
using System.Data;

namespace QwMicroKernel.Data
{
    public interface IDbAffairSession : IDbSession
    {
        event EventHandler<DbAffairEventArgs> Error;
        event EventHandler Complute;
        void Begin();
        void Begin(IsolationLevel level);
        void Commit();
    }
}
