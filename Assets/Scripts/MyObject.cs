using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MyObject : MonoBehaviour
{
    protected virtual int paintedOrder => 0;
    protected virtual bool isFlippable => true;

    [field: SerializeField]
    public float hp { get; protected set; } = -1;

    [field: SerializeField]
    public float maxHp { get; protected set; } = -1;

    [field: SerializeField]
    public int waitTime = 0;

    protected virtual bool viewHealthBar => true;

    protected virtual bool autoDamage => false;

    protected virtual bool hasInvincibleTime => true;
    public int invincibleTime { get; protected set; } = 0;


    protected virtual void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = paintedOrder;
        }
        
        if (viewHealthBar)
        {
            CreateChild("HealthBar", (Vector2)transform.position + new Vector2(0, 1.5f));
        }
    }

    protected virtual void Update()
    {
        // 待ち時間
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        if(rigidbody != null)
        {
            if (waitTime > 0)
            {
                waitTime--;
                rigidbody.gravityScale = 0;
                return;
            }
            if (waitTime == 0)
            {
                rigidbody.gravityScale = 1;
            }
        }

        invincibleTime = Mathf.Max(invincibleTime - 1, 0);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = invincibleTime % 4 < 2 ? 1 : 0;
            GetComponent<SpriteRenderer>().color = color;
        }

        if (autoDamage && hp > 0)
        {
            Damaged(1);
        }

        if (hp == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (viewHealthBar)
        {
            GameObject healthBar = GetChild("HealthBar");
            if (healthBar != null)
            {
                healthBar.SetActive(hp != maxHp);
                if (hp >= 0 && maxHp >= 0)
                {
                    healthBar.GetComponent<HealthBar>().health = hp / maxHp;
                }
            }
        }
        
        // 落下死
        if(transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {

    }

    public void Damaged(float damage)
    {
        if (hp != -1 && invincibleTime == 0)
        {
            hp = Mathf.Max(hp - damage, 0);

            if (hasInvincibleTime)
                invincibleTime = (int)(1.0f * Application.targetFrameRate);
        }
    }

    public void Healed(float healing)
    {
        if(hp > 0)
        {
            hp = Mathf.Min(hp + healing, maxHp);
        }
    }

    public void Killed()
    {
        if (hp != -1)
        {
            hp = 0;
        }
    }

    public float rotation
    {
        get => transform.eulerAngles.z;
        protected set => transform.eulerAngles = new Vector3(0, 0, value);
    }

    public Vector2 velocity
    {
        get
        {
            Rigidbody2D r = GetComponent<Rigidbody2D>();
            if (r == null)
            {
                throw new System.InvalidOperationException(
                    "This object \"" + GetType().Name + "\" has no component \"Rigidbody2D\""
                );
            }
            return r.velocity;
        }
        protected set
        {
            Rigidbody2D r = GetComponent<Rigidbody2D>();
            if (r == null)
            {
                throw new System.InvalidOperationException(
                    "This object \"" + GetType().Name + "\" has no component \"Rigidbody2D\""
                );
            }
            r.velocity = value;
            if(r.velocity.x != 0)
            {
                GetComponent<SpriteRenderer>().flipX = r.velocity.x < 0;
            }
        }
    }

    public static GameObject Create(string name, Vector2 position, float rotation = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(prefab, position, Quaternion.Euler(0, 0, rotation));
        obj.name = name;
        return obj;
    }

    public GameObject CreateChild(string name, Vector2 position, float rotation = 0)
    {
        GameObject obj = Create(name, position, rotation);
        obj.transform.SetParent(gameObject.transform);
        return obj;
    }

    public GameObject[] children
    {
        get
        {
            GameObject[] ret = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                ret[i] = transform.GetChild(i).gameObject;
            }
            return ret;
        }
    }

    public GameObject GetChild(string name)
    {
        foreach (GameObject child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    public bool HasChild(string name) => GetChild(name) != null;

    protected bool IsGround()
    {
        float width = transform.localScale.x;
        float height = transform.localScale.y;
        Vector3 leftStartPoint = transform.position - Vector3.right * 0.3f * width + Vector3.down * height * 0.5f;
        Vector3 rightStartPoint = transform.position + Vector3.right * 0.3f * width + Vector3.down * height * 0.5f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f + Vector3.down * height * 0.5f;
        return Physics2D.Linecast(leftStartPoint, endPoint)
                || Physics2D.Linecast(rightStartPoint, endPoint);
    }

    public void ChangeImage(string imagePath)
    {
        GetComponent<SpriteRenderer>().sprite 
            = Resources.Load<Sprite>(@"Images/" + imagePath);
    }

    // https://baba-s.hatenablog.com/entry/2019/10/03/090000
    /// <summary>
    /// ワールド座標を Screen Space - Camera の Canvas 内のローカル座標に変換します
    /// </summary>
    /// <param name="worldCamera">ワールド座標を描画するカメラ</param>
    /// <param name="canvasCamera">Canvas を描画するカメラ</param>
    /// <param name="canvasRectTransform">Canvas の RectTransform</param>
    /// <param name="worldPosition">変換前のワールド座標</param>
    /// <returns>変換後のローカル座標</returns>
    public static Vector3 WorldToScreenSpaceCamera
    (
        Camera worldCamera,
        Camera canvasCamera,
        RectTransform canvasRectTransform,
        Vector3 worldPosition
    )
    {
        var screenPoint = RectTransformUtility.WorldToScreenPoint
        (
            cam: worldCamera,
            worldPoint: worldPosition
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            rect: canvasRectTransform,
            screenPoint: screenPoint,
            cam: canvasCamera,
            localPoint: out var localPoint
        );

        return localPoint;
    }
}