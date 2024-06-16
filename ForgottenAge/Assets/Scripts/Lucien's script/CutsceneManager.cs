using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CutsceneManager : MonoBehaviour
{
    public VideoClip[] cutscenes; // Array to hold the cutscene video clips
    public GameObject cutsceneCanvas; // Canvas to display the video
    public RawImage cutsceneRawImage; // RawImage to display the video
    public VideoPlayer videoPlayer; // VideoPlayer component
    public RenderTexture renderTexture; // RenderTexture for the video

    public Sprite[] backgroundImages; // Array of background images
    public Image backgroundImage; // Reference to the background image component
    public RawImage blackScreen; // RawImage for turning the screen black

    public float delayBeforeVideo = 1.0f; // Delay before the video starts

    public void PlayCutscene(int cutsceneIndex, System.Action onCutsceneComplete)
    {
        StartCoroutine(PlayCutsceneCoroutine(cutsceneIndex, onCutsceneComplete));
    }

    private IEnumerator PlayCutsceneCoroutine(int cutsceneIndex, System.Action onCutsceneComplete)
    {
        Debug.Log("Starting cutscene: " + cutsceneIndex);

        // Pause the game
        Time.timeScale = 0f;

        // Enable the cutscene canvas
        cutsceneCanvas.SetActive(true);
        Debug.Log("Cutscene canvas enabled");

        // Turn the screen black
        blackScreen.gameObject.SetActive(true);

        // Delay before starting the video
        yield return new WaitForSecondsRealtime(delayBeforeVideo);

        videoPlayer.targetTexture = renderTexture;
        cutsceneRawImage.texture = renderTexture;

        videoPlayer.clip = cutscenes[cutsceneIndex];
        Debug.Log("Video clip set: " + videoPlayer.clip.name);

        videoPlayer.Prepare();

        // Add a timeout to prevent getting stuck in an endless loop
        float timeout = 5f;
        while (!videoPlayer.isPrepared && timeout > 0)
        {
            Debug.Log("Preparing video...");
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (!videoPlayer.isPrepared)
        {
            Debug.LogError("Video preparation timed out");
            cutsceneCanvas.SetActive(false);
            blackScreen.gameObject.SetActive(false);
            Time.timeScale = 1f; // Resume the game
            onCutsceneComplete?.Invoke();
            yield break;
        }

        Debug.Log("Video prepared, starting playback");
        videoPlayer.Play();

        // Wait until the video finishes playing
        while (videoPlayer.isPlaying)
        {
            Debug.Log("Video is playing...");
            yield return null;
        }

        Debug.Log("Video finished");

        // Disable the cutscene canvas
        cutsceneCanvas.SetActive(false);
        Debug.Log("Cutscene canvas disabled");

        // Update the background image
        UpdateBackgroundImage(cutsceneIndex);

        // Turn off the black screen
        blackScreen.gameObject.SetActive(false);

        // Resume the game
        Time.timeScale = 1f;

        // Call the callback function to notify that the cutscene is complete
        onCutsceneComplete?.Invoke();
    }

    private void UpdateBackgroundImage(int cutsceneIndex)
    {
        // Update the background image based on the cutscene index
        if (cutsceneIndex < backgroundImages.Length)
        {
            backgroundImage.sprite = backgroundImages[cutsceneIndex];
            Debug.Log("Background image updated to: " + backgroundImages[cutsceneIndex].name);
        }
        else
        {
            Debug.LogWarning("No background image available for cutscene index: " + cutsceneIndex);
        }
    }
}
