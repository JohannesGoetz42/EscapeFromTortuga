using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public delegate void KeyItemDelegate(KeyItem item);

    public KeyItemDelegate keyItemStored;
    public KeyItemDelegate keyItemRemoved;

    List<KeyItem> _keyRing = new List<KeyItem>();

    public void StoreItem(KeyItem item)
    {
        _keyRing.Add(item);
        keyItemStored.Invoke(item);
    }

    public void RemoveItem(KeyItem item)
    {
        _keyRing.Remove(item);
        keyItemRemoved.Invoke(item);
    }
}
