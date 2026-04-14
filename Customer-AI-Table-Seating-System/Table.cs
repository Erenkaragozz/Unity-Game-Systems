using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public List<SeatPoint> seats = new List<SeatPoint>();

    private void Awake()
    {
        seats.Clear();
        SeatPoint[] foundSeats = GetComponentsInChildren<SeatPoint>();
        seats.AddRange(foundSeats);
    }

    //bu fonksiyon hem bos yer bulur hem de o an ayirir 
    public SeatPoint OccupyFirstFreeSeat()
    {
        foreach (SeatPoint seat in seats)
        {
            if (!seat.isOccupied)
            {
                seat.isOccupied = true; //yoldayken burayi kapatiyoruz baska npc gelmesin diye
                return seat;
            }
        }
        return null;
    }
}

// Erenkaragozz's custom script for [*****]
