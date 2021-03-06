// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.TypedFactory.Internal
{
	using System;

	using Castle.Core.Internal;
	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Generators;
	using Castle.MicroKernel;

	public class DelegateProxyFactory : IProxyFactoryExtension
	{
		private readonly Type targetDelegateType;

		public DelegateProxyFactory(Type targetDelegateType)
		{
			this.targetDelegateType = targetDelegateType;
		}

		public object Generate(IProxyBuilder builder, ProxyGenerationOptions options, IInterceptor[] interceptors)
		{
			var type = GetProxyType(builder);
			var instance = GetProxyInstance(type, interceptors);
			var method = GetInvokeDelegate(instance);
			return method;
		}

		private object GetInvokeDelegate(object instance)
		{
			return Delegate.CreateDelegate(targetDelegateType, instance, "Invoke");
		}

		private object GetProxyInstance(Type type, IInterceptor[] interceptors)
		{
			return type.CreateInstance<object>(null, interceptors);
		}

		private Type GetProxyType(IProxyBuilder builder)
		{
			var scope = builder.ModuleScope;
			var logger = builder.Logger;
			var generator = new DelegateProxyGenerator(scope, targetDelegateType)
			{
				Logger = logger
			};
			return generator.GetProxyType();
		}
	}
}