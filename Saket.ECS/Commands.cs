using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    /// <summary>
    /// The Commands are used to defer world mutations until a hard syncronisation point.
    /// 
    /// 
    /// 
    /// </summary>
    /*
    public unsafe class Commands
    {
        public World world;
        // Writer
        public SerializerWriter data;

        //
        public EntityCommands CreateEntity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get already created entity
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public EntityCommands GetEntity(EntityPointer pointer)
        {
            throw new NotImplementedException();
        }
        
        public void DestroyEntity(ref EntityPointer pointer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get already created entity
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public bool TryGetEntity(EntityPointer pointer, out EntityCommands entity)
        {
            throw new NotImplementedException();
        }
    }

    public unsafe class EntityCommands
    {
        public Commands commands;
        // Type, data pointer
        public Dictionary<Type, int> newComponents;
        public HashSet<Type> removedComponents;

        bool IsNewEntity => pointer == EntityPointer.Default;
        EntityPointer pointer;

        public EntityCommands(Commands commands, EntityPointer pointer)
        {
            this.commands = commands;
            this.pointer = pointer;

            removedComponents = new();
            newComponents = new();
        }

        public EntityCommands Remove<T>()
        {
            removedComponents.Add(typeof(T));
            return this;
        }
        public EntityCommands Add<T>(T value) where T : unmanaged
        {
            newComponents.Add(typeof(T), commands.data.AbsolutePosition);
            commands.data.Write(value);
            return this;
        }

        public T Get<T>()
           where T : unmanaged
        {
            if (newComponents.ContainsKey(typeof(T)))
            {
                SerializerReader reader = new SerializerReader(commands.data.DataRaw, newComponents[typeof(T)]);
                return reader.Read<T>();
            }
            if (!IsNewEntity)
            {
                commands.world.archetypes[commands.world.entities[pointer.ID].Archetype].Get<T>(commands.world.entities[pointer.ID].Row);
            }
            throw new NotImplementedException();
        }

        public bool Has<T>(T value)
        {
            // This should both searched queued commands and exisiting values in the world
            throw new NotImplementedException();
        }
        public bool Has(Type type)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(T value)
             where T : unmanaged
        {
            throw new NotImplementedException();
        }
        public unsafe void* Get(Type type)
        {
            throw new NotImplementedException();
        }
        public unsafe void Set(Type type, void* value)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {

        }
    }

    */
}
