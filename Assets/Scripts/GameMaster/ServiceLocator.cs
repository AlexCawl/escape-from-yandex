using System;
using System.Collections.Generic;

namespace GameMaster
{
    public class ServiceLocator
    {
        public static ServiceLocator Get => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;
        
        private ServiceLocator()
        {
        }

        private readonly Dictionary<Tuple<Type, string>, Func<object>> _data = new()
        {
            { new Tuple<Type, string>(typeof(string), "log"), () => "my message" }
        };

        public T Locate<T>(string name = null) => (T)_data[new Tuple<Type, string>(typeof(T), name)].Invoke();
    }
}