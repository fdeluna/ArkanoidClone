using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DisableParticleDie : MonoBehaviour
{
    ParticleSystem _particles;
    void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(DisableGameObject());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DisableGameObject()
    {
        yield return new WaitForSeconds(_particles.main.duration);
        gameObject.SetActive(false);
    }
}

