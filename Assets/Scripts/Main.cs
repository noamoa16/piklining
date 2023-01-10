using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/*
TODO:
MersenneTwisterの削除を検討
普通のボールをビー玉に変更
バルーンとマグネットボールの追加
タイトル画面のUI
*/

public class Main : MonoBehaviour
{
    public static HashSet<string> prefabNames { get; private set; } = new HashSet<string>();

    public static readonly MersenneTwister random = new MersenneTwister();

    public static bool isQuitting { get; private set; } = false;

    public static int tick { get; private set; } = 0;

    public static int score { get; private set; } = 0;

    private const float xAspect = 16.0f;
    private const float yAspect = 9.0f;

    private static Queue<MyObjectData> objectQueue;

    // https://meideru.com/archives/556
    /*private Rect CalcAspect(float width, float height)
    {
        float targetAspect = width / height;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (1.0f > scaleHeight)
        {
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            rect.width = 1.0f;
            rect.height = scaleHeight;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0.0f;
            rect.width = scaleWidth;
            rect.height = 1.0f;
        }
        return rect;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        Random.InitState(System.DateTime.Now.Millisecond);


        // Test
        //MyObject.Create("Box1", new Vector2(-4, 6));
        //MyObject.Create("Coin", new Vector2(2, 6));
        //MyObject.Create("Box1", new Vector2(4, 6));

        // オブジェクト一覧マップの作成
        GameObject[] prefabs = Resources.LoadAll<GameObject>(@"Prefabs");
        System.Array.ForEach(prefabs, prefab => prefabNames.Add(prefab.name));

        LoadStageData("test.json");
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public static ScoreText GetScore(Vector2 position, int value)
    {
        score += value;
        GameObject s = MyObject.Create("ScoreText", position);
        ScoreText scoreText = s.GetComponent<ScoreText>();
        scoreText.score = value;
        scoreText.position = position + 0.5f * Vector2.up;

        // ScoreBoardにscoreをセット
        GameObject scoreBoard = GameObject.Find("ScoreBoard");
        Text scoreBoardText = scoreBoard.GetComponent<Text>();
        scoreBoardText.text = $"{score:00000000}";

        return scoreText;
    }

    public static void LoadStageData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(@"Data/" + path.Replace(".json", ""));
        string json = textAsset.text;
        StageData savedObjectsData = JsonUtility.FromJson<StageData>(json);

        objectQueue = new Queue<MyObjectData>();
        foreach (MyObjectData myObjectData in savedObjectsData.OrderBy(o => o.t))
        {
            objectQueue.Enqueue(myObjectData);
        }
    }

    private static string ToStageFilePath(string path)
    {
        return Application.dataPath + "/Data/" + path;
    }

    private static string ConvertToReadableJson(string json)
    {
        string ret = string.Empty;
        bool isString = false;
        int tabs = 0;

        string multiTabs(int n)
        {
            string s = string.Empty;
            for (int i = 0; i < n; i++)
            {
                s += "  ";
            }
            return s;
        }

        foreach (char c in json)
        {
            string before = string.Empty;
            string after = string.Empty;
            if (c == '"') isString ^= true;

            if (!isString)
            {
                switch (c)
                {
                    case '{':
                    case '[':
                        after += '\n';
                        tabs++;
                        after += multiTabs(tabs);
                        break;
                    case '}':
                    case ']':
                        before += '\n';
                        tabs--;
                        before += multiTabs(tabs);
                        break;
                    case ':':
                        before += ' ';
                        after += ' ';
                        break;
                    case ',':
                        after += '\n';
                        after += multiTabs(tabs);
                        break;
                }
            }

            ret += before + c + after;
        }

        return ret;
    }

    // Update is called once per frame
    void Update()
    {
        tick++;

        // ハートのセット
        GameObject olimar = GameObject.Find("Olimar");
        int hp = olimar != null ? (int)olimar.GetComponent<Player>().hp : 0;
        for (int i = 1; i <= 3; i++)
        {
            GameObject heart = GameObject.Find("Heart" + i);
            Color color = heart.GetComponent<Image>().color;
            color.r = hp >= i ? 1 : 0;
            heart.GetComponent<Image>().color = color;
        }

        // オブジェクト生成
        while(objectQueue.Count > 0 && tick >= Application.targetFrameRate * objectQueue.Peek().t)
        {
            MyObjectData myObjectData = objectQueue.Dequeue();
            for(int i = 0; i < myObjectData.additional + 1; i++)
            {
                if(myObjectData.xrange != 0)
                {
                    myObjectData.x += Random.Range(-myObjectData.xrange, myObjectData.xrange);
                }
                Debug.Log(myObjectData);
                GameObject gameObject
                    = MyObject.Create(myObjectData.name, 
                        new Vector2(myObjectData.x, 4.5f));
                MyObject myObject = gameObject.GetComponent<MyObject>();
                myObject.waitTime = 30;
            }
        }
    }
}
