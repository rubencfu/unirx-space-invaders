using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class UfoMovement : MonoBehaviour
{
    private bool isMovingRight = true;

    public float baseSpeed = 0.4f;
    void Start()
    {
        IObservable<Unit> update = this.UpdateAsObservable();
        IObservable<Collider2D> triggerEnterObservable = this.OnTriggerEnter2DAsObservable();

        update.Subscribe(_ => Move());

        triggerEnterObservable
        .Where(IsBorder)
        .Subscribe(_ => ChangeDirection());

        triggerEnterObservable
        .Where(IsPlayer)
        .Subscribe(_ => GameOver());
    }

    private void Move()
    {
        float movement = baseSpeed * Time.fixedDeltaTime * 0.1f * Time.timeScale;
        transform.Translate(isMovingRight ? movement : -movement, 0, 0);
    }

    private void ChangeDirection() 
    {
        isMovingRight = !isMovingRight;
        baseSpeed += 0.3f;
        transform.Translate(0, -0.4f, 0);
    }

    private void GameOver() 
    {
        GameController controller = 
            GameObject.FindGameObjectWithTag("GameController")
            .GetComponent("GameController") 
            as GameController;
        controller.GameOver();
    }

    private bool IsBorder(Collider2D collider) => collider.tag == "MainCamera";
    private bool IsPlayer(Collider2D collider) => collider.tag == "Player";
    
}
