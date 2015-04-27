using System;

namespace QwMicroKernel
{
    public class ActionDisposer : Disposer
    {
        private readonly Action _action;

        public ActionDisposer(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

        protected override void Release()
        {
            _action();
        }
    }
}
