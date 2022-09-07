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
    public class World
    {
        /// <summary> Time delta in seconds </summary>
        public float Delta { get; set; }

        //
        public List<Archetype> archetypes;

        /// <summary> All the entities currently in the world. When entity is removed they still exists in this  </summary>
        public List<EntityPointer> entities;

        // Maintain query when objects are added/removed
        // Queries maintain their valitidy troughout a single update since all modifcation (create/destroy, add/remove component) to entities are defered to end of update
        // 
        // the int indexes into entities list
        internal Dictionary<int, QueryResult> queries;

        // workaround is possible... remove.
        // 
        internal Dictionary<int, bool> queriesdirty;



        public World()
        {
            const int initialSize = 1024;
            this.archetypes = new List<Archetype>();
            this.entities = new List<EntityPointer>(initialSize);
            queries = new();
            queriesdirty = new();
        }

        public Entity CreateEntity()
        {
            // Search for
            var a = new EntityPointer(entities.Count, 0,-1,-1);
            entities.Add(a);
            var entity = new Entity(this, a);
            return entity;
        }

        /*
        public void DestroyEntity(int id_entity)
        {
            // Check if entitypointer is valid
            if(activeEntities[id_entity] == true)
            {
                activeEntities[id_entity] = false;
            }
            else
            {
                throw new Exception("Entity is already destroyed");
            }
        }*/




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
        internal bool Match(Type[] components, Query filter)
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
        internal Archetype CreateOrGetArchetype(Type[] types, out int index)
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
    }
}