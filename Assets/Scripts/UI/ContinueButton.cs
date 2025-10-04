using System;
using Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ContinueButton : MonoBehaviour
    {
        private PlayerData _data;
        
        private void Awake()
        {
            if (!PlayerPrefs.HasKey("PlayerData"))
            {
                gameObject.SetActive(false);
            }
        }
        
        public void OnButtonClick()
        {
            if (!PlayerPrefs.HasKey("PlayerData"))
            {
                return;
            }
            
            _data = DataManager.LoadData();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(_data.sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DataManager.SetData(_data);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
