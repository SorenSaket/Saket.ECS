
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace engine.ecs
{
    // A query is 
    // able to split acess to components between threads
    // 
    //

    public class Query
    {
        public int Count;
    }
    /*
    public ref struct QueryResult<T1>
         where T1 : unmanaged
    {
        Span<T1> span1;
    }
    public ref struct QueryResult<T1,T2>
     where T1 : unmanaged
        where T2 : unmanaged
    {
        public Span<T1> span1;
        public Span<T2> span2;
    }


    public class Query<T1> : Query
        where T1 : unmanaged
    {
        public QueryResult<T1> GetResults()
        {
            return default(QueryResult<T1>);
        }


        public T1 this[int index]
        {
            get
            {
                //
                return default(T1);
            }
            set
            {
                //
            }
        }
    }

    public class Query<T1, T2> : Query
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public QueryResult<T1,T2> GetResults()
        {
            return default(QueryResult<T1,T2>);
        }

        public void GetResults(out MultiArray<T1> one, out MultiArray<T2>two )
        {
            one = default(MultiArray<T1>);
            two = default(MultiArray<T2>);
        }

        public Tuple<T1, T2> this[int index]
        {
            get
            {
                //
                return new Tuple<T1, T2>(default(T1), default(T2));
            }
            set
            {
                //
            }
        }
    }

    */
}
