using Saket.ECS.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public List<EntityPointer> entities;

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
            this.entities = new List<EntityPointer>(initialSize);
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
                return new Entity(this, entities[destroyedEntities.Pop()]);
            }
            else
            {
                var a = new EntityPointer(entities.Count, 0, -1, -1);
                entities.Add(a);
                return new Entity(this, a);
            }
        }

        // Get already created entity
        public Entity? GetEntity(int entityID)
        {
            if(entityID < 0 || entityID >= entities.Count)
                return null;
            if (destroyedEntities.Contains(entityID))
                return null;
            return new Entity(this, entities[entityID]);
        }

        public void DestroyEntity(int entityID)
        {
            if (!destroyedEntities.Contains(entityID))
            {
                destroyedEntities.Push(entityID);
                var old = entities[entityID];
                // Remove from archetype
                if (old.index_archetype != -1 && old.index_row != -1)
                    archetypes[old.index_archetype].RemoveEntity(old.index_row);
                // Reset the pointer and advance version counter
                entities[entityID] = new EntityPointer(entityID, old.version+1);
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
                if (_archetypes.Contains(entities[i].index_archetype) )
                    if(!archetypes[entities[i].index_archetype].avaliableRows.Contains(entities[i].index_row)) // Do not include deleted entities.
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
            other.entities = new List<EntityPointer>(other.entities);

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