using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameMaster.State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class SceneLoadObserver
    {
        private readonly SceneLoadState _sceneState;
        private readonly string _sceneName;

        public SceneLoadObserver(SceneLoadState sceneState, string sceneName)
        {
            _sceneState = sceneState;
            _sceneName = sceneName;
        }

        public void Observe(MonoBehaviour lifecycle)
        {
            lifecycle.StartCoroutine(OpenObserver());
            lifecycle.StartCoroutine(CloseObserver());
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator OpenObserver()
        {
            while (true)
            {
                yield return new WaitUntil(_sceneState.OpenRequest);
                _sceneState.Sync();
                SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CloseObserver()
        {
            while (true)
            {
                yield return new WaitUntil(_sceneState.CloseRequest);
                _sceneState.Sync();
                SceneManager.UnloadSceneAsync(_sceneName);
            }
        }
    }
}