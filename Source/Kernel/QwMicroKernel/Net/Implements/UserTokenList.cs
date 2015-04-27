using System.Collections.Generic;

namespace QwMicroKernel.Net.Implements
{
    internal sealed class UserTokenList
    {
        private readonly IList<UserToken> _list;

        public UserTokenList()
        {
            _list = new List<UserToken>();
        }

        public void Add(UserToken userToken)
        {
            lock (_list)
            {
                _list.Add(userToken);
            }
        }

        public void Remove(UserToken userToken)
        {
            lock (_list)
            {
                _list.Remove(userToken);
            }
        }
    }
}
