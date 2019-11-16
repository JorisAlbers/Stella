using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using SharpYaml.Serialization;
using StellaLib.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation
{
    /// <summary>
    /// Loads a storyboard
    /// </summary>
    public class StoryboardSerializer : ILoader<Storyboard>
    {
        private readonly SerializerSettings _settings;
        private readonly Serializer _serializer;

        public StoryboardSerializer()
        {
            _settings = new SerializerSettings();
            _settings.EmitDefaultValues = true;
            _settings.RegisterAssembly(typeof(Storyboard).Assembly);
            _settings.RegisterTagMapping("Storyboards", typeof(Storyboard[]));
            _serializer = new Serializer(_settings);
        }

        public Storyboard Load(StreamReader streamReader)
        {
            Storyboard storyboard = _serializer.Deserialize<Storyboard>(streamReader);

            // Validate storyboard properties
            if (String.IsNullOrWhiteSpace(storyboard.Name))
            {
                throw new FormatException($"Failed to load the storyboard. The name must be set.");
            }

            // Validate animations
            if (!AnimationsAreValid(storyboard.AnimationSettings, out List<string> errors))
            {
                throw new FormatException($"Failed to load the storyboard. Errors that occured:\n {String.Join("\n",errors)}");
            }

            return storyboard;
        }

        /// <summary>
        /// Writes the storyboard as yaml to the writer
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="writer"></param>
        public void Save(Storyboard storyboard, StreamWriter writer)
        {
            _serializer.Serialize(writer, storyboard, typeof(Storyboard));
        }

        /// <summary>
        /// Writes the list of storyboards as yaml to the writer
        /// </summary>
        /// <param name="storyboard"></param>
        /// <param name="writer"></param>
        public void Save(Storyboard[] storyboard, StreamWriter writer)
        {
            _serializer.Serialize(writer, storyboard, typeof(Storyboard));
        }
        
        public static bool AnimationsAreValid(IAnimationSettings[] storyboardAnimationSettings, out List<string> errors)
        {
            errors = new List<string>();

            for (int i = 0; i < storyboardAnimationSettings.Length; i++)
            {
                IAnimationSettings animationSetting = storyboardAnimationSettings[i];
                string typeName = animationSetting.GetType().Name;

                // First validate generic settings
                ValidateAnimationSetting(animationSetting, i, typeName, ref errors);

                // Then validate setting type specific settings
                switch (typeName)
                {
                    case nameof(MovingPatternAnimationSettings):
                        ValidateMovingPattern((MovingPatternAnimationSettings) animationSetting, i, ref errors);
                        break;
                    case nameof(SlidingPatternAnimationSettings):
                        ValidateSlidingPattern((SlidingPatternAnimationSettings) animationSetting, i, ref errors);
                        break;
                    case nameof(RepeatingPatternAnimationSettings):
                        ValidateRepeatingPatternAnimationSettings((RepeatingPatternAnimationSettings) animationSetting, i,ref errors);
                        break;
                    case nameof(RandomFadeAnimationSettings):
                        ValidateRandomFadeAnimationSettings((RandomFadeAnimationSettings) animationSetting, i, ref errors);
                        break;
                    case nameof(FadingPulseAnimationSettings):
                        ValidateFadingPulseAnimationSettings((FadingPulseAnimationSettings) animationSetting, i,ref errors);
                        break;
                    case nameof(BitmapAnimationSettings):
                        ValidateBitmapAnimationSettings((BitmapAnimationSettings) animationSetting, i, ref errors);
                        break;
                }
            }

            return errors.Count == 0;
        }
        
        private static void ValidateAnimationSetting(IAnimationSettings animationSettings, int animationIndex, string typeName, ref List<string> errors)
        {
            if (animationSettings.StripLength < 1)
            {
                errors.Add($"{typeName} at index {animationIndex}: StripLength must be > 0");
            }
            if (animationSettings.StartIndex < 0)
            {
                errors.Add($"{typeName} at index {animationIndex}: StartIndex must be >= 0");
            }
            if (animationSettings.TimeUnitsPerFrame < 1)
            {
                errors.Add($"{typeName} at index {animationIndex}: TimeUnitsPerFrame must be > 0");
            }
            if (animationSettings.RelativeStart < 0)
            {
                errors.Add($"{typeName} at index {animationIndex}: RelativeStart must be >= 0");
            }
        }

        private static void ValidateMovingPattern(MovingPatternAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (animationSetting.InternalPattern == null || animationSetting.Pattern == null || animationSetting.Pattern.Length == 0)
            {
                errors.Add($"MovingPatternAnimationSettings at index {animationIndex}: Pattern must be set.");
            }
        }

        private static void ValidateSlidingPattern(SlidingPatternAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (animationSetting.InternalPattern == null || animationSetting.Pattern == null || animationSetting.Pattern.Length == 0)
            {
                errors.Add($"SlidingPatternAnimationSettings at index {animationIndex}: Pattern must be set.");
            }
        }

        private static void ValidateRepeatingPatternAnimationSettings(RepeatingPatternAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (animationSetting.InternalPatterns == null || animationSetting.Patterns == null || animationSetting.Patterns.Length == 0)
            {
                errors.Add($"RepeatingPatternAnimationSettings at index {animationIndex}: Patterns must be set.");
            }
            else
            {
                for (int i = 0; i < animationSetting.Patterns.Length; i++)
                {
                    if (animationSetting.Patterns[i].Length < 1)
                    {
                        errors.Add($"RepeatingPatternAnimationSettings at index {animationIndex}: Pattern at index {i} must be set.");

                    }
                }
            }
        }

        private static void ValidateRandomFadeAnimationSettings(RandomFadeAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (animationSetting.InternalPattern == null || animationSetting.Pattern == null || animationSetting.Pattern.Length == 0)
            {
                errors.Add($"RandomFadeAnimationSettings at index {animationIndex}: Pattern must be set.");
            }

            if (animationSetting.FadeSteps < 1)
            {
                errors.Add($"RandomFadeAnimationSettings at index {animationIndex}: FadeSteps must be set.");
            }
        }

        private static void ValidateFadingPulseAnimationSettings(FadingPulseAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (animationSetting.InternalColor == null || animationSetting.InternalColor.Length < 1 || animationSetting.Color == Color.Empty)
            {
                errors.Add($"FadingPulseAnimationSettings at index {animationIndex}: Color must be set.");
            }

            if (animationSetting.FadeSteps < 1)
            {
                errors.Add($"FadingPulseAnimationSettings at index {animationIndex}: FadeSteps must be set.");
            }
        }

        private static void ValidateBitmapAnimationSettings(BitmapAnimationSettings animationSetting, int animationIndex, ref List<string> errors)
        {
            if (String.IsNullOrWhiteSpace(animationSetting.ImageName))
            {
                errors.Add($"FadingPulseAnimationSettings at index {animationIndex}: ImagePath must be set.");
            }
        }

    }
}
