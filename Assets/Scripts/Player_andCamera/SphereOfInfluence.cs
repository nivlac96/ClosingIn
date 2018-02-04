﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereOfInfluence : MonoBehaviour {

	// TRUE = 3D
	// FALSE = 2D
	public bool dimension;
	public int two_shot;
	public float shift_time;

	Transform player;
	public World world_controller;

	// The sphere of influence constantly maintains a roster of the compressables inside of it,
	// so that they can easily be compressed/decompressed. The gameobject's collider is the key,
	// and the value is the object's original transform.
	Dictionary<GameObject, Vector3> compressables = new Dictionary<GameObject, Vector3>();

	void Awake () {

		player = transform.parent;

		if (world_controller == null)
			world_controller = FindObjectOfType<World> ();

		// We subscribe to the worldcontroller's events in Awake() so that we
		// can react to the first events set out during Start() in World.cs 
		world_controller.shiftEvent += Shift;
		world_controller.shotChangeEvent += ShotChange;
	}

	void ShotChange (int tw_shot, int th_shot) {
		two_shot = tw_shot;
		if (!dimension) {
			TwoShotChange (shift_time, true);
		}
	}

	void Shift (bool dim, float time) {
		shift_time = time;
		if (dimension != dim) {
			Debug.Log("Shift: switching dimensions");
			dimension = dim;
			if  (!dimension) {
				TwoShotChange(shift_time, false);
			}
			else {
				foreach (KeyValuePair<GameObject, Vector3> compressable in compressables)
					Decompress (compressable.Key, compressable.Value);
			} 
				
		}
	}

	// Decompress the objects in the sphere when it is rotating between two different 2D
	// shots so that the player sees the world as 3D during the shift (decomp = true)
	IEnumerator TwoShotChange(float speed, bool decomp) {
		Debug.Log("coroutine start");

		//Decompress every object in the sphere
		if (decomp) {
			foreach (KeyValuePair<GameObject, Vector3> compressable in compressables) 
				Decompress (compressable.Key, compressable.Value);
		}
		Debug.Log("waiting");

		// Wait for the shift to complete
		yield return new WaitForSeconds (speed);

		Debug.Log("wait complete");

		// Recompress every object in the sphere
		foreach (KeyValuePair<GameObject, Vector3> compressable in compressables)
			Compress (compressable.Key);
	}

	void OnTriggerEnter (Collider other) {

		if (other.tag.Equals("Compressable")) {
			Debug.Log("compressable detected");
			// Add the compressable to the dictionary, with its original transform as the value
			compressables[other.gameObject] = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);

			if (!dimension)
				Compress (other.gameObject);
		}
	}

	void OnTriggerExit (Collider other) {
		
		if (other.tag.Equals ("Compressable")) {
			// Decompress the object
			Decompress (other.gameObject, compressables[other.gameObject]);
			// Remove the object from the dictionary
			compressables.Remove (other.gameObject);
		}
	}

	void Compress(GameObject compressable) {
		if (compressable.GetComponent<ComplexCompressable>() != null) {
			compressable.GetComponent<ComplexCompressable>().ComplexCompress(two_shot, player.transform.position);
		}
		else {
			if (two_shot % 2 != 1) {
				compressable.transform.position = new Vector3( player.position.x, compressable.transform.position.y, compressable.transform.position.z);
			}
			else {
				compressable.transform.position = new Vector3(compressable.transform.position.x, compressable.transform.position.y, player.position.z);
			}
		}
	}

	void Decompress(GameObject compressable, Vector3 original) {
		if (compressable.GetComponent<ComplexCompressable>() != null) {
			compressable.GetComponent<ComplexCompressable>().ComplexDecompress();
		}
		else {
			compressable.transform.position = original;
		}

	}

}
	
