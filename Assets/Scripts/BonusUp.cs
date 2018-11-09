using UnityEngine;

public class BonusUp : MonoBehaviour
{
    public BonusType bonusType;

    [Range(0, 10)] public float defaultDistance;

    private float distance;
    [Range(0, 100)] public float Duration;
    public IHaveBonus haveBonus;
    [HideInInspector] public bool MagnettoBonus;
    private Vector3 moveVector;
    public AudioClip sound;
    [Range(0, 10)] public float speed;
    public GameObject target;

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
        var step = Vector3.Magnitude(moveVector.normalized * speed);
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