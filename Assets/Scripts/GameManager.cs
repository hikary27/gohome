using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 게임 오버 상태를 표현하고, 게임 점수와 UI를 관리하는 게임 매니저
// 씬에는 단 하나의 게임 매니저만 존재할 수 있다.
public class GameManager : MonoBehaviour {
    public enum GAME_STATE
    {
        STATE_START,
        STATE_GAME_PLAY,
        STATE_GAME_OVER,
        STATE_RANK,
        STATE_NONE,
    }
    public static GameManager instance; // 싱글톤을 할당할 전역 변수

    public Text scoreText;          // 점수를 출력할 UI 텍스트
    public GameObject gameoverUI;   // 게임 오버시 활성화 할 UI 게임 오브젝트
    public GameObject cover;        // 시작화면
    public GameObject backGround;   // 게임배경
    public GameObject canvas;       // 게임텍스트
    public GameObject bestScoreObject;    // 최고점수
    public Text bestScoreText;

    private int score = 0;           // 게임 점수
    private static GAME_STATE state = GAME_STATE.STATE_START;
    private GAME_STATE oldState = GAME_STATE.STATE_NONE;

    const int SCORE_DISPLAY_MAX = 3;
    private static int[] bestScore = new int[SCORE_DISPLAY_MAX];

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("게임 매니저 생성 오류!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //STATE 변경되었을때 처리
        if (state != oldState)
        {
            onSetState(state);
        }

        if (Input.GetMouseButtonDown(0))
            onMouseInput();

        //if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                onEsc();
            }
        }
    }

    public void onSetState(GAME_STATE state)
    {
        Debug.Log("상태 :" + state);

        switch (state)
        {
            case GAME_STATE.STATE_START:
                LoadData();

                gameoverUI.SetActive(false);
                backGround.SetActive(false);
                canvas.SetActive(false);
                bestScoreObject.SetActive(false);
                cover.SetActive(true);
                break;
            case GAME_STATE.STATE_GAME_PLAY:
                gameoverUI.SetActive(false);
                backGround.SetActive(true);
                canvas.SetActive(true);
                bestScoreObject.SetActive(false);
                cover.SetActive(false);
                break;
            case GAME_STATE.STATE_RANK:          
                gameoverUI.SetActive(false);
                backGround.SetActive(false);
                canvas.SetActive(false);
                bestScoreObject.SetActive(true);
                cover.SetActive(false);

                bestScoreText.text = "Best Score" + "\n";
                if (bestScore[0] > 0)
                {
                    bestScoreText.text = "High Score : " + bestScore[0] + "\n";
                }
                else
                    bestScoreText.text = "Not Played" + "\n";

                break;
            case GAME_STATE.STATE_GAME_OVER:
                gameoverUI.SetActive(true);
                backGround.SetActive(true);
                canvas.SetActive(true);
                bestScoreObject.SetActive(false);
                cover.SetActive(false);
                break;
        }

        oldState = state;
    }

    public void onMouseInput()
    {
        //대기화면에서 게임 시작
        if (state == GAME_STATE.STATE_START)
        {
            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            Vector2 mousePoint = Input.mousePosition;
            mousePoint = camera.ScreenToWorldPoint(mousePoint);

            Debug.Log(mousePoint.x + " " + mousePoint.y);
            //랭크 클릭
            if (mousePoint.y >= 2.5 &&
                mousePoint.y <= 4.5 &&
                mousePoint.x >= 7.5 &&
                mousePoint.x <= 10.0)
            {
                state = GAME_STATE.STATE_RANK;
            }
            //종료 클릭
            else if (mousePoint.y >= -4.7 &&
                mousePoint.y <= 3.4 &&
                mousePoint.x >= 7.6 &&
                mousePoint.x <= 9.0)
            {
                Application.Quit();
            }
            else
            {
                state = GAME_STATE.STATE_GAME_PLAY;
            }
        }
        //랭킹에서 메인화면 복귀
        else if (state == GAME_STATE.STATE_RANK)
        {
            state = GAME_STATE.STATE_START;
        }
        //게임오버에서 게임 시작
        else if (state == GAME_STATE.STATE_GAME_OVER)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            state = GAME_STATE.STATE_GAME_PLAY;
        }
    }

    public void onEsc()
    {
        switch(state)
        {
            case GAME_STATE.STATE_GAME_PLAY:
            case GAME_STATE.STATE_RANK:
            case GAME_STATE.STATE_GAME_OVER:
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    state = GAME_STATE.STATE_START;
                }
                break;
        }
    }

    //점수 증가
    public void AddScore(int newScore)
    {
        if (!isGameover())
        {
            score += newScore;
            scoreText.text = "Score : " + score;
        }
    }

    //현재 점수를 리턴
    public int getScore()
    {
        return score;
    }

    //캐릭터 사망
    public void OnPlayerDead()
    {
        state = GAME_STATE.STATE_GAME_OVER;

        //최고 점수 갱신
        for (int i = 0; i < SCORE_DISPLAY_MAX; i++)
        {   
            if (bestScore[i] < score)
            {
                for (int j = SCORE_DISPLAY_MAX - 1; j > i; j--)
                    bestScore[j] = bestScore[j - 1];

                bestScore[i] = score;
                break;
            }
        }
    }

    public bool isGameover()
    {
        return state != GAME_STATE.STATE_GAME_PLAY; 
    }

    public void LoadData()
    {
        Debug.Log("데이터를 로드합니다");
        if (PlayerPrefs.HasKey("best score"))
        {
            Debug.Log("성공");
            string[] dataArr = PlayerPrefs.GetString("best score").Split(',');

            for (int i = 0; i < SCORE_DISPLAY_MAX; i++)
            {
                if (dataArr.Length > i)
                    bestScore[i] = System.Convert.ToInt32(dataArr[i]);
                else
                    bestScore[i] = 0;

                Debug.Log("최고점수" + (i + 1) + "번째 : " + bestScore[i]);
            }
        }
    }

    public void SaveData()
    {
        string strArr = "";
        Debug.Log("데이터를 저장합니다");

        for (int i = 0; i < SCORE_DISPLAY_MAX; i++)
        {
            strArr = strArr + bestScore[i];
            if (i != SCORE_DISPLAY_MAX - 1)
                strArr += ",";
        }

        PlayerPrefs.SetString("best score", strArr);
        PlayerPrefs.Save();
    }
}