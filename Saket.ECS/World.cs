using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;


namespace engine.ecs
{
    struct EntityPointer
    {
        public int version;
        public int index_archetype;
        public int index_row;
    }

    internal class World
    {
        // TOOD make single pipeline
        public List<Pipeline> pipelines;
        //
        public List<Archetype> archetypes;

        public List<EntityPointer> entities;
        public BitArray activeEntities; // false = destoryed, true = alive

        public object lock_entity;

        public World()
        {
            const int initialSize = 1024;

            this.entities = new List<EntityPointer>(initialSize);
            this.activeEntities = new BitArray(initialSize);
        }



        public void Update(float delta)
        {
            for (int i = 0; i < pipelines.Count; i++)
            {
                pipelines[i].Update(delta);
            }
        }




        
        public Entity CreateEntity()
        {
            // Search for
            

            entities.Add(new EntityPointer());


            return new Entity(this, null);
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
        public void AddPipeline(Pipeline pipeline)
        {
            this.pipelines.Add(pipeline);
        }
    }
}
