using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MyTools : MonoBehaviour
{
    protected bool MyGetComponent<T>(out T component, GameObject obj)
    {
        Debug.Log(obj.name);
        component = obj.GetComponent<T>();
        Debug.Log(component);
        if (component != null)
        {
            return true;
        }
        return false;
    }
}

public interface IAlive
{
    float Health { get; set; }

    void GetDamage(float value);
    void PlusHealth(float value);
    void Death();
}

public class RecoilRotation
{
    public Vector2 newRotation;
    public Vector2 oldRotation;
}

public class PlayerController : MyTools, IAlive {

    public float Health
    {
        get
        {
            return helath.value;
        }

        set
        {
            if (value <= 0)
            {
                Death();

                helath.value = 0;
            }
            helath.value = value;
        }
    }

    [Tooltip("Подвижная часть тела (следует за камерой)")]
    public GameObject body;
    [Space(10)]
    [Range(15,30)]
    [Tooltip("Скорость перемещения")]
    public float speed;
    [Tooltip("Скорость поворота по горизонтали")]
    public float xSpeed;
    [Tooltip("Скорость поворота по вертикали")]
    public float ySpeed;

    [Space(10)]
    [Tooltip("Используемое в данный момент оружие")]
    public Weapon weapon;

    [Space(10)]
    [Header("Гравитация")]
    [Tooltip("Ускорение")]
    public float grav;
    [Tooltip("Сила прыжка")]
    public float jumpSpeed;

    [Space(20)]
    [Header("Части интерфейса")]
    [Tooltip("Количество патронов")]
    public Text ammunitionCount;
    [Tooltip("Слайдер для здоровья")]
    public Slider helath;

    [HideInInspector]
    public bool inDialog;


    private Animator anim;
    private CharacterController controller;
    private Vector3 gravVector;
    private Vector3 moveVector;
    private RecoilRotation view;
    private const float minY = -100, maxY = 70;
    private float rotationX, rotationY;
    private float movementMultiplicator;
    private float vertSpeed;
    private float health;
    private bool recoil;
    private bool reload;


    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        gravVector = Vector3.down;
        movementMultiplicator = speed;
        recoil = false;
        ammunitionCount.text = "2/0";
        view = new RecoilRotation();
        GetComponent<PlayerUI>().pc = this;
        Health = 100;
    }

    private void Update(){
        if(!inDialog)
        {
            Move();
            MaxSpeed();
            Attack();
            Reload();
        }
    }

    private void LateUpdate()
    {
        if(!inDialog)
        {
            Rotate();
        }
        if (recoil)
        {
            StartCoroutine("Recoil");
        }
    }

    #region Показатели
    public void GetDamage(float value)
    {

    }
    public void PlusHealth(float value)
    {

    }
    public void Death()
    {

    }
    #endregion

    #region Перемещение
    private void Move()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            moveVector = transform.right * x + transform.forward * z;
        }
        
        if (controller.isGrounded)
        {
            vertSpeed = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                vertSpeed = jumpSpeed;
            }
        }
        vertSpeed += grav * Time.deltaTime;
        moveVector = new Vector3(moveVector.x * speed * movementMultiplicator * Time.fixedDeltaTime, vertSpeed * Time.deltaTime, moveVector.z * speed * movementMultiplicator * Time.fixedDeltaTime);
        if (moveVector != Vector3.zero)
        {
            controller.Move(moveVector);
        }
    }
    private void Rotate()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var h = Input.GetAxis("Mouse X");
            var v = Input.GetAxis("Mouse Y");
            rotationX = transform.localEulerAngles.y + h * xSpeed;
            rotationY += v * ySpeed;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
            body.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
    }
    private void MaxSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementMultiplicator = 2;
        }
        else
        {
            movementMultiplicator = 1;
        }
    }
    #endregion

    #region Атака
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(weapon.MakeShoot())
            {
                DrawAmmo();
                Invoke("AttackEffect", 0.02f);
            }
        }
    }
    private void AttackEffect()
    {
        GetRecoilVector(weapon.backForce);
        recoil = true;
    }
    private void RotateToView(float force, Vector2 forceVector)
    {
        float x, y;
        x = transform.localEulerAngles.y + force/2 * Mathf.Sign(forceVector.x - rotationX) * Time.deltaTime;
        y = force* 3 *  Mathf.Sign(forceVector.y - rotationY) * Time.deltaTime;
        rotationX = x;
        rotationY += y;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
        body.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }
    private IEnumerator Recoil()
    {
        float force = weapon.backForce;
        for(float i = force; i > 0; i-=20)
        {
            RotateToView(i/4, view.newRotation);
            yield return new WaitForSeconds(0.025f);
        }
        for (int i  = 0; i < force; i++)
        {
            RotateToView(0.5f, view.oldRotation);
        }
        recoil = false;
    }
    private void GetRecoilVector(float weaponRecoilForce)
    {
        view.oldRotation = new Vector2(rotationX, rotationY);
        int[] c = { -1, 1 };
        view.newRotation = new Vector2(rotationX + c[Random.Range(0, 2)] * weaponRecoilForce/10, rotationY + weaponRecoilForce); 
    }
    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
            Invoke("DrawAmmo", weapon.reloadTime);
        }
    }
    #endregion

    #region Интерфйс

    private void DrawAmmo()
    {
        ammunitionCount.text = weapon.magazin.ToString() + "/" + weapon.ammo.ToString();
    }

    #endregion

    #region Реакции

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("OnCollisionEnter");
    //    AttackArea at;
    //    if (MyGetComponent(out at, collision.gameObject))
    //    {
    //        Health -= at.Damage;
    //        Debug.Log(at.Damage);
    //    }
    //    else Debug.Log("OnCollisionEnterNull");
    //}

    private void OnTriggerExit(Collider other)
    {
        Ammunition amun;
        if (MyGetComponent(out amun, other.gameObject))
        {
            weapon.ammo += amun.count;
            Destroy(other.gameObject);
            DrawAmmo();
        }

        Debug.Log("OnTriggerEnter");
        AttackArea at;
        
        if (MyGetComponent(out at, other.gameObject))
        {
            Health -= at.Damage;
            Debug.Log(at.Damage);
        }
        else Debug.Log("OnTriggerEnterNull");

    }

    #endregion

    
    
}


