using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xft;
public enum colorgrade {slow,veryslow,medium,fast,veryfast};




public class BatPathParser : MonoBehaviour {
	public string InputText;
	public string PathString;
	public Text FileNameInput;
	public List<int> IDArray = new List<int> ();
	public List<string> ParametersList = new List<string>();
	[SerializeField]
	public  List<Vector3> BatPositions = new List<Vector3>();
	public List<Vector3> BatRealPositions = new List<Vector3>();
	public bool Canmap = false;
	public  List<Vector3> BatRotations = new List<Vector3>();
	public  List<Vector3> SliderBatPositions = new List<Vector3>();
	public  List<Quaternion> SliderBatRotations = new List<Quaternion>();
	public static List<float> BatVelocity = new List<float> ();
	public  List<float> tempBatVelocity = new List<float> ();
	public  List<float> tempBatunsmoothVelocity = new List<float> ();

	public GameObject Bat;
	public GameObject GraphicsPool;
	public int currentID = 0;
	public Slider TimelineSlider;
	public GameObject CanvasUI;
	public float Scalevalue;
	public GameObject Data;
	public Vector3 BatIntialPoint;
	public GameObject Obj;
	public GameObject PathObj;
	public GameObject PlaneObj;
	public GameObject BatDrawn;
	public Vector3 BatDrawnIntialpoint;
	public Quaternion BatDrawnIntialRotation;
	public int repeatid = 0;
	public GameObject FollowThroughObj;
	Vector3 FollowThroughPoint = new Vector3();
	public ShotCalssification CurrentShotclass = ShotCalssification.StraightDrive;
	public shotaxistype CurrentShottype = shotaxistype.Vertical;
	public ParameterClassifier CurrentParameter = ParameterClassifier.BackLift;
	public float FollowthroughAngle = 87; 
	public float BackliftAngle = 55;
	public float TimetoImpact= 10;
	public float MaxBatSpeed= 10;
	public float Maxvelocity;
	public GameObject Dataset;
	GameObject PathObject;
	GameObject PlaneObject;
	public static bool Isplotting;
	public bool tempplotting;
	public delegate void played();
	public static event played onplay;
	public bool Dragged;
	public bool followpolted;
	public bool Backpolted;
	public bool velocitypolted;
	public bool maxbatspeedploted;
	public bool impactpolted;
	public int ShotCat;
	public Vector3 faceangle = new Vector3 (80,0,0);
	public bool IsAxisChange;
	public 	static  float Cangle= 0f;
	public bool IsBackLiftStarted;
	int backLiftIndex = 0;
	Vector3 velocity = Vector3.zero;
	//int index= 0;
	public static Color PlaneColor;
	public static bool  Isplot;
	// Use this for initialization
	public  static bool ShowHeatmap;
	public colorgrade CurrentGrade = colorgrade.slow;
	public GameObject Trail;
	public GameObject OldTrail;
	public float OffsetAngle;
	string TrailName = "VerySlow";
	public Material[] PlaneColors;
	public Material CurrentColor;
	public static bool UIReset = false;
	int varindex =1;
	public GameObject Dummy;
	public Text VelocityValueTest;
	public Text FaceValueTest;
	public Text ShotTypeTest;



	public float TimeSinceRotationStart = 0f;
	public float RotationTime = 1f;
	float progress;
	float lerpTime = .11f;
	public DataControllerDemoClass DDC;
	public GameObject PT1;
	public GameObject LoadingBar;
	public static float maxbatliftvalue;

	public int backliftoffset;
	public int impactoffset;
	public int followthroughoffset;

	public float unsmoothenedlength;
	public float smoothenedlength;
	public int TotalInputLength;
	public int ExtraPoints;
	public int packetcount;

	public int tempbackvalue;
	public int tempimpactvalue;
	public int tempfollowthrough;

	public int inbackliftoffset;
	public int inimpactoffset;
	public int infollowthroughoffset;

