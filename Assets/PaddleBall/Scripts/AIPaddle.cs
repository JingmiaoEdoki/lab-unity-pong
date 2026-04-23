using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// AI-controlled paddle that follows the ball with a configurable reaction delay and
    /// positional offset to remain beatable by a human player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class AIPaddle : MonoBehaviour
    {
        [Header("Movement Limits")]
        [SerializeField] private float m_MinY = -4f;
        [SerializeField] private float m_MaxY = 4f;

        [Header("Listen to Event Channels")]
        [Tooltip("Signal to reset paddle to starting position")]
        [SerializeField] private VoidEventChannelSO m_PaddleReset;
        [SerializeField] private Rigidbody2D m_Rigidbody;

        private GameDataSO m_GameData;
        private AIPaddleDataSO m_AIData;
        private Transform m_BallTransform;

        private float m_TargetY;
        private float m_ReactionTimer;

        private void Reset()
        {
            if (m_Rigidbody == null)
                m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() => m_PaddleReset.OnEventRaised += ResetPosition;
        private void OnDisable() => m_PaddleReset.OnEventRaised -= ResetPosition;

        private void FixedUpdate()
        {
            if (m_BallTransform == null)
                return;

            // Update target position on a timer to simulate reaction delay
            m_ReactionTimer -= Time.fixedDeltaTime;
            if (m_ReactionTimer <= 0f)
            {
                m_ReactionTimer = m_AIData.ReactionDelay;
                float offset = Random.Range(-m_AIData.PositionOffset, m_AIData.PositionOffset);
                m_TargetY = m_BallTransform.position.y + offset;
            }

            float diff = m_TargetY - transform.position.y;
            if (Mathf.Abs(diff) < 0.05f)
                return;

            CalculateMovement(new Vector2(0f, Mathf.Sign(diff)));
        }

        /// <summary>
        /// Sets up dependencies. Called by GameSetup after instantiation.
        /// </summary>
        public void Initialize(GameDataSO gameData, AIPaddleDataSO aiData)
        {
            m_GameData = gameData;
            m_AIData = aiData;
            InitializeRigidbody();
            NullRefChecker.Validate(this);
        }

        /// <summary>
        /// Provides the ball Transform the AI will track. Called by GameSetup after ball creation.
        /// </summary>
        public void SetBallTransform(Transform ballTransform)
        {
            m_BallTransform = ballTransform;
        }

        private void CalculateMovement(Vector2 inputVector)
        {
            float speed = m_GameData.PaddleSpeed * m_AIData.SpeedFraction;

            if (inputVector.y > 0f && transform.position.y <= m_MaxY)
                m_Rigidbody.AddForce(inputVector * speed);

            if (inputVector.y < 0f && transform.position.y >= m_MinY)
                m_Rigidbody.AddForce(inputVector * speed);
        }

        private void InitializeRigidbody()
        {
            m_Rigidbody.linearDamping = m_GameData.PaddleLinearDrag;
            m_Rigidbody.mass = m_GameData.PaddleMass;
            m_Rigidbody.gravityScale = 0f;
        }

        private void ResetPosition()
        {
            transform.position = m_GameData.LevelLayout.Paddle2StartPosition;
        }
    }
}
