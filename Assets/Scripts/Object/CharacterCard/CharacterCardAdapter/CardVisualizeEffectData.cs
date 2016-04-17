using UnityEngine;
using System.Collections;

public abstract class CardVisualizeEffectData
{
    public class CardVisualizeEffectAttack : CardVisualizeEffectData
    {
        public float m_EffectTime = 0.0f;
        public float m_DistanceToSource = 0.0f;

        public CardVisualizeEffectAttack(float effectTime, float distanceToSource)
        {
            m_EffectTime = effectTime;
            m_DistanceToSource = distanceToSource;
        }
    }

    public class CardVisualizeEffectHit : CardVisualizeEffectData
    {
        public int m_VibrationCount = 0;
        public float m_DistanceToSource = 0.0f;

        public CardVisualizeEffectHit(int vibrationCount, float distanceToSource)
        {
            m_VibrationCount = vibrationCount;
            m_DistanceToSource = distanceToSource;
        }
    }

    public class CardVisualizeEffectGlowing : CardVisualizeEffectData
    {
        public float m_EffectTime = 0.0f;
        public float m_AlphaDownThreshold = 0.0f;
        public Color m_GlowingColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        public CardVisualizeEffectGlowing(float effectTime, float alphaDownThreshold, Color glowingColor)
        {
            m_EffectTime = effectTime;
            m_AlphaDownThreshold = alphaDownThreshold;
            m_GlowingColor = glowingColor;
        }
    }
}
