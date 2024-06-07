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
            var miniGamePassedState = new State();
            var miniGameOverlayState = new IntentState();
            var pauseOverlayState = new IntentState();
            var tooltipVisibilityState = new State();
            _data.Add(new Tuple<Type, string>(typeof(State), "miniGamePassedState"), () => miniGamePassedState);
            _data.Add(new Tuple<Type, string>(typeof(IntentState), "miniGameOverlayState"), () => miniGameOverlayState);
            _data.Add(new Tuple<Type, string>(typeof(IntentState), "pauseOverlayState"), () => pauseOverlayState);
            _data.Add(new Tuple<Type, string>(typeof(State), "tooltipVisibilityState"), () => tooltipVisibilityState);
        }

        private readonly Dictionary<Tuple<Type, string>, Func<object>> _data = new()
        {
            { new Tuple<Type, string>(typeof(string), "log"), () => "my message" }
        };

        public T Locate<T>(string name = null) => (T)_data[new Tuple<Type, string>(typeof(T), name)].Invoke();
    }
}