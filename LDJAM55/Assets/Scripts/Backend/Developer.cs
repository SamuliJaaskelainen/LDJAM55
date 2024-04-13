using UnityEngine;

namespace DataTypes
{
    public class Developer
    {
        public enum RoleType
        {
            Designer,
            Programmer,
            QA,
            Artist,
            Audio,
            Producer,
            Influencer
        }

        [SerializeField]
        readonly string name = "DeveloperName";
        [SerializeField]
        readonly string dialogue = "Hello World!";
        [SerializeField]
        readonly RoleType role = RoleType.Designer;
        [SerializeField]
        [Range(1, 3)]
        readonly int power = 1;
        [SerializeField]
        [Range(1, 5)]
        int durability = 3;

        public string Name { get => dialogue; }
        public string Dialogue { get => dialogue; }

        public RoleType Role { get => role; }
        public int Power { get => power; }
        public int Durability { get => durability; set => durability = value; }

    };
}