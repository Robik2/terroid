using HealthAndStats;
using UnityEngine;

public class ApplyHit : MonoBehaviour {
    public float value;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            other.GetComponent<HealthManager>().TryTakeDamage(value);
        }
    }
}
