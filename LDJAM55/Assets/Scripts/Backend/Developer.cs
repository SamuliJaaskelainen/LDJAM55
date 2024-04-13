using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    [System.Serializable]
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
        string name = "DeveloperName";
        [SerializeField]
        List<DialogueManager.Dialogue> dialogue = new List<DialogueManager.Dialogue>();
        [SerializeField]
        RoleType role = RoleType.Designer;
        [SerializeField]
        List<Trait> traits;
        [SerializeField]
        [Range(0f, 1f)]
        float power = 0.5f;
        [SerializeField]
        [Range(0f, 300f)]
        // Seconds
        float durability = 5.0f;

        public string Name { get => name; }
        public List<DialogueManager.Dialogue> Dialogue { get => dialogue; }
        public RoleType Role { get => role; }
        public List<Trait> Traits { get => traits; }
        public float Power { get => power; set => power = value; }
        public float Durability { get => durability; set => durability = value; }
        public bool IsAlive { get => durability > 0f; }

    };
}