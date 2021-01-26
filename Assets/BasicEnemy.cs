using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{

    [SerializeField] ParticleSystem enemyEngineParticles;

    // Start is called before the first frame update
    void Start()
    {
        enemyEngineParticles.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
