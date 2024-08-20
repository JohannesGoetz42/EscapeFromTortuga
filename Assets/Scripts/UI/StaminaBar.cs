using UnityEngine;
using UnityEngine.UIElements;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    private CharacterControllerBase characterController;
    [SerializeField]
    private UnityEngine.UI.Image staminaImage;

    private ProgressBar _progressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _progressBar = GetComponent<UIDocument>().rootVisualElement.Q("StaminaBar") as ProgressBar;
        if ((_progressBar == null && staminaImage == null) || characterController == null)
        {
            enabled = false;
            Debug.LogWarning(string.Format("StaminaBar on {0} has invalid data and will be inactive!", gameObject.name));
            return;
        }

        _progressBar.highValue = characterController.MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (_progressBar != null)
        {
            _progressBar.value = characterController.CurrentStamina;
        }
        if (staminaImage != null)
        {
            Vector2 newSize = staminaImage.rectTransform.sizeDelta;
            newSize.x = 100.0f * (characterController.CurrentStamina / characterController.MaxStamina);
            staminaImage.rectTransform.sizeDelta = newSize;
        }
    }
}
