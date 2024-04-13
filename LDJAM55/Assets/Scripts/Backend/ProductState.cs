using System;

namespace DataTypes
{
    public class ProductState
    {
        const float scoreLowerBound = 0f;
        const float scoreUpperBound = 1f;

        // Primary scores (features). Affected by power of certain roles.
        float mechanicsFeature = 0f;
        float audioFeature = 0f;
        float visualsFeature = 0f;
        float polishFeature = 1f;

        // Auxiliary scores. Affected by traits.
        float funScore = 0f;
        float innovationScore = 0f;
        float themeScore = 0f;
        float graphicsScore = 0f;
        float audioScore = 0f;
        float humorScore = 0f;
        float moodScore = 0f;

        public float MechanicsFeature { get => mechanicsFeature; set => mechanicsFeature = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioFeature { get => audioFeature; set => audioFeature = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float VisualsFeature { get => visualsFeature; set => visualsFeature = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioVisualsFeature => (AudioFeature + VisualsFeature) / 2f;
        // Note: polish is not lower bounded to 0 currently
        public float PolishFeature { get => polishFeature; set => polishFeature = Math.Clamp(value, float.MinValue, scoreUpperBound); }

        // TODO: weighted avg of all?
        public float OverallScore { get => 1.0f; }
        public float FunScore { get => funScore; set => funScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float InnovationScore { get => innovationScore; set => innovationScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float ThemeScore { get => themeScore; set => themeScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float GraphicsScore { get => graphicsScore; set => graphicsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioScore { get => audioScore; set => audioScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float HumorScore { get => humorScore; set => humorScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float MoodScore { get => moodScore; set => moodScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
    }
}
