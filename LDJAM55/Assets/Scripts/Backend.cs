using DataTypes;
using System.Collections.Generic;
using UnityEngine;
public class Backend : MonoBehaviour
{
    [SerializeField]
    List<Developer> developerPool;

    // "Desks"
    Developer[] activeDevelopers = new Developer[4];

    List<Task> hiddenBugs;
    List<Task> foundBugs;
    List<Task> backlog;

    ProductState state;

    public void progressTick()
    {
        foreach (Developer developer in activeDevelopers)
        {

            if (developer.d)
        }
    }
}