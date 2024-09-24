using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SpawnLocationType
{
    Item,
    CrewMate,
    Player
}

public class SpawnLocation : MonoBehaviour
{
    [field: SerializeField]
    SpawnLocationType[] locationTypes;

    static Dictionary<SpawnLocationType, List<GameObject>> _allSpawnLocations;

    public static List<GameObject> GetRandomSpawnLocations(SpawnLocationType locationType, int amount)
    {
        if (_allSpawnLocations == null || !_allSpawnLocations.ContainsKey(locationType))
        {
            return new List<GameObject>();
        }

        List<GameObject> result = new List<GameObject>();
        List<GameObject> availableLocations = _allSpawnLocations[locationType];

        while (amount > 0 && availableLocations.Count > 0)
        {
            int randomIndex = Random.Range(0, availableLocations.Count);
            result.Add(availableLocations[randomIndex]);
            availableLocations.RemoveAt(randomIndex);
            amount--;
        }

        return result;
    }

    public static GameObject GetRandomSpawnLocation(SpawnLocationType locationType)
    {
        if (_allSpawnLocations == null || !_allSpawnLocations.ContainsKey(locationType))
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
        SceneManager.sceneLoaded += ResetSpawnLocations;
    }

    private void ResetSpawnLocations(Scene arg0, LoadSceneMode arg1)
    {
        _allSpawnLocations.Clear();

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
                case SpawnLocationType.Player:
                    Gizmos.color = Color.magenta;
                    break;
            }

            // draw the innermost spere solid for easier clicking
            if (typeIndex == 0)
            {

                Gizmos.DrawSphere(transform.position + Vector3.up * 0.5f, 0.5f + typeIndex * 0.1f);
                Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + transform.forward + Vector3.up * 0.5f);
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