	public GameObject HotspotParent;
	public GameObject Hotpoint;
	public GameObject player3d;
	public int PlayerProfile = 1;
	public List<Vector3> HotspotArray = new List<Vector3>();

	void Start () {
		#if UNITY_ANDROID 
		ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
		#endif
		BatIntialPoint = Obj.transform.position;
		BatDrawnIntialpoint =Bat.transform.position;
		BatDrawnIntialRotation = Bat.transform.rotation;
		Isplotting = false;

	}



	// Update is called once per frame
	void Update () {



		if (Canmap) {



			print ("completed");


			LoadingBar.SetActive (false);

		
			if (varindex < SliderBatRotations.Count) {
				print ("completed1");
				PlotParameters (varindex);

				progress += (Time.deltaTime / lerpTime)*10;
				Quaternion Q1 = SliderBatRotations [varindex - 1];
				Quaternion Q2= SliderBatRotations [varindex];

				Quaternion Qmid =  Quaternion.Slerp (Q1, Q2, progress);

				var vend = Q2.eulerAngles;
				var Vmid = Qmid.eulerAngles;
				Bat.transform.eulerAngles = new Vector3 (Vmid.x, Vmid.y, vend.z);

				if ( Quaternion.Angle( Bat.transform.rotation ,SliderBatRotations [varindex])<.1f) {
					varindex++;
					progress = 0;
				}
				TimelineSlider.value = varindex;
				ChangeParameters (varindex);
				print ("completed2");

			}
			else {
				Canmap = false;
				//PlotParameters ();
				AddtoGraphicsPool ();
				Invoke ("DisplayDataPoints",2f);
				VelocityValueTest.text = "00";
				if (!followpolted) {
					CurrentParameter = ParameterClassifier.FollowThrough;
					ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter, FollowthroughAngle,varindex);
					varindex = 1;
					followpolted = true;
				}

			}
		
		}
	}



	 void DisplayDataPoints()
	{
		DDC.DataobjectAnimate ();

	}



	public void ChangeParameters(int value)
	{

		var CurrentVelocity = (BatVelocity [value])*10;
		int currentfaceangle =(int) BatRotations [value].x;
		if (currentfaceangle <= 0) {
			//currentfaceangle = 180 + currentfaceangle;
		}
		var VTR = (CurrentVelocity).ToString();
		char[] DeLim = new char[] { '.', ' ' };
		var VelocityStringArray = VTR.Split (DeLim);

		VelocityValueTest.text =VelocityStringArray[0];
		if (currentfaceangle < 100f) {
			if(currentfaceangle <10f) {
				FaceValueTest.text = "0"+currentfaceangle + "\u00B0".ToString ();
			}
			FaceValueTest.text =currentfaceangle + "\u00B0".ToString ();
		} else {
			FaceValueTest.text = currentfaceangle + "\u00B0".ToString ();
		}
	}







	void OnEnable()
	{
		ResetController.ontapped += resetall;
	}




	public void resetall()
	{

		if (GraphicsPool.transform.childCount > 0) {
			foreach (Transform child in GraphicsPool.transform) {
				Destroy (child.gameObject);
				child.gameObject.SetActive (false);
			}
		}


	}






	public	void  PathData(string data)
	{
			InputText = data;
			StartCoroutine ("DataParser");

	}




	public	void  ShotData(string data)
	{
		backliftoffset = 0;
		followthroughoffset = 0;
		impactoffset = 0;
			Debug.Log ("Shot data Init");
			char[] delimeters = new char[] { ';', ' ' };
			var temps = data.Split (delimeters);
			float.TryParse (temps [13], out  FollowthroughAngle);
			AngletoPosition.GetPosition (8, FollowthroughAngle, new Vector3 (0.5f, -.8f, 0f), out FollowThroughPoint);
			FollowThroughObj.transform.position = FollowThroughPoint;
			int tempshotClass;
			int.TryParse (temps [2], out  tempshotClass);
			GetSwingCatAsString (tempshotClass);
			float.TryParse (temps [8], out  BackliftAngle);
			float.TryParse (temps [3], out  TimetoImpact);
			int.TryParse (temps [15], out  TotalInputLength);
			int.TryParse (temps [16], out  inbackliftoffset);
			inbackliftoffset = inbackliftoffset + 1;
			int.TryParse (temps [17], out  inimpactoffset);
			inimpactoffset = inimpactoffset + 1;
			int.TryParse (temps [18], out  infollowthroughoffset);
			int.TryParse (temps [19], out PlayerProfile);
			BatsmanProfile (PlayerProfile.ToString());

	}


