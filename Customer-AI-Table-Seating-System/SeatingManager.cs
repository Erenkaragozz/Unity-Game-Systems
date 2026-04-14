using UnityEngine;
using System.Collections.Generic;
using System.Linq; // listeyi karıstırmak ıcın lazim

public class SeatingManager : MonoBehaviour
{
    public static SeatingManager Instance;
    public Table[] tables;

    private void Awake()
    {
        Instance = this;
    }

    public SeatPoint GetAndReserveFreeSeat()
    {
        //masalari gecici bi listeye alıp rastgele karıstırma
        List<Table> shuffledTables = tables.ToList();
        for (int i = 0; i < shuffledTables.Count; i++)
        {
            Table temp = shuffledTables[i];
            int randomIndex = Random.Range(i, shuffledTables.Count);
            shuffledTables[i] = shuffledTables[randomIndex];
            shuffledTables[randomIndex] = temp;
        }

        // karıstırılmıs lstede bos yer ara
        foreach (Table table in shuffledTables)
        {
            SeatPoint freeSeat = table.OccupyFirstFreeSeat();
            if (freeSeat != null) return freeSeat;
        }
        return null;
    }
}

// Erenkaragozz's custom script for [*****]