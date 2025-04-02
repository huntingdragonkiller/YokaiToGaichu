using System;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private GameManager _gm;
    private void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(_gm.FadeScreen(1f));
            other.gameObject.GetComponent<PlayerCheckpoint>().Respawn();
        }
    }
}
