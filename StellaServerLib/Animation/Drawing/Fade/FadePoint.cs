namespace StellaServerLib.Animation.Drawing.Fade
{
    internal class FadePoint
    {
        /// <summary>First index the fade point starts from </summary>
        public int Point { get; }


        /// <summary> The step the fade point is currently at </summary>
        public int Step { get; set; }
        
        public FadePoint(int point)
        {
            Point = point;
        }
    }
}