using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour {
	[SerializeField]
	private float glowIntensity = 0.7f;
	[SerializeField]
	private float pulseSpeed = 1.0f;

	private MeshRenderer[] mRenderers;
	private float pulsePhase = 0;


	// Use this for initialization
	void Start () {
		mRenderers = GetComponentsInChildren<MeshRenderer>(true);
	}


	private void OnMouseEnter() {
		pulsePhase = 0f;
	}

	// Update is called once per frame
	private void OnMouseOver() {
		foreach (var renderer in mRenderers) {
			var emission = Mathf.PingPong(pulsePhase, 1.0f);
			var finalColor = renderer.material.color * glowIntensity * Mathf.LinearToGammaSpace(emission);
			renderer.material.SetColor("_EmissionColor", finalColor);
		}

		pulsePhase += Time.deltaTime * pulseSpeed;
	}

	private void OnMouseExit() {
		foreach (var renderer in mRenderers) {
			renderer.material.SetColor("_EmissionColor", Color.black);
		}
	}
}
