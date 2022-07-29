namespace Saket.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public struct EntityPointer 
    {
        public int ID;
        /// <summary>
        /// Current version
        /// </summary>
        public int version;
        /// <summary>
        /// 
        /// </summary>
        public int index_archetype;
        public int index_row;

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
    }
}