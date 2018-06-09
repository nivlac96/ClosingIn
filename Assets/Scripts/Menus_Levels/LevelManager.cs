﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class LevelManager : MonoBehaviour 
{
	// True = Title as Menu State
	// False = Gameplay Pause as Menu State
	public bool menu_state = true;
	public bool key_enabled = false; 		// Player cannot press ESC in Title. 

    public Transform main_menu, options_menu, pause_menu;
	public ThirdPersonUserControl user;

	public CinemachineVirtualCamera look;
	public AudioManager audioManager;
	public GameObject mainobject;


	World worldController;

	public bool fade;

	void Start ()
	{
		fade = false;

		user = FindObjectOfType<ThirdPersonUserControl>();
		user.allow_movement = false;
	}

	void Awake()
	{
		worldController = FindObjectOfType<World>();
		audioManager = FindObjectOfType<AudioManager> ();
		look.Priority = 40;
	}

	/*void Update (){
		if (fade) {
			StartCoroutine (fadeout (music));
		}
	}*/


	// Loads a scene when a player clicks Play button. 
	public void LoadScene(string name)
	{
		//canvas.enabled = false;
		//this.gameObject.SetActive (false);
		//this.gameObject.layer = 0;
		//audioManager.StartCoroutine(audioManager.fadeout());

		menu_state = false;				// Player has left title to gameplay.
		key_enabled = true;				// Player cannot use ESC while in Title screen.
		user.allow_movement = true;		// Player will now move during gameplay.

		fade = true;

		look.Priority = -100;
		worldController.StartAnimation ();

		this.gameObject.SetActive (false);
	}

	// Opens Option menu when player clicks Options button.
	public void OptionsMenu(bool clicked)
	{
		if (menu_state)  // is Title screen
		{
			if (clicked == true) 
			{
				options_menu.gameObject.SetActive (clicked);
				main_menu.gameObject.SetActive (false);
			} 
			else
			{
				options_menu.gameObject.SetActive (clicked);
				main_menu.gameObject.SetActive (true);
			}
		} 
		else
		{
			if (clicked == true) 
			{
				options_menu.gameObject.SetActive(clicked);
				pause_menu.gameObject.SetActive(false);
			} 
			else
			{
				options_menu.gameObject.SetActive(clicked);
				pause_menu.gameObject.SetActive(true);
			}
		}
	}

	// Quits the game when player clicks Quit button.
	public void QuitGame()
	{
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}

	/*IEnumerator fadeout(GameObject temp){
		float original = temp.GetComponent<AudioSource> ().volume;
		for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / 22.0f) {
			temp.GetComponent<AudioSource> ().volume = Mathf.Lerp (original, 0, i);
			yield return 1;
		}
		if (temp.GetComponent<AudioSource> ().volume < 0.02f) {
			temp.GetComponent<AudioSource> ().Stop ();
			levelmusic.Play ();
			levelmusic.loop = true;
			yield break;
		}
	}*/
			
	public void InvertMouseY()
	{
		FindObjectOfType<CinemachineFreeLook> ().gameObject.GetComponent<FreelookFindPlayer> ().ToggleInvertMouseY ();
	}

}
