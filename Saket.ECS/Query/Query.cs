
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
            return this;
        }
        public Query With<T1>()
        {
            Inclusive.UnionWith(GetComponentsGenerics(typeof(T1)));
            dirty = true;

            return this;
        }
        public Query Without<T1>()
        {
            Exclusive.UnionWith(GetComponentsGenerics(typeof(T1)));
            dirty = true;
            return this;
        }

        




        /// <summary>
        /// Extracts all types from eventual tuple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static HashSet<Type> GetComponentsGenerics(Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            Type[] generics = type.GetGenericArguments();

            for (int i = 0; i < generics.Length; i++)
            {
                if (generics[i].IsAssignableFrom(typeof(Tuple)))
                {
                    types.UnionWith(GetComponentsGenerics(generics[i]));
                }
                else
                {
                    types.Add(generics[i]);
                }
            }
            return types;
        }

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