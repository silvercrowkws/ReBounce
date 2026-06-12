using UnityEngine;

public enum CardGrade
{
    Rare,
    Epic,
    Legendary
}

public class TestCardGlow : MonoBehaviour
{
    /// <summary>
    /// 영웅, 전설 카드 반짝이는 효과용 스프라이트
    /// </summary>
    [SerializeField] Transform glowSprite;

    /// <summary>
    /// 이 카드의 등급
    /// </summary>
    [SerializeField] private CardGrade cardGrade;

    private Vector3 originScale;
    private Color originColor;
    private SpriteRenderer glowframeSpriteRenderer;

    private SpriteRenderer frameSpriteRenderer;

    Color rareColor = new Color(0.2f, 0.55f, 1f);
    Color epicColor = new Color(0.65f, 0.3f, 1f);
    Color legendaryColor = new Color(1f, 0.75f, 0.15f);

    Color legendaryGlowColor = new Color(1f, 0.6f, 0f, 0.4f);
    Color epicGlowColor = new Color(0.45f, 0.15f, 0.85f, 0.4f);

    private void OnEnable()
    {
        Transform child = transform.GetChild(0);
        frameSpriteRenderer = child.GetComponent<SpriteRenderer>();

        child = transform.GetChild(1);
        glowframeSpriteRenderer = child.GetComponent<SpriteRenderer>();

        switch (cardGrade)
        {
            case CardGrade.Rare:
                frameSpriteRenderer.color = rareColor;                
                break;

            case CardGrade.Epic:
                frameSpriteRenderer.color = epicColor;
                glowframeSpriteRenderer.color = epicGlowColor;
                break;

            case CardGrade.Legendary:
                frameSpriteRenderer.color = legendaryColor;
                glowframeSpriteRenderer.color = legendaryGlowColor;
                break;
        }
    }

    private void Start()
    {
        // 희귀 등급이 아니면
        if(cardGrade != CardGrade.Rare)
        {
            originScale = glowSprite.localScale;

            glowframeSpriteRenderer = glowSprite.GetComponent<SpriteRenderer>();
            originColor = glowframeSpriteRenderer.color;
        }

        // 영웅 등급은 기본적으로 1.05배 크게
        if (cardGrade == CardGrade.Epic)
        {
            glowSprite.localScale = originScale * 1.05f;
        }
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f;

        switch (cardGrade)
        {
            // 희귀는 아무 효과 없음
            case CardGrade.Rare:
                break;

            // 영웅
            case CardGrade.Epic:
                {
                    Color c = originColor;
                    c.a = Mathf.Lerp(0f, 0.6f, t);
                    glowframeSpriteRenderer.color = c;

                    break;
                }

            // 전설
            case CardGrade.Legendary:
                {
                    glowSprite.localScale =
                        originScale * Mathf.Lerp(1f, 1.1f, t);

                    Color c = originColor;
                    c.a = Mathf.Lerp(0.2f, 0.6f, t);
                    glowframeSpriteRenderer.color = c;
                    break;
                }
        }
    }
}