using System;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.ECS
{
    // Consider the removal of bunde in favor of just using play type arrays
    // Convert bundle
    public abstract class Bundle
    {
        public abstract Type[] Components { get; }
        public abstract object[] Data { get; }

		public static implicit operator Type[](Bundle b) => b.Components;
	}
}