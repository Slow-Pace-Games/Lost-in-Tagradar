using UnityEngine;
using System;

public class DateComparer : MonoBehaviour
{
    public DateTime CompareDates(DateTime date1, DateTime date2)
    {
        // Comparer les deux dates
        if (date1 > date2)
        {
            return date1; // date1 est plus récente
        }
        else
        {
            return date2; // date2 est plus récente ou les deux dates sont égales
        }
    }
}
