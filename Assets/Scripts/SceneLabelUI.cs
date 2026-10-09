using UnityEngine;
using TMPro;

namespace ThanethHamburut
{
    public class SceneLabelUI : MonoBehaviour
    {
        [SerializeField] private string sceneLabelText = "";
        [SerializeField] private TMP_Text labelComponent;

        private void Start()
        {
            if (labelComponent == null)
            {
                labelComponent = GetComponent<TMP_Text>();
            }

            if (labelComponent != null && !string.IsNullOrEmpty(sceneLabelText))
            {
                labelComponent.text = sceneLabelText;
            }
        }
    }
}
