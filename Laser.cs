using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class Laser : MonoBehaviour
{
    void Start()
    {
        IObservable<Collision2D> collisionEnterObservable = this.OnCollisionEnter2DAsObservable();

        collisionEnterObservable
        .Do(Impact)
        .Where(IsUfo)
        .Subscribe(DestroyUfo);

        collisionEnterObservable
        .Where(IsPlayer)
        .Subscribe(DestroyPlayer);
    }

    private void Impact(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void DestroyUfo(Collision2D collision)
    {
        Ufo ufo = collision.gameObject.GetComponent<Ufo>();
        ufo.Explode();
    }

    private void DestroyPlayer(Collision2D collision)
    {
        ShipController player = collision.gameObject.GetComponent<ShipController>();
        player.Explode();
    }

    private bool IsUfo(Collision2D collision) => collision.gameObject.tag == "Ufo";

    private bool IsPlayer(Collision2D collision) => collision.gameObject.tag == "Player";
}
