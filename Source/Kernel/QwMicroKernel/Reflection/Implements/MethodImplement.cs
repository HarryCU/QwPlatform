﻿
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QwMicroKernel.Reflection.Implements
{
    internal class MethodImplement : MemberImplement, IMethod
    {
        private Func<object, object[], object> _funcInvoker;

        public MethodInfo Method
        {
            get { return Member as MethodInfo; }
        }

        public ICollection<ParameterInfo> Args
        {
            get { return Method.GetParameters(); }
        }

        public bool IsStatic
        {
            get { return Method.IsStatic; }
        }

        public MethodImplement(MethodInfo method)
            : base(method)
        {
        }

        protected override void CreateExpression()
        {
            var methodInfo = Method;

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                _funcInvoker = (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<Func<object, object[], object>>(
                    castMethodCall, instanceParameter, parametersParameter);

                _funcInvoker = lambda.Compile();
            }
        }

        public object Invoke(object instance, params object[] @params)
        {
            if (_funcInvoker != null)
                return _funcInvoker(instance, @params);
            return null;
        }
    }
}
