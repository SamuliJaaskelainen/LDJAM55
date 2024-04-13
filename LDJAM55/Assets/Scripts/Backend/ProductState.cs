using System;

namespace DataTypes
{
    // Final results for game end screen
    public struct FinalResult
    {
        public float funScore;
        public float innovationScore;
        public float themeScore;
        public float graphicsScore;
        public float audioScore;
        public float humorScore;
        public float moodScore;
        public float overallScore;
    }

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

        public float FunScore { get => funScore; set => funScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float InnovationScore { get => innovationScore; set => innovationScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float ThemeScore { get => themeScore; set => themeScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float GraphicsScore { get => graphicsScore; set => graphicsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float AudioScore { get => audioScore; set => audioScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float HumorScore { get => humorScore; set => humorScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public float MoodScore { get => moodScore; set => moodScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }

        public FinalResult computeFinalResult()
        {
            FinalResult result = new();

            float polishPenalty = 0.25f + 0.75f * polishFeature;

            result.funScore = ((funScore * 4f + mechanicsFeature * 2f + audioFeature + visualsFeature) / 8f) * polishPenalty;
            result.innovationScore = (innovationScore * 3f + mechanicsFeature + audioFeature * 0.5f + visualsFeature * 0.5f) / 5f;
            result.themeScore = (themeScore * 3f + mechanicsFeature * 0.5f + audioFeature * 0.75f + visualsFeature * 0.75f) / 5f;
            result.graphicsScore = ((graphicsScore + visualsFeature) / 2f) * polishPenalty;
            result.audioScore = ((audioScore + audioFeature) / 2f) * polishPenalty;
            result.humorScore = (humorScore * 2f + mechanicsFeature * 0.5f + audioFeature * 0.5f + visualsFeature) / 4f;
            result.moodScore = ((moodScore * 2f + mechanicsFeature * 0.5f + audioFeature + visualsFeature * 0.5f) / 4f) * polishPenalty;
            result.overallScore = (result.funScore + result.innovationScore + result.themeScore + result.graphicsScore + result.audioScore + result.humorScore + result.moodScore) / 7f;

            return result;
        }
    }
}
