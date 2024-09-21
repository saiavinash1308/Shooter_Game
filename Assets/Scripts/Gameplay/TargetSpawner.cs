using System.Collections;
using UnityEngine;

namespace Avinash.ArcheryGame
{
    public class TargetSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform firstTargetSpawnPoint = default;

        [SerializeField]
        private GameObject targetPrefab1 = default; // First target prefab
        [SerializeField]
        private GameObject targetPrefab2 = default; // Second target prefab

        private bool willSpawnTargets = false;

        private void OnEnable()
        {
            ArcheryGame_FirstTargetWasShot();
        }

        private void ArcheryGame_FirstTargetWasShot()
        {
            willSpawnTargets = true;
            StartCoroutine(SpawnTargetsCoroutine());
        }

        private IEnumerator SpawnTargetsCoroutine()
        {
            yield return new WaitForSeconds(1f);

            while (willSpawnTargets)
            {
                GameObject targetPrefab = Random.Range(0, 2) == 0 ? targetPrefab1 : targetPrefab2;
                GameObject spawnedTarget = Instantiate(targetPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                TargetMovement targetMovement = spawnedTarget.GetComponent<TargetMovement>();

                // Set the score value based on prefab type
                if (targetPrefab.CompareTag("Target"))
                {
                    targetMovement.SetScoreValue(1);
                }
                else if (targetPrefab.CompareTag("Target5"))
                {
                    targetMovement.SetScoreValue(5);
                }

                targetMovement.InitializeMovement();

                yield return new WaitForSeconds(5f);
            }
        }


        private Vector3 GetRandomSpawnPosition()
        {
            int randomIndex = Random.Range(0, transform.childCount);
            return transform.GetChild(randomIndex).position;
        }
    }
}
