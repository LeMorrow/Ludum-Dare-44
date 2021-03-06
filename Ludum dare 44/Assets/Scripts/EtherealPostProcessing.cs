﻿using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EtherealPostProcessing : MonoBehaviour
{
    [Header("Fade speed")]
    [SerializeField] [Range(0f, 0.1f)] private float _fadeSpeed = 0.02f;

    [Header("Vignette")]
    [SerializeField] private Color _vignetteColor;
    [SerializeField] [Range(0.3f, 1f)] private float _vignetteBaseIntensity = 0.55f;
    [SerializeField] [Range(0f, 0.2f)] private float _vignettePulseStrength = 0.02f;
    [SerializeField] [Range(0f, 20f)] private float _vignettePulseSpeed = 2f;

    [Header("Chromatic aberration")]
    [SerializeField] [Range(0f, 1f)] private float _chromaticAberationBaseIntensity = 0.3f;
    [SerializeField] [Range(0f, 0.2f)] private float _chromaticAberationPulseStrength = 0.015f;
    [SerializeField] [Range(0f, 20f)] private float _chromaticAberationPulseSpeed = 20f;

    [Header("Color grading")]
    [SerializeField] private Color _colorGradingColor;

    [Header("Lens distortion")]
    [SerializeField] [Range(0, 25f)] private float _lensDistortionPulseStrength = 1f;
    [SerializeField] [Range(0f, 20f)] private float _lensDistortionPulseSpeed = 20f;

    private PostProcessVolume _postProcessVolume;

    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private ColorGrading _colorGrading;
    private LensDistortion _lensDistortion;

    private void Start()
    {
        // Vignette
        _vignette = ScriptableObject.CreateInstance<Vignette>();
        _vignette.enabled.Override(true);
        _vignette.intensity.Override(1f);
        _vignette.color.Override(_vignetteColor);

        // Chromatic aberration
        _chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
        _chromaticAberration.enabled.Override(true);

        // Color grading
        _colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
        _colorGrading.enabled.Override(true);
        _colorGrading.gradingMode.Override(GradingMode.LowDefinitionRange);
        _colorGrading.colorFilter.Override(_colorGradingColor);

        // Lens distortion
        _lensDistortion = ScriptableObject.CreateInstance<LensDistortion>();
        _lensDistortion.enabled.Override(true);
    }

    private void OnEnable()
    {
        Victim.OnDeath += OnDeathBeahaviour;
    }

    private void OnDeathBeahaviour(object sender, System.EventArgs args)
    {
        StartCoroutine(EnableAndFadeProcessing());
    }

    private IEnumerator EnableAndFadeProcessing()
    {
        _postProcessVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, _vignette, _chromaticAberration, _colorGrading, _lensDistortion);
        _postProcessVolume.weight = 0;

        while (_postProcessVolume.weight < 1f)
        {
            _postProcessVolume.weight += _fadeSpeed;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void Update()
    {
        // Replace Time.time with Time.realtimeSinceStartup if Time.timeScale is modified later?
        _vignette.intensity.value = _vignetteBaseIntensity + Mathf.Sin(Time.time * _vignettePulseSpeed) * _vignettePulseStrength;
        _chromaticAberration.intensity.Override(_chromaticAberationBaseIntensity + Mathf.Sin(Time.time * _chromaticAberationPulseSpeed) * _chromaticAberationPulseStrength);
        _lensDistortion.intensity.Override(Mathf.Sin(Time.time * _lensDistortionPulseSpeed) * _lensDistortionPulseStrength);

        // Does not need to be in Update, only for testing
        _colorGrading.colorFilter.Override(_colorGradingColor);
        _vignette.color.Override(_vignetteColor);
    }

    private void OnDisable()
    {
        Victim.OnDeath -= OnDeathBeahaviour;
    }

    private void OnDestroy()
    {
        if (_postProcessVolume != null)
        {
            RuntimeUtilities.DestroyVolume(_postProcessVolume, true, true);
        }
    }
}
