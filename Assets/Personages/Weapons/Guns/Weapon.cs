using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool ready;

    public event ShootHelper ShootNow;

    // private ParticleSystem[] particleSystem;
    private DamageScript[] damageScript;

    public IHit player;

    private void Start()
    {
        ready = true;
        magazin = maxAmmo;
        ammo = 0;
        sound = GetComponent<AudioSource>();
        sound.clip = sounds.shoot;
        damageScript = new DamageScript[pS.Length];
        for (int i = 0; i < pS.Length; i++)
        {
            //damageScript[i].hit += player.IHit;
            damageScript[i] = pS[i].GetComponent<DamageScript>();
            damageScript[i].PauseTime = pauseTime;
            damageScript[i].Auto = auto;
            damageScript[i].Damage = weaponDamage;
            damageScript[i].BulletInShoot = bulletInShoot;
            ShootNow += damageScript[i].Fire;
        }
    }

    private void Start2()
    {
        
    }

    public void ShootNowInvoke(bool start)
    {
        if (ShootNow != null)
        {
            ShootNow.Invoke(start);
        }
    }

    public bool MakeShoot()
    {
        if (ready)
        {
            if (magazin > 0)
            {
                ShootNowInvoke(true);
                //ShootProject();
                magazin -= 1;
                PlayThisClip(sounds.shoot);
                ready = false;
                Invoke("ReadyAttack", pauseTime);
                return true;
            }
            else
            {
                PlayThisClip(sounds.noAmmo);
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


    private void PlayThisClip(AudioClip audioClip)
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
        if (ammo > 0 && magazin < maxAmmo && ready)
        {
            ready = false;
            PlayThisClip(sounds.reload);
            Invoke("ReadyAttack", reloadTime);
            while (magazin < maxAmmo && ammo > 0)
            {
                magazin++;
                ammo--;
            }
        }
        else
        {
            PlayThisClip(sounds.noAmmo);
        }
    }


    private void ReadyAttack()
    {
        ready = true;
    }


}
