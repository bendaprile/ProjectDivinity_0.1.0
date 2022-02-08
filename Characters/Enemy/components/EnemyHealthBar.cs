using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Image HealthBar;
    private Image HealthBarGhost;
    private Image PlatingBar;
    private Image ShieldBar;

    private GameObject EnemyInfo;

    private RectTransform BackgroundBar;

    private void Awake()
    {
        HealthBar = transform.Find("HealthBar").GetComponent<Image>();
        HealthBarGhost = transform.Find("HealthBarGhost").GetComponent<Image>();
        PlatingBar = transform.Find("PlatingBar").GetComponent<Image>();
        ShieldBar = transform.Find("ShieldBar").GetComponent<Image>();

        BackgroundBar = transform.Find("Background").GetComponent<RectTransform>();

        if (transform.Find("EnemyInfo"))
        {
            EnemyInfo = transform.Find("EnemyInfo").gameObject;
        }
    }

    public void Enable()
    {
        HealthBar.gameObject.SetActive(true);
        HealthBarGhost.gameObject.SetActive(true);
        PlatingBar.gameObject.SetActive(true);
        ShieldBar.gameObject.SetActive(true);
        BackgroundBar.gameObject.SetActive(true);
        if (EnemyInfo)
        {
            EnemyInfo.SetActive(true);
        }
    }

    public void Disable()
    {
        HealthBar.gameObject.SetActive(false);
        HealthBarGhost.gameObject.SetActive(false);
        PlatingBar.gameObject.SetActive(false);
        ShieldBar.gameObject.SetActive(false);
        BackgroundBar.gameObject.SetActive(false);
        if (EnemyInfo)
        {
            EnemyInfo.SetActive(false);
        }
    }

    private void Update()
    {
        if (!HealthBar.enabled)
        {
            return;
        }
        rotate();
        UpdateGhost();
    }

    // Update is called once per frame
    private void rotate()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.eulerAngles.z);
    }

    private void UpdateGhost()
    {
        if (HealthBarGhost.fillAmount > HealthBar.fillAmount)
        {
            HealthBarGhost.fillAmount -= .2f * Time.deltaTime;
        }
        else
        {
            HealthBarGhost.fillAmount = HealthBar.fillAmount;
        }
    }
}
