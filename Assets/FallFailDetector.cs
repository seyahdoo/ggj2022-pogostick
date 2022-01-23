using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFailDetector : MonoBehaviour {
    [SerializeField] private float limitY = -50;
    private void Update() {
        if (transform.position.y < limitY) {
            SceneLoader.ReloadScene();
        }
    }
}
