using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS.Storage
{
    public enum ComponentStorageType
    {
        AOS,
        SOA
    }

    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentStorageAttribute : Attribute
    {
        public readonly ComponentStorageType Type;
        public ComponentStorageAttribute(ComponentStorageType type)
        {
            this.Type = type;
        }
    }

    public  interface IComponentStorage
    {
        /// <summary> The number of components stored </summary>
        public int Capacity { get; }

        /// <summary> The Type of the stored object </summary>
        public Type ComponentType { get; }

        public T Get<T>(in int index) where T : unmanaged;

        public void Set<T>(in int index, in T value) where T : unmanaged;


        public void CopyTo(in int index, in IntPtr destination);
        public void Set(in int index,in IntPtr value);
        public IntPtr Get(in int index);
    }

}
