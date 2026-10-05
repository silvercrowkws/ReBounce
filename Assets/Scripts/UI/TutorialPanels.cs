using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanels : MonoBehaviour
{
    /// <summary>
    /// 이전 버튼
    /// </summary>
    [SerializeField]
    Button previousButton;

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    [SerializeField]
    Button startButton;

    /// <summary>
    /// 다음 버튼
    /// </summary>
    [SerializeField]
    Button nextButton;

    [SerializeField]
    Sprite[] kings;

    /// <summary>
    /// Slide2 말풍선 안 설명 텍스트
    /// </summary>
    [SerializeField]
    TextMeshProUGUI speechBalloonText;

    /// <summary>
    /// Slide2 국왕 이미지
    /// </summary>
    [SerializeField]
    Image kingImage;

    [SerializeField]
    GameObject Slide_1_FourPanelComic;

    [SerializeField]
    GameObject Slide_2_Description;

    /// <summary>
    /// 현재 진행 중인 듀토리얼 번호
    /// 0: 듀토리얼 미진행
    /// --- 1번 패널 ---
    /// 1: 4컷 만화
    /// 
    /// --- 2번 패널---
    /// 2: 어서오게.
    /// 3: 부임한지 얼마 안되어 정신이 없겠지만,
    /// 4: 자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세!
    /// 5: 손가락을 끌어 대포의 방향을 조준하고,
    /// 
    /// 6: 매 턴이 시작되면
    /// 7: 각 카드는 희귀, 영웅, 전설 등급으로 나뉘며,
    /// 8: 한번 선택하면 되돌릴 수 없으니,
    /// 
    /// 9: 이번에는 속성에 대해 알려주겠네.
    /// 10: 공과 몬스터는
    /// 11: 속성마다 서로 강하고 약한 관계가 있으니,
    /// 12: 상대의 속성에 유리한 공을 사용한다면
    /// 
    /// 13 : 몬스터 중에는 특별한 능력을 가진 녀석들도 있다네.
    /// 14: 우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지.
    /// 15: 특히, 매 10턴마다 강력한 보스 몬스터가 등장하니
    /// 16: 각 몬스터의 특징을 잘 살피고,
    /// 
    /// 17: 마지막으로, 꼭 기억해야 할 것이 있네.
    /// 18: 몬스터가 가장 아래 줄까지 내려오면
    /// 19: 그러니 몬스터 머리 위의 체력을 잘 살피게.
    /// 20: 한 놈도 아래까지 내려오지 못하게 막아야 하네!
    /// </summary>
    public int currentTutorialNumber = 0;

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    [SerializeField]
    GameStartButton gameStartButton;

    private void Awake()
    {
        previousButton.onClick.AddListener(Pevious);
        startButton.onClick.AddListener(StartGame);
        nextButton.onClick.AddListener(Next);

        Slide_1_FourPanelComic.gameObject.SetActive(true);
        Slide_2_Description.gameObject.SetActive(false);
    }

    /// <summary>
    /// 이전 버튼
    /// </summary>
    private void Pevious()
    {
        switch (currentTutorialNumber)
        {
            case 0:
                break;

            case 1:
                // 4컷 만화에서 이전 키를 누르면 작동X
                break;

            case 2:
                // 어서오게. 에서 이전 버튼을 누르면 1번 패널 오픈
                currentTutorialNumber--;
                Slide_2_Description.gameObject.SetActive(false);
                Slide_1_FourPanelComic.gameObject.SetActive(true);
                break;

            case 3:
                // 부임한지 얼마 안되어 정신이 없겠지만, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "어서 오게.\r\n자네가 이번에 새로 부임한 장교인가?";
                kingImage.sprite = kings[0];
                break;

            case 4:
                // 자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세! 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "부임한지 얼마 안되어 정신이 없겠지만,\r\n상황이 급박하니 짧게 설명해주겠네.";
                kingImage.sprite = kings[0];
                break;

            case 5:
                // 손가락을 끌어 대포의 방향을 조준하고, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세!";
                kingImage.sprite = kings[1];
                break;

            case 6:
                // 매 턴이 시작되면 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "손가락을 끌어 대포의 방향을 조준하고,\r\n손을 놓으면 공이 발사된다네.";
                kingImage.sprite = kings[1];
                break;

            case 7:
                // 각 카드는 희귀, 영웅, 전설 등급으로 나뉘며, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "매 턴이 시작되면\r\n세 장의 카드 중 하나를 선택할 수 있다네.";
                kingImage.sprite = kings[2];
                break;

            case 8:
                // 한번 선택하면 되돌릴 수 없으니, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "각 카드는 희귀, 영웅, 전설 등급으로 나뉘며,\r\n각기 다른 효과를 가지고 있지.\r\n";
                kingImage.sprite = kings[2];
                break;

            case 9:
                // 이번에는 속성에 대해 알려주겠네. 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "한번 선택하면 되돌릴 수 없으니,\r\n신중하게 선택하게나.";
                kingImage.sprite = kings[2];
                break;

            case 10:
                // 공과 몬스터는 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "이번에는 속성에 대해 알려주겠네.";
                kingImage.sprite = kings[3];
                break;

            case 11:
                // 속성마다 서로 강하고 약한 관계가 있으니, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "공과 몬스터는\r\n노말, 불, 물, 흙, 번개, 바람의 속성을 가지고 있다네.\r\n";
                kingImage.sprite = kings[4];
                break;

            case 12:
                // 상대의 속성에 유리한 공을 사용한다면 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "각 속성마다 서로 강하고 약한 관계가 있으니,\r\n상성을 잘 파악하는 것이 중요하다네.";
                kingImage.sprite = kings[4];
                break;

            case 13:
                // 몬스터 중에는 특별한 능력을 가진 녀석들도 있다네. 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "상대의 속성에 유리한 공을 사용한다면\r\n더 큰 피해를 줄 수 있을 걸세.";
                kingImage.sprite = kings[4];
                break;


            case 14:
                // 우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지. 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "몬스터 중에는 특별한 능력을 가진 녀석들도 있다네.";
                kingImage.sprite = kings[5];
                break;

            case 15:
                // 각 몬스터의 특징을 잘 살피고, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지.";
                kingImage.sprite = kings[5];
                break;

            case 16:
                // 각 몬스터의 특징을 잘 살피고, 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "특히, 매 10턴마다 강력한 보스 몬스터가 등장하니\r\n항상 주의하도록 하게.";
                kingImage.sprite = kings[6];
                break;
            
            case 17:
                // 마지막으로, 꼭 기억해야 할 것이 있다네. 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "각 몬스터의 특징을 잘 살피고,\r\n상황에 맞게 상대하도록 하게.";
                kingImage.sprite = kings[6];
                break;

            case 18:
                // 몬스터가 가장 아래 줄까지 내려오면 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "마지막으로, 꼭 기억해야 할 것이 있다네.";
                kingImage.sprite = kings[0];
                break;
            
            case 19:
                // 그러니 몬스터 머리 위의 체력을 잘 살피게. 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "몬스터가 가장 아래 줄까지 내려오면\r\n우리 왕국은 패배하게 된다네.";
                kingImage.sprite = kings[0];
                break;
            
            case 20:
                // 한 놈도 아래까지 내려오지 못하게 막아야 하네! 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "그러니 몬스터 머리 위의 체력을 잘 살피게. 체력이 모두 소진되면\r\n몬스터를 쓰러뜨릴 수 있을 걸세.";
                kingImage.sprite = kings[0];
                break;
            
            case 21:
                // ~~~ 에서 이전 버튼을 누르면
                currentTutorialNumber--;
                speechBalloonText.text = "한 놈도 아래까지 내려오지 못하게 막아야 하네!\r\n자, 이제 출정하게! 왕국의 운명이 자네 손에 달렸네!";
                kingImage.sprite = kings[0];
                break;
        }
    }

    private void StartGame()
    {
        gameStartButton.OnGameStartButton();
    }

    /// <summary>
    /// 다음 버튼
    /// </summary>
    private void Next()
    {
        switch (currentTutorialNumber)
        {
            case 0:
                break;

            case 1:
                // 4컷 만화에서 다음 버튼을 누르면
                // 어서오게. 로 이동
                currentTutorialNumber++;
                Slide_1_FourPanelComic.gameObject.SetActive(false);
                Slide_2_Description.gameObject.SetActive(true);
                speechBalloonText.text = "어서 오게.\r\n자네가 이번에 새로 부임한 장교인가?";
                kingImage.sprite = kings[0];
                break;

            case 2:
                // 어서오게. 에서 다음 버튼을 누르면
                // 부임한지 얼마 안되어 정신이 없겠지만,
                currentTutorialNumber++;
                speechBalloonText.text = "부임한지 얼마 안되어 정신이 없겠지만,\r\n상황이 급박하니 짧게 설명해주겠네.";
                kingImage.sprite = kings[0];
                break;

            case 3:
                // 부임한지 얼마 안되어 정신이 없겠지만, 에서 다음 버튼을 누르면
                // 자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세!
                currentTutorialNumber++;
                speechBalloonText.text = "자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세!";
                kingImage.sprite = kings[1];
                break;

            case 4:
                // 자, 이것이 우리 왕국의 믿음직한 자랑, 대포일세! 에서 다음 버튼을 누르면
                // 손가락을 끌어 대포의 방향을 조준하고,
                currentTutorialNumber++;
                speechBalloonText.text = "손가락을 끌어 대포의 방향을 조준하고,\r\n손을 놓으면 공이 발사된다네.";
                kingImage.sprite = kings[1];
                break;

            case 5:
                // 손가락을 끌어 대포의 방향을 조준하고, 에서 다음 버튼을 누르면
                // 매 턴이 시작되면
                currentTutorialNumber++;
                speechBalloonText.text = "매 턴이 시작되면\r\n세 장의 카드 중 하나를 선택할 수 있다네.";
                kingImage.sprite = kings[2];
                break;

            case 6:
                // 매 턴이 시작되면 에서 다음 버튼을 누르면
                // 각 카드는 희귀, 영웅, 전설 등급으로 나뉘며,
                currentTutorialNumber++;
                speechBalloonText.text = "각 카드는 희귀, 영웅, 전설 등급으로 나뉘며,\r\n각기 다른 효과를 가지고 있지.";
                kingImage.sprite = kings[2];
                break;

            case 7:
                // 각 카드는 희귀, 영웅, 전설 등급으로 나뉘며, 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "한번 선택하면 되돌릴 수 없으니,\r\n신중하게 선택하게나.";
                kingImage.sprite = kings[2];
                break;

            case 8:
                // 한번 선택하면 되돌릴 수 없으니, 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "이번에는 속성에 대해 알려주겠네.";
                kingImage.sprite = kings[3];
                break;

            case 9:
                // 이번에는 속성에 대해 알려주겠네. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "공과 몬스터는\r\n노말, 불, 물, 흙, 번개, 바람의 속성을 가지고 있다네.";
                kingImage.sprite = kings[4];
                break;

            case 10:
                // 공과 몬스터는, 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "각 속성마다 서로 강하고 약한 관계가 있으니,\r\n상성을 잘 파악하는 것이 중요하다네.";
                kingImage.sprite = kings[4];
                break;

            case 11:
                // 속성마다 서로 강하고 약한 관계가 있으니, 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "상대의 속성에 유리한 공을 사용한다면\r\n더 큰 피해를 줄 수 있을 걸세.";
                kingImage.sprite = kings[4];
                break;

            case 12:
                // 상대의 속성에 유리한 공을 사용한다면 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "몬스터 중에는 특별한 능력을 가진 녀석들도 있다네.";
                kingImage.sprite = kings[5];
                break;

            case 13:
                // 몬스터 중에는 특별한 능력을 가진 녀석들도 있다네. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지.";
                kingImage.sprite = kings[5];
                break;

            case 14:
                // 우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "특히, 매 10턴마다 강력한 보스 몬스터가 등장하니\r\n각별히 주의하게.";
                kingImage.sprite = kings[6];
                break;

            case 15:
                // 우린 그들을 기믹 몬스터와 보스 몬스터라고 부르지. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "각 몬스터의 특징을 잘 살피고,\r\n상황에 맞게 상대하도록 하게.";
                kingImage.sprite = kings[6];
                break;

            case 16:
                // 각 몬스터의 특징을 잘 살피고, 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "마지막으로, 꼭 기억해야 할 것이 있다네.";
                kingImage.sprite = kings[0];
                break;
            
            case 17:
                // 마지막으로, 꼭 기억해야 할 것이 있다네. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "몬스터가 가장 아래 줄까지 내려오면\r\n우리 왕국은 패배하게 된다네.";
                kingImage.sprite = kings[0];
                break;
            
            case 18:
                // 몬스터가 가장 아래 줄까지 내려오면 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "그러니 몬스터 머리 위의 체력을 잘 살피게. 체력이 모두 소진되면\r\n몬스터를 쓰러뜨릴 수 있을 걸세.";
                kingImage.sprite = kings[0];
                break;
            
            case 19:
                // 그러니 몬스터 머리 위의 체력을 잘 살피게. 에서 다음 버튼을 누르면
                currentTutorialNumber++;
                speechBalloonText.text = "한 놈도 아래까지 내려오지 못하게 막아야 하네!\r\n자, 이제 출정하게! 왕국의 운명이 자네 손에 달렸네!";
                kingImage.sprite = kings[0];
                break;

        }
    }
}
