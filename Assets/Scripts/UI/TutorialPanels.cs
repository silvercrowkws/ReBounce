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
    /// </summary>
    public int currentTutorialNumber = 0;

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
                speechBalloonText.text = "한번 선택하면 되돌릴 수 없으니,\r\n신중하게 고르게나.";
                kingImage.sprite = kings[2];
                break;
        }
    }

    private void StartGame()
    {

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
                speechBalloonText.text = "한번 선택하면 되돌릴 수 없으니,\r\n신중하게 고르게나.";
                kingImage.sprite = kings[2];
                break;
        }
    }
}
