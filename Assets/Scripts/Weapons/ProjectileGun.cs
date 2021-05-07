using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private int m_ProjectileCount;
    [SerializeField] private int m_BurstCount;
    [SerializeField] private float m_BurstRate;
    [SerializeField] private float m_Firerate;
    [SerializeField] private bool m_Singlefire;
    [SerializeField] private float m_Spray;

    [Space]
    [SerializeField] private float m_AimDownSightsTime;
    [SerializeField] private AnimationCurve m_AimDownSightsCurve;

    [Space]
    [SerializeField] private int m_MagazineSize;
    [SerializeField] private float m_Reloadtime;

    [Space]
    [SerializeField] private Transform m_Muzzle;

    [Space]
    [SerializeField] private Animator m_Animator;
    [SerializeField] private string m_FireAnimationName;
    [SerializeField] private string m_ReloadAnimationName;
    [SerializeField] private string m_ADSParameterName;
    [SerializeField] private string m_SpeedParameterName;
    [SerializeField] private float m_MaxSpeed;

    private Rigidbody m_Rigidbody;

    private int m_CurrentMagazine;
    private float m_NextFireTime;
    private bool m_IsReloading;
    private bool m_IsAimDownSights;
    private float m_AimDownSightsPercent;

    private IWeaponInputProvider InputProvider { get; set; }

    private void Awake()
    {
        m_Rigidbody = GetComponentInParent<Rigidbody>();

        InputProvider = GetComponentInParent<IWeaponInputProvider>();

        InputProvider.FireEvent += () =>
        {
            if (m_Singlefire)
            {
                Shoot();
            }
        };
        InputProvider.ReloadEvent += Reload;

        Reload();
    }

    private void Update()
    {
        if (InputProvider != null)
        {
            if (InputProvider.WantsToFire && !m_Singlefire)
            {
                Shoot();
            }

            m_IsAimDownSights = InputProvider.IsAimingDownSights;

            m_AimDownSightsPercent += (1f / m_AimDownSightsTime) * (m_IsAimDownSights ? 1f : -1f) * Time.deltaTime;
            m_AimDownSightsPercent = Mathf.Clamp01(m_AimDownSightsPercent);

            m_Animator.SetFloat(m_ADSParameterName, m_AimDownSightsCurve.Evaluate(m_AimDownSightsPercent));

            if (m_Rigidbody)
            {
                float normalizedSpeed = Vector3.Scale(m_Rigidbody.velocity, new Vector3(1f, 0f, 1f)).magnitude / m_MaxSpeed;
                m_Animator.SetFloat(m_SpeedParameterName, normalizedSpeed);
            }
        }
        else
        {
            InputProvider = GetComponentInParent<IWeaponInputProvider>();
        }
    }

    public void Shoot ()
    {
        if (Time.time > m_NextFireTime)
        {
            StartCoroutine(ShootRoutine());
            m_NextFireTime = Time.time + (60f / m_Firerate) * m_BurstCount;
        }
    }

    private IEnumerator ShootRoutine()
    {
        for (int i = 0; i < m_BurstCount; i++)
        {
            if (m_CurrentMagazine > 0)
            {
                Instantiate(m_ProjectilePrefab, m_Muzzle.position, Quaternion.Euler(Random.insideUnitSphere * m_Spray) * m_Muzzle.rotation);

                m_Animator.Play(m_FireAnimationName, -1, 0f);

                m_CurrentMagazine--;
                yield return new WaitForSeconds(60f / m_BurstRate);
            }
            else
            {
                Reload();
                yield break;
            }
        }
    }

    public void Reload ()
    {
        if (m_CurrentMagazine < m_MagazineSize && !m_IsReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        m_IsReloading = true;
        m_CurrentMagazine = 0;

        m_Animator.Play(m_ReloadAnimationName, -1, 0f);

        yield return new WaitForSeconds(m_Reloadtime);

        m_CurrentMagazine = m_MagazineSize;
        m_IsReloading = false;
    }
}
