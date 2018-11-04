using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MyTools : MonoBehaviour
{
    protected bool MyGetComponent<T>(out T component, GameObject obj)
    {
        component = obj.GetComponent<T>();
        if (component != null)
        {
            return true;
        }
        return false;
    }
}

public interface IHaveWeapons
{
    void AddAmmos(WeaponType type, int qty);
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

public class SinglePlayerController : MyTools, IAlive, IHaveWeapons, Bonus.IHaveBonus
{
    public GameObject plCam;
    private GameObject sceneCam;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    protected virtual void Awake()
    {
        sceneCam = GameObject.Find("Main Camera");
        sceneCam.SetActive(false);
        plCam.SetActive(true);
    }

    public float Health
    {
        get { return health.value; }

        set
        {
            if (value <= 0)
            {
                Death();

                health.value = 0;
            }

            health.value = value;
        }
    }

    [Tooltip("Подвижная часть тела (следует за камерой)")]
    public GameObject body;

    [Space(10)]
    [Range(15, 30)]
    [Tooltip("Скорость перемещения")]
    public float speed;

    [Tooltip("Скорость поворота по горизонтали")]
    public float xSpeed;

    [Tooltip("Скорость поворота по вертикали")]
    public float ySpeed;

    [Space(10)]
    [Tooltip("Используемое в данный момент оружие")]
    public Weapon[] weapon;

    [Space(10)]
    [Header("Гравитация")]
    [Tooltip("Ускорение")]
    public float grav;

    [Tooltip("Сила прыжка")] public float jumpSpeed;

    [Space(20)]
    [Header("Части интерфейса")]
    [Tooltip("Количество патронов")]
    public Text ammunitionCount;

    [Tooltip("Слайдер для здоровья")] public Slider health;

    [HideInInspector] public bool inDialog;

    public int weaponNumber;
    public int weaponPre;

    private Animator anim;
    private CharacterController controller;
    private Vector3 gravVector;
    private Vector3 moveVector;
    private RecoilRotation view;
    private const float minY = -100, maxY = 70;
    private float rotationX, rotationY;
    private float movementMultiplicator;
    private float vertSpeed;
    //private bool recoil;
    private bool reload;

    private void Start()
    {
        weaponNumber = 0;
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        gravVector = Vector3.down;
        movementMultiplicator = speed;
        //recoil = false;
        ammunitionCount.text = "2/0";
        view = new RecoilRotation();
        GetComponent<PlayerUI>().pc = this;
        Health = 100;
    }

    protected virtual void FixedUpdate()
    {
        if (!inDialog)
        {
            Move();
            MaxSpeed();
            Attack();
            Reload();
            Rotate();
            anim.SetBool("OnGround", controller.isGrounded);
            LeftWeapon();
            RightWeapon();
        }
        //if (recoil)
        //{
        //    StartCoroutine(Recoil());
        //}
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
            Vector3 forvardVec = new Vector3(plCam.transform.forward.x, 0, plCam.transform.forward.z);
            Vector3 rightVec = new Vector3(plCam.transform.right.x, 0, plCam.transform.right.z);
            moveVector = rightVec * x + forvardVec * z;
            anim.SetFloat("Xstate", x);
            anim.SetFloat("Zstate", z * movementMultiplicator / 2);
        }
        else
        {
            anim.SetFloat("Xstate", 0);
            anim.SetFloat("Zstate", 0);
        }

        if (controller.isGrounded)
        {
            vertSpeed = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("Jump");
                vertSpeed = jumpSpeed;
            }
        }

