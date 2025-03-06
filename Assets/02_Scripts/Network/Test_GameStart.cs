using UnityEngine;

public class Test_GameStart : MonoBehaviour
{
    GameReadyNetworkManager gameReadyNetworkManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameReadyNetworkManager = GetComponent<GameReadyNetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            gameReadyNetworkManager.GameStart();
        }
    }
}
