using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            SelectMaterial(GameManager.Instance.SelectedMaterial);
        }
    }

    public void SelectNextMaterial()
    {
        SelectMaterial(GameManager.Instance.SelectedMaterial + 1);
    }
    public void SelectPreviousMaterial()
    {
        SelectMaterial(GameManager.Instance.SelectedMaterial - 1);
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

        GameManager.Instance.SelectedMaterial = materialIndex;
    }
}
