using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace engine.ecs
{
    /// <summary>
    /// A Handle to an entity
    /// Allows 
    /// </summary>
    public struct Entity
    {
        public EntityPointer EntityPointer { get; private set; }
        public readonly World World { get; }

        internal Entity(World world, EntityPointer pointer)
        {
            this.World = world;
            this.EntityPointer = pointer;
        }


        public void Add(Bundle bundle)
        {

        }

        public void Add<T>()
        {

        }
        public void Remove<T>()
        {

        }

        public T Get<T>()
            where T : unmanaged
        {
            return World.archetypes[EntityPointer.index_archetype].Get<T>(EntityPointer.index_row);
        }

        public void Set<T>(T value)
             where T : unmanaged
        {
            World.archetypes[EntityPointer.index_archetype].Set<T>(EntityPointer.index_row, value);
        }

    }

}
