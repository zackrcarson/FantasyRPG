using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float effectLength = 1f;
    [SerializeField] int soundEffect = 0;

    private void Start()
    {
        AudioManager.instance.PlaySFX(soundEffect);
    }

    private void Update()
    {
        Destroy(gameObject, effectLength);
    }
}
