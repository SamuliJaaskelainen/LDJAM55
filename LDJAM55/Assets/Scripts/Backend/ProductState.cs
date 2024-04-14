using System;
using System.Collections.Generic;

namespace DataTypes
{
    // Final results for game end screen
    public struct FinalResult
    {
        public int funRank;
        public int innovationRank;
        public int themeRank;
        public int graphicsRank;
        public int audioRank;
        public int humorRank;
        public int moodRank;
        public int overallRank;
    }

    public class ProductState
    {
        const int otherContestantCount = 999;
        const float scoreLowerBound = 0f;
        const float scoreUpperBound = 1f;
        const float gameLengthSeconds = 900.0f;

        float timeLeft = gameLengthSeconds;

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

        public float RelativeTimeLeft => timeLeft / gameLengthSeconds;
        public float TimeLeft { get => timeLeft; set => timeLeft = value; }

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

        public FinalResult ComputeFinalResult()
        {
            FinalResult result = new();

            float polishPenalty = 0.25f + 0.75f * polishFeature;

            float finalFunScore = ((funScore * 4f + mechanicsFeature * 2f + audioFeature + visualsFeature) / 8f) * polishPenalty;
            float finalInnovationScore = (innovationScore * 3f + mechanicsFeature + audioFeature * 0.5f + visualsFeature * 0.5f) / 5f;
            float finalThemeScore = (themeScore * 3f + mechanicsFeature * 0.5f + audioFeature * 0.75f + visualsFeature * 0.75f) / 5f;
            float finalGraphicsScore = ((graphicsScore + visualsFeature) / 2f) * polishPenalty;
            float finalAudioScore = ((audioScore + audioFeature) / 2f) * polishPenalty;
            float finalHumorScore = (humorScore * 2f + mechanicsFeature * 0.5f + audioFeature * 0.5f + visualsFeature) / 4f;
            float finalMoodScore = ((moodScore * 2f + mechanicsFeature * 0.5f + audioFeature + visualsFeature * 0.5f) / 4f) * polishPenalty;
            float overallScore = (finalFunScore + finalInnovationScore + finalThemeScore + finalGraphicsScore + finalAudioScore + finalHumorScore + finalMoodScore) / 7f;

            result.funRank = scoreToRank(finalFunScore);
            result.innovationRank = scoreToRank(finalInnovationScore);
            result.themeRank = scoreToRank(finalThemeScore);
            result.graphicsRank = scoreToRank(finalGraphicsScore);
            result.audioRank = scoreToRank(finalAudioScore);
            result.humorRank = scoreToRank(finalHumorScore);
            result.moodRank = scoreToRank(finalMoodScore);
            result.overallRank = scoreToRank(overallScore);

            return result;
        }

        private int scoreToRank(float score)
        {
            int rank = generateScores().FindLastIndex(x => x < score);
            if (rank < 0)
            {
                return otherContestantCount + 1;
            }
            return otherContestantCount - rank;
        }

        private List<float> generateScores()
        {
            List<float> scores = new();

            System.Random rand = new System.Random();
            const double mean = 0.6f;
            const double stdDev = 0.15f;
            for (int i = 0; i < otherContestantCount; ++i)
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                double randNormal = mean + stdDev * randStdNormal;
                scores.Add(Math.Clamp((float)randNormal, 0f, 0.99f));
            }

            scores.Sort();

            return scores;
        }
    }
}
