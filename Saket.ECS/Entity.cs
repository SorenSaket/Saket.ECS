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
            if(EntityPointer.index_archetype == -1)
            {

            }
            else
            {
                // Move

            }
        }

        public void Add<T>()
             where T : unmanaged
        {
            // Entity already has component
            // return warning
            // Get or create new archetype
            // Add Component 
        }

        public void Remove<T>()
             where T : unmanaged
        {

        }

        public T Get<T>()
            where T : unmanaged
        {
            return World.archetypes[EntityPointer.index_archetype].Get<T>(EntityPointer.index_row);
        }

        public T? TryGet<T>()
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
