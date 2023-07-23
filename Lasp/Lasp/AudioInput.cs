// LASP - Low-latency Audio Signal Processing plugin for Unity
// https://github.com/keijiro/Lasp

using UnityEngine;

namespace Lasp
{
    // High-level audio input interface that provides the basic functionality
    // of LASP. 
    public static class AudioInput
    {
        #region Public methods
        const float kSilence = -40; // -40 dBFS = silence

        // Returns the peak level during the last frame.
        public static float GetPeakLevel(FilterType filter)
        {
            UpdateState();
            return _filterBank[(int)filter].peak;
        }

        // Returns the peak level during the last frame in dBFS.
        public static float GetPeakLevelDecibel(FilterType filter)
        {
            // Full scale square wave = 0 dBFS : refLevel = 1
            return ConvertToDecibel(GetPeakLevel(filter), 1);
        }

        public static float GetPeakLevelNormalized(FilterType filter)
        {
            var peak = GetPeakLevelDecibel(filter);
            return Mathf.Clamp01(1 - peak / kSilence);
        }

        // Calculates the RMS level of the last frame.
        public static float CalculateRMS(FilterType filter)
        {
            UpdateState();
            return _filterBank[(int)filter].rms;
        }

        // Calculates the RMS level of the last frame in dBFS.
        public static float CalculateRMSDecibel(FilterType filter)
        {
            // Full scale sin wave = 0 dBFS : refLevel = 1/sqrt(2)
            return ConvertToDecibel(CalculateRMS(filter), 0.7071f);
        }

        public static float CalculateRMSNormalized(FilterType filter)
        {
            var rms = CalculateRMSDecibel(filter);
            return Mathf.Clamp01(1 - rms / kSilence);
        }

        // Retrieve and copy the waveform.
        public static void RetrieveWaveform(FilterType filter, float[] dest)
        {
            UpdateState();
            _stream.RetrieveWaveform(filter, dest, dest.Length);
        }

        #endregion

        #region Internal methods

        static LaspStream _stream;
        static FilterBlock[] _filterBank;
        static int _lastUpdateFrame;

        static float ConvertToDecibel(float level, float refLevel)
        {
            const float zeroOffset = 1.5849e-13f;
            return 20 * Mathf.Log10(level / refLevel + zeroOffset);
        }

        static void Initialize()
        {
            _stream = new Lasp.LaspStream();

            if (!_stream.Open())
                Debug.LogWarning("LASP: Failed to open the default audio input device.");

            LaspTerminator.Create(Terminate);

            _filterBank = new[] {
                new FilterBlock(FilterType.Bypass, _stream),
                new FilterBlock(FilterType.LowPass, _stream),
                new FilterBlock(FilterType.BandPass, _stream),
                new FilterBlock(FilterType.HighPass, _stream)
            };

            _lastUpdateFrame = -1;
        }

        static void UpdateState()
        {
            if (_stream == null) Initialize();

            if (_lastUpdateFrame < Time.frameCount)
            {
                foreach (var fb in _filterBank) fb.InvalidateState();
                _lastUpdateFrame = Time.frameCount;
            }
        }

        static void Terminate()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
                _filterBank = null;
            }
        }

        #endregion
    }
}
