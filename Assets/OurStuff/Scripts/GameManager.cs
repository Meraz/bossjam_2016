using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Transform startPosition;

    public Transform[] spawnPoints;

    public float offsetSpawnX = 5;
    public Transform player1obj;
    public Transform player2obj;
    public Transform cameraObj;
    public Transform thirdWheelGuyobj;
    public GameObject SpawnEffect;
    float m_playerRespawnTimer = 1.0f;

    private Transform[] players = new Transform[2];
    private Transform mainCamera;
    private Transform thirdWheelGuy;

    public GameObject UIMenu;
	// Use this for initialization
	void Start () {
        UIMenu.SetActive(false);

        StartGame();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToogleMenu();
        }
    }

    public void StartGame()
    {
        thirdWheelGuy = (Instantiate(thirdWheelGuyobj.gameObject, startPosition.position, startPosition.rotation) as GameObject).transform; //viktigt han är först
        players[0] = (Instantiate(player1obj.gameObject, startPosition.position + new Vector3(offsetSpawnX, 0, 0), startPosition.rotation) as GameObject).transform;
        players[1] = (Instantiate(player2obj.gameObject, startPosition.position + new Vector3(-offsetSpawnX, 0, 0), startPosition.rotation) as GameObject).transform;
        
        mainCamera = (Instantiate(cameraObj.gameObject, new Vector3(1.0f, 5.0f, 0.0f), Quaternion.identity) as GameObject).transform;

        RespawnAll(startPosition.position);

    }

    public void RespawnPlayer(int playerNumber)
    {
        if(players[playerNumber - 1].gameObject.activeSelf == true)
        {
            StartCoroutine(OnPlayerDeath(playerNumber));
        }
    }

    IEnumerator OnPlayerDeath(int playerNumber)
    {
        players[playerNumber - 1].gameObject.SetActive(false);
        yield return new WaitForSeconds(m_playerRespawnTimer);
        players[playerNumber - 1].gameObject.SetActive(true);

        // Reset velocity
        players[playerNumber - 1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        players[playerNumber - 1].position = thirdWheelGuy.position + new Vector3(0, offsetSpawnX, 0);


        GameObject tempEffect;
        tempEffect = Instantiate(SpawnEffect.gameObject, players[playerNumber - 1].position, this.transform.rotation) as GameObject;
        Destroy(tempEffect, 4.0f);

        players[playerNumber - 1].gameObject.SetActive(true);
    }

    public void RespawnAll(Vector3 position)
    {
        players[0].gameObject.SetActive(true);
        players[1].gameObject.SetActive(true);
        thirdWheelGuy.gameObject.SetActive(true);

        players[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        players[1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        thirdWheelGuy.GetComponent<Rigidbody>().velocity = Vector3.zero;

        Vector3 pos1 = position + new Vector3(offsetSpawnX, 0, 0);
        Vector3 pos2 = position + new Vector3(-offsetSpawnX, 0, 0);

        GameObject tempEffect;
        tempEffect = Instantiate(SpawnEffect.gameObject, pos1, this.transform.rotation) as GameObject;
        Destroy(tempEffect, 4.0f);
        tempEffect = Instantiate(SpawnEffect.gameObject, pos2, this.transform.rotation) as GameObject;
        Destroy(tempEffect, 4.0f);
        tempEffect = Instantiate(SpawnEffect.gameObject, position, this.transform.rotation) as GameObject;
        Destroy(tempEffect, 4.0f);

        players[0].position = pos1;
        players[1].position = pos2;
        thirdWheelGuy.position = position;
    }

    public void Won()
    {
        Debug.Log("Won");
        ToMainMenu();
    }

    public void Lost()
    {
        float x = thirdWheelGuy.transform.position.x;
        //Debug.Log(x);
        Transform activeSpawnPoint = startPosition;
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if(spawnPoints[i].position.x < x)
            {
                activeSpawnPoint = spawnPoints[i];
            }
        }
        RespawnAll(activeSpawnPoint.position);

        //if(x >= 90 && x < 170)
        //{
        //    RespawnAll(new Vector3(90,7,0));
        //}
        //else if(x >= 170 && x < 310)
        //{
        //    RespawnAll(new Vector3(170, 11, 0));

        //}
        //else if (x >= 310)
        //{
        //    RespawnAll(new Vector3(310, 11, 0));
        //}
        //else
        //{
        //    RespawnAll(startPosition.position);
        //}

        //Debug.Log("Lost the game");
        //ToMainMenu();
    }

    public void ToogleMenu()
    { 
        UIMenu.SetActive(!UIMenu.activeSelf);
        if(Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
