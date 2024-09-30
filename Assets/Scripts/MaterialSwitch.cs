using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    [SerializeField]
    private bool isPlayerOwned;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (gameObject.CompareTag("Player"))
        {
            SelectMaterial(GameManager.Instance.SelectedMaterial);
        }
        else
        {
            int selectedModel = Random.Range(0, GameManager.Instance.AvailableMaterials.Length);
            SelectMaterial(selectedModel);
        }
    }

    public void SelectNextMaterial()
    {
        int newMaterial = GameManager.Instance.SelectedMaterial + 1;
        SelectMaterial(newMaterial);
    }
    public void SelectPreviousMaterial()
    {
        int newMaterial = GameManager.Instance.SelectedMaterial - 1;
        SelectMaterial(newMaterial);
    }

    void SelectMaterial(int materialIndex)
    {
        if (materialIndex < 0)
        {
            materialIndex = GameManager.Instance.AvailableMaterials.Length + materialIndex;
        }

        materialIndex %= GameManager.Instance.AvailableMaterials.Length;

        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("ModelOption"))
            {
                foreach (SkinnedMeshRenderer model in child.GetComponents<SkinnedMeshRenderer>())
                {
                    if (model != null)
                    {
                        model.material = GameManager.Instance.AvailableMaterials[materialIndex];
                    }
                }
            }
        }

        if (isPlayerOwned)
        {
            GameManager.Instance.SelectedMaterial = materialIndex;
        }
    }
}
