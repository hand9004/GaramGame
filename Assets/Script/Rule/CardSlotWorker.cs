using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CustomUtility;
using Constants;

public class CardSlotWorker : MonoBehaviour
{
    private class CardSlot
    {
        public Vector3 SlotPosition;
        public CharacterCard SlotObject;
    }

    private Vector3[] m_PlayerCardStandardPosition = null;
    private Vector3[] m_EnemyCardStandardPosition = null;
    private Vector3[] m_EnemyCharacterStandardPosition = null;

    private List<CharacterCard> m_PlayerCardList = null;
    private List<CharacterCard> m_EnemyCardList = null;

    private List<CardSlot> m_PlayerCardSlotList = new List<CardSlot>();
    private List<CardSlot> m_EnemyCardSlotList = new List<CardSlot>();

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
    }

    public void AttachPlayerCardToSlot(ref List<CharacterCard> targetCardList)
    {
        SyncCardWithSlot(ref targetCardList, ref m_PlayerCardSlotList);
        m_PlayerCardList = targetCardList;
    }

    public void AttachEnemyCardToSlot(ref List<CharacterCard> targetCardList)
    {
        SyncCardWithSlot(ref targetCardList, ref m_EnemyCardSlotList);
        m_EnemyCardList = targetCardList;
    }

    public int GetAttackableRowCountInRange(bool targetIsPlayerTeam, int realRange)
    {
        int retAttackableRowCount = 0;

        if(targetIsPlayerTeam)
        {
            retAttackableRowCount = GetAttackableRowCountOperation(ref m_PlayerCardSlotList, realRange);
        }
        else
        {
            retAttackableRowCount = GetAttackableRowCountOperation(ref m_EnemyCardSlotList, realRange);
        }

        return retAttackableRowCount;
    }

    public void ReArrangeCardSlot()
    {
        ReArrangeOperation(ref m_PlayerCardSlotList);
        ReArrangeOperation(ref m_EnemyCardSlotList);
    }

    public IEnumerator Run()
    {
        CheckAndEmptySlot(ref m_PlayerCardSlotList);
        CheckAndEmptySlot(ref m_EnemyCardSlotList);

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
            for(int j = 0; j < cardSlotList.Count; ++j)
            {
                int rowOfSlot = (j / 2) + 1;
                CardSlot cardSlotObject = cardSlotList[j];
                if(cardSlotObject.SlotObject == null)
                {
                    if (targetCardObject.RowNumber == rowOfSlot)
                    {
                        targetCardObject.transform.position = cardSlotObject.SlotPosition;
                        cardSlotObject.SlotObject = targetCardObject;
                        break;
                    }
                }
                else
                {
                    if(targetCardObject == cardSlotObject.SlotObject)
                    {
                        if (targetCardObject.RowNumber == rowOfSlot)
                        {
                            targetCardObject.transform.position = cardSlotObject.SlotPosition;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void CheckAndEmptySlot(ref List<CardSlot> targetCardSlotList)
    {
        for (int i = 0; i < targetCardSlotList.Count; ++i)
        {
            CardSlot playerCardSlot = targetCardSlotList[i];

            if (playerCardSlot.SlotObject != null)
            {
                if (playerCardSlot.SlotObject.CurrentHealthPoint <= 0)
                {
                    targetCardSlotList[i].SlotObject = null;
                }
            }
        }
    }

    private void ReArrangeOperation(ref List<CardSlot> targetCardSlotList)
    {
        for (int i = 0; i < targetCardSlotList.Count - 1; ++i)
        {
            int rowOfSlot = (i / 2) + 1;

            CardSlot targetSlot = targetCardSlotList[i];
            CardSlot nextTargetSlot = targetCardSlotList[i + 1];
            if (targetSlot.SlotObject == null)
            {
                if (nextTargetSlot.SlotObject != null)
                {
                    if(rowOfSlot != nextTargetSlot.SlotObject.RowNumber)
                    {
                        nextTargetSlot.SlotObject.RowNumber = rowOfSlot;
                    }

                    UtilityFunctions.Swap(ref targetSlot.SlotObject, ref nextTargetSlot.SlotObject);
                }
            }
        }
    }

    private int GetAttackableRowCountOperation(ref List<CardSlot> targetList, int realRange)
    {
        int retAttackableRowCount = 0;
        int prevRowIndex = 0;

        for (int i = 0; i < targetList.Count; ++i)
        {
            int rowIndex = (i / 2) + 1;
            CardSlot playerSlot = targetList[i];
            if (playerSlot.SlotObject != null)
            {
                if (playerSlot.SlotObject.RowNumber <= realRange)
                {
                    if (prevRowIndex != rowIndex)
                    {
                        prevRowIndex = rowIndex;
                        ++retAttackableRowCount;
                    }
                }
            }
            else
            {
                break;
            }
        }

        return retAttackableRowCount;
    }
}
