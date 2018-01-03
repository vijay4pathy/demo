using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;


public class OPTIONCONTROLLER : MonoBehaviour {
	[SerializeField]
	VRStandardAssets.Utils.VRInteractiveItem VRInteract;
	public delegate void ClickAction();
	public static event ClickAction OnClicked;
	public GameObject CurrentObject;
	public GameObject[] OtherObjects;
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		VRInteract.OnClick += ShowOptions;



	}



	void ShowOptions()
	{
		

			foreach (GameObject G in OtherObjects) {
			
				G.SetActive (false);
			}
	//	if (CurrentObject.activeSelf != true) {
			CurrentObject.SetActive (true);
		//}
		//else
			//CurrentObject.SetActive (false);
		



	}

	
	// Update is called once per frame
	void Update () {
	
	}

}
