using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class IntroManager : MonoBehaviour
{
    public Camera mainCamera;
    public Transform cameraTransform;
    public Transform startPos;  
    public Transform tablePos;  
    public float cameraMoveTime = 3f;
    public float panDuration = 4f;

    public GameObject welcomeScreen;
    public GameObject mainMenu;
    public GameObject frontPanel;  
    public GameObject backPanel;
    public GameObject pausePanel;

    public float duration = 5f;
    public float fallDistance = 500f; 
    public float fallDuration = 1.0f;
    public PathType pathType = PathType.CatmullRom;
    public PathMode pathMode = PathMode.Full3D;
    public Ease fallEase = Ease.OutBounce;
    public bool canStart = false;

    public static IntroManager Instance { get; private set; }

    bool isFrontShowing = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        mainCamera.transform.position = startPos.position;
        
        mainCamera.transform.rotation = startPos.rotation;

        welcomeScreen.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnContinueClicked()
    {
        welcomeScreen.SetActive(false); 
        StartCoroutine(CameraMoveRoutine());
    }

    IEnumerator CameraMoveRoutine()
    {
        Vector3[] pathPoints = new Vector3[]
        {
            new Vector3(12.5f, 6.5f, -7.8f),
            new Vector3(12.43f, 6.5f, -2.01f),  
            
            //new Vector3(11.33f, 6.5f, -0.91f),
            new Vector3(10.79f, 6.5f, -0.6f)
        };

        cameraTransform.DOPath(pathPoints, duration, pathType, pathMode)
                       .SetEase(Ease.InOutSine)
                       .SetLookAt(0.01f)
                       .OnComplete(() =>
                       {
                           Quaternion targetRotation = Quaternion.LookRotation(tablePos.position - cameraTransform.position);
                           cameraTransform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.InOutSine);
                       });

        yield return new WaitForSeconds(duration + 1f + 0.5f);
        ShowMenu();
    }

    public void ShowMenu()
    {
        canStart = false;

        RectTransform panelRect = mainMenu.GetComponent<RectTransform>();
        Vector2 originalPos = panelRect.anchoredPosition;
        Vector2 startPos = originalPos + new Vector2(0, 400f);
        panelRect.anchoredPosition = startPos;

        mainMenu.SetActive(true);

        panelRect.DOLocalMoveY(originalPos.y, 2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, () => {
                panelRect.DOLocalMoveY(originalPos.y + 0.6f, 1.2f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(0.1f, () => {
                        panelRect.DOLocalMoveY(originalPos.y, 1f).SetEase(Ease.InOutSine);
                    });
                });
            });
        });
    }

    public void StartGame()
    {
        FlipMenu();
    }

    public void FlipMenu()
    {
        float halfDuration = 0.5f;

        RectTransform menuFrontRect = frontPanel.GetComponent<RectTransform>();
        RectTransform menuBackRect = backPanel.GetComponent<RectTransform>();

        if (isFrontShowing)
        {
            
            menuFrontRect.DORotate(new Vector3(0, 90, 0), halfDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                frontPanel.SetActive(false);
                backPanel.SetActive(true);

                menuBackRect.localRotation = Quaternion.Euler(0, 90, 0); 
                menuBackRect.DORotate(new Vector3(0, 0, 0), halfDuration).SetEase(Ease.InOutSine);

                isFrontShowing = false;
            });
        }
        else
        {
           
            menuBackRect.DORotate(new Vector3(0, 90, 0), halfDuration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                backPanel.SetActive(false);
                frontPanel.SetActive(true);

                menuFrontRect.localRotation = Quaternion.Euler(0, 90, 0); 
                menuFrontRect.DORotate(new Vector3(0, 0, 0), halfDuration).SetEase(Ease.InOutSine);

                isFrontShowing = true;
            });
        }
     }
    public void Ready()
    {
        StartCoroutine(PanCameraTo());
    }
    public IEnumerator PanCameraTo()
    {
        Vector3 targetPosition = new Vector3(9.44f, 7.2f, -0.68f);
        Quaternion targetRotation = Quaternion.Euler(90, -180, -90);

        StartCoroutine(PanelRemove());

        Tween moveTween = mainCamera.transform.DOMove(targetPosition, panDuration).SetEase(Ease.InOutSine);
        Tween rotateTween = mainCamera.transform.DORotateQuaternion(targetRotation, panDuration).SetEase(Ease.InOutSine);

        yield return moveTween.WaitForCompletion();
        yield return rotateTween.WaitForCompletion();

        canStart = true;

    }

    public void ExitToMain()
    {
        StartCoroutine(PanCameraBack());
    }

    IEnumerator PanCameraBack()
    {
        Vector3 targetPosition = new Vector3(10.79f, 6.5f, -0.6f);
        Quaternion targetRotation = Quaternion.Euler(-3.392f, -91.462f, 0f);

        Tween moveTween = mainCamera.transform.DOMove(targetPosition, panDuration).SetEase(Ease.InOutSine);
        Tween rotateTween = mainCamera.transform.DORotateQuaternion(targetRotation, panDuration).SetEase(Ease.InOutSine);

        yield return moveTween.WaitForCompletion();
        yield return rotateTween.WaitForCompletion();

        yield return new WaitForSeconds(0.2f);
        canStart = false;
        ResetMenuPanels();
        ShowMenu();
    }

    IEnumerator PanelRemove()
    {
        yield return new WaitForSeconds(0.1f);
        RectTransform backRect = backPanel.GetComponent<RectTransform>();
        Vector2 backAnchorPosition = backRect.anchoredPosition;
        frontPanel.SetActive(false);

        Vector2 targetPos = backAnchorPosition - new Vector2(0, fallDistance);

        yield return backRect.DOAnchorPos(targetPos, fallDuration)
                       .SetEase(fallEase)
                       .WaitForCompletion();

        backPanel.SetActive(false);
        backRect.anchoredPosition = backAnchorPosition;
    }

    void ResetMenuPanels()
    {
        frontPanel.SetActive(true);
        backPanel.SetActive(false);

        frontPanel.transform.localRotation = Quaternion.identity;
        backPanel.transform.localRotation = Quaternion.identity;
    }
    public void ActivePausePanel()
    {
        pausePanel.SetActive(true);
        canStart = false;
    }

    public void DisablePausePanel()
    {
        pausePanel.SetActive(false );
        canStart = true;
    }



    public void LoadGame() => Debug.Log("Load Game Clicked");
    public void Settings() => Debug.Log("Settings Clicked");
    public void ExitGame()
    {
        Application.Quit();
    }
}


