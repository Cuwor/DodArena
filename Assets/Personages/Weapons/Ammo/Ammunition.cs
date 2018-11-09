using UnityEngine;

public class Ammunition : MonoBehaviour
{
    public short count;
    [Range(0, 10)] public float defaultDistance;
    private float distance;
    public IHaveWeapons haveWeapons;
    [HideInInspector] public bool MagnettoBonus;
    private Vector3 moveVector;

    [Header("Звук во время поднятия предмета")]
    public AudioClip sound;

    [Range(0, 10)] public float speed;
    public GameObject target;
    public WeaponType weaponType;

    //Update is called once per frame//
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
            TranslateToPlayer(step);
        }
        else
        {
            if (distance <= defaultDistance)
            {
                TranslateToPlayer(step);
            }
        }

    }

    private void TranslateToPlayer(float step)
    {
        if (step < distance)
        {
            transform.position += moveVector.normalized * speed;
        }
        else
        {
            transform.position = target.transform.position;
            Adder();
            Destroy(gameObject);
        }
    }

    private void TheEnd()
    {
        Destroy(gameObject);
    }

    public virtual void Adder()
    {
        haveWeapons.AddAmmos(weaponType, count, sound);
    }
}