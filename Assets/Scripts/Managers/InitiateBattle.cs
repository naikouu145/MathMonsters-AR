using UnityEngine;

public class InitiateBattle : MonoBehaviour
{
    public Camera arCamera;  // assign ARCamera
    public GameManager gameManager;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Monster"))
                {
                    MonsterController monster = hit.collider.GetComponent<MonsterController>();
                    if (monster != null)
                    {
                        gameManager.StartBattle(monster);
                    }
                }
            }
        }
    }
}
