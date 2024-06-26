using System;
using System.Collections.Generic;
using GameMaster.State;

namespace GameMaster
{
    public class ServiceLocator
    {
        public static ServiceLocator Get => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;

        private readonly Dictionary<Tuple<Type, string>, Func<object>> _data = new()
        {
            { new Tuple<Type, string>(typeof(BooleanState), "visibilityState"), () => new BooleanState() }
        };

        private ServiceLocator()
        {
            var gameState = new GameLevelState();
            var flashLightState = new BooleanState();
            _data.Add(new Tuple<Type, string>(typeof(GameLevelState), null), () => gameState);
            _data.Add(new Tuple<Type, string>(typeof(BooleanState), "flashLightState"), () => flashLightState);
        }

        public T Locate<T>(string name = null) => (T)_data[new Tuple<Type, string>(typeof(T), name)].Invoke();

        public T Create<T>(T someObject, string name = null)
        {
            _data[new Tuple<Type, string>(typeof(T), name)] = () => someObject;
            return Locate<T>(name);
        }
    }
}