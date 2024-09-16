using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SpawnLocationType
{
    Item,
    CrewMate
}

public class SpawnLocation : MonoBehaviour
{
    [field: SerializeField]
    SpawnLocationType[] locationTypes;

    static Dictionary<SpawnLocationType, List<GameObject>> _allSpawnLocations;

    public static GameObject GetRandomSpawnLocation(SpawnLocationType locationType)
    {
        if (_allSpawnLocations == null || _allSpawnLocations[locationType] == null)
        {
            return null;
        }

        int randomIndex = Random.Range(0, _allSpawnLocations[locationType].Count);
        return _allSpawnLocations[locationType][randomIndex];
    }

    private void Awake()
    {
        if (_allSpawnLocations != null)
        {
            return;
        }

        _allSpawnLocations = new Dictionary<SpawnLocationType, List<GameObject>>();

        // initialize spawn locations
        SpawnLocation[] spawnLocations = FindObjectsByType<SpawnLocation>(FindObjectsSortMode.None);
        foreach (SpawnLocation location in spawnLocations)
        {
            foreach (SpawnLocationType type in location.locationTypes)
            {
                if (_allSpawnLocations.ContainsKey(type))
                {
                    _allSpawnLocations[type].Add(location.gameObject);
                }
                else
                {
                    List<GameObject> locationsForType = new List<GameObject>() { location.gameObject };
                    _allSpawnLocations.Add(type, locationsForType);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (locationTypes == null || locationTypes.Length == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.5f);
            return;
        }

        int typeIndex = 0;
        foreach (SpawnLocationType type in locationTypes)
        {
            switch (type)
            {
                case SpawnLocationType.Item:
                    Gizmos.color = Color.yellow;
                    break;
                case SpawnLocationType.CrewMate:
                    Gizmos.color = Color.green;
                    break;
            }

            // draw the innermost spere solid for easier clicking
            if (typeIndex == 0)
            {

                Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.5f + typeIndex * 0.1f);
            }
            // draw the other speres as wire
            else
            {
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.5f + typeIndex * 0.1f);
            }

            typeIndex++;
        }
    }
#endif
}
