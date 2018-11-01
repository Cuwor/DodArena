using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{

    [Range(0, 10)]
    public float speed;
    public byte count;
    public GameObject target;
    private Vector3 moveVector;
    public bool move;
    public SinglePlayerController PC;


    private void Start()
    {
        move = false;
    }

    // Update is called once per frame
    private void Update()
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
            PC.weapon[PC.weaponNumber].ammo += count;
            PC.DrawAmmo();
            Destroy(gameObject);
        }
    }

}
