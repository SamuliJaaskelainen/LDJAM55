using System.Collections.Generic;
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

        public enum Trait
        {
            Fun,
            Innovation,
            Theme,
            Graphics,
            Audio,
            Humor,
            Mood
        }

        [SerializeField]
        readonly string name = "DeveloperName";
        [SerializeField]
        readonly string dialogue = "Hello World!";
        [SerializeField]
        readonly RoleType role = RoleType.Designer;
        [SerializeField]
        readonly List<Trait> traits;
        [SerializeField]
        [Range(0f, 1f)]
        readonly float power = 0.5f;
        [SerializeField]
        [Range(0f, 60f)]
        // Seconds
        float durability = 5.0f;

        public string Name { get => dialogue; }
        public string Dialogue { get => dialogue; }
        public RoleType Role { get => role; }
        public List<Trait> Traits { get => traits; }
        public float Power { get => power; }
        public float Durability { get => durability; set => durability = value; }
        public bool IsAlive { get => durability > 0f; }

    };
}