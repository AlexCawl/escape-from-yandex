using System.Collections;
using UnityEngine;

namespace GameMaster
{
    public class MusicPlayObserver
    {
        private readonly AudioSource _controller;
        private readonly AudioClip _music;

        public MusicPlayObserver(AudioClip music, AudioSource controller)
        {
            _music = music;
            _controller = controller;
        }

        public void Observe(MonoBehaviour lifecycle, bool loop = false) => lifecycle.StartCoroutine(Play(loop));

        private IEnumerator Play(bool loop)
        {
            _controller.loop = loop;
            _controller.clip = _music;
            _controller.Play();
            yield break;
        }
    }
}