//Align the batsman based on the playerprofile


	void BatsmanProfile(string value)
	{
		print (value);

		switch (value) {
		case "0":
			player3d.transform.localScale = new Vector3 (1.1f, 1.1f, -1.1f);
			player3d.transform.localPosition = new Vector3 (0.44f, player3d.transform.localPosition.y, player3d.transform.localPosition.z);

			break;

		case "1":
			player3d.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
			player3d.transform.localPosition = new Vector3 (-0.44f, player3d.transform.localPosition.y, player3d.transform.localPosition.z);


			break;

		default:
			player3d.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
			player3d.transform.localPosition = new Vector3 (-0.44f, player3d.transform.localPosition.y, player3d.transform.localPosition.z);


			break;
		}

	}





	public	void  ShotDataCanned(string data)
	{
		backliftoffset = 0;
		followthroughoffset = 0;
		impactoffset = 0;
		char[] delimeters = new char[] { ';', ' ' };
		var temps = data.Split (delimeters);
		float.TryParse (temps [13] ,out  FollowthroughAngle);
		AngletoPosition.GetPosition (8, FollowthroughAngle, new Vector3 (0.5f, -.8f, 0f), out FollowThroughPoint);
		FollowThroughObj.transform.position = FollowThroughPoint;
		int tempshotClass;
		int.TryParse (temps [2] ,out  tempshotClass);
		GetSwingCatAsString (tempshotClass);
		var AC = new AxisClassifier (tempshotClass);
		float.TryParse (temps [8] ,out  BackliftAngle);
		float.TryParse (temps [3] ,out  TimetoImpact);
		int.TryParse (temps [15], out  TotalInputLength);
		int.TryParse (temps [16], out  inbackliftoffset);
		inbackliftoffset = inbackliftoffset + 1;
		int.TryParse (temps [17], out  inimpactoffset);
		inimpactoffset = inimpactoffset + 1;
		int.TryParse (temps [18], out  infollowthroughoffset);
	}







	public void StartPlotting()
	{

		//CheckID ();
		CanvasUI.SetActive (false);

	}







	IEnumerator DataParser()
	{
		packetcount = 0;
		//print ("The parser is ");

		ResetBatPositions();
		char[] delimeters = new char[] {';'};
		var TempParameterArray =  InputText.Split (delimeters);
		unsmoothenedlength = TempParameterArray.Length;
		ExtraPoints = (int)unsmoothenedlength - TotalInputLength;
		foreach (string STR in TempParameterArray) {
			if (packetcount < TotalInputLength) {
				if (STR != "") {
					if (!STR.Contains ("0,0,0,0,0".ToString ())) {
						if (!STR.Contains ("0.0,0.0,0.0,0.0,0.0".ToString ())) {
							if (!STR.Contains ("Infinity".ToString ())) {
								if (!STR.Contains ("NaN".ToString ())) {
									ParametersList.Add (STR);
									char[] Pathdelimeters = new char[] { ',' };
									Vector3 BP = new Vector3 ();
									Vector3 RP = new Vector3 ();
									packetcount++;
									float VP = 0f;
									var PathObjectArray = STR.Split (Pathdelimeters);
									float.TryParse (PathObjectArray [0], out BP.x);
									float.TryParse (PathObjectArray [1], out  BP.y);
									float.TryParse (PathObjectArray [2], out BP.z);
									GameObject tempg = new GameObject ();
									tempg.transform.position = BP;
									float.TryParse (PathObjectArray [3], out RP.x);

									float.TryParse (PathObjectArray [4], out  VP);

							
									BatRealPositions.Add (BP);




							
									if (CurrentShottype == shotaxistype.Horizontal) {
									

										BP = Quaternion.Euler (180, 00, 00) * BP;
									} else {
										BP = Quaternion.Euler (180, 0, 00) * BP;
									}


  





									if (BatPositions.Count > 1 && Vector3.Distance (BatPositions [BatPositions.Count - 1], BP) > .1f) {
										Vector3[] tempsArray = new [] {
											BatPositions [BatPositions.Count - 1], BatPositions [BatPositions.Count - 2],
											BP
										};

										Vector3[] tempsArray1 = new [] {BatRotations [BatRotations.Count - 1], BatRotations [BatRotations.Count - 2],
											RP
										};

										LinearExtrapolation (tempsArray,tempsArray1);


									}

									//else {
									BatPositions.Add (BP);
									float rollAngle = Vector3.Angle (BP, tempg.transform.right);
									float yawAngle = Vector3.Angle (BP, tempg.transform.up);
									float PitchAngle = Vector3.Angle (BP, tempg.transform.forward);
									RP.z = yawAngle;
									RP.y = PitchAngle;
									BatRotations.Add (RP);
									BatVelocity.Add (VP);
									tempBatVelocity.Add (VP);
									//}
								}

							}
						}


					}
				}
			}

		}


/////////////////////////////////////////////////////////Smoothness//////////////////////////////////////////////////////////////
		Vector3[] FtempsArray = new Vector3[BatPositions.Count];
		for (int i = 0; i < BatPositions.Count - 1; i++) {
			FtempsArray [i] = BatPositions [i];
		}

		var pointss = Curver.MakeSmoothCurve(FtempsArray,3f);
		BatPositions  = pointss.ToList();
		Vector3[] FtempsArray1 = new Vector3[BatRotations.Count];

		for (int i = 0; i < BatRotations.Count - 1; i++) {
			FtempsArray1 [i] = BatRotations [i];
		}

		var pointses = Curver.MakeSmoothCurve(FtempsArray1,3f);
		BatRotations  = pointses.ToList() ;
		Vector3[] tempsArray2 = new Vector3[BatVelocity.Count];
		for (int i = 0; i < BatVelocity.Count - 1; i++) {
			tempsArray2 [i] = new Vector3(BatVelocity [i],BatVelocity[i],BatVelocity[i]);
		}
		var pointsees = Curver.MakeSmoothCurve(tempsArray2,3f);
		BatVelocity.Clear ();
		for(int i = 0;i<pointsees.Length;i++)
		{
			BatVelocity.Add( pointsees[i].x );
	}
		smoothenedlength = BatPositions.Count;
		tempBatunsmoothVelocity = BatVelocity;
		yield return new WaitForEndOfFrame ();
		}




	public void playShot()
	{	
		

		if (BatPositions.Count > 0) {
			if (!Isplotting) {
				ResetAndPlay ();
			}
		}
	}


	//TODO: Apply Linear Extrapolation 
	/*add _sensordata_positions and sensordata_rotations to additive smoothing algortihm
	 * if the the current packet id Less than the backlift offset, then add the _sensorpoints length
	 * if the the current packet id Less than the impact offset, then add the _sensorpoints length
	 * if the the current packet id Less than the followthrough offset, then add the _sensorpoints length

	 * else ignore
	 * 
	 */






	public void LinearExtrapolation(Vector3[] tempsArray ,Vector3[] tempsArray1)
	{


		var pointss = Curver.MakeSmoothCurve(tempsArray,1f);
		for (int i = 0; i < pointss.Length; i++) {
			BatPositions.Add(pointss[i]);
		}


		var pointses = Curver.MakeSmoothCurve(tempsArray1,1f);
		for (int j = 0; j < pointses.Length; j++) {
			BatRotations.Add(pointses[j]);
		}
		for(int k = 0;k<pointses.Length;k++)
		{
			BatVelocity.Add(BatVelocity[BatVelocity.Count-1]);
			tempBatVelocity.Add (tempBatVelocity[tempBatVelocity.Count-1]);
		}

		ExtraPoints = ExtraPoints + pointses.Length;

		if (packetcount <= inbackliftoffset) {
			tempbackvalue = tempbackvalue + pointses.Length;
		}


		if (packetcount <= inimpactoffset) {
			tempimpactvalue = tempimpactvalue + pointses.Length;
		}
		if (packetcount < infollowthroughoffset) {
			tempfollowthrough = tempfollowthrough + pointses.Length;
		}
	}





	/*
	Play button Logic to Initalize the paint screen
	
	*If the there is currently another instance of the plot running  then will ignore the event
	*Reset all the array and refernces.
	*Start the convert data coroutine.
	*/



	public void ResetAndPlay()
	{
		if (Canmap == false) {
			StopCoroutine ("PaintScreen");
			varindex = 1;
			VelocityValueTest.text = "- -";
			FaceValueTest.text = "- -";
			LoadingBar.SetActive (true);
			ShotTypeTest.gameObject.transform.parent.transform.parent.gameObject.SetActive (false);
			BatPathParser.UIReset = true;
			resetbooleans ();
			TimelineSlider.gameObject.SetActive (false);
			TimelineSlider.interactable = false;
			ResetSliderpositions ();
			ResetDataPoints ();
			backliftoffset = inbackliftoffset;
			impactoffset = inimpactoffset;
			followthroughoffset = infollowthroughoffset;
			StartCoroutine ("PaintScreen");

		}
	}






	public void ResetDataPoints()
	{


		if (Dataset != null) {


			for (int i = Dataset.transform.childCount - 1; i >= 0; i--) {
				DestroyImmediate (Dataset.transform.GetChild (i).gameObject);
			}
		} 
	}





	public void resetbooleans()
	{
		followpolted = false;
		Backpolted = false;
		impactpolted = false;
		velocitypolted = false;
		maxbatspeedploted = false;
	}


	public void ResetSliderpositions()
	{
		SliderBatPositions.Clear ();
		SliderBatRotations.Clear ();
	}

	void ResetBatPositions()
	{
		BatPositions.Clear ();
		BatRotations.Clear ();
		BatVelocity.Clear ();
	
	}


	public void changeSliderStatus()
	{
		Dragged = !Dragged;
	}


	public void sliderController()
	{
		if(Dragged&&!Isplotting)
		{
			var j = (int)TimelineSlider.value;
			ChangeParameters (j);
			Bat.transform.rotation = SliderBatRotations [j];
		}
	}


	public void ChangeSliderValue(int val)
	{
		ChangeParameters (val);
		Bat.transform.rotation = SliderBatRotations [val-1];
		TimelineSlider.value = val;
	}






	void BatSet()
	{
		Gamemanager.CanDraw = false;
		var tpoint = new Vector3(-BatPositions[0].x, BatPositions[0].z, BatPositions[0].y);
		Quaternion Q = Quaternion.LookRotation(tpoint);
		Bat.transform.rotation = Q;
		StartCoroutine ("CreatePath");
		Gamemanager.CanDraw = true;

	}






	IEnumerator CreatePath()
	{
		PathObject = Instantiate (PathObj);
		PathObject.transform.parent = Bat.transform;
		PathObject.name = "BatPath";
		PathObject.transform.position = PathObj.transform.position;
		PathObject.SetActive (true);
		yield return new WaitForSeconds (0f);

	}






	IEnumerator PaintScreen()
	{
		int Correction = 0;
		GetParameterOffsets ();

		if (BatPositions.Count > 0) 
		{
			Isplotting = true;
			resetall ();
			BatSet ();

			yield return new WaitForSeconds (0.1f);
			var value = 0;
			float colorGrade = BatVelocity[0];
			for (var i = 0; i < BatPositions.Count - 1; i++) {
				

				yield return new WaitForSeconds (0f);
				BatDrawn.GetComponent<MeshRenderer> ().enabled = true;



//------------------------------------------------------------------------------------------------------------------------------------------------------------------//



				Vector3 tpoint;
				tpoint = new Vector3 (-BatPositions [i].x, BatPositions [i].z, BatPositions [i].y);
				Quaternion Q = Quaternion.LookRotation (tpoint);
				Dummy.transform.rotation = Q;



				//HORIZONTAL SHOT Algorithm
				/*
				 * The Logic is of two parts, The axis till backlift and the axis  from backlift to followthrough.

				 * Bat rotation from start to backlift

				 * 	• S1: put the bat into bat vector [0, -1, 0]
					• S2: rotate bat itself to the position given by face angle
					• S3: rotate around x axis by (90-vertical angle) degree 
					• S4: rotate around z axis by (horizontal angle) degree
				  
				 * Bat rotation from backlift to followthrough
				 
				 *  • S5: put the bat into bat vector [0, 0, -1]
					• S6: rotate the bat itself to the position give by face angle
					• S7: rotate around y axis by -(horizontal angle) degree
					• S8: rotate around x axis by -(vertical angle) degree
				 * 
				*/
				if (CurrentShottype == shotaxistype.Horizontal) {

					if (i < backliftoffset) {
						Dummy.transform.Rotate (0, 0, -BatRotations [i].x - 90, Space.Self);
					} else {
						Dummy.transform.Rotate (0, 0, -BatRotations [i].x , Space.Self);

					}




					if (((tpoint.y < 0) || (tpoint.z < 0)) && (i > backLiftIndex)) {
						Dummy.transform.Rotate (0, 0, 90f, Space.Self);
					}

					if ((tpoint.x < 0) &&
						(tpoint.y > 0) &&
						(tpoint.z > 0) && (i > backLiftIndex)) {
						Dummy.transform.Rotate (0, 0, 90f, Space.Self);
					} 
				}



				//VERTICAL SHOT  Algorithm
				/*
				 * The Logic is of two parts, The axis till backlift and the axis  from backlift to followthrough.

				 * Bat rotation from start to backlift

				 * 	• S1: put the bat into bat vector [0, -1, 0]
					• S2: rotate bat itself to the position given by face angle
					• S3: rotate around x axis by (90-vertical angle) degree 
					• S4: rotate around z axis by (horizontal angle) degree
				  
				 * Bat rotation from backlift to followthrough
				 
				 *Same steps as in start to backlift 
				*/






				if (CurrentShottype == shotaxistype.Vertical) {
					Q = Quaternion.LookRotation (tpoint, -Vector3.right);
					Dummy.transform.rotation = Q;
					Dummy.transform.Rotate (0, 0, -BatRotations [i].x - 90f, Space.Self);
				} 






				SliderBatRotations.Add (Dummy.transform.rotation);

				}
				
			//	Trail.gameObject.transform.parent = GraphicsPool.transform;
				TimelineSlider.minValue = 1;
				TimelineSlider.maxValue = SliderBatRotations.Count-2;
				yield return new WaitForSeconds (0f);
			}
	



		Gamemanager.CanDraw = false;
		TimelineSlider.gameObject.SetActive (true);
		TimelineSlider.interactable = true;
		TimelineSlider.value = 0;
		Isplotting = false;
		ShowHeatmap = true;
		Isplot = true;
		Canmap = true;
	}








	void GetParameterOffsets()
	{
		float multiplier = ( smoothenedlength / unsmoothenedlength);
		backliftoffset = (int)(backliftoffset * multiplier);
		followthroughoffset = (int)(followthroughoffset * multiplier);
		impactoffset = (int)(impactoffset * multiplier);
		backliftoffset = backliftoffset+tempbackvalue+inbackliftoffset;
		followthroughoffset = followthroughoffset + tempfollowthrough+infollowthroughoffset;
		impactoffset = impactoffset+tempimpactvalue+inimpactoffset;
	}





	void findbackliftangle()

	{	
		float maxY = BatPositions[0].y;
		for (int i = 0; i < BatPositions.Count; i++) {
			if (BatPositions[i].y > maxY) 
				maxY = BatPositions[i].y;
		}

	}



	void PlotParameters()
	{
		for (int i = 0; i < HotspotArray.Count; i++) {
			Vector3 tpoint = new Vector3 (-HotspotArray[i].x,HotspotArray [i].z, HotspotArray [i].y);
		
			tpoint = HotspotArray[i];
			Quaternion Q = Quaternion.LookRotation (tpoint);
			Bat.transform.rotation = Q;
		//	print (Q.eulerAngles);

			if (i == 0) {
				CurrentParameter = ParameterClassifier.BackLift;
				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter, BackliftAngle,i);
			}
		}


	}




	void PlotParameters(int i)

	{
		Maxvelocity = Mathf.Max (BatVelocity.ToArray ());
		var index = BatVelocity.IndexOf ((float)Maxvelocity);

		if(i== backliftoffset)
		{
			if (!Backpolted) {
				CurrentParameter = ParameterClassifier.BackLift;
				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter, BackliftAngle,i);

				Backpolted = true;
			}
		}


		if (i == impactoffset) {
			if (!velocitypolted) {
				CurrentParameter = ParameterClassifier.Velocity;
				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter,( BatVelocity[i])*10 ,i);
				velocitypolted = true;
			}
		}

		if (i == impactoffset) {

			if (!impactpolted) {
				CurrentParameter = ParameterClassifier.TimeOfImpact;
				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter, TimetoImpact,i);

				impactpolted = true;
			}
		}


		if (i == index) {
			 
			if (!maxbatspeedploted) {
				CurrentParameter = ParameterClassifier.MaxBatspeed;
				var Vel = (Maxvelocity)*10;

				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter,Vel,i);
				maxbatspeedploted = true;
			}
		}

