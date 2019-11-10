namespace StellaServerLib.Animation.Transformation
{
    public class PlayListTransformationSettings
    {
        public StoryboardTransformationController StoryboardController { get; }

        public PlayListTransformationSettings(StoryboardTransformationController storyboardController)
        {
            StoryboardController = storyboardController;
        }
    }
}