using UnityEngine;
using UnityEngine.Events;

public class AnimationEventInvoker : MonoBehaviour
{
    public UnityEvent unityEvent;

    public void Execute()
    {
        unityEvent.Invoke();
    }
}