//		if ((i == followthroughoffset)||(i == smoothenedlength)) {
//			if (!followpolted) {
//				CurrentParameter = ParameterClassifier.FollowThrough;
//				//CreateParameterSpot (TimetoImpact, CurrentParameter, BatPositions [i]);
//				ParameterController.CreateParameterSpot (Data, Dataset, PathObj, CurrentParameter, FollowthroughAngle,i);
//				followpolted = true;
//			}
//		}


	}


















	void AddtoGraphicsPool()
	{
		PathObject.transform.parent = GraphicsPool.transform;
		if (CurrentShottype == shotaxistype.Vertical) {
			//PlaneObject.transform.parent = GraphicsPool.transform;
		}
	}











	void GetSwingCatAsString(int swingCat) {

		switch (swingCat){
		case 0:
			CurrentShotclass = ShotCalssification.Hook;
			CurrentShottype = shotaxistype.Horizontal;
			break;
		case 1:
			CurrentShotclass = ShotCalssification.StraightDrive;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 2:
			CurrentShotclass = ShotCalssification.SquareCut;
			CurrentShottype = shotaxistype.Horizontal;

			break;
		case 3:
			CurrentShotclass = ShotCalssification.Sweep;
			CurrentShottype = shotaxistype.Horizontal;

			break;
		case 128:
			CurrentShotclass = ShotCalssification.BackFootPunch;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 129:
			CurrentShotclass = ShotCalssification.Cover;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 130:
			CurrentShotclass = ShotCalssification.Flick;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 131:
			CurrentShotclass = ShotCalssification.LoftedStraight;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 132:
			CurrentShotclass = ShotCalssification.OnDrive;
			CurrentShottype = shotaxistype.Vertical;

			break;
		case 133:
			CurrentShotclass = ShotCalssification.StraightDrive;
			CurrentShottype = shotaxistype.Vertical;

			break;
		default:
			CurrentShotclass = ShotCalssification.UnknownSwing;
			CurrentShottype = shotaxistype.Vertical;

			break;
		}

	}
}