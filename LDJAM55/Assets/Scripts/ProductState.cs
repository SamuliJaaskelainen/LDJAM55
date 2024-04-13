using UnityEngine;

namespace DataTypes
{
    public class ProductState
    {
        // Primary scores
        [Range(0, 100)]
        int mechanics = 0;
        [Range(0, 100)]
        int audioVisuals = 0;
        [Range(0, 100)]
        int polish = 50;

        // Auxiliary scores
        [Range(0, 100)]
        int overallScore = 0;
        [Range(0, 100)]
        int funScore = 0;
        [Range(0, 100)]
        int innovationScore = 0;
        [Range(0, 100)]
        int themeScore = 0;
        [Range(0, 100)]
        int graphicsScore = 0;
        [Range(0, 100)]
        int audioScore = 0;
        [Range(0, 100)]
        int humorScore = 0;
        [Range(0, 100)]
        int moodScore = 0;
    }
}
