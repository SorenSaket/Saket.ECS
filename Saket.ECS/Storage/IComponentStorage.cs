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

    public interface IComponentStorage
    {
        /// <summary> The number of components stored </summary>
        public int Capacity { get; }

        /// <summary> The Type of the stored object </summary>
        public Type ComponentType { get; }

        public int ItemSizeInBytes { get; }
        // Safe

        public T Get<T>(in int index) where T : unmanaged;

        public void Set<T>(in int index, in T value) where T : unmanaged;


        // Unsafe 

        /// <summary>
        /// Copy value from pointer to element
        /// </summary>
        /// <param name="index">Index of the element to copy to</param>
        /// <param name="value">Pointer to the item to be copied</param>
        public unsafe void Set(in int index, in void* value);
        
        /// <summary>
        /// Gets a pointer to specific element
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A pointer to element at that index</returns>
        public unsafe void* Get(in int index);


        public void EnsureCapacity(in int requiredCapacity);
    }

}
