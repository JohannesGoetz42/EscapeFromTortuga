using UnityEngine;

public class MaterialSwitch : MonoBehaviour
{
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
