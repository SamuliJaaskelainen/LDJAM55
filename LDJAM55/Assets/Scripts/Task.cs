using UnityEngine;

namespace DataTypes
{
    public class Task
    {
        [SerializeField]
        [Range(1, 10)]
        const int cost = 5;

        [Range(0, 10)]
        int progress = 0;
    }
}