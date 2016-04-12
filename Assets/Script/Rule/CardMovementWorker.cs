using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Constants;

public class CardMovementWorker : MonoBehaviour
{
    private class CardSlot
    {
        public Vector3 SlotPosition;
        public CharacterCard SlotObject;
    }

    private Vector3[] m_PlayerCardStandardPosition = null;
    private Vector3[] m_EnemyCardStandardPosition = null;
    private Vector3[] m_EnemyCharacterStandardPosition = null;

    private List<CardSlot> m_PlayerCardSlotList = new List<CardSlot>();
    private List<CardSlot> m_EnemyCardSlotList = new List<CardSlot>();
    private List<CardSlot> m_EnemyCharacterSlotList = new List<CardSlot>();

    public Vector3[] PlayerCardPosition
    {
        set { m_PlayerCardStandardPosition = value; }
    }
    public Vector3[] EnemyCardPosition
    {
        set { m_EnemyCardStandardPosition = value; }
    }
    public Vector3[] EnemyCharacterPosition
    {
        set { m_EnemyCharacterStandardPosition = value; }
    }

    public void InitCardPositionSlots()
    {
        InitCardSlotsInList(m_PlayerCardSlotList, m_PlayerCardStandardPosition, false);
        InitCardSlotsInList(m_EnemyCardSlotList, m_EnemyCardStandardPosition, false);
        InitCardSlotsInList(m_EnemyCharacterSlotList, m_EnemyCharacterStandardPosition, true);
    }

    public void AttachPlayerCardToSlot(ref List<CharacterCard> targetCardList)
    {
        Debug.Assert(targetCardList.Count == m_PlayerCardSlotList.Count, "Slot and Object Count Should be Equal.");
        SyncCardWithSlot(ref targetCardList, ref m_PlayerCardSlotList);
    }

    public void AttachEnemyCardToSlot(ref List<CharacterCard> targetCardList)
    {
        Debug.Assert(targetCardList.Count == m_EnemyCardSlotList.Count, "Slot and Object Count Should be Equal.");
        SyncCardWithSlot(ref targetCardList, ref m_EnemyCardSlotList);
    }

    public void AttachEnemyCharacterToSlot(ref List<CharacterCard> targetCardList)
    {
        Debug.Assert(targetCardList.Count == m_EnemyCharacterSlotList.Count, "Slot and Object Count Should be Equal.");
        SyncCardWithSlot(ref targetCardList, ref m_EnemyCharacterSlotList);
    }

    public IEnumerator Run()
    {
        ObjectPoolManager poolMgr = GameObject.Find("CharacterCardPool").GetComponent<ObjectPoolManager>();

        CheckAndRemoveDeadSlot(ref m_PlayerCardSlotList, ref poolMgr);
        CheckAndRemoveDeadSlot(ref m_EnemyCardSlotList, ref poolMgr);



        yield return null;
    }

    private void InitCardSlotsInList(List<CardSlot> targetSlotList, Vector3[] standardPosition, bool isCharacterSlot)
    {
        bool onLeftSlotCreate = true;
        float distToStandardPosition = (isCharacterSlot == false) ? ConstantDefine.DISTANCE_FROM_STANDARD_POSITION_CARD :
                                            ConstantDefine.DISTANCE_FROM_STANDARD_POSITION_CHARACTER;
        for (int i = 0; i < ConstantDefine.MAX_CARD_DECK_COUNT; ++i)
        {
            CardSlot createdCardSlot = new CardSlot();

            Vector3 standardTemp = standardPosition[i / 2];
            Vector3 realPosition = new Vector3(0.0f, 0.0f, 0.0f);
            if (onLeftSlotCreate)
            {
                realPosition = new Vector3(standardTemp.x - distToStandardPosition, standardTemp.y, standardTemp.z);
            }
            else
            {
                realPosition = new Vector3(standardTemp.x + distToStandardPosition, standardTemp.y, standardTemp.z);
            }
            createdCardSlot.SlotPosition = realPosition;
            createdCardSlot.SlotObject = null;

            targetSlotList.Add(createdCardSlot);

            onLeftSlotCreate = !onLeftSlotCreate;
        }
    }

    private void SyncCardWithSlot(ref List<CharacterCard> targetCardList, ref List<CardSlot> cardSlotList)
    {
        for (int i = 0; i < targetCardList.Count; ++i)
        {
            CharacterCard targetCardObject = targetCardList[i];

            targetCardObject.transform.position = cardSlotList[i].SlotPosition;
            cardSlotList[i].SlotObject = targetCardObject;
        }
    }

    private void CheckAndRemoveDeadSlot(ref List<CardSlot> targetCardSlotList, ref ObjectPoolManager poolMgr)
    {
        for (int i = 0; i < targetCardSlotList.Count; ++i)
        {
            CardSlot playerCardSlot = targetCardSlotList[i];

            if (playerCardSlot.SlotObject != null)
            {
                if (playerCardSlot.SlotObject.CurrentHealthPoint <= 0)
                {
                    targetCardSlotList[i].SlotObject.ResetStatus();
                    poolMgr.ReleaseObject(targetCardSlotList[i].SlotObject.gameObject);
                    targetCardSlotList[i].SlotObject = null;
                }
            }
        }
    }
}
