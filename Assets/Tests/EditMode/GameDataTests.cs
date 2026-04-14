using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using GameSystemsCookbook;
using GameSystemsCookbook.Demos.PaddleBall;

namespace PaddleBall.Tests
{
    [TestFixture]
    public class GameDataTests
    {
        private GameDataSO m_GameData;
        private PlayerIDSO m_Player1;
        private PlayerIDSO m_Player2;

        [SetUp]
        public void SetUp()
        {
            m_Player1 = ScriptableObject.CreateInstance<PlayerIDSO>();
            m_Player2 = ScriptableObject.CreateInstance<PlayerIDSO>();

            // Suppress NullRefChecker errors during SO construction (fields not yet assigned)
            LogAssert.ignoreFailingMessages = true;
            m_GameData = ScriptableObject.CreateInstance<GameDataSO>();
            LogAssert.ignoreFailingMessages = false;

            var so = new SerializedObject(m_GameData);
            so.FindProperty("m_Player1").objectReferenceValue = m_Player1;
            so.FindProperty("m_Player2").objectReferenceValue = m_Player2;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_GameData);
            Object.DestroyImmediate(m_Player1);
            Object.DestroyImmediate(m_Player2);
        }

        [Test]
        public void GameData_IsPlayer1_ReturnsTrueForCorrectPlayer()
        {
            Assert.That(m_GameData.IsPlayer1(m_Player1), Is.True);
        }

        [Test]
        public void GameData_IsPlayer1_ReturnsFalseForWrongPlayer()
        {
            Assert.That(m_GameData.IsPlayer1(m_Player2), Is.False);
        }

        [Test]
        public void GameData_IsPlayer2_ReturnsTrueForCorrectPlayer()
        {
            Assert.That(m_GameData.IsPlayer2(m_Player2), Is.True);
        }

        [Test]
        public void GameData_IsPlayer2_ReturnsFalseForWrongPlayer()
        {
            Assert.That(m_GameData.IsPlayer2(m_Player1), Is.False);
        }
    }
}
