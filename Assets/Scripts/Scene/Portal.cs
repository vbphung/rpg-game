using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace Scene
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int loadingSceneIndex = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Destination destination;

        enum Destination
        {
            intoLevel, exitLevel
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
                StartCoroutine(SceneTransition());
        }

        private IEnumerator SceneTransition()
        {
            if (loadingSceneIndex >= 0)
            {
                DontDestroyOnLoad(gameObject);

                yield return SceneManager.LoadSceneAsync(loadingSceneIndex);
                UpdatePlayer(GetLoadingPortal());

                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No scene to load!");
                yield break;
            }
        }

        private void UpdatePlayer(Portal portal)
        {
            if (portal != null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.transform.position = portal.spawnPoint.position;
                player.transform.rotation = portal.spawnPoint.rotation;
                player.GetComponent<NavMeshAgent>().enabled = true;
            }
        }

        private Portal GetLoadingPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
                if (portal != this && portal.destination != destination)
                    return portal;
            return null;
        }
    }
}