using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class ShipController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Rigidbody2D rigidBody2D;
    Vector2 movement;

    [Header("Fire")]
    public GameObject laser;
    public float fireRate = 0.8f;
    public float laserSpeed = 600f;
    private float nextFire = 0f;

    [Header("Explosion")]
    public GameObject explosion;

    [Header("Other")]
    public float immortalityDuration = 1f;

    void Start()
    {
        StartCoroutine(TemporalImmortalityCoroutine());
        rigidBody2D = GetComponent<Rigidbody2D>();

        IObservable<Unit> update = this.UpdateAsObservable();

        update
        .Do(_ => HandleMovement())
        .Where(_ => CanShoot())
        .Subscribe(_ => Fire());
    }

    private void HandleMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        rigidBody2D.MovePosition(rigidBody2D.position + movement * speed * Time.fixedDeltaTime);
    }

    public void Explode() 
    {
        GameObject explosionInstance = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(explosionInstance, 0.4f);
        Destroy(gameObject);

        GameController controller = 
            GameObject.FindGameObjectWithTag("GameController")
            .GetComponent("GameController") 
            as GameController;
        
        controller.SubstractLive();
    }

    private void Fire() 
    {
        GameObject firedLaser = Instantiate(
            laser,
            transform.position,
            transform.rotation
        );

        Rigidbody2D firedLaserRigidBody2D = firedLaser.GetComponent<Rigidbody2D>();
        firedLaserRigidBody2D.AddForce(Vector3.up * laserSpeed);

        Physics2D.IgnoreCollision(firedLaser.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        nextFire = Time.time + fireRate;
    }

    private bool CanShoot() => Input.GetButton("Fire1") && Time.time > nextFire;

    IEnumerator TemporalImmortalityCoroutine()
    {
        float startTime = Time.time;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(immortalityDuration);

        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}