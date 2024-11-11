using FootBall;
using UnityEngine;

namespace Football
{
    public class UIManager : MonoBehaviour
    {
        public GameObject SettingsMenu;

        private void Update()
        {
            if (InputManager.Data.ToggleMenuTriggered)
            {
                SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
            }
        }
    }
}

