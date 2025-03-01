using UnityEngine;

public class Orphanizer : MonoBehaviour
{
    static Transform parent = null;

    private void Awake()
    {
        if (parent == null)
            parent = new GameObject("UnparentedGroup").transform;

        transform.parent = parent;
    }
}
