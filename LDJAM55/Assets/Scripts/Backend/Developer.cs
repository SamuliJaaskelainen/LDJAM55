using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    [System.Serializable]
    public class Developer
    {
        public enum RoleType
        {
            Designer = 0,
            Programmer = 1,
            QA = 2,
            Artist = 3,
            Audio = 4,
            Producer = 5,
            Influencer = 6
        }

        public enum Trait
        {
            Fun = 0,
            Innovation = 1,
            Theme = 2,
            Graphics = 3,
            Audio = 4,
            Humor = 5,
            Mood = 6
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
        [SerializeField]
        public Sprite portrait;

        public string Name { get => name; }
        public List<DialogueManager.Dialogue> Dialogue { get => dialogue; set => dialogue = value; }
        public RoleType Role { get => role; }
        public List<Trait> Traits { get => traits; }
        public float Power { get => power; set => power = value; }
        public float Durability { get => durability; set => durability = value; }
        public bool IsAlive { get => durability > 0f; }
        public Developer Clone()
        {
            return (Developer)MemberwiseClone();
        }
    };
}