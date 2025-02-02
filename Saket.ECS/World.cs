﻿using Saket.ECS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Saket.ECS
{

    /// <summary>
    /// 
    /// </summary>
    public class World :  IEnumerable<Entity>
    {
        /// <summary> Time delta in seconds </summary>
        public float DeltaTime { get; set; }
        public float Time { get; set; }
        //
        public List<Archetype> Archetypes;

        /// <summary> 
        /// All the entities currently in the world. 
        /// When entity is destroyed they still exists in this data structure .
        /// </summary>
        public List<InternalEntityPointer> entities;

        // Maintain query when objects are added/removed
        // Queries maintain their validity troughout a single update since all modifcation (create/destroy, add/remove component) to entities are defered to end of update
        // 
        // the int indexes into entities list
        internal Dictionary<int, QueryResult> queries;

        // workaround is possible... remove.
        // 
        internal Dictionary<int, bool> queriesdirty;

        /// <summary>
        /// Generic reference storage to managed/unmanged resources.
        /// </summary>
        internal Dictionary<Type, object> resources;

        /// <summary>
        /// Stack of entity ID/Indexes that are destroyed.
        /// </summary>
        internal Stack<int> destroyedEntities;



        private List<int> temp = new List<int>();
 
        public World()
        {
            const int initialSize = 1024;
            this.Archetypes = new List<Archetype>();
            this.entities = new List<InternalEntityPointer>(initialSize);
            this.destroyedEntities = new();
            this.resources = new();
            queries = new();
            queriesdirty = new();
        }

        public Entity CreateEntity()
        {
            // Search for destroyed entities for reuse
            if(destroyedEntities.Count > 0)
            {
                int id = destroyedEntities.Pop();
                return new Entity(this, new ECSPointer(id, entities[id].Version));
            }
            else
            {
                // Create new entity 
                // entities.Count will point to the last
                // Version starts off at 0
                var newEntity = new ECSPointer(entities.Count, 0);

                entities.Add(new InternalEntityPointer(-1,-1,0));

                return new Entity(this, newEntity);
            }
        }



        /// <summary>
        /// Get already created entity
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public Entity GetEntity(ECSPointer pointer)
        {
            // The entity exists but is destroyed
            if (destroyedEntities.Contains(pointer.ID))
                throw new ArgumentException("GetEntity Failed: Entity is destroyed");

            // If the entity.ID is within bounds
            if (pointer.ID >= 0 && pointer.ID < entities.Count) {
                // if the version number matches with the one stored
                if (entities[pointer.ID].Version == pointer.Version)
                    return new Entity(this, pointer);
                throw new ArgumentException("GetEntity Failed: Entity doesn't anymore");
            }

            throw new ArgumentException("GetEntity Failed: Pointer out of range");
        }

        /// <summary>
        /// Get already created entity
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public bool TryGetEntity(ECSPointer pointer, out Entity entity)
        {
            // The entity exists but is destroyed
            if (destroyedEntities.Contains(pointer.ID))
            {
                entity = new Entity(this, ECSPointer.Default);
                return false;
            }
                

            // If the entity.ID is within bounds
            if (pointer.ID >= 0 && pointer.ID < entities.Count)
                // if the version number matches with the one stored
                if (entities[pointer.ID].Version == pointer.Version)
                {
                    entity = new Entity(this, pointer);
                    return true;
                }

            entity = new Entity(this, ECSPointer.Default);
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointer"></param>
        /// <exception cref="InvalidOperationException"> When the entity already is destroyed</exception>
        public void DestroyEntity(ref ECSPointer pointer)
        {
            // 
            if (!destroyedEntities.Contains(pointer.ID) && pointer.Valid && pointer.ID < entities.Count)
            {
                // Add to destroyed entities
                destroyedEntities.Push(pointer.ID);
                
                //
                InternalEntityPointer old = entities[pointer.ID];
                
                // If the entity was assigned an archetype
                if (old.Archetype != -1 && old.Row != -1)
                    // Remove from archetype
                    Archetypes[old.Archetype].RemoveEntity(old.Row);

                
                // Unchecked will cause the version number to wrap around
                unchecked
                {
                    // Deassign archetype and increment version on the internal pointer
                    entities[pointer.ID] = new InternalEntityPointer(-1, -1, old.Version + 1);
                }

                // Invalidate the pointer
                pointer = ECSPointer.Default;
            }
            else 
            {
                throw new InvalidOperationException("Entity is already destroyed");
            }
        }


        public void SetResource<T>(T resource) //where T : notnull
        {
            if (resources.ContainsKey(typeof(T)))
            {
                resources[typeof(T)] = resource;
                return;
            }
            resources.Add(typeof(T), resource);
        }

        public T GetResource<T>()
        {
            return (T)resources[typeof(T)];
            //throw new ArgumentException($"Resource of type {typeof(T)} does not exsist on world");
        }

        public bool TryGetResource<T>(out T value)
        {
            if (resources.ContainsKey(typeof(T)))
            {
                value = (T)resources[typeof(T)];
                return true;
            }
            value = default(T);
            return false;
        }


        public QueryResult Query(Query query)
        {
            // The number of entities to account for
            int entityCount = 0;
           
            List<int> _archetypes = new List<int>();

            // List of all indexes of archetypes that match query

            for (int i = 0; i < Archetypes.Count; i++)
            {
                // If the archetype mach to the query
                if (Match(Archetypes[i].ComponentTypes, query))
                {
                    entityCount += Archetypes[i].Count;
                    _archetypes.Add(i);
                }
            }

            List<int> _entities = new List<int>();
            // iterate over all entities matching their archetype 
            // 
            // TODO this is slow
            for (int i = 0; i < entities.Count; i++)
            {
                if (_archetypes.Contains(entities[i].Archetype))
                    if (!Archetypes[entities[i].Archetype].avaliableRows.Contains(entities[i].Row)) // Do not include deleted entities. (This is also slow)
                        _entities.Add(i);
            }

            return new QueryResult(this, _entities, _archetypes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="_archetypes"></param>
        /// <param name="entityCount"></param>
        /// <returns>the number of matching archetypes</returns>
        public int QueryArchetypes(Query query, ref List<Archetype> _archetypes, out int entityCount)
        {
            // List of all indexes of archetypes that match query
            _archetypes.Clear();
            Int32 archetypeCount = 0;
            entityCount = 0;

            for (int i = 0; i < Archetypes.Count; i++)
            {
                // If the archetype mach to the query
                if (Match(Archetypes[i].ComponentTypes, query))
                {
                    archetypeCount++;
                    entityCount += Archetypes[i].Count;
                    _archetypes.Add(Archetypes[i]);
                }
            }

            return archetypeCount;
        }
        


        // todo add inclusive groups
        /// <summary>
        /// 
        /// </summary>
        /// <param name="components"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Match(HashSet<Type> components, Query filter)
        {
            foreach (var item in filter.Inclusive)
            {
                if (!components.Contains(item))
                    return false;
            }
            foreach (var item in filter.Exclusive)
            {
                if (components.Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        internal Archetype CreateOrGetArchetype(HashSet<Type> types, out int index)
        {
            // Get hashcode for combination components
            int hash = Archetype.GetComponentGroupHashCode(types);

            // Iterate all archetypes and search one that matches hash
            for (int i = 0; i < Archetypes.Count; i++)
            {
                // If combination already exists
                if(Archetypes[i].ID == hash)
                {
                    index = i;
                    return Archetypes[i];
                }
            }

            // Seach unsuccessful. Create new Archetype
            var arc = new Archetype(types);
            Archetypes.Add(arc);
            index = Archetypes.Count - 1;
            return arc;
        }

        /// <summary>
        /// Clears all entity related data
        /// Does not deallocate memory
        /// </summary>
        public void Clear()
        {
            destroyedEntities.Clear();
            entities.Clear();

            foreach (var archetype in Archetypes)
            {
                archetype.Clear();
            }
        }

        /// <summary>
        /// Clones all state data and populates the other world
        /// </summary>
        /// <param name="other"></param>
        public void Overwrite(World other)
        {
            // This is guranteed to generate garbage. Possible option is to clear and fill arrays manually.
            // Problem is that stack doesn't have an indexer. ugh. default lib sucks.
            other.destroyedEntities = new Stack<int>(other.destroyedEntities);
            other.entities = new List<InternalEntityPointer>(other.entities);

            // Overwrite all archetypes
            foreach (var archetype in Archetypes)
            {
                other.CreateOrGetArchetype(archetype.ComponentTypes, out int index);

                archetype.Overwrite(other.Archetypes[index]);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return new WorldEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}