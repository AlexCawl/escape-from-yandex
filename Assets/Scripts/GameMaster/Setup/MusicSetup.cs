using UnityEngine;

namespace GameMaster.Setup
{
    public class MusicSetup : MonoBehaviour
    {
        public AudioSource backgroundMusic;
        public AudioSource soundEffects;

        public AudioClip ost;
        public AudioClip hurt;
        public AudioClip shoot;
        public AudioClip heal;
        public AudioClip buttonClick;
        public AudioClip gameCompleted;
        public AudioClip gameFailed;

        private MusicPlayObserver _backgroundOstObserver;

        private void Awake()
        {
            _backgroundOstObserver = ServiceLocator.Get.Create(new MusicPlayObserver(ost, backgroundMusic), "backgroundMusic");
            ServiceLocator.Get.Create(new MusicPlayObserver(hurt, soundEffects), "hurtSound");
            ServiceLocator.Get.Create(new MusicPlayObserver(shoot, soundEffects), "shootSound");
            ServiceLocator.Get.Create(new MusicPlayObserver(heal, soundEffects), "healSound");
            ServiceLocator.Get.Create(new MusicPlayObserver(buttonClick, soundEffects), "clickSound");
            ServiceLocator.Get.Create(new MusicPlayObserver(gameCompleted, soundEffects), "gameDoneSound");
            ServiceLocator.Get.Create(new MusicPlayObserver(gameFailed, soundEffects), "gameFailedSound");
        }

        private void Start() => _backgroundOstObserver.Observe(this, true);
    }
}
