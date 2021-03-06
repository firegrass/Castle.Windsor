﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using Castle.Core;

	public partial class DefaultKernel
	{
		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			var handler = (this as IKernelInternal).LazyLoadComponentByKey(key, service, null);
			if (handler == null)
			{
				throw new ComponentNotFoundException(key);
			}

			return ResolveComponent(handler, service);
		}

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, IDictionary arguments)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}

			var handler = (this as IKernelInternal).LazyLoadComponentByKey(key, service, arguments);
			if (handler == null)
			{
				throw new ComponentNotFoundException(key);
			}

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		///   Returns the component instance by the service type
		///   using dynamic arguments
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public T Resolve<T>(IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType, arguments);
		}

		/// <summary>
		///   Returns the component instance by the service type
		///   using dynamic arguments
		/// </summary>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Resolve<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		///   Returns the component instance by the component key
		/// </summary>
		/// <returns></returns>
		public T Resolve<T>()
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType);
		}

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name="key">Component's key</param>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>
		///   The Component instance
		/// </returns>
		public T Resolve<T>(String key)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType);
		}

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="key">Component's key</param>
		/// <param name="arguments"></param>
		/// <returns>
		///   The Component instance
		/// </returns>
		public T Resolve<T>(String key, IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType, arguments);
		}

		/// <summary>
		///   Returns the component instance by the service type
		/// </summary>
		public object Resolve(Type service)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}

			var handler = (this as IKernelInternal).LazyLoadComponentByType(null, service, null);
			if (handler == null)
			{
				throw new ComponentNotFoundException(service);
			}

			return ResolveComponent(handler, service);
		}

		/// <summary>
		///   Returns the component instance by the service type
		///   using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(Type service, IDictionary arguments)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}

			var handler = (this as IKernelInternal).LazyLoadComponentByType(null, service, arguments);
			if (handler == null)
			{
				throw new ComponentNotFoundException(service);
			}

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		///   Returns the component instance by the service type
		///   using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		///   Returns the component instance by the component key
		///   using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(string key, IDictionary arguments)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}

			var handler = (this as IKernelInternal).LazyLoadComponentByKey(key, null, arguments);
			if (handler == null)
			{
				throw new ComponentNotFoundException(key);
			}

			return ResolveComponent(handler, arguments);
		}

		/// <summary>
		///   Returns the component instance by the component key
		///   using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(string key, object argumentsAsAnonymousType)
		{
			return Resolve(key, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		///   Returns all the valid component instances by
		///   the service type
		/// </summary>
		/// <param name="service">The service type</param>
		public Array ResolveAll(Type service)
		{
			return ResolveAll(service, new Arguments());
		}

		/// <summary>
		///   Returns all the valid component instances by
		///   the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="arguments">
		///   Arguments to resolve the services
		/// </param>
		public Array ResolveAll(Type service, IDictionary arguments)
		{
			var resolved = new Dictionary<IHandler, object>();
			foreach (var handler in GetAssignableHandlers(service))
			{
				if (handler.IsBeingResolvedInContext(currentCreationContext))
				{
					continue;
				}

				var component = TryResolveComponent(handler, service, arguments);
				if (component != null)
				{
					resolved.Add(handler, component);
				}
			}

			var components = Array.CreateInstance(service, resolved.Count);
			((ICollection)resolved.Values).CopyTo(components, 0);
			return components;
		}

		/// <summary>
		///   Returns all the valid component instances by
		///   the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="argumentsAsAnonymousType">
		///   Arguments to resolve the services
		/// </param>
		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		///   Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(object argumentsAsAnonymousType)
		{
			return (TService[])ResolveAll(typeof(TService), argumentsAsAnonymousType);
		}

		/// <summary>
		///   Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(IDictionary arguments)
		{
			return (TService[])ResolveAll(typeof(TService), arguments);
		}

		/// <summary>
		///   Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		public TService[] ResolveAll<TService>()
		{
			return (TService[])ResolveAll(typeof(TService), new Arguments());
		}
	}
}