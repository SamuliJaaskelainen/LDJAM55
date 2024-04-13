using UnityEngine;

namespace DataTypes
{
    public class Task
    {
        // TODO: should we have separate values for the size (required work) and impact (effect on features/polish etc metrics) ?
        [SerializeField]
        [Range(1, 10)]
        readonly int size;

        [Range(0, 10)]
        int progress = 0;

        public Task(int cost)
        {
            this.size = cost;
        }

        public int Size { get => size; }

        public int Progress { get => progress; set => progress = value; }

        public bool IsCompleted { get => progress == size; }
    }
}