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
        public RoleType Role { get => role; set => role = value; }
        public List<Trait> Traits { get => traits; set => traits = value; }
        public float Power { get => power; set => power = value; }
        public float Durability { get => durability; set => durability = value; }
        public bool IsAlive { get => durability > 0f; }
        public Developer Clone()
        {
            return (Developer)MemberwiseClone();
        }
        public static int RandomDeveloperIndex(bool allowAudioSpawn)
        {
            // Weights
            int designerIndexes = 6;
            int programmerIndexes = 15 + designerIndexes;
            int qaIndexes = 9 + programmerIndexes;
            int artistIndexes = 6 + qaIndexes;
            int audioIndexes = (allowAudioSpawn ? 15 : 0) + artistIndexes;
            int producerIndexes = 2 + audioIndexes;
            int influencerIndexes = 2 + producerIndexes;

            int rollDeveloper = UnityEngine.Random.Range(0, influencerIndexes);
            if (rollDeveloper < designerIndexes)
            {

                return 0;
            }
            else if (rollDeveloper < programmerIndexes)
            {

                return 1;
            }
            else if (rollDeveloper < qaIndexes)
            {

                return 2;
            }
            else if (rollDeveloper < artistIndexes)
            {

                return 3;
            }
            else if (rollDeveloper < audioIndexes)
            {

                return 4;
            }
            else if (rollDeveloper < producerIndexes)
            {
                return 5;
            }
            return 6;
        }
        public static float RandomPower(float boost)
        {
            const double mean = 0.5;
            const double stdDev = 0.2;
            double u1 = (double)(1.0f - UnityEngine.Random.Range(0.0f, 1.0f));
            double u2 = (double)(1.0f - UnityEngine.Random.Range(0.0f, 1.0f));
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;
            return Math.Clamp((float)randNormal + boost, 0.1f, 1f);
        }

        public static float RandomDurability()
        {
            const double mean = 30.0;
            const double stdDev = 10.0;
            double u1 = (double)(1.0f - UnityEngine.Random.Range(0.0f, 1.0f));
            double u2 = (double)(1.0f - UnityEngine.Random.Range(0.0f, 1.0f));
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;
            return Math.Clamp((float)randNormal, 20f, 70f);
        }

        public static List<Trait> RandomTraits()
        {
            List<Trait> traits = new();
            while (traits.Count < 4 && (traits.Count == 0 || UnityEngine.Random.Range(0, 2) > 0))
            {
                Trait trait = (Trait)UnityEngine.Random.Range(0, 7);
                if (!traits.Contains(trait))
                {
                    traits.Add(trait);
                }
            }
            return traits;
        }
    }
}