using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[VFXBinder("Audio")]
public class AudioBinder : VFXBinderBase
{
    [SerializeField] protected Lasp.FilterType _filterType;
    protected float peak, rms;

    [VFXPropertyBinding("System.Single")]
    public ExposedProperty peakProperty;
    public ExposedProperty rmsProperty;

    protected virtual void Update()
    {
        if (!Application.isPlaying) return;
        peak = Lasp.AudioInput.GetPeakLevelNormalized(_filterType);
        rms = Lasp.AudioInput.CalculateRMSNormalized(_filterType);
    }
    public override bool IsValid(VisualEffect component)
    {
        return component.HasFloat(peakProperty) || component.HasFloat(rmsProperty);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        if (component.HasFloat(peakProperty))
            component.SetFloat(peakProperty, peak);
        if (component.HasFloat(rmsProperty))
            component.SetFloat(rmsProperty, rms);
    }
}
