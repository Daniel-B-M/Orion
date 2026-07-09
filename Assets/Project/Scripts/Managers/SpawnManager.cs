using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Seat[] seats;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float spawnIntervalMin = 2f;
    [SerializeField] private float spawnIntervalMax = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Start()
    {
        StartCoroutine(SpawnRutine());
    }
    private void SpawnCustomer()
    {
        List<Seat> freeSeats = new List<Seat>();
        foreach (Seat seat in seats)
        {
            if (!seat.IsOccupied)
            {
                freeSeats.Add(seat);
            }
        }
        
        if (freeSeats.Count == 0) return; // No free seats available
        
        int randomIndex = Random.Range(0, freeSeats.Count);
        Seat availableSeat = freeSeats[randomIndex];

        GameObject newCustomer = Instantiate(customerPrefab, doorTransform.position, Quaternion.identity);
        Customer customerScript = newCustomer.GetComponent<Customer>();
        availableSeat.OccupySeat(customerScript);
        customerScript.Initialize(availableSeat, doorTransform);
    }

    private IEnumerator SpawnRutine()
    {
        while (true)
        {
            float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(waitTime);
            SpawnCustomer();
        }
    }
}
