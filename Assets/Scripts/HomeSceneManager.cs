using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HomeSceneManager : MonoBehaviour
{
    [Header("===========Main Menu===========")]
    public GameObject MainMenuCanvas;
    public Button NewGameButton;
    public Button LevelSelectButton;
    public Button QuitButton;
    
    [Header("===========Level Select Menu===========")]
    public GameObject LevelSelectCanvas;
    public GameObject LevelCanvas;
    public string[] LevelNames;
    public Button LevelButtonPrefab;
    public Button ReturnButton;

    [Header("===========Scene Names===========")]
    public string NewGameScene;

    private string levelName;
    // Start is called before the first frame update
    void Start()
    {
        //Set up the button listeners
        if(NewGameButton)
            NewGameButton.onClick.AddListener(StartNewGame);
        if(LevelSelectButton)
            LevelSelectButton.onClick.AddListener(SelectLevel);
        if(ReturnButton)
            ReturnButton.onClick.AddListener(ShowMainMenu);
        if(QuitButton)
            QuitButton.onClick.AddListener(EndGame);
        if (!MainMenuCanvas)
            Debug.Log("Drag your MainMenuCanvas to the GameManager!");
        if(!LevelSelectCanvas)
            Debug.Log("Drag your LevelSelectCanvas to the GameManager!");

        //Set up the levels
        SetLevels();

        //Display proper menu

        ShowMainMenu();
    }
    void EndGame()
    {
        //This will be ignored in Unity Editor Play Mode. It only works in Build version
        Application.Quit();
    }
    void StartNewGame()
    {
        if (NewGameScene != null)
            SceneManager.LoadScene(NewGameScene);
        else
            Debug.Log("Please enter the new game scene name in the Game Manager!");
    }
    void LoadLevel()
    {
        //This method is invoked by a button click, so the currentSelectedGameObject is the button
        //Get the button name from the event system
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        //get level name from the buttonName. Previously the button name was generated as levelName + "Button"
        string levelName = buttonName.Substring(0, buttonName.Length - 6);
        Debug.Log("Level to load: " + levelName);
        SceneManager.LoadScene(levelName);
    }
    void ShowMainMenu()
    {
        LevelSelectCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
    void SelectLevel()
    {
        //Switch canvas
        MainMenuCanvas.SetActive(false);
        LevelSelectCanvas.SetActive(true);
    }
    void SetLevels()
    {
        //Set up the level buttons based on the level names
        for (int i = 0; i < LevelNames.Length; i++)
        {
            levelName = LevelNames[i];
            //Create a button from the button template LevelButtonPrefab
            Button levelButton = Instantiate(LevelButtonPrefab, Vector3.zero, Quaternion.identity);
            //Give the button a unique name
            levelButton.name = levelName + "Button";
            //Set the label of the button
            if (levelButton.GetComponentInChildren<Text>())
            {
                Text levelButtonLabel = levelButton.GetComponentInChildren<Text>();
                levelButtonLabel.text = levelName;
            }
            else
                Debug.Log("Check your Level Button Prefab! It's missing a Text component!");

            //Set up the button listener
            Button levelButtonScript = levelButton.GetComponent<Button>();
            levelButtonScript.onClick.RemoveAllListeners();
            //levelButtonScript.onClick.AddListener(() => LoadLevel(levelName));
            levelButtonScript.onClick.AddListener(LoadLevel);
            //Debug.Log("Level name-" + levelName + "-added to button -" + levelButton.name);

            // set the parent of the button as the LevelSelectCanvas so it will be dynamically arranged based on the defined layout
            levelButton.transform.SetParent(LevelCanvas.transform, false);



            // You can even set the button interactivity based on whether the level has been played thru or not
        }
    }
}