using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject CurrentCube;
    public GameObject LastCube;
    public Text Text;
    public int level;
    public bool Done;
    public GameObject Camera1;
    public GameObject Camera2;

    public GameObject FX;

    public GameObject PauseMenu;
    public GameObject GamePanel;


    private int blockSize = 150;


    public Text HighScore, CurScore;


    public GameObject GameOver;
    public Text GameOverHigh;
    public Text GameOverScore;

    // Start is called before the first frame update
    void Start()
    {
        newBlock();
    }

    void newBlock()
    {
        if(LastCube!=null)
        {
            CurrentCube.transform.position = new Vector3(Mathf.Round(CurrentCube.transform.position.x),
                CurrentCube.transform.position.y,
                Mathf.Round(CurrentCube.transform.position.z)
            );
            CurrentCube.transform.localScale = new Vector3(LastCube.transform.localScale.x - Mathf.Abs(CurrentCube.transform.position.x - LastCube.transform.position.x),
                                                            LastCube.transform.localScale.y,
                                                            LastCube.transform.localScale.z - Mathf.Abs(CurrentCube.transform.position.z - LastCube.transform.position.z)

                );
            CurrentCube.transform.position = Vector3.Lerp(CurrentCube.transform.position,LastCube.transform.position,0.5f)+Vector3.up*5f;
            if(CurrentCube.transform.localScale.x <= 0f ||
                CurrentCube.transform.localScale.z <= 0f)
            {
                Camera1.SetActive(false);
                Camera2.SetActive(true);
                Camera2.transform.position -= new Vector3(0, -level*1.5f, level*2);


                Destroy(CurrentCube);
                Done = true;
                Text.gameObject.SetActive(true);
                Text.text = "Your Score: " + level;


                GameOver.SetActive(true);
                PauseMenu.SetActive(false);
                GamePanel.SetActive(false);
                GameOverScore.text = "Score: " + level;
                GameOverHigh.text = "High Score: " +PlayerPrefs.GetInt("highscore",0);


                return;

            }
        }
        LastCube = CurrentCube;
        CurrentCube = Instantiate(LastCube);
        CurrentCube.name = level + "";
        CurrentCube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((level / 100f) % 1f, 1f, 1f));
        level++;

        setHigh();

        Text.text = level + "";
        Camera.main.transform.position = CurrentCube.transform.position + new Vector3(blockSize, blockSize, blockSize) * 1.5f -new Vector3(level,level,level)*0.2f ;
        Camera.main.transform.LookAt(CurrentCube.transform.position);


    }
    void setHigh()
    {
        int temp = PlayerPrefs.GetInt("highscore", 0);
        if (temp <= level)
            PlayerPrefs.SetInt("highscore", level);

    }

    public void Restart()
    {
        SceneManager.LoadScene("game");
    }

    // Update is called once per frame
    void Update()
    {
        if (Done)
            return;

        var time = Mathf.Abs(Time.realtimeSinceStartup % 2f - 1f);

        var pos1 = LastCube.transform.position + Vector3.up * 10f;
        var pos2 = pos1 + ((level % 2 == 0) ? Vector3.left : Vector3.forward)*120;

        if (level % 2 == 0)
            CurrentCube.transform.position = Vector3.LerpUnclamped(pos2, pos1, time);
        else
            CurrentCube.transform.position = Vector3.LerpUnclamped(pos1, pos2, time);

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.currentSelectedGameObject )
        {
            GameObject obj=Instantiate(FX, LastCube.transform.position,Quaternion.Euler(-90,0,0));
            Destroy(obj, 0.9f);
            newBlock();
        }

    }



    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        GamePanel.SetActive(false);
        CurScore.text ="Score: " + level + "";

        int temp = PlayerPrefs.GetInt("highscore", 0);
        if (temp <= level)
            HighScore.text ="High Score: " +  level + "";
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        GamePanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }

}
