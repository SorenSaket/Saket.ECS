namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public struct EntityPointer 
    {
        /// <summary>
        /// Current version
        /// </summary>
        public int version = 0;
        /// <summary>
        /// 
        /// </summary>
        public int index_archetype = -1;
        public int index_row = -1;

        public EntityPointer(int version, int index_archetype, int index_row)
        {
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
            return version == other.version &&
                   index_archetype == other.index_archetype &&
                   index_row == other.index_row;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(version, index_archetype, index_row);
        }
    }
}