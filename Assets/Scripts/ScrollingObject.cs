using UnityEngine;

// 게임 오브젝트를 계속 왼쪽으로 움직이는 스크립트
public class ScrollingObject : MonoBehaviour {
    public float speed = 10f; // 이동 속도
    public float plusSpeed = 0f;

    private void Update() {
        // 게임 오버가 아니라면
        if (!GameManager.instance.isGameover())
        {
            int score = GameManager.instance.getScore();
            plusSpeed = Mathf.Floor(score / 2) * .2f;       //10점 단위로 점점 빨라지게 한다.

            // 초당 speed의 속도로 왼쪽으로 평행 이동
            transform.Translate(Vector3.left * (speed + plusSpeed) * Time.deltaTime);
//          Debug.Log(plusSpeed * 10);
        }
    }
}