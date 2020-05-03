using System;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using UnityEngine.PlayerLoop;

public class Goal : Side
{
    public event Action<GameObject, GameObject> OnGoalScored;
    private AudioSource goalSound;

    [SerializeField] private ParticleSystem goalEffectPrefab;
    private ParticleSystem goalEffect;

    private string destroyAudioName;
    
    private void Start()
    {
        if (CompareTag("Player"))
        {
            lineRenderer.material.color = Color.green;
            destroyAudioName = "PlayerDestroyed";
        }
        else if (CompareTag("Enemy"))
        {
            lineRenderer.material.color = Color.red;
            destroyAudioName = "EnemyDestroyed";
        }
        goalEffect = goalEffectPrefab ? Instantiate(goalEffectPrefab) : null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BallGoalCollider"))
        {
            if (goalEffect != null)
            {
                goalEffect.transform.position = collision.transform.position;
                goalEffect.Play();
            }
            AudioManager.instance.Play(destroyAudioName);
            OnGoalScored?.Invoke(this.gameObject, collision.gameObject.transform.parent.gameObject);
        }
    }
}
