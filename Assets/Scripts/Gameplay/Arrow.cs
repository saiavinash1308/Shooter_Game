using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Avinash.ArcheryGame
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Arrow : ArcheryPlayer
    {
		private const float DestroyDelay = 1f;

		[SerializeField]
		private Transform arrowHead = default;
		[SerializeField]
		private float arrowHeadDownForce = default;

		private Rigidbody2D rb;

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate()
		{
			rb.AddForceAtPosition(Vector2.down * arrowHeadDownForce, arrowHead.position);
		}

		private void OnBecameInvisible()
		{
			Destroy(gameObject);
		}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Target) || collision.CompareTag(Tags.Target5))
            {
                // Get the TargetMovement component to access the score value
                TargetMovement targetMovement = collision.GetComponent<TargetMovement>();

                if (targetMovement != null)
                {
                    // Increase the score based on the target value
                    FindObjectOfType<ScoreManager>().IncreaseScore(targetMovement.ScoreValue);
                    Debug.Log("Score Added: " + targetMovement.ScoreValue); // Debug line

                    // Increase combo count based on the current score
                    ScoreManager.IncreaseCombo(targetMovement.ScoreValue); // This should now work correctly
                    Debug.Log("Current Combo: " + ScoreManager.CurrentCombo); // Debug line
                }

                // Destroy the arrow and the target
                Destroy(gameObject); // Destroy the arrow
                Destroy(collision.gameObject); // Destroy the target
               // Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity); // Optional explosion effect
            }
        }


        public void SetPositionRotationAndSpeed(Vector3 position, float rotation, float speed)
		{
			rb.position = position;
			rb.rotation = rotation - 90f;
			rb.velocity = Vector2.zero;
			rb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);

			transform.SetPositionAndRotation(rb.position, Quaternion.Euler(0f, 0f, rb.rotation));
		}
	}
}
