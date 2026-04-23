using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Configurable difficulty parameters for the AI-controlled paddle.
    /// </summary>
    [CreateAssetMenu(menuName = "PaddleBall/AI Paddle Data", fileName = "AIPaddleData")]
    public class AIPaddleDataSO : DescriptionSO
    {
        [Header("AI Difficulty")]
        [Tooltip("Seconds between each AI target update — higher = slower reaction")]
        [SerializeField, Range(0f, 1f)] private float m_ReactionDelay = 0.2f;

        [Tooltip("Random vertical offset added to the target position for imperfection")]
        [SerializeField, Range(0f, 2f)] private float m_PositionOffset = 0.5f;

        [Tooltip("Fraction of paddle speed applied by the AI (1 = same as player)")]
        [SerializeField, Range(0.1f, 1f)] private float m_SpeedFraction = 0.8f;

        public float ReactionDelay => m_ReactionDelay;
        public float PositionOffset => m_PositionOffset;
        public float SpeedFraction => m_SpeedFraction;
    }
}
