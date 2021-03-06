// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Lifestyle.Pool
{
	using System;
	using System.Collections.Generic;

	using Castle.Core;
	using Castle.Core.Internal;
	using Castle.MicroKernel.Context;

	[Serializable]
	public class DefaultPool : IPool, IDisposable
	{
		private readonly Stack<object> available = new Stack<object>();
		private readonly List<object> inUse = new List<object>();
		private readonly int initialsize;
		private readonly int maxsize;
		private readonly Lock rwlock = Lock.Create();
		private readonly IComponentActivator componentActivator;
		private readonly IKernel kernel;
		private bool evicting;

		public DefaultPool(int initialsize, int maxsize, IComponentActivator componentActivator, IKernel kernel)
		{
			this.initialsize = initialsize;
			this.maxsize = maxsize;
			this.componentActivator = componentActivator;
			this.kernel = kernel;

			InitPool();
		}

		#region IPool Members

		public virtual object Request(CreationContext context)
		{
			object instance;

			using(rwlock.ForWriting())
			{

				if (available.Count != 0)
				{
					instance = available.Pop();

					if (instance == null)
					{
						throw new PoolException("Invalid instance on the pool stack");
					}
				}
				else
				{
					instance = componentActivator.Create(context);

					if (instance == null)
					{
						throw new PoolException("Activator didn't return a valid instance");
					}
				}

				inUse.Add(instance);
			}

			return instance;
		}

		public virtual bool Release(object instance)
		{
			using(rwlock.ForWriting())
			{
				if (inUse.Contains(instance) == false)
				{
					return evicting;
				}

				inUse.Remove(instance);

				if (available.Count < maxsize)
				{
					if (instance is IRecyclable)
					{
						(instance as IRecyclable).Recycle();
					}

					available.Push(instance);
					
					return false;
				}
				else
				{
					// Pool is full
					componentActivator.Destroy(instance);
					
					return true;
				}
			}
		}

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			evicting = true;

			foreach(object instance in available)
			{
				kernel.ReleaseComponent(instance);
			}
		}

		#endregion

		/// <summary>
		/// Initializes the pool to a initial size by requesting
		/// n components and then releasing them.
		/// </summary>
		private void InitPool()
		{
			List<object> tempInstance = new List<object>();

			for(int i=0; i < initialsize; i++)
			{
				tempInstance.Add(Request(CreationContext.CreateEmpty()));
			}

			for(int i=0; i < initialsize; i++)
			{
				Release(tempInstance[i]);
			}
		}
	}
}
