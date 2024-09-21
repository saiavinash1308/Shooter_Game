using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

namespace Avinash.ArcheryGame
{
	[RequireComponent(typeof(AudioSource))]
	public class ArcheryPlayer : MonoBehaviour
	{
		private const float ShotForceMultiplier = 16f;
		private const float ShotForceBase = 4f;
		private const float ReloadCooldownSeconds = 0.5f;
		private const float MinDragDistance = 0.25f;
		private const float MaxDragDistance = 2f;

		private static readonly int StretchAmountHash = Animator.StringToHash("Stretch Amount");

		[SerializeField]
		private Transform bow = default;
		[SerializeField]
		private Animator bowAnimator = default;
		[SerializeField]
		private GameObject arrowOnBow = default;
		[SerializeField]
		private Arrow arrowProjectilePrefab = default;
		[SerializeField]
		internal GameObject deathExplosionPrefab = default;
        [SerializeField]
        internal GameObject targetExplosionPrefab = default;	
		[SerializeField] private GameObject gameOverPanel;
        public Text scoreText;

        [SerializeField]
		private AudioClip pull = default;
		[SerializeField]
		private AudioClip shoot = default;

		private bool isPullingBow;
		private float pullStartDistance;
		private bool isReloaded;
		private float lastShotTime;

		private bool allowShooting;
		private AudioSource audioSource;

		private static ArcheryPlayer instance;

		public static Vector2 Position
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ArcheryPlayer>();
					if (instance == null)
					{
						return Vector2.zero;
					}
				}
				return instance.transform.position;
			}
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			isPullingBow = false;
			isReloaded = true;
		}

		private void OnEnable()
		{
			ArcheryGame_NewGameStarted();

            //ArcheryGame.NewGameStarted += ArcheryGame_NewGameStarted;
			//ArcheryGame.GameLost += ArcheryGame_GameLost;
		}

		private void OnDisable()
		{
			//ArcheryGame.NewGameStarted -= ArcheryGame_NewGameStarted;
			//ArcheryGame.GameLost -= ArcheryGame_GameLost;
		}

		private void ArcheryGame_NewGameStarted()
		{
			allowShooting = true;
		}

		private void ArcheryGame_GameLost()
		{
			allowShooting = false;
		}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Target") || collision.CompareTag("Target5"))
            {
                TargetMovement targetMovement = collision.GetComponent<TargetMovement>();
                if (targetMovement != null)
                {
                   // FindObjectOfType<ScoreManager>().IncreaseScore(targetMovement.ScoreValue); // Increase score based on target value
                    Debug.Log("Hit Target with Score Value: " + targetMovement.ScoreValue); // Debug line
                }

                Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
                
				ShowGameOverUI();
            }
        }



        private void ShowGameOverUI()
        {
            gameOverPanel.SetActive(true); // Show the Game Over UI
            scoreText.text = "Final Score: " + ScoreManager.CurrentScore;              // Optionally, you can pause the game here
            Time.timeScale = 0; // Stop the game
        }

        private void Update()
		{
			if (!allowShooting)
			{
				isPullingBow = false;
				//arrowOnBow.SetActive(false);
				return;
			}

			// Reload if cooldown has passed
			if (Time.time > lastShotTime + ReloadCooldownSeconds)
			{
				isReloaded = true;
				//arrowOnBow.SetActive(true);
			}

			Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
			Vector2 pullVector = position2D - worldMousePosition;
			if (isPullingBow)
			{
				float angle = Mathf.Atan2(pullVector.y, pullVector.x) * Mathf.Rad2Deg;
				float pullMagnitudeDifference = pullVector.magnitude - pullStartDistance;
				float pullAmount = Mathf.Clamp01(Mathf.InverseLerp(MinDragDistance, MaxDragDistance, pullMagnitudeDifference));
				UpdateBowGraphics(angle, pullAmount);

				if (isReloaded && Input.GetMouseButtonUp(0))
				{
					PerformShot(angle, pullAmount);
					isPullingBow = false;
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					isPullingBow = true;
					pullStartDistance = pullVector.magnitude;

					audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
					audioSource.clip = pull;
					audioSource.Play();
				}
			}
		}

		private void UpdateBowGraphics(float angle, float stretchAmount)
		{
			bow.eulerAngles = new Vector3(0f, 0f, angle);

			bowAnimator.SetFloat(StretchAmountHash, stretchAmount);
		}

		private void PerformShot(float angle, float stretchAmount)
		{
			isReloaded = false;
			lastShotTime = Time.time;

			//arrowOnBow.SetActive(false);
			bowAnimator.SetFloat(StretchAmountHash, 0f);

			var arrowProjectile = Instantiate(arrowProjectilePrefab);
			float speed = (ShotForceBase + stretchAmount * ShotForceMultiplier);
			arrowProjectile.SetPositionRotationAndSpeed(arrowOnBow.transform.position, angle, speed);

			audioSource.Stop();
			audioSource.pitch = 0.8f + 0.4f * stretchAmount;
			audioSource.PlayOneShot(shoot);
		}
        public void RestartGame()
        {
            // Load the current scene again
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
