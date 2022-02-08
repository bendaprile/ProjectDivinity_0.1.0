using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextScript : MonoBehaviour
{
    [SerializeField] private float maxDuration = 3f;

    private float Duration;
    private float AlphaReduction;
    private float prevDamage;
    TextMeshPro Text;

    public void Setup(Vector3 location, float damage, DamageType DT)
    {

        Duration = maxDuration;
        AlphaReduction = 1 / Duration;
        Text = GetComponentInChildren<TextMeshPro>();

        transform.position = location;
        prevDamage = damage;
        Text.text = damage.ToString("0");


        if (DT == DamageType.Regular)
        {
            Text.color = Color.black;
        }
    }

    public void UpdateNumber(Vector3 location, float damage)
    {
        Duration = maxDuration;
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 1f);
        transform.position = location;
        prevDamage += damage;
        Text.text = prevDamage.ToString("0");
    }

    public void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, transform.eulerAngles.z);

        if (Duration > 0)
        {
            Duration -= Time.deltaTime;
        }
        else if(Text.color.a > 0)
        {
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Text.color.a - (AlphaReduction * Time.deltaTime));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

