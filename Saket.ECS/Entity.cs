using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
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


        
        /// <summary>
        /// Adds component to entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Entity Add<T>(T value)
             where T : unmanaged
        {
            List<Type> newComponents = new List<Type>();
            Archetype currentArchetype = null;
            // If the archtype is not unassigned
            if (EntityPointer.index_archetype != -1)
            {
                currentArchetype = World.archetypes[EntityPointer.index_archetype];

                // Entity already has component
                if (currentArchetype.Has<T>())
                {
                    // TODO Consider not making exception
                    throw new InvalidOperationException("Entity already has component");
                }

                // Get or create new archetype
                newComponents.AddRange(currentArchetype.ComponentTypes);
            }
            // Add new Component to the back
            newComponents.Add(typeof(T));

            // Get archetype
            Archetype newArchetype = World.CreateOrGetArchetype(newComponents.ToArray(), out int newArchetypeIndex);
            int entityIndex = newArchetype.AddEntity();

            // Copy old values to new archetype
            if (currentArchetype != null)
            {
                // This can be done since the order of the components are the same
                for (int i = 0; i < currentArchetype.ComponentTypes.Length; i++)
                {
                    IntPtr source = currentArchetype.Get(i, EntityPointer.index_row);
                    newArchetype.Set(i, entityIndex, source);
                }

                // Remove from old  
                currentArchetype.RemoveEntity(EntityPointer.index_row);
            }
           
            // Set new value
            newArchetype.Set(entityIndex, value);

            // change the entity pointer

            EntityPointer = new EntityPointer(EntityPointer.ID, EntityPointer.version, newArchetypeIndex, entityIndex);
            // ALSO CHANGE IN THE WORLD
            World.entities[EntityPointer.ID] = EntityPointer;
            return this;
        }

        /// <summary>
        /// Removes component from entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Remove<T>()
             where T : unmanaged
        {
            throw new NotImplementedException();
        }


        public void Add(Bundle bundle)
        {
            throw new NotImplementedException();
        }
        public void Remove(Bundle bundle)
        {
            throw new NotImplementedException();
        }


        public T Get<T>()
            where T : unmanaged
        {
            return World.archetypes[EntityPointer.index_archetype].Get<T>(EntityPointer.index_row);
        }

        public T? TryGet<T>()
          where T : unmanaged
        {
            // TODO Check if has component

            return World.archetypes[EntityPointer.index_archetype].Get<T>(EntityPointer.index_row);
        }

        public void Set<T>(T value)
             where T : unmanaged
        {
            World.archetypes[EntityPointer.index_archetype].Set<T>(EntityPointer.index_row, value);
        }
    }

}
