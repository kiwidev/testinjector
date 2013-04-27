// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TestInjector.Client;

namespace TestInjector
{
    public static class ApplicationInstanceExtensions
    {
        public static void Run(this ApplicationInstance applicationInstance,
                               Action staticMethodToExecute, 
            TestInjectorSettings testInjectorSettings = null)
        {
            var methodInfo = staticMethodToExecute.GetMethodInfo();
            if (!methodInfo.IsStatic)
                throw new InvalidOperationException("Method must be static - don't reference any outside parameters");


            RunMethodImpl(applicationInstance, methodInfo, testInjectorSettings);
        }

        public static void Run(this ApplicationInstance applicationInstance,
                               Func<Task> staticMethodToExecute,
            TestInjectorSettings testInjectorSettings = null)
        {
            var methodInfo = staticMethodToExecute.GetMethodInfo();
            if (!methodInfo.IsStatic)
                throw new InvalidOperationException("Method must be static - don't reference any outside parameters");


            RunMethodImpl(applicationInstance, methodInfo, testInjectorSettings);
        }

        public static void RunMethod(this ApplicationInstance applicationInstance,
                                     Expression<Action> staticMethodToExecute,
            TestInjectorSettings testInjectorSettings = null)
        {
            //Retrieve the property that has changed 
            var body = staticMethodToExecute.Body as System.Linq.Expressions.MethodCallExpression;
            if (body == null)
                throw new InvalidOperationException("Could not convert expression into a method call");
            var method = body.Method;

            if (method == null)
                throw new InvalidOperationException("Could not convert expression into a method call");

            RunMethodImpl(applicationInstance, method, testInjectorSettings);
        }

        public static void RunMethod(this ApplicationInstance applicationInstance,
                                     Expression<Func<Task>> staticMethodToExecute,
            TestInjectorSettings testInjectorSettings = null)
        {
            //Retrieve the property that has changed 
            var body = staticMethodToExecute.Body as System.Linq.Expressions.MemberExpression;
            if (body == null)
                throw new InvalidOperationException("Could not convert expression into a method call");

            var method = body.Member as MethodInfo;
            if (method == null)
                throw new InvalidOperationException("Could not convert expression into a method call");

            RunMethodImpl(applicationInstance, method, testInjectorSettings);
        }

        private static void RunMethodImpl(ApplicationInstance applicationInstance, MethodInfo method, TestInjectorSettings testInjectorSettings)
        {
            if (!method.IsStatic)
                throw new InvalidOperationException("Method must be static");

            new TestInjectorClient().RunMethod(applicationInstance.Process,
                                               method.DeclaringType,
                                               method.Name,
                                               testInjectorSettings ?? TestInjectorSettings.Default);
        }

    }
}
