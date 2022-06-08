
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace engine.ecs
{
    /// <summary>
    /// Garantees that at least one of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct Or<T1>
          where T1 : unmanaged
    {

    }

    /// <summary>
    /// Garantees that at none of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct Without<T>
        where T : unmanaged
    {

    }

    /// <summary>
    /// Garantees that at none of the components are on the entity
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public struct AnyOf<T>
        where T : unmanaged
    {

    }





    // A query is 
    // able to split acess to components between threads
    // 
    // Queries should be able to iterate across multiple archetypes

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
                return (T)(query[position]);
            }
        }

        public Entity Current
        {
            get
            {
                try
                {
                    return query.GetEntity(position);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }


    public static class QueryUtils
    {
        public static int GetQuerySignature(Type type)
        {
            if(!type.IsAssignableTo(typeof(Query)))
            {
                throw new Exception("type is not a query");
            }

            Type[] generics = type.GetGenericArguments();

            for (int i = 0; i < length; i++)
            {

            }
        }
    }


    public abstract class Query : IEnumerable<Entity>
    {
        public int Count;
        internal Archetype[] archetypes;
        internal int[] archetypeSizes;

        internal int start;
        internal int end;

        internal void SetArchetypes(Archetype[] archetypes)
        {
            this.archetypes = archetypes;
            this.archetypeSizes = new int[archetypes.Length];
            for (int i = 0; i < archetypes.Length; i++)
            {
                archetypeSizes[i] = archetypes[i].Count;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FloorIndex(int value, int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                if (value < arr[i])
                    return i - 1;
            }
            return 0;
        }

        public Entity GetEntity(int position)
        {
            throw new NotImplementedException();
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

    public class Query<T1> : Query where T1 : unmanaged {}
    public class Query<T1, T2> : Query where T1 : unmanaged where T2 : unmanaged  {}
    public class Query<T1, T2, T3> : Query where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged {}
}