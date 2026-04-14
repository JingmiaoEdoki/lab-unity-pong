using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using GameSystemsCookbook;
using GameSystemsCookbook.Demos.PaddleBall;

namespace PaddleBall.Tests
{
    [TestFixture]
    public class ScoreObjectiveTests
    {
        private ScoreObjectiveSO m_ScoreObjective;
        private ScoreListEventChannelSO m_ScoreManagerUpdated;
        private PlayerScoreEventChannelSO m_TargetScoreReached;
        private VoidEventChannelSO m_ObjectiveCompleted;
        private PlayerIDSO m_Player1;
        private PlayerIDSO m_Player2;

        [SetUp]
        public void SetUp()
        {
            m_ScoreManagerUpdated = ScriptableObject.CreateInstance<ScoreListEventChannelSO>();
            m_TargetScoreReached = ScriptableObject.CreateInstance<PlayerScoreEventChannelSO>();
            m_ObjectiveCompleted = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            m_Player1 = ScriptableObject.CreateInstance<PlayerIDSO>();
            m_Player2 = ScriptableObject.CreateInstance<PlayerIDSO>();

            // Suppress NullRefChecker errors during SO construction (fields not yet assigned)
            LogAssert.ignoreFailingMessages = true;
            m_ScoreObjective = ScriptableObject.CreateInstance<ScoreObjectiveSO>();
            LogAssert.ignoreFailingMessages = false;

            // Inject private [SerializeField] fields via SerializedObject
            var so = new SerializedObject(m_ScoreObjective);
            so.FindProperty("m_TargetScore").intValue = 3;
            so.FindProperty("m_ScoreManagerUpdated").objectReferenceValue = m_ScoreManagerUpdated;
            so.FindProperty("m_TargetScoreReached").objectReferenceValue = m_TargetScoreReached;
            so.FindProperty("m_ObjectiveCompleted").objectReferenceValue = m_ObjectiveCompleted;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Re-trigger OnEnable so it subscribes to the now-assigned event channel
            typeof(ScoreObjectiveSO)
                .GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(m_ScoreObjective, null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_ScoreObjective);
            Object.DestroyImmediate(m_ScoreManagerUpdated);
            Object.DestroyImmediate(m_TargetScoreReached);
            Object.DestroyImmediate(m_ObjectiveCompleted);
            Object.DestroyImmediate(m_Player1);
            Object.DestroyImmediate(m_Player2);
        }

        [Test]
        public void ScoreObjective_ScoreBelowTarget_IsNotCompleted()
        {
            var scores = CreatePlayerScores(m_Player1, 2);

            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(m_ScoreObjective.IsCompleted, Is.False);
        }

        [Test]
        public void ScoreObjective_ScoreEqualsTarget_IsCompleted()
        {
            var scores = CreatePlayerScores(m_Player1, 3);

            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(m_ScoreObjective.IsCompleted, Is.True);
        }

        [Test]
        public void ScoreObjective_ScoreAboveTarget_IsCompleted()
        {
            var scores = CreatePlayerScores(m_Player1, 5);

            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(m_ScoreObjective.IsCompleted, Is.True);
        }

        [Test]
        public void ScoreObjective_ScoreReachesTarget_RaisesTargetScoreReachedEvent()
        {
            bool eventRaised = false;
            PlayerScore receivedScore = default;
            m_TargetScoreReached.OnEventRaised += ps => { eventRaised = true; receivedScore = ps; };

            var scores = CreatePlayerScores(m_Player1, 3);
            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(eventRaised, Is.True);
            Assert.That(receivedScore.playerID, Is.EqualTo(m_Player1));
        }

        [Test]
        public void ScoreObjective_ScoreReachesTarget_RaisesObjectiveCompletedEvent()
        {
            bool eventRaised = false;
            m_ObjectiveCompleted.OnEventRaised += () => eventRaised = true;

            var scores = CreatePlayerScores(m_Player1, 3);
            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void ScoreObjective_MultiplePlayersOnlyOneReachesTarget_CorrectPlayerIdentified()
        {
            PlayerScore receivedScore = default;
            m_TargetScoreReached.OnEventRaised += ps => receivedScore = ps;

            var scores = new List<PlayerScore>
            {
                CreateSinglePlayerScore(m_Player1, 1),
                CreateSinglePlayerScore(m_Player2, 3)
            };

            m_ScoreManagerUpdated.RaiseEvent(scores);

            Assert.That(receivedScore.playerID, Is.EqualTo(m_Player2));
        }

        private List<PlayerScore> CreatePlayerScores(PlayerIDSO player, int scoreValue)
        {
            return new List<PlayerScore> { CreateSinglePlayerScore(player, scoreValue) };
        }

        private PlayerScore CreateSinglePlayerScore(PlayerIDSO player, int scoreValue)
        {
            var score = new Score();
            for (int i = 0; i < scoreValue; i++)
                score.IncrementScore();

            return new PlayerScore
            {
                playerID = player,
                scoreUI = null,
                score = score
            };
        }
    }
}
