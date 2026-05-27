using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour
{
    [Header("Laser References")]
    [SerializeField] LineRenderer laserLine;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Transform laserStartPos;

    [Header("Laser Settings")]
    [SerializeField] int laserMaxDist = 50;
    [SerializeField] int laserDamage = 10;
    [SerializeField] float damageRate = 1f;

    [Header("On / Off Timing")]
    [SerializeField] float laserOnTime = 3f;
    [SerializeField] float laserOffTime = 2f;

    bool isDamaging;
    bool laserActive = true;

    void Start()
    {
        StartCoroutine(LaserOnOffCycle());
    }

    void Update()
    {
        if (laserActive)
        {
            createLaser();
        }
    }

    void createLaser()
    {
        RaycastHit hit;

        if (Physics.Raycast(laserStartPos.position, laserStartPos.forward, out hit, laserMaxDist))
        {
            laserLine.SetPosition(0, laserStartPos.position);
            laserLine.SetPosition(1, hit.point);

            hitEffect.SetActive(true);
            hitEffect.transform.position = hit.point;

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null && !isDamaging)
            {
                StartCoroutine(damageTime(dmg));
            }
        }
        else
        {
            laserLine.SetPosition(0, laserStartPos.position);
            laserLine.SetPosition(1, laserStartPos.position + laserStartPos.forward * laserMaxDist);

            hitEffect.SetActive(false);
        }
    }

    IEnumerator damageTime(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(laserDamage);

        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }

    IEnumerator LaserOnOffCycle()
    {
        while (true)
        {
            // Turn laser ON
            laserActive = true;
            laserLine.enabled = true;

            yield return new WaitForSeconds(laserOnTime);

            // Turn laser OFF
            laserActive = false;
            laserLine.enabled = false;
            hitEffect.SetActive(false);
            isDamaging = false;

            yield return new WaitForSeconds(laserOffTime);
        }
    }
}