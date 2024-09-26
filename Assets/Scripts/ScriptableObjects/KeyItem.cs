using UnityEngine;

[CreateAssetMenu(fileName = "KeyItem", menuName = "Scriptable Objects/KeyItem")]
public class KeyItem : ScriptableObject, IHasThumbnail
{
    public GameObject pickUpPrefab;
    public Sprite thumbnail;
    public Sprite Thumbnail => thumbnail;
    public Color color = Color.white;
    public Color BackgroundColor => color;

}
