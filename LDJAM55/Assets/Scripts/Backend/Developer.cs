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
        [Range(0f, 1f)]
        readonly float power = 1;
        [SerializeField]
        [Range(0f, 60f)]
        // Seconds
        float durability = 3;

        public string Name { get => dialogue; }
        public string Dialogue { get => dialogue; }

        public RoleType Role { get => role; }
        public float Power { get => power; }
        public float Durability { get => durability; set => durability = value; }
        public bool isAlive { get => durability > 0f; }

    };
}