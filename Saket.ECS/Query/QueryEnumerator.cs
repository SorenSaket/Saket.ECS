using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    // 
    public struct QueryEnumerator : IEnumerator<Entity>
    {
        private QueryResult query;
        private int start;
        private int end;
        int position;

        public QueryEnumerator(QueryResult query, int start, int end)
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
            position = start - 1;
        }

        public void Dispose()
        {

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

        object IEnumerator.Current => Current;
    }

}
