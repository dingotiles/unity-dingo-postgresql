using UnityEngine;
using thelab.mvc;
#if UNITY_EDITOR
public class GestureController : Controller<DingoApplication> {}
#else
using UnityEngine.VR.WSA.Input;
public class GestureController : Controller<DingoApplication> {

    public GameObject FocusedObject = null;
    GestureRecognizer gestureRecognizer;

    void Awake()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;
        gestureRecognizer.StartCapturingGestures();
    }

    void OnDestroy()
    {
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
    }

    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            case "gesture.focused-object.changed":
                FocusedObject = p_target as GameObject;
                gestureRecognizer.StopCapturingGestures();
                gestureRecognizer.StartCapturingGestures();
                break;
        }
    }

    void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (FocusedObject != null)
        {
            FocusedObject.SendMessage("ActivateAction");
        }
    }

}
#endif
