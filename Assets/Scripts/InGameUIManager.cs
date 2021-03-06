﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    private GameManager gameManager;
    private LevelLoader levelLoader;
    private MainCamera _mainCamera;
    private GameObject cinemachineCamera;

    [Header("Level Completed Text")]
    public float levelCompletedText_displayDelay;
    public float levelCompletedText_distanceToTravel;
    public float levelCompletedText_timeOfTravel;
    public bool levelCompletedText_fadeIsDone;

    [Header("Next Level Button")]
    public float nextLevelButton_displayDelay;
    public float nextLevelButton_distanceToTravel;
    public float nextLevelButton_timeOfTravel;
    public bool nextLevelButton_fadeIsDone;

    [Header("Level Name")]
    public TextMeshProUGUI levelNameText;

    [Header("Tile Prefabs")]
    public Transform blankTilePrefab;
    public Transform greenArrowPrefab;

    [Header("General UI")]
    public GameObject inGameUI;
    public GameObject pauseMenu;
    public GameObject levelHub;
    public GameObject controlsScheme;
    public GameObject winScreen;

    [Header("Level Completed UI")]
    public GameObject levelCompletedText;
    public GameObject nextLevelButton;

    [Header("Button Outlines")]
    public GameObject deleteTileSelectedOutline;
    public GameObject greenArrowSelectedOutline;

    [Header("Delete Tile Button")]
    public Button deleteButtonScript;
    public Image deleteTileButtonImage;

    [Header("Arrow Tile Button")]
    public Button arrowButtonScript;
    public Image greenArrowButtonImage;
    public GreenArrowStockText greenArrowStockText;

    [Header("Slow Down Button")]
    public Button slowDownButtonScript;
    public Image slowDownButtonImage;

    [Header("Speed Up Button")]
    public Button speedUpButtonScript;
    public Image speedUpButtonImage;

    [Header("Play Simulation Button")]
    public GameObject playSimulationButton;
    public Button playButtonScript;
    public Image playButtonImage;

    [Header("Pause Simulation Button")]
    public GameObject pauseSimulationButton;
    public Button pauseButtonScript;
    public Image pauseButtonImage;

    [Header("Stop Simulation Button")]
    public Button stopButtonScript;
    public Image stopSimulationButtonImage;

    [Header("Pause Menu Button")]
    public Button pauseMenuButtonScript;
    public Image pauseMenuButtonImage;

    [Header("Camera Buttons")]
    public Button resetCameraPosButtonScript;
    public Image resetCameraPosButtonImage;

    public Image cameraControlButtonImage;

    [Header("Interactable Colors")]
    public Color notInteractableButtonColor;
    public Color interactableButtonColor;

    public static bool isDeleteTileSelected;
    public static bool isGreenArrowSelected;
    public static bool nothingIsSelected;

    public static bool changeSimulationSpeed;

    [Header("Scene Fade")]
    public GameObject imageToFade;
    public float fadeInDuration;
    public float fadeOutDuration;
    public bool isFadingOut;

    [HideInInspector] public bool isOverPlayerArrowTile;

    public static bool nextLevelIsLoading;

    private bool disappearingAnimation_isFinished;

    private bool levelCompletedText_fadeStarted = false;

    private void Start()
    {
        if (nextLevelIsLoading)
        {
            nextLevelIsLoading = false;
        }

        if (LevelLoader.previousScene == 0 || LevelLoader.loadedFromLevelHub)
        {
            imageToFade.SetActive(true);
            StartCoroutine(ScreenFade(imageToFade, fadeInDuration, 1f, 0f));
            LevelLoader.loadedFromLevelHub = false;
        }

        SetLevelNameText();

        GameManager.gameIsPaused = false;
        nothingIsSelected = true;
        isDeleteTileSelected = false;
        isGreenArrowSelected = false;
        changeSimulationSpeed = false;

        levelCompletedText_fadeIsDone = false;

        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (!_mainCamera)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();

        if (!levelLoader)
            levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();  //if you encounter a null reference exception here it means that you have launched the game without going through the main menu
    }

    private void Update()
    {
        if (LevelLoader.loadingFromLevelHub)
        {
            if (!imageToFade.activeSelf)
                imageToFade.SetActive(true);
            StartCoroutine(ScreenFade(imageToFade, fadeOutDuration, 0f, 1f));
            LevelLoader.loadingFromLevelHub = false;
        }

        if (!cinemachineCamera)
        {
            cinemachineCamera = GameObject.FindGameObjectWithTag("CMFreeLookCamera");
            if (cinemachineCamera)
                cinemachineCamera.SetActive(false);
        }

        if (!GameManager.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PauseMenu();

            if (MainCamera.isFreeLookActive)
            {
                ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                ToggleUIButton(playButtonScript, false, playButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseButtonScript, false, pauseButtonImage, notInteractableButtonColor);
                ToggleUIButton(slowDownButtonScript, false, slowDownButtonImage, notInteractableButtonColor);
                ToggleUIButton(speedUpButtonScript, false, speedUpButtonImage, notInteractableButtonColor);
                ToggleUIButton(stopButtonScript, false, stopSimulationButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseMenuButtonScript, false, pauseMenuButtonImage, notInteractableButtonColor);
                ToggleUIButton(resetCameraPosButtonScript, false, resetCameraPosButtonImage, notInteractableButtonColor);
            }
            else if (!MainCamera.isFreeLookActive)
            {
                ToggleUIButton(resetCameraPosButtonScript, true, resetCameraPosButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseButtonScript, true, pauseButtonImage, interactableButtonColor);
                ToggleUIButton(pauseMenuButtonScript, true, pauseMenuButtonImage, interactableButtonColor);
                ToggleUIButton(slowDownButtonScript, true, slowDownButtonImage, interactableButtonColor);
                ToggleUIButton(speedUpButtonScript, true, speedUpButtonImage, interactableButtonColor);


                if (gameManager.allEndTilesAreValidated)
                    ToggleUIButton(playButtonScript, false, playButtonImage, notInteractableButtonColor);
                else
                    ToggleUIButton(playButtonScript, true, playButtonImage, interactableButtonColor);


                if (CurrentLevelManager.isGreenArrowStockEmpty)
                {
                    isGreenArrowSelected = false;
                    greenArrowSelectedOutline.SetActive(false);
                    ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                }
                else
                {
                    ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, interactableButtonColor);
                }

                if (!CurrentLevelManager.greenArrowStockIsFull)
                {
                    if (Input.GetKeyDown(KeyCode.S) && !isDeleteTileSelected)
                        DeleteSelection();
                    ToggleUIButton(deleteButtonScript, true, deleteTileButtonImage, interactableButtonColor);
                }
                else if (CurrentLevelManager.greenArrowStockIsFull)
                {
                    isDeleteTileSelected = false;
                    deleteTileSelectedOutline.SetActive(false);
                    ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                }


                if (!GameManager.simulationHasBeenLaunched)
                    ToggleUIButton(stopButtonScript, false, stopSimulationButtonImage, notInteractableButtonColor);
                else if (GameManager.simulationHasBeenLaunched)
                {
                    ToggleUIButton(stopButtonScript, true, stopSimulationButtonImage, interactableButtonColor);
                    ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                    ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                }
            }

            if (!GameManager.simulationIsRunning && pauseSimulationButton.activeSelf)
            {
                playSimulationButton.SetActive(true);
                pauseSimulationButton.SetActive(false);
            }
            else if (GameManager.simulationIsRunning && GameManager.simulationHasBeenLaunched && playSimulationButton.activeSelf)
            {
                playSimulationButton.SetActive(false);
                pauseSimulationButton.SetActive(true);
            }

            if (GameManager.levelIsCompleted)
            {
                if (!winScreen.activeSelf)
                {
                    winScreen.SetActive(true);
                }
                else if (winScreen.activeSelf && !levelCompletedText_fadeStarted)
                {
                    AudioManager.instance.Play("ig level completed");
                    StartCoroutine(FadeInAndMoveText(levelCompletedText, levelCompletedText_displayDelay, levelCompletedText_timeOfTravel,
                    new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y),
                    new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y + levelCompletedText_distanceToTravel)));

                    StartCoroutine(FadeInAndMoveText(nextLevelButton, nextLevelButton_displayDelay, nextLevelButton_timeOfTravel,
                    new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y),
                    new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y + nextLevelButton_distanceToTravel)));

                    levelCompletedText_fadeStarted = true;
                }

                if (nextLevelIsLoading && !disappearingAnimation_isFinished)
                {
                    StartCoroutine(FadeOutAndMoveText(levelCompletedText, 0.1f,
                        new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y),
                        new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y)));

                    StartCoroutine(FadeOutAndMoveText(nextLevelButton, 0.1f,
                        new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y),
                        new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y)));

                    disappearingAnimation_isFinished = true;
                }
            }
        }

        else if (GameManager.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (controlsScheme.activeSelf)
                {
                    controlsScheme.SetActive(false);
                    pauseMenu.SetActive(true);
                }
                else if (levelHub.activeSelf)
                {
                    levelHub.SetActive(false);
                    pauseMenu.SetActive(true);
                }
                else if (pauseMenu.activeSelf)
                    ResumeGame();
            }
        }

    }

    public void ToggleUIButton(Button button, bool toggleOn, Image buttonImage, Color imageColor)
    {
        button.interactable = toggleOn;
        buttonImage.color = imageColor;
    }

    public void DeleteSelection()
    {
        if (!MainCamera.isFreeLookActive)
        {
            isDeleteTileSelected = true;
            deleteTileSelectedOutline.SetActive(true);
        }
    }



    public void SimulationButton()
    {
        if (!MainCamera.isFreeLookActive)
        {
            if (!GameManager.simulationIsRunning)
            {
                gameManager.LaunchSimulation();
                greenArrowButtonImage.color = notInteractableButtonColor;
                deleteTileButtonImage.color = notInteractableButtonColor;

                stopButtonScript.interactable = true;
                stopSimulationButtonImage.color = interactableButtonColor;

                AudioManager.instance.Play("ig simulation play");
            }
            else if (GameManager.simulationIsRunning)
            {
                gameManager.LaunchSimulation();
                AudioManager.instance.Play("ig simulation pause");
            }
        }
    }

    public void SetSimulationSpeed(float simulationSpeed)
    {
        if (GameManager.simulationSpeed < simulationSpeed)
            AudioManager.instance.Play("ig simulation speed up");
        else if (GameManager.simulationSpeed > simulationSpeed)
            AudioManager.instance.Play("ig simulation slow down");

        changeSimulationSpeed = true;
        GameManager.simulationSpeed = simulationSpeed;
        if (simulationSpeed == 1)
            gameManager.turnTime = 0.5f;
        else if (simulationSpeed == 2)
            gameManager.turnTime = 0.47f;
        if (simulationSpeed == 3)
            gameManager.turnTime = 0.44f;
    }

    public void StopSimulation()
    {
        if (!MainCamera.isFreeLookActive)
        {
            gameManager.StopSimulation();
            ToggleUIButton(playButtonScript, true, playButtonImage, interactableButtonColor);

            stopButtonScript.interactable = false;
            stopSimulationButtonImage.color = notInteractableButtonColor;

            greenArrowButtonImage.color = interactableButtonColor;
            deleteTileButtonImage.color = interactableButtonColor;

            AudioManager.instance.Play("ig simulation stop");
        }
    }

    public void ToggleFreeLook()
    {
        _mainCamera.FreeLook();
    }

    public void ResetCameraPosition()
    {
        _mainCamera.SetToStartPos();
    }

    public void PauseMenu()
    {
        if (!MainCamera.isFreeLookActive)
        {
            AudioManager.instance.Play("ig button click menu");
            AudioManager.instance.PauseMenuMusicCutoff();
            GameManager.gameIsPaused = true;
            GameManager.playerCanModifyBoard = false;
            Time.timeScale = 0f;
            inGameUI.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        AudioManager.instance.Play("menu button close");
        AudioManager.instance.ResumeMusicCutoff();
        pauseMenu.SetActive(false);
        GameManager.gameIsPaused = false;
        inGameUI.gameObject.SetActive(true);
        if (!GameManager.simulationIsRunning && !GameManager.simulationHasBeenLaunched)
            GameManager.playerCanModifyBoard = true;
    }

    public void DisplayControls()
    {
        pauseMenu.SetActive(false);
        controlsScheme.SetActive(true);
    }

    public void DisplayLevelHub()
    {
        pauseMenu.SetActive(false);
        levelHub.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        StartCoroutine(ScreenFade(imageToFade, fadeOutDuration, 0f, 1f));
        GameManager.simulationSpeed = Time.timeScale = 1f;
        levelLoader.LoadSpecificLevel(0);
        AudioManager.instance.ExitToMainMenuCrossFade();
        GameManager.levelIsCompleted = false;
    }

    public void BackButton()
    {
        if (controlsScheme.activeSelf)
        {
            controlsScheme.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else if (levelHub.activeSelf)
        {
            levelHub.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        if (!MainCamera.isFreeLookActive)
        {
            nextLevelIsLoading = true;
            levelLoader.LoadNextLevel();
        }
    }

    private void SetLevelNameText()
    {
        levelNameText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeInAndMoveText(GameObject target, float timeToWaitBeforeFade, float timeOfTravel, Vector2 startPos, Vector2 endPos)
    {
        yield return new WaitForSecondsRealtime(timeToWaitBeforeFade);

        float currentTime = 0f;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / timeOfTravel;

            target.GetComponent<CanvasGroup>().alpha = EasingFunction.EaseInOutSine(0f, 1f, normalizedValue);
            target.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, EasingFunction.EaseOutExpo(0f, 1f, normalizedValue));
            yield return null;
        }
    }

    IEnumerator FadeOutAndMoveText(GameObject target, float duration, Vector2 startPos, Vector2 endPos)
    {
        float currentTime = 0f;
        float normalizedValue;

        AudioManager.instance.Play("ig level tileboard disappearance");

        while (currentTime <= duration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / duration;

            target.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, normalizedValue);
            target.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, EasingFunction.EaseOutExpo(0f, 1f, normalizedValue));
            yield return null;
        }
        target.GetComponent<CanvasGroup>().alpha = 0f;
    }

    IEnumerator ScreenFade(GameObject target, float fadeDuration, float startFadeValue, float endFadeValue)
    {
        float currentTime = 0f;
        float normalizedValue;
        CanvasRenderer _cR;
        while (currentTime <= fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeDuration;
            _cR = target.GetComponent<CanvasRenderer>();
            _cR.SetAlpha(Mathf.Lerp(startFadeValue, endFadeValue, normalizedValue));

            yield return null;
        }
    }
}