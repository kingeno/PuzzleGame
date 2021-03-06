﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyTile : MonoBehaviour
{
    private GameObject boardManager;
    private TileSelectionSquare tileSelectionSquare;

    private Vector3 above_AdjacentPos;
    private Transform[,,] tilesBoard;
    private Transform above_AdjacentTile;

    private Renderer _renderer;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    private bool disappearingAnimation_isFinished;

    [Header("Textures")]
    public Texture greyTileTexture;

    private float initialVerticalPos;
    private float lastVerticalPos;
    private float appearingDelay_RelativeToPos;
    private float disappearingDelay_RelativeToPos;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");

        initialVerticalPos = transform.position.y;
        appearingDelay_RelativeToPos = 0.1f + (initialVerticalPos * GameManager._timeBetweenWaves);
        StartCoroutine(AppearingAnimation(GameManager._startingOffset, GameManager._duration, appearingDelay_RelativeToPos, appearingDelay_RelativeToPos + GameManager._appearingWaveDuration, GameManager._timeToWaitBeforeFirstInitialization));
    }

    private void Update()
    {
        if (GameManager.simulationIsRunning || Input.GetMouseButtonDown(1) || MainCamera.isFreeLookActive)
        {
            _renderer.material.SetTexture("_MainTex", greyTileTexture);
        }

        if (InGameUIManager.nextLevelIsLoading && !disappearingAnimation_isFinished)
        {
            lastVerticalPos = transform.position.y;
            disappearingDelay_RelativeToPos = lastVerticalPos * GameManager._timeBetweenWaves;
            StartCoroutine(DisappearingAnimation(GameManager._dEndingOffset, GameManager._dDuration, disappearingDelay_RelativeToPos, disappearingDelay_RelativeToPos + GameManager._disappearingWaveDuration));
            disappearingAnimation_isFinished = true;
        }
    }

    private void OnMouseEnter()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
                tileSelectionSquare.transform.rotation = transform.rotation;
                tileSelectionSquare.material.color = tileSelectionSquare.onGreyTileColor;
            }
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;
            if (tileSelectionSquare.transform.position != transform.position)
                tileSelectionSquare.transform.position = transform.position;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                AudioManager.instance.Play("ig tile grey hovering");

        }
    }

    private void OnMouseExit()
    {
        if (!GameManager.simulationIsRunning && tileSelectionSquare.canBeMoved)
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;

        _renderer.material.SetTexture("_MainTex", greyTileTexture);
    }

    public Transform TileCheck(Vector3 tilePos)
    {
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
            return null;
    }

    public void FirstInitialization()
    {
        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
        above_AdjacentTile = TileCheck(above_AdjacentPos);
    }

    IEnumerator AppearingAnimation(float startingOffset, float duration, float minDelay, float maxDelay, float timeToWaitBeforeFirstInitialization)
    {
        float elapsedTime = 0;

        Vector3 endPos = transform.position;

        transform.position = new Vector3(transform.position.x, transform.position.y + startingOffset, transform.position.z);
        Vector3 startingPos = transform.position;

        _renderer.material.color = transparantColor;

        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        while (elapsedTime < duration)
        {
            _renderer.material.color = Color.Lerp(transparantColor, opaqueColor, (elapsedTime / duration));
            float newYPos = EasingFunction.EaseOutCirc(startingPos.y, endPos.y, (elapsedTime / duration));
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _renderer.material.color = opaqueColor;
        transform.position = endPos;
        yield return new WaitForSecondsRealtime(timeToWaitBeforeFirstInitialization);
        FirstInitialization();
    }

    IEnumerator DisappearingAnimation(float endingOffset, float duration, float minDelay, float maxDelay)
    {
        float elapsedTime = 0;

        Vector3 startingPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + endingOffset, transform.position.z);

        _renderer.material.color = opaqueColor;

        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        while (elapsedTime < duration)
        {
            _renderer.material.color = Color.Lerp(opaqueColor, transparantColor, (elapsedTime / duration));
            float newYPos = EasingFunction.EaseInCirc(startingPos.y, endPos.y, (elapsedTime / duration));
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _renderer.material.color = transparantColor;
        transform.position = endPos;
    }
}
