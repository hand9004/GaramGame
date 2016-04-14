using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour {
    public GameObject ObjectType = null;
    public int InitialObjectCount = 0;

    private int m_ObjectIndex = 0;
    private List<GameObject> m_PooledObjectList = new List<GameObject>();
    private List<GameObject> m_UsedObjectList = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < InitialObjectCount; ++i)
        {
            GameObject poolingGameObject = Instantiate(ObjectType);

            poolingGameObject.SetActive(false);
            poolingGameObject.name = "PooledObject";
            m_PooledObjectList.Add(poolingGameObject);
        }
    }

    public GameObject GetObject()
    {
        GameObject retObject = null;
        if(m_PooledObjectList.Count > 0)
        {
            retObject = (GameObject)m_PooledObjectList[0];
            m_PooledObjectList.RemoveAt(0);
            m_UsedObjectList.Add(retObject);
        }
        else
        {
            retObject = Instantiate(ObjectType);
            m_UsedObjectList.Add(retObject);
        }
        retObject.name = "ActiveObject" + m_ObjectIndex++;
        retObject.SetActive(true);

        return retObject;
    }

    public void ReleaseObject(GameObject targetObject)
    {
        m_UsedObjectList.Remove(targetObject);
        m_PooledObjectList.Add(targetObject);

        targetObject.name = "PooledObject" + m_ObjectIndex--;
        targetObject.SetActive(false);
    }

    public void CleanUpObject(GameObject targetObject)
    {
        m_UsedObjectList.Remove(targetObject);
        m_PooledObjectList.Remove(targetObject);

        Destroy(targetObject);
        targetObject = null;
    }
}
