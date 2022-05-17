using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberPopup : MonoBehaviour
{
    private static NumberPopup _instance;
    private static NumberPopup instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<NumberPopup>("NumberPopup"));
            }

            return _instance;
        }
    }

    public static NumberPopup Create(Vector3 position, int number, Color color)
    {
        var cameraPos = Camera.main.transform.position;
        var popupObj = Instantiate(instance, position, Quaternion.identity);
        popupObj.Setup(number, color);
        popupObj.transform.LookAt(new Vector3(cameraPos.x, position.y, cameraPos.z));

        return popupObj;
    }

    [SerializeField] TextMeshPro text;
    [SerializeField] float timer = 2f;

    Color color;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Setup(int number, Color color)
    {
        text.SetText(number.ToString());
        this.color = color;
        text.color = color;
    }

    private void Update()
    {
        const float moveYSpeed = 0.125f;
        const float disappearSpeed = 1.25f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            color.a -= disappearSpeed * Time.deltaTime;
            text.color = color;

            if (color.a < 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
