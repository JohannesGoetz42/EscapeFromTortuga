using UnityEngine;
using System.Collections.Generic;

public class ModelSwitch : MonoBehaviour
{
    [SerializeField]
    private bool isPlayerOwned;
    private List<GameObject> _availableModels = new List<GameObject>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("ModelOption"))
            {
                _availableModels.Add(child.gameObject);
            }
        }

        if (GameManager.Instance != null)
        {
            if (gameObject.CompareTag("Player"))
            {
                SelectModel(GameManager.Instance.SelectedCharacter);
            }
            else
            {
                int selectedModel = Random.Range(0, _availableModels.Count);
                SelectModel(selectedModel);
            }
        }
    }

    public void SelectNextModel()
    {
        int newIndex = GameManager.Instance.SelectedCharacter + 1;
        SelectModel(newIndex);
    }

    public void SelectPreviousModel()
    {
        int newIndex = GameManager.Instance.SelectedCharacter - 1;
        SelectModel(newIndex);
    }

    void SelectModel(int modelIndex)
    {
        if (modelIndex < 0)
        {
            modelIndex = _availableModels.Count + modelIndex;
        }

        modelIndex %= _availableModels.Count;

        // set the selected model active and all other available models inactive
        for (int i = 0; i < _availableModels.Count; i++)
        {
            _availableModels[i].SetActive(modelIndex == i);
        }

        if (isPlayerOwned)
        {
            GameManager.Instance.SelectedCharacter = modelIndex;
        }
    }
}
