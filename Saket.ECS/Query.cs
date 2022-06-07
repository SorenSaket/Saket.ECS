
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

    public class Query<T1> : Query
        where T1 : unmanaged
    {
        

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

    public class Query<T1, T2> : Query, IEnumerable<Tuple<Entity, T1, T2>>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        
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

        public IEnumerator<Tuple<Entity, T1, T2>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
