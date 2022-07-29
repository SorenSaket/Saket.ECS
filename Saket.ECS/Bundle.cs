namespace Saket.ECS
{
    public class Bundle
    {
        public Type[] components;

        private Bundle(int count)
        {
            components = new Type[count];
        }
        public static Bundle Create(Type t1, Type t2) 
        {
            var bundle = new Bundle(2);
            bundle.components[0] = (t1);
            bundle.components[1] = (t2);
            return bundle;
        }
        public static Bundle Create<T1>() where T1 : struct
        {
            var bundle = new Bundle(1);
            bundle.components[0] = (typeof(T1));
            return bundle;
        }

        public static Bundle Create<T1, T2>() 
        {
            var bundle = new Bundle(2);
            bundle.components[0] = (typeof(T1));
            bundle.components[1] = (typeof(T2));
            return bundle;
        }
        /*
        internal Query ToQuery()
        {
            
        }*/
    }
}