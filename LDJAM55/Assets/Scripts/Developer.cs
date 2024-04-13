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
        [Range(1, 3)]
        int power;
        [SerializeField]
        [Range(1, 5)]
        int durability;
        public int Durability { get => durability; set => durability = value; }
    }
}