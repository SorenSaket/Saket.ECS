using System;

namespace Saket.ECS
{
    /// <summary>
    /// Consists of an ID and version Number
    /// </summary>
    public struct EntityPointer 
    {
        public static readonly EntityPointer Default = new(-1,-1);
        

        public bool Valid
        {
            get => ID >= 0 && Version >= 0;
                
        }

        public int ID;
        /// <summary>
        /// Current version
        /// </summary>
        public int Version;

        public EntityPointer(int ID, int version = 0)
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