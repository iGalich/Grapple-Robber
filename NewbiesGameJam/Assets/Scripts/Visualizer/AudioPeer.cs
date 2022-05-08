using UnityEngine;

public class AudioPeer : MonoBehaviour
{
    private static float[] _samples = new float[512];
    private static float[] _frequencyBand = new float[8];
    private static float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];
    private AudioSource _audioSource;

    public static float[] Samples => _samples;
    public static float[] FrequencyBand => _frequencyBand;
    public static float[] BandBuffer => _bandBuffer;

    private void Awake()
    { 
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBand();
        BandBufferCalculation();
    }

    private void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    private void MakeFrequencyBand()
    {
        int count = 0;
        for (int i = 0; i < _frequencyBand.Length; i++)
        {
            float average = 0f;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (++count);
            }
            
            average /= count;

            _frequencyBand[i] = average * 10f;
        }
    }

    private void BandBufferCalculation()
    {
        for (int i = 0; i < _bandBuffer.Length; i++)
        {
            if (_frequencyBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _frequencyBand[i];
                _bufferDecrease[i] = 0.005f;
            }
            if (_frequencyBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }
        }
    }
}