using System;

namespace DataTypes
{
    public class ProductState
    {
        const int scoreLowerBound = 0;
        const int scoreUpperBound = 100;

        // Primary scores
        int mechanics = 0;
        int audioScore = 0;
        int visualsScore = 0;
        int polish = 50;

        // Auxiliary scores
        int overallScore = 0;
        int funScore = 0;
        int innovationScore = 0;
        int themeScore = 0;
        int graphicsScore = 0;
        int humorScore = 0;
        int moodScore = 0;

        public int Mechanics { get => mechanics; set => mechanics = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int AudioScore { get => audioScore; set => audioScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int VisualsScore { get => visualsScore; set => visualsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        // Note: polish is not minimum limited to 0 currently
        public int Polish { get => polish; set => polish = Math.Clamp(value, int.MinValue, scoreUpperBound); }
        // TODO: weighted avg of all?
        public int OverallScore { get => overallScore; set => overallScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int FunScore { get => funScore; set => funScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int InnovationScore { get => innovationScore; set => innovationScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int ThemeScore { get => themeScore; set => themeScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int GraphicsScore { get => graphicsScore; set => graphicsScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int HumorScore { get => humorScore; set => humorScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }
        public int MoodScore { get => moodScore; set => moodScore = Math.Clamp(value, scoreLowerBound, scoreUpperBound); }

        public int AudioVisuals => (audioScore + visualsScore) / 2;
    }
}
