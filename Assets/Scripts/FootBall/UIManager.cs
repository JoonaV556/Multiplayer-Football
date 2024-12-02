using FootBall;
using UnityEngine;
using UnityEngine.UI;

namespace Football
{
    public class UIManager : MonoBehaviour
    {
        public GameObject SettingsMenu;

        public Image TeamUiImage;

        public GameObject ReadyUIParent;
        public GameObject ReadyUI;
        public GameObject NotReadyUI;

        private void OnEnable()
        {
            PlayerTeamHandler.OnLocalPlayerTeamChanged += UpdateTeamUI;

            ReadyHandler.OnLocalReadyHandlerSpawned += ShowReadyParent;
            ReadyHandler.OnLocalReadyStateChanged += UpdateReadyUI;
            ReadyHandler.OnLocalReadyHandlerDespawned += HideReadyParent;
        }

        private void OnDisable()
        {
            PlayerTeamHandler.OnLocalPlayerTeamChanged -= UpdateTeamUI;

            ReadyHandler.OnLocalReadyHandlerSpawned -= ShowReadyParent;
            ReadyHandler.OnLocalReadyStateChanged -= UpdateReadyUI;
            ReadyHandler.OnLocalReadyHandlerDespawned -= HideReadyParent;
        }

        private void ShowReadyParent()
        {
            ReadyUIParent.SetActive(true);
        }

        private void HideReadyParent()
        {
            ReadyUIParent.SetActive(true);
        }

        private void UpdateReadyUI(bool readyState)
        {
            if (readyState)
            {
                ReadyUI.SetActive(true);
                NotReadyUI.SetActive(false);
            }
            else
            {
                ReadyUI.SetActive(false);
                NotReadyUI.SetActive(true);
            }
        }

        private void UpdateTeamUI(Team team)
        {
            TypeLogger.TypeLog(this, $"Changed team UI color to {team.ToString()}", 1);
            TeamUiImage.gameObject.SetActive(true);
            TeamUiImage.color = Colors.TeamColors[team];
        }

        private void Update()
        {
            if (InputManager.Data.ToggleMenuTriggered)
            {
                SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
            }
        }
    }
}

