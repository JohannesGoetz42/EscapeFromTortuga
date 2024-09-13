using UnityEngine;

[CreateAssetMenu(fileName = "KeyItem", menuName = "Scriptable Objects/KeyItem")]
public class KeyItem : ScriptableObject
{
    public GameObject pickUpPrefab;
    public Sprite thumbnail;
}
