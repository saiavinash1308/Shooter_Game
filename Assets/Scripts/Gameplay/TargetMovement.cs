using UnityEngine;
using System.Collections;

namespace Avinash.ArcheryGame
{
    public class TargetMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        private Vector3 targetPosition;

        public int ScoreValue { get; private set; }

        public void InitializeMovement()
        {
            targetPosition = GetRandomScreenPosition();
            StartCoroutine(MoveToTarget());
        }

        public void SetScoreValue(int score)
        {
            ScoreValue = score;
        }

        private IEnumerator MoveToTarget()
        {
            while (true)
            {
                UpdateSpeedBasedOnScore(); // Check and update speed

                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    targetPosition = GetRandomScreenPosition();
                }

                yield return null;
            }
        }

        private void UpdateSpeedBasedOnScore()
        {
            if (ScoreManager.CurrentScore > 25)
            {
                speed = 4.8f; // Double the speed (original speed was 2f)
            }
            else
            {
                speed = 2f; // Reset to original speed
            }
        }

        private Vector3 GetRandomScreenPosition()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            return Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(-screenWidth, screenWidth), Random.Range(-screenHeight, screenHeight), Camera.main.nearClipPlane + 1));
        }
    }
}
