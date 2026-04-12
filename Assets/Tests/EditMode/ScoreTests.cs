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
            Assert.AreEqual(0, m_Score.Value);
        }

        [Test]
        public void Score_IncrementScore_IncreasesValueByOne()
        {
            m_Score.IncrementScore();

            Assert.AreEqual(1, m_Score.Value);
        }

        [Test]
        public void Score_ResetScore_SetsValueToZero()
        {
            m_Score.IncrementScore();
            m_Score.IncrementScore();
            m_Score.IncrementScore();

            m_Score.ResetScore();

            Assert.AreEqual(0, m_Score.Value);
        }

        [Test]
        public void Score_MultipleIncrements_AccumulateCorrectly()
        {
            int expected = 10;

            for (int i = 0; i < expected; i++)
            {
                m_Score.IncrementScore();
            }

            Assert.AreEqual(expected, m_Score.Value);
        }
    }
}
