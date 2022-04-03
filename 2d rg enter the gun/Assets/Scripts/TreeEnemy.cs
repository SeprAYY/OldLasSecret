﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEnemy : MonoBehaviour
{
    public Room theRoom;

    [Header("Variables")]
    public float speed;
    public float minDistance; // minimum distance from the player
    public float maxDistance; // maximum distance from the player to go close to it
    public float reatreatDistance; //distandce to run away from player

    public float distanceToTrigger;

    [Header("Shooting")]
    public float startTimeBtwnShots;
    float timeBtwShots;
    public float shootDistance; // min distance to shoot
    public GameObject projectile;
    public int howManyBulletsToSpawn; // Set it to 6
    public float TimeBetweenBullets; // set it to 0.1f
    public float TimeToReload; // set it to 2f
    Transform player;

    [Header("Taking Damage")]
    public Material matWhite;
    Material matDefault;
    SpriteRenderer bodySR;
    public GameObject expolsionRef;

    public float speedWhenHit;

    float counter;
    public float TimeToGetHit;


    int numBulletsShoot = 0;
    float lastBullet = 0;

    Animator animator;
    string currentAnimationState;

    Rigidbody2D rb;

    enum TreeEnemyStates { Idle, Walk, Hit, Die , PostMortem };
    TreeEnemyStates currentState;

    public float howLongAwayAfterDestroyed;

    public float speedWhenDie;

    public GameObject coin;

    GeneralEnemy genEnemy;

    void Start()
    {
        genEnemy = GetComponent<GeneralEnemy>();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeBtwShots = startTimeBtwnShots;

        bodySR = GetComponentInChildren<SpriteRenderer>();
        //matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material; //castuje na material bo deafultowo jest obiekt
        matDefault = bodySR.material;

    }

    void Update()
    {
        if (!theRoom.roomActive) return;

        switch (currentState)
        {
            case TreeEnemyStates.Idle:

                if(Vector2.Distance(transform.position, player.transform.position) < distanceToTrigger)
                {
                    currentState = TreeEnemyStates.Walk;
                    Debug.Log("idle");
                }

                break;

            case TreeEnemyStates.Walk:
                ChangeAnimationState("TreeMonster_Move");

                Debug.Log("walk");

                float distance = Vector2.Distance(transform.position, player.position);

                // If we are too close or too far away we will stop. If not we will go towards the player
                if (distance < reatreatDistance)
                {
                    Debug.Log("Run");
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -speed * Time.deltaTime);
                }

                else if (distance > minDistance && distance < maxDistance)
                {
                    Debug.Log("chase");
                    transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                }

                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, transform.position, speed * Time.deltaTime);
                }

                // If we are close enough we will shoot
                if (distance < shootDistance)
                {
                    if (lastBullet > 0)
                    {
                        lastBullet -= Time.deltaTime;
                    }
                    else
                    {
                        numBulletsShoot++;
                        if (numBulletsShoot < howManyBulletsToSpawn)
                        { // Max "howManyBulletsToSpawn" bullets before reloading
                            lastBullet = TimeBetweenBullets;
                            Instantiate(projectile, transform.position, Quaternion.identity); // The bullet should move in some way, there is no code here to move the bullet
                        }
                        else
                        { // Reloading
                            lastBullet = TimeToReload; // 2 seconds reload
                            numBulletsShoot = 0;
                        }
                    }

                }
                break;

            case TreeEnemyStates.Hit:
                ChangeAnimationState("Tree_Monster_Hit");
                if (counter > 0)
                {
                    counter -= Time.deltaTime;
                    Vector3 moveDirection = transform.position - player.transform.position;
                    moveDirection.Normalize();
                    rb.velocity = moveDirection * speedWhenHit;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    counter = TimeToGetHit;
                    bodySR.flipX = false;
                    currentState = TreeEnemyStates.Walk;
                }
                break;

            case TreeEnemyStates.Die:

                //CameraShaker.Shake(0.10f, 0.15f);

                Invoke("ResetMaterial", 0.1f); //jak dlugo ma byc bialy

                ChangeAnimationState("Tree_Monster_Hit");

                Vector3 movingWay = transform.position - player.transform.position;
                movingWay.Normalize();
                rb.velocity = movingWay * speedWhenDie;
                Invoke("KillSelf", howLongAwayAfterDestroyed);


                CameraShaker.ShakeStrong(.20f, .25f);

                currentState = TreeEnemyStates.PostMortem;

                break;

            case TreeEnemyStates.PostMortem:

                break;

        }
        // Calculate the distance
        

    }
    void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        animator.Play(newState);

        currentAnimationState = newState;
    }

    void KillSelf()
    {
        Instantiate(expolsionRef, transform.position, Quaternion.identity);
        Destroy(gameObject);

        gameObject.GetComponent<GeneralEnemy>().DropCoin();
    }
    void ResetMaterial()
    {
        bodySR.material = matDefault;
    }

  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBullet")
        {
            Destroy(collision.gameObject);
            bodySR.material = matWhite;
            if(player.transform.position.x > transform.position.x)
            {
                bodySR.flipX = false;
            }
            else
            {
                bodySR.flipX = true;
            }
            if (genEnemy.health <= 0)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                currentState = TreeEnemyStates.Die;
            }
            else
            {
                counter = TimeToGetHit;
                currentState = TreeEnemyStates.Hit;
                Invoke("ResetMaterial", 0.1f); //jak dlugo ma byc bialy
            }
        }

    }
}