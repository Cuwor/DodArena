using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    Shotgun,
    AutoGun,
    Pistol,
}

[System.Serializable]
public struct WeaponSound
{
    [Tooltip("Выстрел")]
    public AudioClip shoot;
    [Tooltip("Перезарядка")]
    public AudioClip reload;
    [Tooltip("Нет патронов")]
    public AudioClip noAmmo;
}

public delegate void ShootHelper(bool start);

public class Weapon : MonoBehaviour
{
    [Tooltip("Объект со скриптом")]
    public GameObject[] pS;
    [Tooltip("Кол-во пуль в выстреле")]
    public short bulletInShoot;
    //[Tooltip("Снаряд")]
    //public GameObject projectile;
    //[Tooltip("Гильза")]
    //public GameObject gilz;
    //[Tooltip("Масто спавна снаряда")]
    //public Transform fireZone;
    //[Tooltip("Масто спавна гильзы")]
    //public Transform gilzZone;

    [Space(10)]
    [Header("Характеристики оружия")]
    public WeaponType type;
    [Tooltip("Урон пули")]
    public float weaponDamage;
    [Tooltip("Разброс")]
    public float range;
    [Tooltip("Потронов в обойме")]
    public int maxAmmo;
    [Tooltip("Автоматический режим")]
    public bool auto;
    //[Tooltip("Сила полёта снаряда")]
    //public float shootForce;
    //[Range(0, 100)]
    //[Tooltip("Отдача")]
    //public int backForce;
    [Tooltip("Время выстрела")]
    [Range(0.05f, 1.5f)]
    public float pauseTime;
    [Tooltip("Время перезарядки")]
    [Range(0.5f, 4f)]
    public float reloadTime;

    [Space(10)]
    [Header("Звуки оружия")]
    public WeaponSound sounds;

    [HideInInspector]
    public int ammo;
    [HideInInspector]
    public int magazin;

    private AudioSource sound;
    private bool ready = true;
    private bool readyReload = true;

    public event ShootHelper ShootHendler;

    // private ParticleSystem[] particleSystem;
    private DamageScript[] damageScript;
    public Sprite sprite;

    public PlayerUI playerUI;

    private void Start()
    {
        ShootHendler+=playerUI.PinChanged;
        magazin = maxAmmo;
        ammo = magazin;
        sound = GetComponent<AudioSource>();
        sound.clip = sounds.shoot;
        damageScript = new DamageScript[pS.Length];
        for (int i = 0; i < pS.Length; i++)
        {
            damageScript[i] = pS[i].GetComponent<DamageScript>();
            damageScript[i].PauseTime = pauseTime;
            damageScript[i].cam = playerUI.cam;
            damageScript[i].Auto = auto;
            damageScript[i].Radius = range;
            damageScript[i].Damage = weaponDamage;
            damageScript[i].BulletInShoot = bulletInShoot;
            damageScript[i].hitHandler += playerUI.Hit;
            ShootHendler += damageScript[i].Fire;
        }
    }

    public void ShootNowInvoke(bool start)
    {
        if (ShootHendler != null)
            ShootHendler.Invoke(start);
    }

    public bool MakeShoot()
    {
        if (ready && readyReload)
        {
            if (magazin > 0)
            {
                ShootNowInvoke(true);
                magazin -= 1;
                PlayThisClip(sound,sounds.shoot);
                ready = false;
                Invoke("ReadyAttack", pauseTime);
                return true;
            }
            else
            {
                PlayThisClip(sound,sounds.noAmmo);
            }
        }
        return false;
    }

    public void StopShooting()
    {
        ShootNowInvoke(false);
    }
    //private void ShootProject()
    //{
    //    GameObject proj = Instantiate(projectile, fireZone.position, Quaternion.identity);
    //    proj.GetComponent<Projectile>().damage = weaponDamage;
    //    proj.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * shootForce, ForceMode.Impulse);
    //    proj = Instantiate(gilz, gilzZone.position, Quaternion.identity);
    //    Vector3 v = transform.up * 2 + transform.right;
    //    proj.GetComponent<Rigidbody>().AddForce(v.normalized * 8, ForceMode.Impulse);
    //}


    private void PlayThisClip(AudioSource sound,AudioClip audioClip)
    {
        if (sound.isPlaying)
        {
            sound.Stop();
        }
        sound.clip = audioClip;
        sound.Play();
    }

    public void Reload()
    {
        if (ammo > 0 && magazin < maxAmmo && readyReload)
        {
            readyReload = false;
            PlayThisClip(sound,sounds.reload);
            Invoke("ReadyReload", reloadTime);
            var x = maxAmmo - magazin;
            if (x < ammo)
            {
                ammo -= x;
                magazin += x;
            }
            else
            {
                magazin += ammo;
                ammo = 0;
            }
        }
        else
        {
            PlayThisClip(sound,sounds.noAmmo);
        }
    }

    private void ReadyReload()
    {
        readyReload = true;
    }

    private void ReadyAttack()
    {
        ready = true;
    }


}
