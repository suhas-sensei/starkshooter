using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Move the bullet forward
        rb.velocity = transform.forward * speed;
        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Handle collision with enemies
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Hit an Enemy!");
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.constraints = RigidbodyConstraints.None;
                enemyRb.AddForce(transform.forward * 500);
            }
        }
        // Destroy bullet on any collision
        Destroy(gameObject);
    }
}
