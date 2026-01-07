using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Follower : MonoBehaviour
{
    public Transform parent_transform;
    private Vector3 localPositionOffset;
    private Quaternion localRotationOffset;
    private bool initialized;
    
    private void OnEnable() {
        CacheOffsets();
    }

    private void OnValidate() {
        CacheOffsets();
    }

    void CacheOffsets() {
        if (parent_transform == null) {
            initialized = false;
            return;
        }
        localPositionOffset = Quaternion.Inverse(parent_transform.rotation) *
                              (transform.position - parent_transform.position);
        localRotationOffset = Quaternion.Inverse(parent_transform.rotation) *
                              transform.rotation;
        initialized = true;
    }

    void LateUpdate() {
        if (parent_transform == null || !initialized) return;
        transform.position =
            parent_transform.position + parent_transform.rotation * localPositionOffset;
        transform.rotation =
            parent_transform.rotation * localRotationOffset;
    }
}
