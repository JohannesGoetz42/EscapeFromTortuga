using UnityEngine;
using System.Collections.Generic;

public class ModelSwitch : MonoBehaviour
{
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
        SelectModel(GameManager.Instance.SelectedCharacter + 1);
    }
    public void SelectPreviousModel()
    {
        SelectModel(GameManager.Instance.SelectedCharacter - 1);
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

        GameManager.Instance.SelectedCharacter = modelIndex;
    }
}
