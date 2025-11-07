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
                        // Make the monster face the camera
                        Vector3 directionToCamera = arCamera.transform.position - monster.transform.position;
                        directionToCamera.y = 0; // Keep rotation on horizontal plane only
                        
                        if (directionToCamera != Vector3.zero)
                        {
                            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                            monster.transform.rotation = targetRotation;
                        }
                        
                        gameManager.StartBattle(monster);
                    }
                }
            }
        }
    }
}
