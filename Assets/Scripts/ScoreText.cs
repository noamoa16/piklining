using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    protected int m_score = 0;
    protected Text text;
    private Vector2 virtualPosition = Vector2.zero;

    [field: SerializeField]
    public int disappearCount { get; protected set; } = -1;

    public int score
    {
        get
        {
            return m_score;
        }
        set
        {
            m_score = value;
            if (text != null)
            {
                text.text = m_score.ToString();
            }
        }
    }

    public Vector2 position
    {
        get
        {
            return virtualPosition;
        }
        set
        {
            virtualPosition = value;
            transform.GetChild(0).position
                = RectTransformUtility.WorldToScreenPoint(Camera.main, virtualPosition);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        text = transform.GetChild(0).GetComponent<Text>();
        text.text = m_score.ToString();

        position = virtualPosition;
    }

    // Update is called once per frame
    void Update()
    {
        position += new Vector2(0, 0.02f);

        if(disappearCount == 0)
        {
            Destroy(gameObject);
        }
        else if(disappearCount > 0)
        {
            disappearCount--;
        }
    }
}
