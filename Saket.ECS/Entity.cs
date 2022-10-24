using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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
        public bool Destroyed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => EntityPointer.ID == -1;
        } 
        public int ID {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => EntityPointer.ID; 
        }
        internal int Archetype
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => World.entities[ID].Archetype;
        }
        internal int Row
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => World.entities[ID].Row;
        }


        public Pointer EntityPointer { get => entityPointer; set{ entityPointer = value; } }


        public readonly World World { get; }

        private Pointer entityPointer;

        internal Entity(World world, Pointer pointer)
        {
            this.World = world;
            entityPointer = pointer;
        }

        public void Destroy()
        {
            World.DestroyEntity(ref entityPointer);
        }

        
        /// <summary>
        /// Adds component to entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Entity Add<T>(T value)
             where T : unmanaged
        {
			HashSet<Type> newComponents = GetExsistingComponentTypes();
			if (newComponents.Contains(typeof(T)))
			{
				// TODO Consider not making exception
				throw new InvalidOperationException("Entity already has this component");
			}
			newComponents.Add(typeof(T));

			MoveToNewArchetype(newComponents, out var newArchetype);

            // Set new value
            newArchetype.Set(Row, value);

            return this;
        }

        /// <summary>
        /// Removes component from entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Entity Remove<T>()
             where T : unmanaged
		{
			HashSet<Type> newComponents = GetExsistingComponentTypes();
			if (!newComponents.Contains(typeof(T)))
			{
				// TODO Consider not making exception
				throw new InvalidOperationException("Entity doesn't have this component");
			}
			// Remove Component
			newComponents.Remove(typeof(T));
			MoveToNewArchetype(newComponents, out _);
			return this;
		}


        public Entity Add(Bundle bundle)
        {
            HashSet<Type> newComponents = GetExsistingComponentTypes();
            newComponents.UnionWith(bundle.Components);

			MoveToNewArchetype(newComponents, out var newArchetype);

            // set new values 
            for (int i = 0; i < bundle.Components.Length; i++)
            {
				// See to learn more : https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.gchandle.tointptr?view=net-6.0
				// Cannot handle bools
				// https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
				var hdl = GCHandle.Alloc(bundle.Data[i], GCHandleType.Pinned);
                unsafe
                {
                    newArchetype.Set(bundle.Components[i], Row, hdl.AddrOfPinnedObject().ToPointer());
                }
                hdl.Free();
            }
            return this;
        }
        public Entity Remove(Type[] components)
        {
			HashSet<Type> newComponents = GetExsistingComponentTypes();
			newComponents.ExceptWith(components);
			MoveToNewArchetype(newComponents, out var newArchetype);
			return this;
		}



		internal unsafe void CopyAllComponentsToArchetype(Archetype target, int targetRow)
        {
#if DEBUG
            if(Archetype == -1)
            {
                throw new Exception("Cannot copy to archetype since entity is not registered in any archetype");
            }
#endif
            Archetype currentArchetype = World.archetypes[Archetype];

            foreach (var storage_from in currentArchetype.storage)
            {
                if (target.storage.TryGetValue(storage_from.Value.ComponentType, out var storage_to))
                {
                    storage_to.Set(targetRow, storage_from.Value.Get(Row));
                }
            }
        }

		/// <summary>
		/// Return a hashset of the exsisting components on the entity/archetype
		/// </summary>
		/// <returns></returns>
		internal HashSet<Type> GetExsistingComponentTypes()
		{
			if (Archetype != -1)
			{
				Archetype currentArchetype = World.archetypes[Archetype];

				return new HashSet<Type>(currentArchetype.ComponentTypes);
			}
			return new HashSet<Type>();
		}

		internal void MoveToNewArchetype(HashSet<Type> components, out Archetype newArchetype)
		{
			Archetype currentArchetype = null;
            
			if (Archetype != -1)
			{
				currentArchetype = World.archetypes[Archetype];
                // If the new archetype is the same as old
                if (currentArchetype.ComponentTypes.SetEquals(components))
				{
					newArchetype = currentArchetype;
					return;
				}
			}

			// Get New archetype
			newArchetype = World.CreateOrGetArchetype(components, out int newArchetypeIndex);
			int newEntityRow = newArchetype.AddEntity();
		
			// Copy old values to new archetype
			if (currentArchetype != null)
			{
				CopyAllComponentsToArchetype(newArchetype, newEntityRow);
				// Remove from old  
				currentArchetype.RemoveEntity(Row);
			}
		    
			// Update pointer in world
			World.entities[EntityPointer.ID] = new InternalEntityPointer(newArchetypeIndex, newEntityRow, World.entities[EntityPointer.ID].Version);
		}

        public T Get<T>()
            where T : unmanaged
        {
            return World.archetypes[Archetype].Get<T>(Row);
        }
        public T? TryGet<T>()
          where T : unmanaged
        {
			if(World.archetypes[Archetype].Has<T>())
				return World.archetypes[Archetype].Get<T>(Row);
			return null;
        }
        public bool Has<T>()
          where T : unmanaged
        {
            return World.archetypes[Archetype].Has<T>();
        }
        public bool Has(Type type)
        {
            return World.archetypes[Archetype].Has(type);
        }

        public void Set<T>(T value)
             where T : unmanaged
        {
            World.archetypes[Archetype].Set<T>(Row, value);
        }

        public unsafe void* Get(Type type)
        {
            return World.archetypes[Archetype].storage[type].Get(Row);
        }
        public unsafe void Set(Type type, void* value)
        {
            World.archetypes[Archetype].storage[type].Set(Row, value);
        }
    }
}