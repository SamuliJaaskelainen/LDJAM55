using UnityEngine;

namespace DataTypes
{
    public class Developer
    {
        enum Role
        {
            Designer,
            Programmer,
            Artist,
            QA,
            Audio,
            Producer,
            Influencer
        }

        [SerializeField]
        string name;
        [SerializeField]
        string dialogue;

        [SerializeField]
        Role role;
        [SerializeField]
        int power;
        [SerializeField]
        int durability;
    }
}