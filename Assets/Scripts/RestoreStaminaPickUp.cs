using UnityEngine;

public class RestoreStaminaPickUp : PickUpBase
{
    [SerializeField]
    float restoredAmount;

    protected override void OnPickedUp(GameObject player)
    {
        CharacterControllerBase controller = player.GetComponent<CharacterControllerBase>();
        if (controller != null)
        {
            controller.RestoreStamina(restoredAmount);
            Destroy(gameObject);
        }
    }
}
