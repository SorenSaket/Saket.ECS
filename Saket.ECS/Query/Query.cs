
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.ECS
{
    /// <summary>
    /// 
    /// Queries should be able to iterate across multiple archetypes
    /// </summary>
    public class Query
    {
        public HashSet<Type> Inclusive;
        public HashSet<Type> Exclusive;
        public HashSet<Type[]> AnyGroups;

        public int Signature
        {
            get
            {
                // Lazy recomputing of signature
                if (dirty)
                {
                    signature = GetSignature();
                    dirty = false;
                }
                return signature;
            }
        }

        private int signature;
        private bool dirty;

        public Query()
        {
            Inclusive = new HashSet<Type>();
            Exclusive = new HashSet<Type>();
            AnyGroups = new HashSet<Type[]>();
        }
        public Query Any<T1>()
        {
            throw new NotImplementedException();
        }
        public Query With<T1>()
        {
            Inclusive.UnionWith(GetComponentsFromType(typeof(T1)));
            dirty = true;
            return this;
        }
        public Query Without<T1>()
        {
            Exclusive.UnionWith(GetComponentsFromType(typeof(T1)));
            dirty = true;
            return this;
        }

        




        /// <summary>
        /// Extracts all types from eventual tuple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static HashSet<Type> GetComponentsFromType(Type type)
        {
            // If the type is a tuple
            if(typeof(ITuple).IsAssignableFrom(type))
            {
                HashSet<Type> types = new HashSet<Type>();
                Type[] generics = type.GetGenericArguments();

                for (int i = 0; i < generics.Length; i++)
                {
                    if (typeof(ITuple).IsAssignableFrom(type))
                    {
                        types.UnionWith(GetComponentsFromType(generics[i]));
                    }
                    else if(type.IsValueType)
                    {
                        types.Add(generics[i]);
                    }
                }
                return types;
            }
            else if (type.IsValueType)
            {
                return new HashSet<Type>() { type };
            }

            return new HashSet<Type>();
        }


        // TODO. Validate collision occurence.
        private int GetSignature()
        {
            int code = 0;
            foreach (var item in Inclusive)
            {
                code += item.GetHashCode();
            }
            foreach (var item in Exclusive)
            {
                code -= item.GetHashCode();
            }
            return code;
        }

    }
}