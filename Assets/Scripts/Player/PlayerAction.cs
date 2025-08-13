using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerAction : MonoBehaviour
    {
        private Pickaxe _pickaxe;

        private void Awake()
        {
            _pickaxe = transform.GetComponentInChildren<Pickaxe>(true);
        }

        private void Update()
        {
            Mining();
        }

        private void Mining()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _pickaxe.StopMining();
                _pickaxe.Mining();
            }
        }
    }
}
