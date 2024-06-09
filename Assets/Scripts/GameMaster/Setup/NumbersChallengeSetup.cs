using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private State _miniGamePassedState;
        private SceneLoadState _miniGameState;

        private void Start()
        {
            _miniGamePassedState = ServiceLocator.Get.Locate<State>("miniGamePassedState");
            _miniGameState = ServiceLocator.Get.Locate<SceneLoadState>("miniGameState");
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
            label.text = win ? "Success!" : "Wrong Order!";
            label.color = win ? Color.green : Color.red;
            _miniGamePassedState.Set(win);
            yield return new WaitForSeconds(1f);
            _miniGameState.Toggle();
        }
    }
}