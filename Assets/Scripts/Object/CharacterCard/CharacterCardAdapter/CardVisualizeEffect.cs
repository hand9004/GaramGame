using UnityEngine;
using System.Collections;
using System;

public abstract class CardVisualizeEffect
{
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
        GlowingEffect,
    };

    protected float m_CurrentElapsedTime = 0.0f;
    protected Vector3 m_EffectStartPosition = new Vector3(0.0f, 0.0f, 0.0f);
    protected GameObject m_EffectTarget = null;
    protected CardVisualizeEffectData m_VisualizeEffectData = null;
    protected VisualizeEffectStatus m_VisualizeStatus = VisualizeEffectStatus.Idle;

    public abstract IEnumerator UpdateVisualizeEffect();
    public abstract EffectType GetVisualizeEffectType();
    public abstract void ForceQuitVisualizeEffect();

    public void InitVisualizeEffect(GameObject targetCard, CardVisualizeEffectData effectData)
    {
        m_EffectTarget = targetCard;
        m_VisualizeStatus = VisualizeEffectStatus.Start;

        m_VisualizeEffectData = effectData;
        m_EffectStartPosition = m_EffectTarget.transform.position;
    }

    public VisualizeEffectStatus GetVisualizeEffectStatus()
    {
        return m_VisualizeStatus;
    }
}