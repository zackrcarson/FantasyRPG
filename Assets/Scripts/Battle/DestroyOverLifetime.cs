using UnityEngine;

public class DestroyOverLifetime : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
