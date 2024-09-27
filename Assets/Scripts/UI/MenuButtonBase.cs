using UnityEngine;
using UnityEngine.UIElements;

public abstract class MenuButtonBase : MonoBehaviour
{
    [SerializeField]
    protected string buttonName;

    private Button _button;

    protected void Awake()
    {
        UIDocument document = GetComponent<UIDocument>();
        if (document == null)
        {
            return;
        }

        _button = document.rootVisualElement.Q(buttonName) as Button;
        if (_button == null)
        {
            Debug.LogWarning(string.Format("script requires a Button named '{0}'!", buttonName));
            return;
        }

        _button.RegisterCallback<ClickEvent>(OnClick);
    }
    private void OnDisable()
    {
        if (_button == null)
        {
            return;
        }

        _button.UnregisterCallback<ClickEvent>(OnClick);
    }
    virtual protected void OnClick(ClickEvent click) 
    { 
        UISoundManager.OnClick(click);
    }
}
