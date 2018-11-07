using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : SinglePlayerController
{
    public PhotonView photonView;
    private GameObject sceneCam;
    private Vector3 selfPos;
    private Quaternion selfRot;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    protected override void Awake()
    {
        if (photonView.isMine)
        {
            sceneCam = GameObject.Find("Main Camera");
            sceneCam.SetActive(false);
            plCam.SetActive(true);
        }

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.isMine)
        {
            LocalPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }
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


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        gravVector = Vector3.down;
        movementMultiplicator = speed;
       // recoil = false;
        ammunitionCount.text = "2/0";
        view = new RecoilRotation();
        GetComponent<PlayerUI>().pc = this;
        Health = 100;
    }

    protected override void FixedUpdate()
    {
        if (!inDialog)
        {
            if (photonView.isMine)
            {
                Move();
                MaxSpeed();
                Attack();
                Reload();
                Rotate();
            }
            else
            {
                SmoothNetMovement();
            }
        }
        //if (recoil)
        //{
        //    StartCoroutine(Recoil());
        //}
    }

    void SmoothNetMovement()
    {
        transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * 8);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, selfRot, 500 * Time.deltaTime);
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
            if (weapon[weaponNumber].MakeShoot())
            {
                DrawAmmo();
                //Invoke("AttackEffect", 0.02f);
            }
        }
    }

    //private void AttackEffect()
    //{
    //    //GetRecoilVector(weapon[weaponNumber].backForce);
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
        int[] c = {-1, 1};
        view.newRotation = new Vector2(rotationX + c[Random.Range(0, 2)] * weaponRecoilForce / 10,
            rotationY + weaponRecoilForce);
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon[weaponNumber].Reload();
            Invoke("DrawAmmo", weapon[weaponNumber].reloadTime);
        }
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
        }


        AttackArea at;
        if (MyGetComponent(out at, other.gameObject))
        {
            Health -= at.Damage;
        }
    }

    #endregion

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            selfPos = (Vector3) stream.ReceiveNext();
            selfRot = (Quaternion) stream.ReceiveNext();
        }
    }
}