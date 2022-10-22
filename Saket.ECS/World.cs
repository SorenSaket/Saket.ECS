using Saket.ECS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Saket.ECS
{

    /// <summary>
    /// 
    /// </summary>
    public class World :  IEnumerable<Entity>
    {
        /// <summary> Time delta in seconds </summary>
        public float Delta { get; set; }

        //
        public List<Archetype> archetypes;

        /// <summary> All the entities currently in the world. When entity is removed they still exists in this data structure </summary>
        public List<InternalEntityPointer> entities;

        // Maintain query when objects are added/removed
        // Queries maintain their validity troughout a single update since all modifcation (create/destroy, add/remove component) to entities are defered to end of update
        // 
        // the int indexes into entities list
        internal Dictionary<int, QueryResult> queries;

        // workaround is possible... remove.
        // 
        internal Dictionary<int, bool> queriesdirty;

        internal Dictionary<Type, object> resources;

        internal Stack<int> destroyedEntities;

        public World()
        {
            const int initialSize = 1024;
            this.archetypes = new List<Archetype>();
            this.entities = new List<InternalEntityPointer>(initialSize);
            this.destroyedEntities = new();
            this.resources = new();
            queries = new();
            queriesdirty = new();
        }

        public Entity CreateEntity()
        {
            // Search for
            if(destroyedEntities.Count > 0)
            {
                return new Entity(this, entities[destroyedEntities.Pop()].Pointer);
            }
            else
            {
                var a = new EntityPointer(entities.Count, 0);
                entities.Add(new InternalEntityPointer(-1,-1,a));
                return new Entity(this, a);
            }
        }


        // Get and destroy should use entitypointers and not id. Since Version can change.
        // Get already created entity
        public Entity? GetEntity(EntityPointer entity)
        {
            if (destroyedEntities.Contains(entity.ID))
                return null;

            if(entities[entity.ID].Pointer == entity)
                return new Entity(this, entity);

            return null;
        }

        public void DestroyEntity(ref EntityPointer entity)
        {
            // 
            if (!destroyedEntities.Contains(entity.ID) && entity.Valid && entity.ID < entities.Count)
            {
                destroyedEntities.Push(entity.ID);
                InternalEntityPointer old = entities[entity.ID];
                // Remove from archetype
                if (old.Archetype != -1 && old.Row != -1)
                    archetypes[old.Archetype].RemoveEntity(old.Row);
                // Reset the pointer and advance version counter
                entity = new EntityPointer(entity.ID, old.Pointer.Version + 1);
                entities[entity.ID] = new InternalEntityPointer(-1, -1, entity);
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

        public T? GetResource<T>()
        {
            if (resources.ContainsKey(typeof(T)))
            {
                return (T)resources[typeof(T)];
            }
            return default(T);
        }

        public bool HasResource<T>(out T value)
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
        {/*
            if (!queries.ContainsKey(query.Signature))
            {
                queries.Add(query.Signature, GetMatchingEntities(query));
                queriesdirty.Add(query.Signature, false);
            }
            else if(queriesdirty[query.Signature])
            {
                queries[query.Signature] = GetMatchingEntities(query);
                queriesdirty[query.Signature] = false;
            }

            return new QueryResult(this, queries[query.Signature]);*/

            GetMatchingEntities(query, out var _entities, out var _archetypes);

            return new QueryResult(this, _entities, _archetypes);
        }


        // Todo make allocation free
        internal void GetMatchingEntities(Query query, out List<int> _entities, out List<int> _archetypes) 
        { 
            // List of all indexes of archetypes that match query
            _archetypes = new List<int>();
            
            // The number of entities to account for
            int size = 0;

            for (int i = 0; i < archetypes.Count; i++)
            {
                // If the archetype mach to the query
                if(Match(archetypes[i].ComponentTypes, query))
                {
                    size += archetypes[i].Count;
                    _archetypes.Add(i);
                }
            }

            _entities = new List<int>(size);

            // iterate over all entities matching their archetype 
            // 
            // TODO this is slow
            for (int i = 0; i < entities.Count; i++)
            {
                if (_archetypes.Contains(entities[i].Archetype) )
                    if(!archetypes[entities[i].Archetype].avaliableRows.Contains(entities[i].Row)) // Do not include deleted entities.
                        _entities.Add(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="components"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
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
            for (int i = 0; i < archetypes.Count; i++)
            {
                // If combination already exists
                if(archetypes[i].ID == hash)
                {
                    index = i;
                    return archetypes[i];
                }
            }

            // Seach unsuccessful. Create new Archetype
            var arc = new Archetype(types);
            archetypes.Add(arc);
            index = archetypes.Count - 1;
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

            foreach (var archetype in archetypes)
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
            foreach (var archetype in archetypes)
            {
                other.CreateOrGetArchetype(archetype.ComponentTypes, out int index);

                archetype.Overwrite(other.archetypes[index]);
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