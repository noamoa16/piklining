using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class HealthBar : MonoBehaviour
{
    [Range(0, 1)]
    public float health = 1;

    // Start is called before the first frame update
    void Start()
    {
        SetTransform();
    }

    // Update is called once per frame
    void Update()
    {
        front.GetComponent<Image>().fillAmount = health;
        Color color = front.GetComponent<Image>().color;
        color.r = Mathf.Min(2 * (1 - health), 1);
        color.g = Mathf.Min(1.88f * health, 1);
        color.b = 0;
        front.GetComponent<Image>().color = color;

        SetTransform();
    }

    private void SetTransform()
    {
        transform.position = transform.parent.position + new Vector3(0, 1.5f, 0);
        transform.eulerAngles = Vector3.zero;
    }

    private GameObject front
        => transform.GetChild(1).gameObject;
}
