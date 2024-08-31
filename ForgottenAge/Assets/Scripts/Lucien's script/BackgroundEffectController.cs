using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundEffectController : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;
    public ParticleSystem voidParticles;
    public float transitionDuration = 300f;
    public Color startColor = new Color(0.1f, 0.29f, 0.52f); // #1A4B84
    public Color midColor = new Color(0.56f, 0f, 0f); // #900000
    public Color endColor = new Color(0.16f, 0.16f, 0.16f); // #292929
    [Range(0f, 1f)]
    public float initialEmissionMultiplier = 0.1f;
    [Range(1f, 10f)]
    public float finalEmissionMultiplier = 5f;
    [Range(0f, 1f)]
    public float particleLightnessOffset = 0.2f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float initialNoiseStrength = 0.1f;
    public float finalNoiseStrength = 1f;
    public float noiseFrequency = 0.5f;

    [Header("Erratic Background Movement")]
    public float maxPositionOffset = 0.1f;
    public float maxRotationOffset = 5f;
    public float movementFrequency = 0.1f;
    public float initialMovementAmplitude = 0.1f;
    public float finalMovementAmplitude = 1f;

    [Header("Glitch Effects")]
    public float initialGlitchInterval = 10f;
    public float finalGlitchInterval = 2f;
    public float glitchDuration = 0.1f;
    public float maxColorOffset = 0.1f;
    public float maxGlitchPositionOffset = 0.1f;

    private float elapsedTime = 0f;
    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule particleEmission;
    private ParticleSystem.ColorOverLifetimeModule particleColorOverLifetime;
    private ParticleSystem.NoiseModule particleNoise;
    private float initialEmissionRate;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isGlitching = false;

    void Start()
    {
        if (backgroundSprite == null)
        {
            Debug.LogError("Background Sprite not assigned!");
        }
        else
        {
            originalPosition = backgroundSprite.transform.position;
            originalRotation = backgroundSprite.transform.rotation;
            backgroundSprite.color = startColor; // Set initial color
        }
        if (voidParticles == null)
        {
            Debug.LogError("Void Particles not assigned!");
        }
        else
        {
            particleMain = voidParticles.main;
            particleEmission = voidParticles.emission;
            particleColorOverLifetime = voidParticles.colorOverLifetime;
            particleNoise = voidParticles.noise;
            SetupParticles();
        }

        // Ensure camera is set up for 2D
        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;

        StartCoroutine(GlitchRoutine());
        StartCoroutine(ErraticBackgroundMovement());
    }

    void SetupParticles()
    {
        var particleRenderer = voidParticles.GetComponent<ParticleSystemRenderer>();

        // Preserve initial emission rate
        initialEmissionRate = particleEmission.rateOverTime.constant;

        // Set initial emission rate
        particleEmission.rateOverTime = initialEmissionRate * initialEmissionMultiplier;

        // Set particle sorting
        particleRenderer.sortingLayerID = backgroundSprite.sortingLayerID;
        particleRenderer.sortingOrder = backgroundSprite.sortingOrder + 1;

        // Setup noise module
        particleNoise.enabled = true;
        particleNoise.strength = initialNoiseStrength;
        particleNoise.frequency = noiseFrequency;
        particleNoise.octaveCount = 2;
        particleNoise.scrollSpeed = 0.5f;
        particleNoise.damping = false;
        particleNoise.quality = ParticleSystemNoiseQuality.High;

        // Ensure Color over Lifetime is enabled
        particleColorOverLifetime.enabled = true;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / transitionDuration);
        float curvedT = transitionCurve.Evaluate(t);

        // Calculate new background color
        Color newBackgroundColor = CalculateBackgroundColor(curvedT);

        // Update background color
        if (backgroundSprite != null)
        {
            backgroundSprite.color = newBackgroundColor;
        }

        // Calculate particle colors (lighter version of background color)
        Color particleStartColor = LightenColor(newBackgroundColor, particleLightnessOffset);
        Color particleEndColor = particleStartColor;
        particleEndColor.a = 0f; // Fade out to transparent

        // Update particle properties
        if (voidParticles != null)
        {
            // Update Color over Lifetime
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(particleStartColor, 0.0f), new GradientColorKey(particleEndColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            particleColorOverLifetime.color = gradient;

            // Increase emission rate over time
            float currentEmissionMultiplier = Mathf.Lerp(initialEmissionMultiplier, finalEmissionMultiplier, curvedT);
            particleEmission.rateOverTime = initialEmissionRate * currentEmissionMultiplier;

            // Increase noise strength over time
            float currentNoiseStrength = Mathf.Lerp(initialNoiseStrength, finalNoiseStrength, curvedT);
            particleNoise.strength = currentNoiseStrength;
        }
    }

    Color CalculateBackgroundColor(float t)
    {
        if (t < 0.5f)
        {
            return Color.Lerp(startColor, midColor, t * 2);
        }
        else
        {
            return Color.Lerp(midColor, endColor, (t - 0.5f) * 2);
        }
    }

    Color LightenColor(Color color, float amount)
    {
        return new Color(
            Mathf.Clamp01(color.r + amount),
            Mathf.Clamp01(color.g + amount),
            Mathf.Clamp01(color.b + amount),
            color.a
        );
    }

    IEnumerator GlitchRoutine()
    {
        while (true)
        {
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            float glitchInterval = Mathf.Lerp(initialGlitchInterval, finalGlitchInterval, t);

            yield return new WaitForSeconds(glitchInterval);

            StartCoroutine(ApplyGlitchEffect());
        }
    }

    IEnumerator ApplyGlitchEffect()
    {
        if (!isGlitching)
        {
            isGlitching = true;

            // Store original values
            Color originalColor = backgroundSprite.color;
            Vector3 originalScale = backgroundSprite.transform.localScale;

            // Apply glitch effect
            float colorOffset = Random.Range(-maxColorOffset, maxColorOffset);
            backgroundSprite.color = new Color(
                originalColor.r + colorOffset,
                originalColor.g + colorOffset,
                originalColor.b + colorOffset
            );

            float positionOffset = Random.Range(-maxGlitchPositionOffset, maxGlitchPositionOffset);
            backgroundSprite.transform.position = originalPosition + new Vector3(positionOffset, positionOffset, 0);

            float scaleOffset = Random.Range(-0.1f, 0.1f);
            backgroundSprite.transform.localScale = originalScale + new Vector3(scaleOffset, scaleOffset, 0);

            // Wait for glitch duration
            yield return new WaitForSeconds(glitchDuration);

            // Restore original values
            backgroundSprite.color = originalColor;
            backgroundSprite.transform.position = originalPosition;
            backgroundSprite.transform.localScale = originalScale;

            isGlitching = false;
        }
    }

    IEnumerator ErraticBackgroundMovement()
    {
        while (true)
        {
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            float currentAmplitude = Mathf.Lerp(initialMovementAmplitude, finalMovementAmplitude, t);

            float offsetX = Mathf.PerlinNoise(Time.time * movementFrequency, 0) * 2 - 1;
            float offsetY = Mathf.PerlinNoise(0, Time.time * movementFrequency) * 2 - 1;

            Vector3 newPosition = originalPosition + new Vector3(
                offsetX * maxPositionOffset * currentAmplitude,
                offsetY * maxPositionOffset * currentAmplitude,
                0
            );

            float rotationOffset = (Mathf.PerlinNoise(Time.time * movementFrequency, Time.time * movementFrequency) * 2 - 1)
                                   * maxRotationOffset * currentAmplitude;

            backgroundSprite.transform.position = newPosition;
            backgroundSprite.transform.rotation = Quaternion.Euler(0, 0, rotationOffset);

            yield return null;
        }
    }
}