        vertSpeed += grav * Time.deltaTime;
        moveVector = new Vector3(moveVector.x * speed * movementMultiplicator * Time.fixedDeltaTime,
            vertSpeed * Time.deltaTime, moveVector.z * speed * movementMultiplicator * Time.fixedDeltaTime);
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
            anim.SetFloat("Rotate", h);
            rotationX = transform.localEulerAngles.y + h * xSpeed;
            rotationY += v * ySpeed;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            anim.SetFloat("Ystate", rotationY);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
            body.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
        else
        {
            anim.SetFloat("Rotate", 0);
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

        if (Input.GetMouseButton(0))
        {
            if (weapon[weaponNumber].MakeShoot())
            {
                anim.SetBool("Shoot", true);
                DrawAmmo();
                //Invoke("AttackEffect", 0.02f);
            }
        }
        if (weapon[weaponNumber].auto)
            if (Input.GetMouseButtonDown(0))
                if (weapon[weaponNumber].MakeShoot())
                {
                    //anim.SetBool("Shoot", true);
                    DrawAmmo();
                    //Invoke("AttackEffect", 0.02f);
                }
        if (Input.GetMouseButtonUp(0))
        {
            //weapon[weaponNumber].StopShooting();
            anim.SetBool("Shoot", false);
        }

    }
    //private void AttackEffect()
    //{
    //    GetRecoilVector(weapon[weaponNumber].backForce);
    //    recoil = true;
    //}

    private void RotateToView(float force, Vector2 forceVector)
    {
        float x, y;
        x = transform.localEulerAngles.y + force / 2 * Mathf.Sign(forceVector.x - rotationX) * Time.deltaTime;
        y = force * 3 * Mathf.Sign(forceVector.y - rotationY) * Time.deltaTime;
        rotationX = x;
        rotationY += y;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
        body.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }

    //private IEnumerator Recoil()
    //{
    //    float force = weapon[weaponNumber].backForce;
    //    for (float i = force; i > 0; i -= 20)
    //    {
    //        RotateToView(i / 4, view.newRotation);
    //        yield return new WaitForSeconds(0.025f);
    //    }

    //    for (int i = 0; i < force; i++)
    //    {
    //        RotateToView(0.5f, view.oldRotation);
    //    }

    //    recoil = false;
    //}

    private void GetRecoilVector(float weaponRecoilForce)
    {
        view.oldRotation = new Vector2(rotationX, rotationY);
        int[] c = { -1, 1 };
        view.newRotation = new Vector2(rotationX + c[Random.Range(0, 2)] * weaponRecoilForce / 10,
            rotationY + weaponRecoilForce);
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && weapon[weaponNumber].ammo > 0 && weapon[weaponNumber].magazin < weapon[weaponNumber].maxAmmo)
        {
            anim.SetTrigger("Reload");
            weapon[weaponNumber].Reload();
            Invoke("DrawAmmo", weapon[weaponNumber].reloadTime);
        }
    }

    private void LeftWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetWeapon(false);
        }
    }

    private void RightWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetWeapon(true);
        }
    }

    private void SetWeapon(bool up)
    {
        weaponPre = weaponNumber;
        if (up)
            weaponNumber = (weaponNumber == weapon.Length - 1 ? 0 : weaponNumber + 1);
        else
            weaponNumber = (weaponNumber == 0 ? weapon.Length - 1 : weaponNumber - 1);
        anim.SetInteger("WeaponNumber", weaponNumber);
    }

    public void SetVisibleWeapon()
    {
        weapon[weaponPre].gameObject.SetActive(false);
        weapon[weaponNumber].gameObject.SetActive(true);
        DrawAmmo();
    }

    #endregion

    #region Интерфйс

    public void DrawAmmo()
    {
        ammunitionCount.text = weapon[weaponNumber].magazin.ToString() + "/" + weapon[weaponNumber].ammo.ToString();
    }

    #endregion

    #region Реакции

    private void OnTriggerEnter(Collider other)
    {
        Ammunition amun;
        if (MyGetComponent(out amun, other.gameObject))
        {
            amun.target = gameObject;
            amun.haveWeapons = this;
            amun.move = true;
        }


        BonusUp bon;
        if (MyGetComponent(out bon, other.gameObject))
        {
            bon.target = gameObject;
            bon.move = true;
        }

        AttackArea at;
        if (MyGetComponent(out at, other.gameObject))
        {
            Health -= at.Damage;
        }
    }

    public void AddAmmos(WeaponType type, int qty)
    {
        foreach (var wea in weapon)
        {
            if (wea.type == type)
            {
                wea.ammo += qty;
            }
        }
        DrawAmmo(); 
    }

    public void AddBonus(Bonus.BonusType bonusType)
    {
        if (bonusType == Bonus.BonusType.SpeedUp)
        {
            speed = 40;
        }
    }

    #endregion


}