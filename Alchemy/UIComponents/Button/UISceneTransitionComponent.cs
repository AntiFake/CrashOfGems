using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Alchemy.UI
{
    public class UISceneTransitionComponent : MonoBehaviour
    {
        public string nextSceneName;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nextSceneName);
            });
        }
    }
}
