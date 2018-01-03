using UnityEngine;
using System.Collections;

public class BatPath : MonoBehaviour
{
	public GameObject onText;
	public GameObject offText;

	private bool display_path;

	void Start ()
	{
		display_path = StaticVariables.bat_path;
		if (display_path) 
		{
			onText.SetActive (true);
			offText.SetActive (false);
		}
		else
		{
			offText.SetActive (true);
			onText.SetActive (false);
		}
	}

	public void On_Off()
	{
		display_path = StaticVariables.Path_on_off ();
		if (display_path) 
		{
			onText.SetActive (true);
			offText.SetActive (false);
		}
		else
		{
			offText.SetActive (true);
			onText.SetActive (false);
		}
	}
}