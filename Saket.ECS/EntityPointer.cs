using System;
using System.Runtime.CompilerServices;

namespace Saket.ECS
{
    /// <summary>
    /// Consists of an ID and version Number
    /// </summary>
    public struct EntityPointer 
    {
        public static readonly EntityPointer Default = new(-1,0);
        
        /// <summary>
        /// If the Pointer is pointing to a potentionally valid entity
        /// Note that this check doesn't prevent the pointer from being out of range
        /// </summary>
        public bool Valid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ID >= 0;        
        }

        /// <summary>
        /// ID, Index into world
        /// </summary>
        public int ID;
        /// <summary>
        /// Current version
        /// </summary>
        public uint Version;

        public EntityPointer(int ID, uint version = 0)
        {
            this.ID = ID;
            this.Version = version;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityPointer pointer && Equals(pointer);
        }
        public bool Equals(EntityPointer other)
        {
            return ID == other.ID && Version == other.Version;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ID,Version);
        }

        public static bool operator ==(EntityPointer left, EntityPointer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityPointer left, EntityPointer right)
        {
            return !(left == right);
        }
    }
}