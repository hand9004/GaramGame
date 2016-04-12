using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStatusCheckWorker : MonoBehaviour {
    public delegate void DiedCardObjectListener(bool hasDiedObject);

    private DiedCardObjectListener m_DiedCardObjectListener;

    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_EnemyCardList = null;

    public void InitCardStatusCheckWorker(ref List<CharacterCard> playerCardList, ref List<CharacterCard> enemyCardList)
    {
        m_PlayerCardList = playerCardList;
        m_EnemyCardList = enemyCardList;
    }

    public void RegisterDiedCardObjectListener(DiedCardObjectListener diedCardObjectListener)
    {
        m_DiedCardObjectListener = diedCardObjectListener;
    }

    public IEnumerator Run()
    {
        yield return null;
    }

    private void CheckHasDiedCardObject()
    {
        for (int i = 0; i < m_PlayerCardList.Count; ++i)
        {
            if (m_PlayerCardList[i].CurrentHealthPoint <= 0)
            {
                m_DiedCardObjectListener(true);
                break;
            }
        }

        for (int i = 0; i < m_EnemyCardList.Count; ++i)
        {
            if (m_EnemyCardList[i].CurrentHealthPoint <= 0)
            {
                m_DiedCardObjectListener(true);
                break;
            }
        }

    }
}
