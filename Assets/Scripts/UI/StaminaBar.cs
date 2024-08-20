using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    private CharacterControllerBase characterController;

    private ProgressBar _progressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        _progressBar = document.rootVisualElement.Q("StaminaBar") as ProgressBar;
        if (_progressBar == null || characterController == null)
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
        _progressBar.value = characterController.CurrentStamina;
    }
}
