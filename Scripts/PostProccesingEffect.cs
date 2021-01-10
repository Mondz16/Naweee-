using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProccesingEffect : MonoBehaviour
{
    private Bloom bloom = null;
    private Vignette vignette = null;
    public PostProcessVolume volume;

    #region Singleton
    private static PostProccesingEffect _instance;
    public static PostProccesingEffect Instance 
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out vignette);
    }

    public void ChangeVignetteColor(Color color)
    {
        vignette.color.value = color;
    }
}
