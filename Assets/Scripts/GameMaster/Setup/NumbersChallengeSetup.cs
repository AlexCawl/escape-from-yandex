using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameMaster.State;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameMaster.Setup
{
    public class NumbersChallengeManager : MonoBehaviour
    {
        public List<Button> buttons;
        public Text label;
        
        private List<Button> _shuffledButtons;
        private int _counter;
        private BooleanState _miniGameCompleteState;
        private SceneLoadState _miniGameState;
        private MusicPlayObserver _clickSoundEffect;
        private MusicPlayObserver _winSoundEffect;
        private MusicPlayObserver _failureSoundEffect;

        private void Start()
        {
            _miniGameCompleteState = ServiceLocator.Get.Locate<BooleanState>("miniGameCompleteState");
            _miniGameState = ServiceLocator.Get.Locate<SceneLoadState>("miniGameState");
            _clickSoundEffect = ServiceLocator.Get.Locate<MusicPlayObserver>("clickSound");
            _winSoundEffect = ServiceLocator.Get.Locate<MusicPlayObserver>("gameDoneSound");
            _failureSoundEffect = ServiceLocator.Get.Locate<MusicPlayObserver>("gameFailedSound");
            SetupGame();
        }

        private void SetupGame()
        {
            _counter = 0;
            _shuffledButtons = buttons.OrderBy(_ => Random.Range(1, 100)).ToList();
            for (var i = 0; i < _shuffledButtons.Count; i++)
            {
                var button = _shuffledButtons[i];
                button.GetComponentInChildren<Text>().text = (i + 1).ToString();
                button.interactable = true;
                button.image.color = Color.white;
            }
        }

        public void ButtonPressAction(Button button)
        {
            _clickSoundEffect.Observe(this);
            var buttonNumber = int.Parse(button.GetComponentInChildren<Text>().text);
            if (buttonNumber == _counter + 1)
            {
                _counter += 1;
                button.interactable = false;
                button.image.color = Color.green;
                if (_counter == buttons.Count)
                {
                    AssumeResult(true);
                }
            }
            else
            {
                AssumeResult(false);
            }
        }

        private void AssumeResult(bool win)
        {
            if (!win)
            {
                buttons.ForEach(button =>
                {
                    button.image.color = Color.red;
                    button.interactable = false;
                });
            }
            StartCoroutine(HandleExit(win));
        }

        private IEnumerator HandleExit(bool win)
        {
            var effect = win ? _winSoundEffect : _failureSoundEffect;
            effect.Observe(this);
            label.text = win ? "Success!" : "Wrong Order!";
            label.color = win ? Color.green : Color.red;
            _miniGameCompleteState.Set(win);
            yield return new WaitForSeconds(1f);
            _miniGameState.Toggle();
        }
    }
}