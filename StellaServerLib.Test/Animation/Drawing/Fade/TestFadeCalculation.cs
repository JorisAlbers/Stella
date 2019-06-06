using System.Drawing;
using NUnit.Framework;
using StellaServerLib.Animation.Drawing.Fade;

namespace StellaServerLib.Test.Animation.Drawing.Fade
{
    [TestFixture]
    public class TestFadeCalculation
    {
        [Test]
        public void CalculateFadedPatterns_OneColorInPattern_CorrectlyFades()
        {
            // Expected
            int fadeSteps = 4;
            Color expectedColor0 = Color.FromArgb(0, 0, 0);
            Color expectedColor1 = Color.FromArgb(25, 25, 25);
            Color expectedColor2 = Color.FromArgb(50, 50, 50);
            Color expectedColor3 = Color.FromArgb(75, 75, 75);
            Color expectedColor4 = Color.FromArgb(100, 100, 100);

            // Act
            Color[] pattern = new Color[]{Color.FromArgb(100,100,100)};

            // Assert
            Color[][] fadedPattern = FadeCalculation.CalculateFadedPatterns(pattern, fadeSteps);

                Assert.AreEqual(fadeSteps +1, fadedPattern.Length);
                Assert.AreEqual(expectedColor0, fadedPattern[0][0]);
                Assert.AreEqual(expectedColor1, fadedPattern[1][0]);
                Assert.AreEqual(expectedColor2, fadedPattern[2][0]);
                Assert.AreEqual(expectedColor3, fadedPattern[3][0]);
                Assert.AreEqual(expectedColor4, fadedPattern[4][0]);
            
           
        }
    }
}
