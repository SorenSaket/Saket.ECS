﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;


namespace engine.ecs
{
    public struct EntityPointer
    {
        public int version;
        public int index_archetype;
        public int index_row;

        public EntityPointer(int version, int index_archetype, int index_row)
        {
            this.version = version;
            this.index_archetype = index_archetype;
            this.index_row = index_row;
        }
    }

    public class World
    {
        public float Delta { get; private set; }

        public Pipeline pipeline;
        //
        public List<Archetype> archetypes;

        public List<EntityPointer> entities;
        public BitArray activeEntities; // false = destoryed, true = alive

        // Maintain query when objects are added/removed
        protected List<Query> queries;

        public object lock_entity;

        public World()
        {
            const int initialSize = 1024;

            this.entities = new List<EntityPointer>(initialSize);
            this.activeEntities = new BitArray(initialSize);
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
        public void SetPipeline(Pipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        internal void QueryUpdate()
        {

        }

        internal Archetype CreateOrGetArchetype(Type[] types)
        {
            int hash = Archetype.GetComponentGroupHashCode(types);
            for (int i = 0; i < archetypes.Count; i++)
            {
                if(archetypes[i].GetID() == hash)
                {
                    return archetypes[i];
                }
            }

            var arc = new Archetype(types);
            archetypes.Add(arc);
            return arc;
        }
    }
}
