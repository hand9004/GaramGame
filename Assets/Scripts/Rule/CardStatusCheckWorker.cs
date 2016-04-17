using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStatusCheckWorker : MonoBehaviour {
    public delegate void DiedCardObjectListener();

    private DiedCardObjectListener m_DiedCardObjectListener;

    private bool m_HasCardDie = false;
    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_EnemyCardList = null;
    private ObjectPoolManager m_PoolMgr = null;

    public void InitCardStatusCheckWorker(ref List<CharacterCard> playerCardList, ref List<CharacterCard> enemyCardList)
    {
        m_PlayerCardList = playerCardList;
        m_EnemyCardList = enemyCardList;

        m_PoolMgr = GameObject.Find("CharacterCardPool").GetComponent<ObjectPoolManager>();
    }

    public void RegisterDiedCardObjectListener(DiedCardObjectListener diedCardObjectListener)
    {
        m_DiedCardObjectListener = diedCardObjectListener;
    }

    public IEnumerator Run()
    {
        CheckHasDiedCardObject(ref m_PlayerCardList);
        CheckHasDiedCardObject(ref m_EnemyCardList);

        if (m_HasCardDie)
        {
            m_DiedCardObjectListener();
            m_HasCardDie = false;
        }

        yield return null;
    }

    private void CheckHasDiedCardObject(ref List<CharacterCard> targetList)
    {
        List<CharacterCard> removeList = new List<CharacterCard>();
        for (int i = 0; i < targetList.Count; ++i)
        {
            if (targetList[i].CurrentHealthPoint <= 0)
            {
                removeList.Add(targetList[i]);
                m_HasCardDie = true;
            }
        }

        for (int i = 0; i < removeList.Count; ++i)
        {
            removeList[i].ResetStatus();
            m_PoolMgr.ReleaseObject(removeList[i].gameObject);
            targetList.Remove(removeList[i]);
        }

        removeList = null;
    }
}
