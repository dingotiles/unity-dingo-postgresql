using UnityEngine;
using thelab.mvc;


public class CameraSelector : View<DingoApplication>
{
    public GameObject unityCamera;
    public GameObject hololensCamera;

    void Awake()
    {
#if UNITY_EDITOR
        Debug.Log("#UNITY_EDITOR = true");
        unityCamera.SetActive(true);
        hololensCamera.SetActive(false);
#else
        Debug.Log("#UNITY_EDITOR = false");
        unityCamera.SetActive(false);
        hololensCamera.SetActive(true);
#endif
    }
}
