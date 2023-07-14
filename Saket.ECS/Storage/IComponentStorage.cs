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

    /// <summary>
    /// 
    /// </summary>
    public interface IComponentStorage
    {
        /// <summary> The number of components stored </summary>
        public int Capacity { get; }

        /// <summary> The Type of the stored object </summary>
        public Type ComponentType { get; }

        public int ItemSizeInBytes { get; }
        // Safe

        public T Get<T>(int index) where T : unmanaged;
        public ref T GetRef<T>(int index) where T : unmanaged;

        public void Set<T>(int index, in T value) where T : unmanaged;


        // Unsafe 

        /// <summary>
        /// Copy value from pointer to element
        /// </summary>
        /// <param name="index">Index of the element to copy to</param>
        /// <param name="value">Pointer to the item to be copied</param>
        public unsafe void Set(int index, void* value);
        
        /// <summary>
        /// Gets a pointer to specific element
        /// </summary>
        /// <param name="index"></param>
        /// <returns>A pointer to element at that index</returns>
        public unsafe void* Get(int index);

        /// <summary>
        /// Ensure capacity of number of components
        /// </summary>
        /// <param name="requiredCapacity">The requied number of components</param>
        public void EnsureCapacity(int requiredCapacity);

        public unsafe void CloneTo(IComponentStorage other)
        {
            other.EnsureCapacity(Capacity);
            for (int i = 0; i < Capacity; i++)
            {
                other.Set(i, this.Get(i));
            }
        }


        /// <summary>
        /// Zeros out all associated data with component
        /// </summary>
        /// <param name="index"></param>
        public void Zero(int index);
    }

}
