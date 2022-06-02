using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorchFlicker : MonoBehaviour
{
    public bool doFlicker = true;

    [SerializeField, Min(0)] float MaxIntensity = 0.25f;
    [SerializeField, Min(0)] float MinIntensity = 50f;
    [SerializeField, Min(0)] float MaxFlickerFrequency = 1f;
    [SerializeField, Min(0)] float MinFlickerFrequency = 0.1f;
    [SerializeField, Min(0)] float Strength = 5f;

    float _nextIntensity;
    float _flickerFrequency;
    float _timeOfLastFlicker;
    Light _torchLight;

    private void OnValidate()
    {
        if (MaxIntensity < MinIntensity) MinIntensity = MaxIntensity;
        if (MaxFlickerFrequency < MinFlickerFrequency) MinFlickerFrequency = MaxFlickerFrequency;
    }

    private void Start()
    {
        _torchLight = GetComponent<Light>();
        _timeOfLastFlicker = Time.time;
    }

    private void Update()
    {
        if (_timeOfLastFlicker + _flickerFrequency < Time.time)
        {
            _timeOfLastFlicker = Time.time;
            _nextIntensity = Random.Range(MinIntensity, MaxIntensity);
            _flickerFrequency = Random.Range(MinFlickerFrequency, MaxFlickerFrequency);
        }

        Flicker();
    }

    private void Flicker()
    {
        _torchLight.intensity = Mathf.Lerp(
            _torchLight.intensity,
            _nextIntensity,
            Strength * Time.deltaTime
        );
    }
}
