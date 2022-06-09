
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    public interface FilterItem { }


    /// <summary>
    /// Guarantees that at least one of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct Any<T1> : FilterItem
          where T1 : unmanaged
    {

    }
    /// <summary>
    /// Guarantees that all of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct With<T1> : FilterItem
          where T1 : unmanaged
    {

    }
    /// <summary>
    /// Guarantees that at none of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct Without<T> : FilterItem
        where T : unmanaged
    {

    }

    


    internal struct QueryFilter
    {
        public readonly Type[] Inclusive;
        public readonly Type[] Exclusive;
        public readonly Type[][] AnyGroups;
        public readonly int Signature;

        public QueryFilter(int signature, Type[] inclusive, Type[] exclusive, Type[][] anyGroups)
        {
            this.Signature = signature;
            this.Inclusive = inclusive;
            this.Exclusive = exclusive;
            this.AnyGroups = anyGroups;
        }
    }

    // 
    public struct QueryEnumerator : IEnumerator<Entity>
    {
        private Query query;
        private int start;
        private int end;
        int position = -1;

        public QueryEnumerator(Query query, int start, int end)
        {
            this.query = query;
            this.start = start;
            this.end = end;
            position = start - 1;
        }

        public bool MoveNext()
        {
            position++;
            return (position < end);
        }

        public void Reset()
        {
            position = start-1;
        }

        public void Dispose()
        {
            
        }

        object IEnumerator.Current
        {
            get
            {
                return (query[position]);
            }
        }

        public Entity Current
        {
            get
            {
                try
                {
                    return query[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }


    internal static class QueryUtils
    {


        internal static List<Type> GetComponentsGenerics(Type type)
        {
            List<Type> types =  new List<Type>();
            Type[] generics = type.GetGenericArguments();

            for (int i = 0; i < generics.Length; i++)
            {
                if(generics[i].IsAssignableFrom(typeof(Tuple)))
                {
                    types.AddRange(GetComponentsGenerics(generics[i]));
                }
                else
                {
                    types.Add(generics[i]);
                }
            }

            return types;
        }


        internal static QueryFilter GetQueryFilter(Type type)
        {
            if(!type.IsAssignableTo(typeof(Query)))
            {
                throw new Exception("type is not a query");
            }
            
            Type[] generics = type.GetGenericArguments();

            HashSet<Type> inclusive = new HashSet<Type>();
            HashSet<Type> exclusive = new HashSet<Type>();

            for (int i = 0; i < generics.Length; i++)
            {
                if(generics[i] == typeof(Any<>))
                {
                    
                }
                else if (generics[i] == typeof(Without<>))
                {
                    exclusive.UnionWith(GetComponentsGenerics(generics[i]));
                }
                else
                {
                    inclusive.UnionWith(GetComponentsGenerics(generics[i]));
                }
            }

            return new QueryFilter(type.GetHashCode(), inclusive.ToArray(), exclusive.ToArray(), null);
        
        }

    }



    /// <summary>
    /// A query is 
    /// able to split acess to components between threads
    /// 
    /// Queries should be able to iterate across multiple archetypes
    /// </summary>
    public class Query : IEnumerable<Entity>
    {
        internal List<EntityPointer> entities;
        public int Count { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        private readonly World world;

        internal Query(World world, List<EntityPointer> entities)
        {
            this.world = world;
            this.entities = entities;
            Start = 0;
            End = Count = entities.Count;
        }

        public Entity this[int index]{
            get {
                return new Entity(world, entities[index]);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return new QueryEnumerator(this, 0, Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

    }





    public class Query<T1> : Query where T1 : unmanaged
    {
        public Query(World world, List<EntityPointer> entities) : base(world, entities)
        {
        }
    }
    public class Query<T1, T2> : Query where T1 : unmanaged where T2 : unmanaged
    {
        public Query(World world, List<EntityPointer> entities) : base(world, entities)
        {
        }
    }
    public class Query<T1, T2, T3> : Query where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
    {
        public Query(World world, List<EntityPointer> entities) : base(world, entities)
        {
        }
    }
    public class Query<T1, T2, T3, T4> : Query where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
    {
        public Query(World world, List<EntityPointer> entities) : base(world, entities)
        {
        }
    }
}