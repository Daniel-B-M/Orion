using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private bool isOccupied;
    [SerializeField] private Customer currentCustomer;

    public bool IsOccupied
    {
        get { return isOccupied; }
    }

    public void OccupySeat(Customer customer)
    {
        isOccupied = true;
        currentCustomer = customer;
    }

    public void FreeSeat()
    {
        isOccupied = false;
        currentCustomer = null;
    }
}
