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
    public struct Entity : IEquatable<Entity>
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

        public ECSPointer EntityPointer { get => entityPointer; set{ entityPointer = value; } }


        public readonly World World { get; }

        private ECSPointer entityPointer;

        public Entity(World world, ECSPointer pointer)
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

        public Entity Add<T1, T2>(
            T1 component1, 
            T2 component2)
                where T1 : unmanaged
                where T2 : unmanaged
        {
			HashSet<Type> newComponents = GetExsistingComponentTypes();
			newComponents.Add(typeof(T1));
            newComponents.Add(typeof(T2));

            MoveToNewArchetype(newComponents, out var newArchetype);

            // Set new value
            newArchetype.Set(Row, component1);
            newArchetype.Set(Row, component2);

            return this;
        }

        public Entity Add<T1, T2, T3>(
            T1 component1, 
            T2 component2, 
            T3 component3)
                where T1 : unmanaged
                where T2 : unmanaged
                where T3 : unmanaged
        {
            HashSet<Type> newComponents = GetExsistingComponentTypes();
            newComponents.Add(typeof(T1));
            newComponents.Add(typeof(T2));
            newComponents.Add(typeof(T3));

            MoveToNewArchetype(newComponents, out var newArchetype);

            // Set new value
            newArchetype.Set(Row, component1);
            newArchetype.Set(Row, component2);
            newArchetype.Set(Row, component3);

            return this;
        }


        public Entity Add<T1, T2, T3, T4>(
      T1 component1,
      T2 component2,
      T3 component3,
       T4 component4)
          where T1 : unmanaged
          where T2 : unmanaged
          where T3 : unmanaged
          where T4 : unmanaged
        {
            HashSet<Type> newComponents = GetExsistingComponentTypes();
            newComponents.Add(typeof(T1));
            newComponents.Add(typeof(T2));
            newComponents.Add(typeof(T3));
            newComponents.Add(typeof(T4));

            MoveToNewArchetype(newComponents, out var newArchetype);

            // Set new value
            newArchetype.Set(Row, component1);
            newArchetype.Set(Row, component2);
            newArchetype.Set(Row, component3);
            newArchetype.Set(Row, component4);

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
            Archetype currentArchetype = World.Archetypes[Archetype];

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
				Archetype currentArchetype = World.Archetypes[Archetype];

				return new HashSet<Type>(currentArchetype.ComponentTypes);
			}
			return new HashSet<Type>();
		}

		internal void MoveToNewArchetype(HashSet<Type> components, out Archetype newArchetype)
		{
			Archetype currentArchetype = null;
            
			if (Archetype != -1)
			{
				currentArchetype = World.Archetypes[Archetype];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
            where T : unmanaged
        {
            return World.Archetypes[Archetype].Get<T>(Row);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetRef<T>()
        where T : unmanaged
        {
            return ref World.Archetypes[Archetype].GetRef<T>(Row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? TryGet<T>()
          where T : unmanaged
        {
			if(World.Archetypes[Archetype].Has<T>())
				return World.Archetypes[Archetype].Get<T>(Row);
			return null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>()
          where T : unmanaged
        {
            return World.Archetypes[Archetype].Has<T>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(Type type)
        {
            return World.Archetypes[Archetype].Has(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set<T>(T value)
             where T : unmanaged
        {
            World.Archetypes[Archetype].Set<T>(Row, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void* Get(Type type)
        {
            return World.Archetypes[Archetype].storage[type].Get(Row);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Set(Type type, void* value)
        {
            World.Archetypes[Archetype].storage[type].Set(Row, value);
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity entity && Equals(entity);
        }

        public bool Equals(Entity other)
        {
            return EqualityComparer<World>.Default.Equals(World, other.World) &&
                   EqualityComparer<ECSPointer>.Default.Equals(entityPointer, other.entityPointer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(World, entityPointer);
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}