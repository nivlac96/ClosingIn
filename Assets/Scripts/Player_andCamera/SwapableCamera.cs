﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; 

public class SwapableCamera : MonoBehaviour {
	World world_controller;

	Dictionary<int, GameObject> shot_reference;

	MatrixBlender blender;
	Matrix4x4 pers, ortho;

	// True = 3D
	// False = 2D
	bool dimension;

	// These variables are public for observation only, not
	public int two_shot = 0, three_shot = 0, current_shot = 0;

	// Do we switch projection type when we shift?
	public bool blendingOrtho = true;

	public List<GameObject> shots;

	// Use this for initialization
	void Start () {

		world_controller = FindObjectOfType<World> ();
		blender = gameObject.GetComponent<MatrixBlender> ();

		if (world_controller) {
			world_controller.shiftEvent += Shift;
			world_controller.shotChangeEvent += ShotChange;
		}

		shot_reference = new Dictionary<int, GameObject> ();
		int i = 1;
		foreach (GameObject shot in shots) {
			shot_reference.Add (i, shot);
			i++;
		}

		// Prepare the projection matrices based on the camera's settings.
		Camera cam = Camera.main;
		pers = Matrix4x4.Perspective (cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
		ortho = Matrix4x4.Ortho (-cam.orthographicSize * cam.aspect, cam.orthographicSize * cam.aspect, -cam.orthographicSize, cam.orthographicSize, cam.nearClipPlane, cam.farClipPlane);
	}

	// Moves the camera within the same dimension

	void ShotChange (int tw_shot, int th_shot) {

		two_shot = tw_shot;
		three_shot = th_shot;

		if (dimension) {
			MoveCamera (three_shot + 4);
		} else
			MoveCamera (two_shot);
	}


	// This function handles movement of the camera between 2D and 3D shots,
	// and the changing of projection mode
	void Shift(bool dim, float time) {
		if (dim) {

			MoveCamera (three_shot + 4);

			// Refers to the Matrixblender script to change perspective
			if (blendingOrtho) {
				blender.BlendToMatrix(pers, time);
			}
		} else {
			List<GameObject> temp = shots.GetRange (0, 4);

			ICinemachineCamera locationtemp = GetComponent<CinemachineBrain> ().ActiveVirtualCamera;
			GameObject location = locationtemp.VirtualCameraGameObject;
			float shortest = Mathf.Infinity;
			float distance = 0;
			int index = 0;
			foreach (GameObject camera in temp) {
				distance = Vector3.Distance (location.transform.position, camera.transform.position);
				if (distance < shortest) {
					shortest = distance;
					index = temp.IndexOf (camera);
				}
			}

			dimension = dim;
			if (world_controller) {
				world_controller.ShotChangeOnExternalCall (index + 1);
			}


			// Refers to the Matrixblender script to change perspective
			if (blendingOrtho) {
				blender.BlendToMatrix(ortho, time);
			}
		}
		GetComponent<CinemachineBrain> ().m_DefaultBlend.m_Time = time;
	}

	// Cinemachine handles all camera movement
	void MoveCamera(int shot) {
		CinemachineVirtualCamera temp = shot_reference [shot].GetComponent<CinemachineVirtualCamera>();
		if (temp)
			shot_reference [shot].GetComponent<CinemachineVirtualCamera> ().Priority = 20;
		else
			shot_reference [shot].GetComponent<CinemachineFreeLook> ().Priority = 20;
		if (current_shot != 0 && current_shot < 5)
			shot_reference [current_shot].GetComponent<CinemachineVirtualCamera> ().Priority = 10;
		current_shot = shot;

	}
}
