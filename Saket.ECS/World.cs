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
        public BitArray activeEntities; // false = destoryed, true = alive

        // Maintain query when objects are added/removed
        internal Dictionary<int, List<EntityPointer>> queries;

        public object lock_entity;

        public World()
        {
            const int initialSize = 1024;

            this.entities = new List<EntityPointer>(initialSize);
            this.activeEntities = new BitArray(initialSize);
            queries = new Dictionary<int, List<EntityPointer>>();
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
            var a = new EntityPointer();

            entities.Add(a);

            return new Entity(this, a);
        }
        public void DestroyEntity(int id_entity)
        {
            if(activeEntities[id_entity] == true)
            {
                activeEntities[id_entity] = false;
            }
            else
            {
                throw new Exception("Entity is already destroyed");
            }
        }

        public void Add<T>(EntityPointer entity)
            where T : unmanaged
        {
            // Entity already has component
            Archetype archetype = archetypes[entity.index_archetype];

            if (archetype.ComponentTypes.Contains(typeof(T)))
            {
                // return warning
                return;
            }

            // Get or create new archetype
            Type[] newComponents = new Type[archetype.ComponentTypes.Length + 1];
            for (int i = 0; i < archetype.ComponentTypes.Length; i++)
            {
                newComponents[i] = archetypes[entity.index_archetype].ComponentTypes[i];
            }
            newComponents[^1] = typeof(T);

            Archetype newArchetype = CreateOrGetArchetype(newComponents);

            // Copy all values to new archtype
            for (int i = 0; i < archetype.ComponentTypes.Length; i++)
            {
                archetype.Get<>(i, entity.index_row);
            }



            // Add Component 
        }




        internal void QueryUpdate()
        {
           /* foreach (var query in queries)
            {
                query.Value = GetMatchingEntities()
            }*/
        }


        internal Query GetQuery(int signature)
        {
            if (queries.ContainsKey(signature))
            {
                return new Query(this,queries[signature]);
            }
            else
            {
                throw new ArgumentException("Query with signature does not exist");
            }
        }



        internal void CreateQuery(QueryFilter query)
        {
            if (queries.ContainsKey(query.Signature))
            {
                return;
            }

            queries.Add(query.Signature, GetMatchingEntities(query));

        }


        internal List<EntityPointer> GetMatchingEntities(QueryFilter query) 
        { 
            List<EntityPointer> r = new List<EntityPointer>();

            List<int> archetypeIds = new List<int>();

            for (int i = 0; i < archetypes.Count; i++)
            {
                if(Match(archetypes[i].ComponentTypes, query))
                {
                    archetypeIds.Add(i);
                }
            }

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
        internal bool Match(Type[] components, QueryFilter filter)
        {
            for (int i = 0; i < filter.Inclusive.Length; i++)
            {
                if (!components.Contains(filter.Inclusive[i]))
                    return false;
            }

            for (int i = 0; i < filter.Exclusive.Length; i++)
            {
                if (components.Contains(filter.Exclusive[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        internal Archetype CreateOrGetArchetype(Type[] types)
        {
            // Get hashcode for combination components
            int hash = Archetype.GetComponentGroupHashCode(types);

            // Iterate all archetypes and search one that matches hash
            for (int i = 0; i < archetypes.Count; i++)
            {
                // If combination already exists
                if(archetypes[i].GetID() == hash)
                {
                    return archetypes[i];
                }
            }

            // Seach unsuccessful. Create new Archetype
            var arc = new Archetype(types);
            archetypes.Add(arc);
            return arc;
        }
    }
}