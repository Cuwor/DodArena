using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Serialization;

public class BonusUp : MonoBehaviour
{
    [Range(0, 10)] public float speed;
    public GameObject target;
    [Range(0, 10)]
    public float defaultDistance;
    [Range(0, 100)] public float Duration;
    public AudioClip sound;
    private Vector3 moveVector;
    public BonusType bonusType;
    public IHaveBonus haveBonus;
    [HideInInspector] public bool MagnettoBonus = false;
    private float distance;

    //Update is called once per frame
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
                target.GetComponent<SinglePlayerController>().durationBonus = Duration;
                transform.position = target.transform.position;
                haveBonus.AddBonus(bonusType, sound);
                Destroy(gameObject);
            }
        }
        else
        {
            if (distance <= defaultDistance)
            {
                if (step < distance)
                {
                    transform.position += moveVector.normalized * speed;
                }
                else
                {
                    target.GetComponent<SinglePlayerController>().durationBonus = Duration;
                    transform.position = target.transform.position;
                    haveBonus.AddBonus(bonusType, sound);
                    Destroy(gameObject);
                }
            }
        }
    }
}