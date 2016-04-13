using UnityEngine;
using System.Collections;
using System;

public abstract class CardVisualizeEffect
{
    public struct VisualizeEffectData
    {
        public int m_VibrationCount;
        public float m_EffectTime;
        public float m_DistanceToSource;
    }

    public enum VisualizeEffectStatus
    {
        Idle,
        Start,
        Progress,
        Execute,
        End,
    };

    public enum EffectType
    {
        AttackEffect,
        HitEffect,
    };

    protected float m_CurrentElapsedTime = 0.0f;
    protected Vector3 m_EffectStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
    protected GameObject m_EffectTarget = null;
    protected VisualizeEffectData m_VisualizeEffectData;
    protected VisualizeEffectStatus m_VisualizeStatus = VisualizeEffectStatus.Idle;

    public abstract IEnumerator UpdateVisualizeEffect();
    public abstract EffectType GetVisualizeEffectType();

    public void InitVisualizeEffect(GameObject targetCard, ref VisualizeEffectData effectData)
    {
        m_EffectTarget = targetCard;
        m_VisualizeStatus = VisualizeEffectStatus.Start;

        m_VisualizeEffectData = new VisualizeEffectData();
        m_VisualizeEffectData.m_VibrationCount = effectData.m_VibrationCount;
        m_VisualizeEffectData.m_EffectTime = effectData.m_EffectTime;
        m_VisualizeEffectData.m_DistanceToSource = effectData.m_DistanceToSource;

        m_EffectStartPosition = m_EffectTarget.transform.position;
    }

    public VisualizeEffectStatus GetVisualizeEffectStatus()
    {
        return m_VisualizeStatus;
    }
}