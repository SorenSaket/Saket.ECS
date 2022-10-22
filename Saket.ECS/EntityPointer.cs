﻿using System;

namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public struct EntityPointer 
    {
        public static readonly EntityPointer Default = new EntityPointer();
        // This should never change after construction
        public readonly int ID;
        /// <summary>
        /// Current version
        /// </summary>
        public int version;
        /// <summary>
        /// 
        /// </summary>
        public int index_archetype;
        public int index_row;
        public EntityPointer()
        {
            this.ID = -1;
            this.version = -1;
            this.index_archetype = -1;
            this.index_row = -1;
        }
        public EntityPointer(int ID, int version = 0, int index_archetype = -1, int index_row = -1)
        {
            this.ID = ID;
            this.version = version;
            this.index_archetype = index_archetype;
            this.index_row = index_row;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityPointer pointer && Equals(pointer);
        }
        public bool Equals(EntityPointer other)
        {
            return ID == other.ID && version == other.version;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ID,version);
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