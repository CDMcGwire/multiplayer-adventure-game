using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class TheButton : Useable {
	public string triggerName = "Default";

	[SerializeField]
	private ParticleSystem particles;
	[SerializeField]
	private Animator animator;


	/***************************************************/

	
	private void Start() {
		Debug.AssertFormat(GetComponent<Collider>(), "No collider component attached to object '" + name + "'");
		Debug.AssertFormat(animator, "No animator component attached to object '" + name + "'");
	}

	public override void Use(NetworkInstanceId user) {
		if (triggerName != "Default") animator.SetTrigger(triggerName);
		else animator.SetTrigger(0);
	}


	/***************************************************/


	public void ResetTrigger() {
		if (triggerName == "Default") animator.ResetTrigger(0);
		else animator.ResetTrigger(triggerName);
	}

	public void PlayParticles() {
		Debug.AssertFormat(particles, "No particle system detected for AnimateOnClick component of '" + name + "'");
		particles.Play(true);
	}
}
