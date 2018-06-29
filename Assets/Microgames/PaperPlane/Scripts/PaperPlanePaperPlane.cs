﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlanePaperPlane : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float turningSpeed = 0.1f;
    [SerializeField]
    float maxTurningSpeed = 0.5f;
    [SerializeField]
    float delay = 0.5f;

    Vector2 velocity;
    // Use this for initialization
    void Start () {
        velocity = Vector2.zero;
        Invoke("setVelocity", delay);
	}
	
    void setVelocity()
    {
        velocity = new Vector2(speed, 0);
    }
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity.y -= turningSpeed * Time.deltaTime;
            clampSpeed();
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity.y += turningSpeed * Time.deltaTime;
            clampSpeed();
        }
        else if (velocity.y != 0)
        {
            if (velocity.y > 0)
                velocity.y -= (velocity.y * 0.1f) * Time.deltaTime;
            else if (velocity.y < 0)
                velocity.y += (velocity.y * 0.1f) * Time.deltaTime;
            if (velocity.y < turningSpeed / 2 && velocity.y > -turningSpeed / 2)
                velocity.y = 0;
        }*/
        if (velocity != Vector2.zero)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                velocity.y += -maxTurningSpeed * Time.deltaTime;
                clampSpeed();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                velocity.y += maxTurningSpeed * Time.deltaTime;
                clampSpeed();
            }
            else if (velocity.y != 0)
                velocity.y = 0f;

            transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        velocity = Vector2.zero;

        if (collision.gameObject.tag == "MicrogameTag1")
        {
            MicrogameController.instance.setVictory(false, true);
        }
        if (collision.gameObject.tag == "MicrogameTag2")
        {
            MicrogameController.instance.setVictory(true, true);
        }

    }

    void clampSpeed()
    {
        if (velocity.y > maxTurningSpeed)
            velocity.y = maxTurningSpeed;
        if (velocity.y < -maxTurningSpeed)
            velocity.y = -maxTurningSpeed;
    }
}
