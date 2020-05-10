using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Collider2D))]
public class Goal : MonoBehaviour
{
    [SerializeField] private ParticleSystem goalEffectPrefab;
    [SerializeField] private string destroyAudioName;
    
    private ParticleSystem goalEffect;

    public event Action<Ball> OnGoalScored;
    
    private void Start()
    {
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

            var ball = collision.GetComponentInParent<Ball>();
            OnGoalScored?.Invoke(ball);
        }
    }
}
