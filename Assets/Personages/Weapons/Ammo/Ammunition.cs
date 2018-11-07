﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{

    [Range(0, 10)]
    public float speed;
    public short count;
    public GameObject target;
    private Vector3 moveVector;
    public WeaponType weaponType;
    public IHaveWeapons haveWeapons;
    [HideInInspector]public bool MagnettoBonus = false;
    private float distance;

    //Update is called once per frame//kkj
    private void FixedUpdate()
    {
        transform.Rotate(transform.up, 2 * Time.deltaTime);
        if (target != null)
        {
            MagnettoBonus = target.GetComponent<SinglePlayerController>().magnettoBonus;
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        moveVector = target.transform.position - transform.position;
        float step = Vector3.Magnitude(moveVector.normalized * speed);
        distance = Vector3.Distance(transform.position, target.transform.position);
        DistanceGet(MagnettoBonus, step);
        
    }
    private void DistanceGet(bool bon, float step)
    {
        if (bon)
        {
            if (step < distance)
            {
                transform.position += moveVector.normalized * speed;
            }
            else
            {
                transform.position = target.transform.position;
                haveWeapons.AddAmmos(weaponType,count);
                Destroy(gameObject);
            }
        }
        else
        {
            if (distance <=1)
            {
                if (step < distance)
                {
                    transform.position += moveVector.normalized * speed;
                }
                else
                {
                    transform.position = target.transform.position;
                    haveWeapons.AddAmmos(weaponType,count);
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
