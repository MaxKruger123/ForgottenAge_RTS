using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;  // Add this at the top of the file

public class TutorialManager : MonoBehaviour
{
    // Serializable classes (no changes)
    [System.Serializable]
    public class HighlightInfo
    {
        public GameObject targetObject;
        public bool shouldFlicker = false;  // Determines if the object should flicker
        public bool isUIElement = false;  // Identifies UI elements
    }

    [System.Serializable]
    public class TutorialStep
    {
        [TextArea(3, 10)]
        public List<string> instructions;
        public List<HighlightInfo> highlightObjects;
        public CameraMovementType requiredCameraMovement;
        public float readTime = 3f; // Time given to read the instruction
        public bool requiresInput = false; // Determines if player input is required
        public GameObject requiredInputObject; // Object that needs to be interacted with
        public bool enableEnemies = false;
        public List<GameObject> enemiesToEnable = new List<GameObject>();
        public bool waitForEnemiesDestroyed = false;
    }

    [System.Serializable]
    public class TutorialSection
    {
        public string sectionName;
        public List<TutorialStep> steps;
    }

    // Enum (no changes)
    public enum CameraMovementType
    {
        None,
        RightClick,
        EdgePan,
        Zoom
    }

    // Serialized fields
    [Header("UI References")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private GameObject textBoxPanel;
    [SerializeField] private Button buildInterfaceButton;

    [Header("Tutorial Configuration")]
    [SerializeField] private List<TutorialSection> tutorialSections;
    [SerializeField] private MemoryTile targetMemoryTile;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowGap = 10f;
    [SerializeField] private float arrowWidth = 50f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // Private fields
    private int currentSection = 0;
    private int currentStep = 0;
    private int currentInstruction = 0;
    private List<(GameObject obj, Vector3 originalScale)> activeHighlights = new List<(GameObject, Vector3)>();
    private List<Coroutine> activeFlickerCoroutines = new List<Coroutine>();
    private List<GameObject> activeArrows = new List<GameObject>();
    private bool waitingForClick = false;
    private bool waitingForTileClick = false;
    private bool waitingForBuildMenuOpen = false;
    private bool waitingForBuildingSelection = false;

    // Component references
    private CameraController cameraController;
    private AudioManagerr audioManager;

    // Public properties
    public bool IsWaitingForMovement { get; private set; }

    public void OnMemoryTileClicked()
    {
        Debug.Log("Memory tile clicked");
        waitingForTileClick = false;
    }

    public void OnBuildMenuOpened()
    {
        Debug.Log("Build menu opened in TutorialManager");
        waitingForBuildMenuOpen = false;
    }

    public void OnBuildingSelected(Building.BuildingType buildingType)
    {
        Debug.Log($"Building selected: {buildingType}");
        waitingForBuildingSelection = false;
    }

    void Start()
    {
        // play music
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        audioManager.PlayMusic(audioManager.mainMenuMusic);

        // Set up click listener for the text box
        textBoxPanel.GetComponent<Button>().onClick.AddListener(OnTextBoxClick);
        // Get reference to CameraController
        cameraController = FindObjectOfType<CameraController>();
        // Start the tutorial coroutine
        StartCoroutine(RunTutorial());
    }

    // Coroutine to run through all tutorial sections and steps
    IEnumerator RunTutorial()
    {
        Debug.Log($"Starting tutorial with {tutorialSections.Count} sections");
        foreach (var section in tutorialSections)
        {
            Debug.Log($"Starting section: {section.sectionName} with {section.steps.Count} steps");
            foreach (var step in section.steps)
            {
                Debug.Log($"Executing step with {step.instructions.Count} instructions");
                yield return StartCoroutine(ExecuteStep(step));
                currentStep++;
            }
            currentSection++;
            currentStep = 0;
        }

        EndTutorial();
    }

    // Coroutine to execute a single tutorial step
    IEnumerator ExecuteStep(TutorialStep step)
    {
        HighlightObjects(step.highlightObjects);

        for (currentInstruction = 0; currentInstruction < step.instructions.Count; currentInstruction++)
        {
            DisplayInstruction(step.instructions[currentInstruction]);
            
            if (step.enableEnemies)
            {
                EnableEnemies(step.enemiesToEnable);
            }

            if (step.requiredCameraMovement != CameraMovementType.None)
            {
                cameraController.SetAllowedMovement(step.requiredCameraMovement);
                cameraController.ResetMovementFlag();
                IsWaitingForMovement = true;
                yield return new WaitUntil(() => cameraController.HasCompletedRequiredMovement());
                IsWaitingForMovement = false;
            }
            else if (step.requiresInput)
            {
                if (step.highlightObjects.Any(h => h.targetObject == targetMemoryTile.gameObject))
                {
                    Debug.Log("Waiting for memory tile interaction");
                    waitingForTileClick = true;
                    waitingForBuildMenuOpen = true;
                    waitingForBuildingSelection = true;
                    yield return new WaitUntil(() => InputCompleted(step));
                    Debug.Log("Memory tile interaction completed, continuing tutorial");
                }
                else
                {
                    yield return new WaitUntil(() => InputCompleted(step));
                }
            }
            else
            {
                yield return new WaitForSeconds(step.readTime);
            }
            
            yield return new WaitForSeconds(0.5f);
        }

        RemoveHighlights();
    }

    // Enable specified enemies
    private void EnableEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
                Debug.Log($"Enabled enemy: {enemy.name}");
            }
            else
            {
                Debug.LogWarning("Null enemy reference in tutorial step.");
            }
        }
    }

    // Highlight the build interface button
    void HighlightBuildInterfaceButton()
    {
        RemoveHighlights();
        HighlightInfo buttonHighlight = new HighlightInfo
        {
            targetObject = buildInterfaceButton.gameObject,
            shouldFlicker = true
        };
        HighlightObjects(new List<HighlightInfo> { buttonHighlight });
    }

    // Check if the required input for a step has been completed
    private bool InputCompleted(TutorialStep step)
    {
        if (step.highlightObjects.Any(h => h.targetObject == targetMemoryTile.gameObject))
        {
            return !waitingForTileClick && !waitingForBuildMenuOpen && !waitingForBuildingSelection;
        }
        return true;
    }

    // Display the current instruction
    void DisplayInstruction(string instruction)
    {
        Debug.Log($"Setting instruction text to: {instruction}");
        instructionText.text = instruction;
        textBoxPanel.SetActive(true);
    }

    // Handle text box click
    void OnTextBoxClick()
    {
        if (waitingForClick)
        {
            waitingForClick = false;
        }
    }

    // Highlight specified objects
    void HighlightObjects(List<HighlightInfo> highlightInfos)
    {
        foreach (var info in highlightInfos)
        {
            if (info.targetObject != null)
            {
                if (info.isUIElement)
                {
                    CreateArrowIndicator(info.targetObject);
                }
                else if (info.shouldFlicker)
                {
                    SpriteRenderer spriteRenderer = info.targetObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        Coroutine flickerCoroutine = StartCoroutine(FlickerObject(spriteRenderer));
                        activeFlickerCoroutines.Add(flickerCoroutine);
                    }
                }
                activeHighlights.Add((info.targetObject, info.targetObject.transform.localScale));
            }
        }
    }

    // Create an arrow indicator for UI elements
    void CreateArrowIndicator(GameObject targetObject)
    {
        RectTransform targetRect = targetObject.GetComponent<RectTransform>();
        if (targetRect != null && arrowPrefab != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, targetRect.parent);
            RectTransform arrowRect = arrow.GetComponent<RectTransform>();
            
            // Position the arrow below the target object with a gap
            arrowRect.anchorMin = targetRect.anchorMin;
            arrowRect.anchorMax = targetRect.anchorMax;
            arrowRect.anchoredPosition = targetRect.anchoredPosition - new Vector2(0, targetRect.rect.height + arrowGap + arrowRect.rect.height / 2);

            // Set arrow width to be consistent
            arrowRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, arrowWidth);

            activeArrows.Add(arrow);
        }
    }

    // Coroutine to make an object flicker
    IEnumerator FlickerObject(SpriteRenderer spriteRenderer)
    {
        Color originalColor = spriteRenderer.color;
        Color flickerColor = Color.yellow;
        float flickerDuration = 1f;
        
        while (true)
        {
            yield return StartCoroutine(LerpSpriteColor(spriteRenderer, originalColor, flickerColor, flickerDuration));
            yield return StartCoroutine(LerpSpriteColor(spriteRenderer, flickerColor, originalColor, flickerDuration));
        }
    }

    // Coroutine to lerp sprite color
    IEnumerator LerpSpriteColor(SpriteRenderer spriteRenderer, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endColor;
    }

    // Remove all highlights and arrows
    void RemoveHighlights()
    {
        foreach (var coroutine in activeFlickerCoroutines)
        {
            StopCoroutine(coroutine);
        }
        activeFlickerCoroutines.Clear();

        foreach (var (obj, originalScale) in activeHighlights)
        {
            if (obj != null)
            {
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
                
                Graphic uiGraphic = obj.GetComponent<Graphic>();
                if (uiGraphic != null)
                {
                    uiGraphic.color = Color.white;
                }
            }
        }
        activeHighlights.Clear();

        foreach (var arrow in activeArrows)
        {
            if (arrow != null)
            {
                Destroy(arrow);
            }
        }
        activeArrows.Clear();
    }

    // End the tutorial
    void EndTutorial()
    {
        textBoxPanel.SetActive(false);
        RemoveHighlights();
        Debug.Log("Tutorial completed!");

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}