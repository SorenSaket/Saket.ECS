using System.Collections;

namespace Saket.ECS
{

    /// <summary>
    /// 
    /// </summary>
    public class World
    {
        public float Delta { get; private set; }

        public Pipeline pipeline;
        //
        public List<Archetype> archetypes;

        public List<EntityPointer> entities;

        // Maintain query when objects are added/removed
        internal Dictionary<int, List<EntityPointer>> queries;
        internal Dictionary<int, bool> queriesdirty;



        public object lock_entity;

        public World()
        {
            const int initialSize = 1024;
            this.archetypes = new List<Archetype>();
            this.entities = new List<EntityPointer>(initialSize);
           // this.activeEntities = new BitArray(initialSize);
            queries = new();
            queriesdirty = new();
        }
        public void SetPipeline(Pipeline pipeline)
        {
            this.pipeline = pipeline;
        }
        public void Update(float delta)
        {
            Delta = delta;
            pipeline.Update(this);
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
            return new QueryResult(this, GetMatchingEntities(query));
        }

        internal List<EntityPointer> GetMatchingEntities(Query query) 
        { 
            // List of all indexes of archetypes that match query
            List<int> archetypeIds = new List<int>();
            // The number of entities to account for
            int size = 0;

            for (int i = 0; i < archetypes.Count; i++)
            {
                // If the archetype mach to the query
                if(Match(archetypes[i].ComponentTypes, query))
                {
                    size += archetypes[i].Count;
                    archetypeIds.Add(i);
                }
            }

            List<EntityPointer> r = new List<EntityPointer>(size);
            // iterate over all entities matching their archetype 
            // TODO this is slow
            for (int i = 0; i < entities.Count; i++)
            {
                if (archetypeIds.Contains(entities[i].index_archetype))
                    r.Add(entities[i]);
            }

            return r;
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