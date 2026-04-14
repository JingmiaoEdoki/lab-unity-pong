using NUnit.Framework;
using GameSystemsCookbook.Demos.PaddleBall;

namespace PaddleBall.Tests
{
    [TestFixture]
    public class ScoreTests
    {
        private Score m_Score;

        [SetUp]
        public void SetUp()
        {
            m_Score = new Score();
        }

        [Test]
        public void Score_InitialValue_IsZero()
        {
            Assert.That(m_Score.Value, Is.EqualTo(0));
        }

        [Test]
        public void Score_IncrementScore_IncreasesValueByOne()
        {
            m_Score.IncrementScore();

            Assert.That(m_Score.Value, Is.EqualTo(1));
        }

        [Test]
        public void Score_ResetScore_SetsValueToZero()
        {
            m_Score.IncrementScore();
            m_Score.IncrementScore();
            m_Score.IncrementScore();

            m_Score.ResetScore();

            Assert.That(m_Score.Value, Is.EqualTo(0));
        }

        [Test]
        public void Score_MultipleIncrements_AccumulateCorrectly()
        {
            int expected = 10;

            for (int i = 0; i < expected; i++)
            {
                m_Score.IncrementScore();
            }

            Assert.That(m_Score.Value, Is.EqualTo(expected));
        }
    }
}
