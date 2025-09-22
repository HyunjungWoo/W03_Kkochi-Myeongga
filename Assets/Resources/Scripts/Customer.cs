using UnityEngine;

public class Customer : MonoBehaviour
{
    private Skewer skewerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stick"))
        {
            skewerInRange = other.GetComponent<Skewer>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stick") && skewerInRange == other.GetComponent<Skewer>())
        {
            skewerInRange = null;
        }
    }

    public void TrySellSkewer()
    {
        if (skewerInRange != null)
        {
            Debug.Log($"Sold skewer for {skewerInRange.price} currency!");
            skewerInRange.ResetSkewer();
            skewerInRange = null;
        }
        else
        {
            Debug.Log("No skewer to sell!");
        }
    }

}
