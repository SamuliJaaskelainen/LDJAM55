using UnityEngine;

namespace DataTypes
{
    public class Task
    {
        // TODO: should we have separate values for the size (required work) and impact (effect on features/polish etc metrics) ?
        [SerializeField]
        [Range(0f, 900f)]
        // Required power to complete the task, seconds
        readonly float size;

        [Range(0f, 900f)]
        // Progressed power to complete the task, seconds
        float progress = 0f;

        public Task(float cost)
        {
            this.size = cost;
        }

        public float Size { get => size; }

        public float Progress { get => progress; set => progress = value; }

        public float WorkRemaining { get => size - progress; }
    }
}