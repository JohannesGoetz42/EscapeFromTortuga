using UnityEngine;

public class EscapeCrew : EscapeAgentBase
{
    public delegate void EscapeCrewDelegate(EscapeCrew crewMember);

    public EscapeCrewDelegate crewMateReadyToDepart;
    EscapeAgent _escapeAgent;

    public static EscapeCrew InitializeEscapeCrew(EscapeCrew prefab, EscapeAgent escapeAgent, in Vector3 spawnLocation)
    {
        if (prefab == null)
        {
            return null;
        }

        GameObject createdInstance = Instantiate(prefab.gameObject, spawnLocation, Quaternion.identity);
        EscapeCrew createdCrew = createdInstance.GetComponent<EscapeCrew>();
        createdCrew._escapeAgent = escapeAgent;

        WorldMarker worldMarker = createdInstance.GetComponent<WorldMarker>();
        if (worldMarker != null)
        {
            worldMarker.AddMarkerSource(createdCrew, WorldMarkerVisibility.OverheadAndScreenBorder);
        }

        return createdCrew;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") || blackboardInstance == null || dialogueText == null ||
            blackboardInstance.GetValueAsEnum(EscapeAgentStateKey) == (int)EscapeAgentState.ReadyToDepart)
        {
            return;
        }

        blackboardInstance.SetValueAsEnum(EscapeAgentStateKey, EscapeAgentState.ReadyToDepart);
        blackboardInstance.SetValueAsObject(EscapeAreaKey, _escapeAgent.escapeArea);

        dialogueText.SetText(FormatText(readyToDepartText));

        if (crewMateReadyToDepart != null)
        {
            crewMateReadyToDepart.Invoke(this);
        }
    }

    string FormatText(string format)
    {
        format = format.Replace("[" + nameof(EscapeAgent.escapeArea) + "]", _escapeAgent.escapeArea.DisplayName);
        return format;
    }
}
