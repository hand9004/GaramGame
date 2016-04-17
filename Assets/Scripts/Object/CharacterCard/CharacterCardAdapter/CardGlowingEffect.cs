using UnityEngine;
using System.Collections;

public class CardGlowingEffect : CardVisualizeEffect
{
    private SpriteRenderer m_BackgroundRenderer = null;
    private CardVisualizeEffectData.CardVisualizeEffectGlowing m_GlowingEffectData = null;

    public override IEnumerator UpdateVisualizeEffect()
    {
        if(m_EffectTarget != null)
        {
            if(m_GlowingEffectData == null)
            {
                m_GlowingEffectData = (CardVisualizeEffectData.CardVisualizeEffectGlowing)m_VisualizeEffectData;
            }

            if(m_BackgroundRenderer == null)
            {
                m_BackgroundRenderer = m_EffectTarget.GetComponent<SpriteRenderer>();
            }

            m_VisualizeStatus = VisualizeEffectStatus.Progress;
            float glowUpThresholdTime = m_GlowingEffectData.m_EffectTime * 0.5f;
            float glowTimeLength = m_GlowingEffectData.m_EffectTime - glowUpThresholdTime;
            if (m_CurrentElapsedTime < glowUpThresholdTime)
            {
                float timeStamp = m_CurrentElapsedTime / glowTimeLength;
                float glowUpAlpha = Mathf.Lerp(m_GlowingEffectData.m_AlphaDownThreshold, 1.0f, timeStamp);

                m_BackgroundRenderer.color = new Color(m_GlowingEffectData.m_GlowingColor.r, m_GlowingEffectData.m_GlowingColor.g,
                       m_GlowingEffectData.m_GlowingColor.b, glowUpAlpha);

                m_CurrentElapsedTime += Time.deltaTime;
            }
            else
            {
                if(m_CurrentElapsedTime > m_GlowingEffectData.m_EffectTime)
                {
                    m_CurrentElapsedTime = 0.0f;
                }
                else
                {
                    float timeStamp = (m_CurrentElapsedTime - glowUpThresholdTime) / glowTimeLength;
                    float glowDownAlpha = Mathf.Lerp(1.0f, m_GlowingEffectData.m_AlphaDownThreshold, timeStamp);

                    m_BackgroundRenderer.color = new Color(m_GlowingEffectData.m_GlowingColor.r, m_GlowingEffectData.m_GlowingColor.g,
                           m_GlowingEffectData.m_GlowingColor.b, glowDownAlpha);

                    m_CurrentElapsedTime += Time.deltaTime;
                }
            }
        }

        yield return null;
    }

    public override EffectType GetVisualizeEffectType()
    {
        return EffectType.GlowingEffect;
    }

    public override void ForceQuitVisualizeEffect()
    {
        if(m_BackgroundRenderer != null)
        {
            m_BackgroundRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }
}
