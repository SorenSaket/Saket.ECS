using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    /// <summary>
    /// A Handle to an entity
    /// </summary>
    public struct Entity
    {
        public int ID => EntityPointer.ID;
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
            HashSet<Type> newComponents = new HashSet<Type>();
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
                newComponents.UnionWith(currentArchetype.ComponentTypes);
            }
            // Add new Component to the back
            newComponents.Add(typeof(T));

            // Get New archetype
            Archetype newArchetype = World.CreateOrGetArchetype(newComponents, out int newArchetypeIndex);
            int entityIndex = newArchetype.AddEntity();

            // Copy old values to new archetype
            if (currentArchetype != null)
            {
                CopyAllComponentsToArchetype(newArchetype, EntityPointer.index_row);
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
        public Entity Remove<T>()
             where T : unmanaged
        {
            throw new NotImplementedException();
        }


        public Entity Add(Bundle bundle)
        {
            HashSet<Type> newComponents = new HashSet<Type>();
            Archetype currentArchetype = null;

            // If the archtype is not unassigned
            if (EntityPointer.index_archetype != -1)
            {
                currentArchetype = World.archetypes[EntityPointer.index_archetype];
                
                for (int i = 0; i < bundle.Components.Length; i++)
                {
                    // Entity already has component
                    if (currentArchetype.Has(bundle.Components[i]))
                    {
                        // TODO Consider not making exception
                        throw new InvalidOperationException("Entity already has component");
                    }
                }

                // Get or create new archetype
                newComponents.UnionWith(currentArchetype.ComponentTypes);
            }

            // Add new Component to the back
            newComponents.UnionWith(bundle.Components);

            Archetype newArchetype = World.CreateOrGetArchetype(newComponents, out int newArchetypeIndex);
            int entityIndex = newArchetype.AddEntity();

            // Copy old values to new archetype
            if (currentArchetype != null)
            {
                CopyAllComponentsToArchetype(newArchetype, EntityPointer.index_row);
                // Remove from old  
                currentArchetype.RemoveEntity(EntityPointer.index_row);
            }

            // set new values 
            for (int i = 0; i < bundle.Components.Length; i++)
            {
				// See to learn more : https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.gchandle.tointptr?view=net-6.0
				// Cannot handle bools 
				// https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
				var hdl = GCHandle.Alloc(bundle.Data[i],GCHandleType.Pinned);
                newArchetype.Set(bundle.Components[i], entityIndex, hdl.AddrOfPinnedObject());
                hdl.Free();
            }

            EntityPointer = new EntityPointer(EntityPointer.ID, EntityPointer.version, newArchetypeIndex, entityIndex);
            // ALSO CHANGE IN THE WORLD
            World.entities[EntityPointer.ID] = EntityPointer;
            return this;
        }
        public Entity Remove(Bundle bundle)
        {
            return this;
        }



        internal void CopyAllComponentsToArchetype(Archetype target, int row)
        {
#if DEBUG
            if(EntityPointer.index_archetype == -1)
            {
                throw new Exception("Cannot copy to archetype since entity is not registered in any archetype");
            }
#endif
            Archetype currentArchetype = World.archetypes[EntityPointer.index_archetype];

            foreach (var storage_from in currentArchetype.storage)
            {
                if (target.storage.TryGetValue(storage_from.Value.ComponentType, out var storage_to))
                {
                    storage_from.Value.CopyTo(row, storage_to.Get(row));
                }
            }
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
