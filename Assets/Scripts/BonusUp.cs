using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BonusUp : MonoBehaviour
{
    [Range(0, 10)] public float speed;
    public GameObject target;
    private Vector3 moveVector;
    public bool move;
    public Bonus.BonusType bonusType;
    public Bonus.IHaveBonus haveBonus;


    private void Start()
    {
        move = false;
    }

    //Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(transform.up, 2 * Time.deltaTime);
        if (move)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        moveVector = target.transform.position - transform.position;
        float step = Vector3.Magnitude(moveVector.normalized * speed);
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (step < distance)
        {
            transform.position += moveVector.normalized * speed;
        }
        else
        {
            transform.position = target.transform.position;
            haveBonus.AddBonus(bonusType);
            Destroy(gameObject);
        }
    }
}