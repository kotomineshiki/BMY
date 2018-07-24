using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : System.Object {
	private static Director instance;
	public FirstController currentSceneController {get; set;}
	public static Director getDirector () {
		if(instance == null) {
			instance = new Director();
		}
		return instance;
	}
	
}
