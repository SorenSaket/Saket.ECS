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
    public  struct Entity
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
        public void Remove(Bundle bundle)
        {
            if (EntityPointer.index_archetype == -1)
            {

            }
            else
            {
                // Move

            }
        }
        
        /// <summary>
        /// Adds component to entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(T value)
             where T : unmanaged
        {
            List<Type> newComponents = new List<Type>() { typeof(T) };
            Archetype currentArchetype = null;
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

        
            // Get archetype
            Archetype newArchetype = World.CreateOrGetArchetype(newComponents.ToArray());
            int entityIndex = newArchetype.AddEntity();
      
            if(currentArchetype != null)
            {
                // Copy old values to new archetype
                for (int i = 0; i < currentArchetype.ComponentTypes.Length; i++)
                {
                    newArchetype.Set(i, entityIndex, currentArchetype.Get(i, entityIndex));
                }
            }
           
            // Set new value
            newArchetype.Set(entityIndex, value);
        }
       
        /// <summary>
        /// Removes component from entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
