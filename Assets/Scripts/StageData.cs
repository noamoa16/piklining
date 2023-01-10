using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class StageData : IEnumerable<MyObjectData>
{
    public List<MyObjectData> objects;
    
    public StageData()
    {
        objects = new List<MyObjectData>();
    }

    public StageData(List<MyObjectData> objects)
    {
        this.objects = objects;
    }

    public void Add(MyObjectData obj)
    {
        objects.Add(obj);
    }

    public int Count => objects.Count;

    public IEnumerator<MyObjectData> GetEnumerator()
    {
        return objects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[System.Serializable]
public class MyObjectData
{
    public string name;
    public float x = 0;
    public float t = 0;
    public int additional = 0;
    public float xrange = 0;
    public MyObjectData(string name, float x = 0, float t = 0)
    {
        this.name = name;
        this.x = x;
        this.t = t;
    }
    public MyObjectData(GameObject gameObject)
    {
        this.name = gameObject.name;
        this.x = gameObject.transform.position.x;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

    public MyObjectData Clone()
    {
        return (MyObjectData)MemberwiseClone();
    }
}
