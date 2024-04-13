using System;

namespace DataTypes
{
    public class ProductState
    {
        const float scoreLowerBound = 0f;
        const float scoreUpperBound = 1f;

        // Primary scores
        float mechanics = 0f;
        float audioScore = 0f;
        float visualsScore = 0f;
        float polish = 1f;

        // Auxiliary scores
        float funScore = 0f;
        float innovationScore = 0f;
        float themeScore = 0f;
        float graphicsScore = 0f;
        float humorScore = 0f;
        float moodScore = 0f;

        // TODO: weighted avg of all?
        public float OverallScore { get => 1.0f; }

        public float Mechanics { get => mechanics; set => mechanics = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioScore { get => audioScore; set => audioScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float VisualsScore { get => visualsScore; set => visualsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioVisuals => (audioScore + visualsScore) / 2;
        // Note: polish is not minimum limited to 0 currently
        public float Polish { get => polish; set => polish = Math.Clamp(value, float.MinValue, scoreUpperBound); }
        public float FunScore { get => funScore; set => funScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float InnovationScore { get => innovationScore; set => innovationScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float ThemeScore { get => themeScore; set => themeScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float GraphicsScore { get => graphicsScore; set => graphicsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float HumorScore { get => humorScore; set => humorScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float MoodScore { get => moodScore; set => moodScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
    }
}
