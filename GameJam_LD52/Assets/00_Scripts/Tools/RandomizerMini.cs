#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace Cody.Tools
{
	public class RandomizerMini : EditorWindow
	{
		#region Variables
		//offset
		private bool useRandomOffset;

		private bool useGridSnapping;
		private Vector3 gridsize;

		private bool xOffsetRandom;
		private bool yOffsetRandom;
		private bool zOffsetRandom;

		private Vector2 randomXOffsetMinMax;
		private Vector2 randomYOffsetMinMax;
		private Vector2 randomZOffsetMinMax;

		//rotation
		private bool useRandomRotation;

		private bool xRotateRandom;
		private bool yRotateRandom;
		private bool zRotateRandom;

		private Vector2 randomXRotationMinMax;
		private Vector2 randomYRotationMinMax;
		private Vector2 randomZRotationMinMax;

		//scale
		private bool useRandomScale;

		private bool xScaleRandom;
		private bool yScaleRandom;
		private bool zScaleRandom;

		private Vector2 randomXScaleMinMax;
		private Vector2 randomYScaleMinMax;
		private Vector2 randomZScaleMinMax;

		private bool uniformScale;
		private bool xUniformScale = true;
		private bool yUniformScale = true;
		private bool zUniformScale = true;


		//Physics simulation
		private bool usePhysicsSimulation;
		private int maxSimulationIterations = 1000;
		private SimulatedBody[] simulatedBodies;
		private List<Rigidbody> generatedRigidbodies;
		private List<Collider> generatedColliders;
		private enum SimulatedColiderType
		{
			BoxCollider,
			MeshCollider
		}
		SimulatedColiderType simulatedColliderType = SimulatedColiderType.BoxCollider;
		private bool useForce;
		private Vector2 forceMinMax;
		private float forceAngleInDegrees;
		private bool randomForce;

		//Replace
		private bool useReplace;

		private GameObject newObject;
		private bool resetRotation;
		private bool resetScale;
		private bool keepOldObjects;

		//Rename
		private bool useRename;
		private string searchInName;
		private string replaceName;
		private string newName;

		//Group
		string groupName = "new Group";

		//Raycast Placing
		private bool useRayPlace;
		private LayerMask raycastLayerMask;
		private float raycastOffset = 0f;
		private bool useHitRotation;
		#endregion




		[MenuItem("Tools/Randomizer Mini")]
		public static void ShowWindow()
		{
			var window = GetWindow<RandomizerMini>();

			// Set open size
			window.minSize = new Vector2(360, 600);
			window.maxSize = new Vector2(360, 600);

			// Set min, max size
			window.minSize = new Vector2(250, 240);
			window.maxSize = new Vector2(4000, 4000);

			window.titleContent = new GUIContent("Randomizer Mini");

		}

		private void OnGUI()
		{
			//EditorStyles.boldLabel.normal.textColor = boldLableColor;
			GUILayout.Label("Randomize | Simulate Physics | Replace - Selections", EditorStyles.largeLabel, GUILayout.Height(20));

			GUILayout.BeginVertical();


			#region Offset GUI
			GUILayout.Space(10f);
			GUI.color = Color.red;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Random Offset", EditorStyles.selectionRect);
			useRandomOffset = EditorGUILayout.Toggle("", useRandomOffset);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (useRandomOffset)
			{
				useGridSnapping = EditorGUILayout.Toggle("Grid Snapping", useGridSnapping);
				if (useGridSnapping)
				{
					gridsize = EditorGUILayout.Vector3Field("Gridsize", gridsize);
				}

				GUILayout.Space(10f);

				xOffsetRandom = EditorGUILayout.Toggle("X", xOffsetRandom);
				if (xOffsetRandom)
				{
					randomXOffsetMinMax = EditorGUILayout.Vector2Field("X Min / Max", randomXOffsetMinMax);
				}

				yOffsetRandom = EditorGUILayout.Toggle("Y", yOffsetRandom);
				if (yOffsetRandom)
				{
					randomYOffsetMinMax = EditorGUILayout.Vector2Field("Y Min / Max", randomYOffsetMinMax);
				}

				zOffsetRandom = EditorGUILayout.Toggle("Z", zOffsetRandom);
				if (zOffsetRandom)
				{
					randomZOffsetMinMax = EditorGUILayout.Vector2Field("Z Min / Max", randomZOffsetMinMax);
				}

				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Offset Random (add/subtract)", GUILayout.Height(40)))
				{
					OffsetRandomAddSub(Selection.gameObjects);
				}
				GUILayout.EndHorizontal();
			}
			#endregion


			#region Rotation GUI
			GUILayout.Space(10f);
			GUI.color = Color.green;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Random Rotation", EditorStyles.selectionRect);
			useRandomRotation = EditorGUILayout.Toggle("", useRandomRotation);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (useRandomRotation)
			{
				xRotateRandom = EditorGUILayout.Toggle("X", xRotateRandom);
				if (xRotateRandom)
				{
					randomXRotationMinMax = EditorGUILayout.Vector2Field("X Min / Max", randomXRotationMinMax);
				}

				yRotateRandom = EditorGUILayout.Toggle("Y", yRotateRandom);
				if (yRotateRandom)
				{
					randomYRotationMinMax = EditorGUILayout.Vector2Field("Y Min / Max", randomYRotationMinMax);
				}

				zRotateRandom = EditorGUILayout.Toggle("Z", zRotateRandom);
				if (zRotateRandom)
				{
					randomZRotationMinMax = EditorGUILayout.Vector2Field("Z Min / Max", randomZRotationMinMax);
				}

				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Rotate Random (overwrite)", GUILayout.Height(40)))
				{
					RotateRandomOverwrite(Selection.gameObjects);
				}
				if (GUILayout.Button("Rotate Random (add/subtract)", GUILayout.Height(40)))
				{
					RotateRandomAddSub(Selection.gameObjects);
				}
				GUILayout.EndHorizontal();
			}
			#endregion


			#region Scaling GUI
			GUILayout.Space(10f);
			GUI.color = Color.cyan;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Random Scaling", EditorStyles.selectionRect);
			useRandomScale = EditorGUILayout.Toggle("", useRandomScale);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (useRandomScale)
			{
				uniformScale = EditorGUILayout.Toggle("Uniform Scale", uniformScale);
				GUILayout.BeginHorizontal();
				if (uniformScale)
				{
					GUILayout.Space(40f);
					xUniformScale = EditorGUILayout.Toggle("X", xUniformScale, GUILayout.Width(20f));
					yUniformScale = EditorGUILayout.Toggle("Y", yUniformScale, GUILayout.Width(20f));
					zUniformScale = EditorGUILayout.Toggle("Z", zUniformScale, GUILayout.Width(20f));
				}
				GUILayout.EndHorizontal();

				xScaleRandom = EditorGUILayout.Toggle("X", xScaleRandom);
				if (xScaleRandom)
				{
					randomXScaleMinMax = EditorGUILayout.Vector2Field("X Min / Max", randomXScaleMinMax);
				}

				yScaleRandom = EditorGUILayout.Toggle("Y", yScaleRandom);
				if (yScaleRandom)
				{
					randomYScaleMinMax = EditorGUILayout.Vector2Field("Y Min / Max", randomYScaleMinMax);
				}

				zScaleRandom = EditorGUILayout.Toggle("Z", zScaleRandom);
				if (zScaleRandom)
				{
					randomZScaleMinMax = EditorGUILayout.Vector2Field("Z Min / Max", randomZScaleMinMax);
				}

				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Scale Random (overwrite)", GUILayout.Height(40)))
				{
					ScaleRandomOverwrite(Selection.gameObjects);
				}
				if (GUILayout.Button("Scale Random (add/subtract)", GUILayout.Height(40)))
				{
					ScaleRandomAddSub(Selection.gameObjects);
				}
				GUILayout.EndHorizontal();
			}
			#endregion

			GUILayout.Space(10f);
			GUILayout.Label("-------------- Extra Functions --------------");

			#region Physics Simulation GUI
			GUILayout.Space(10f);
			GUI.color = Color.yellow;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Physics Simulation", EditorStyles.selectionRect);
			usePhysicsSimulation = EditorGUILayout.Toggle("", usePhysicsSimulation);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (usePhysicsSimulation)
			{
				simulatedColliderType = (SimulatedColiderType)EditorGUILayout.EnumPopup("Collider Type", simulatedColliderType);
				useForce = EditorGUILayout.Toggle("use Force", useForce);
				if (useForce)
				{
					forceMinMax = EditorGUILayout.Vector2Field("Force Min / Max", forceMinMax);
					forceAngleInDegrees = EditorGUILayout.Slider(forceAngleInDegrees, 0f, 360f);
					randomForce = EditorGUILayout.Toggle("add random force", randomForce);
				}

				GUILayout.Space(5f);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Simulate Selection", GUILayout.Height(40)))
				{
					SimulateSelection(Selection.gameObjects);
				}
				/*
				if (GUILayout.Button("Reset last Simulation", GUILayout.Height(40)))
				{
					ResetAllBodies(); //custom reset system not using unitys Undo API
				}
				*/
				GUILayout.EndHorizontal();
			}
			#endregion

			#region Replace With GUI
			GUILayout.Space(10f);
			GUI.color = Color.magenta;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Replace", EditorStyles.selectionRect);
			useReplace = EditorGUILayout.Toggle("", useReplace);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (useReplace)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label("New Object");
				newObject = (GameObject)EditorGUILayout.ObjectField(newObject, typeof(GameObject), true);
				GUILayout.EndHorizontal();

				if (newObject != null)
				{
					resetRotation = EditorGUILayout.Toggle("Replace and reset Rotation", resetRotation);
					resetScale = EditorGUILayout.Toggle("Replace and reset Scale", resetScale);
					//keepOldObjects = EditorGUILayout.Toggle("Keep selected Objects", keepOldObjects);
				}

				GUILayout.Space(5f);
				if (GUILayout.Button("Replace Selection", GUILayout.Height(40)))
				{
					if (newObject != null)
					{
						Replace(Selection.gameObjects, newObject);
					}
					else
					{
						Debug.LogWarning("No Object has been assigned to replace the selection");
					}
				}
			}
			#endregion

			#region Rename GUI
			GUILayout.Space(10f);
			GUI.color = Color.white;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Rename", EditorStyles.selectionRect);
			useRename = EditorGUILayout.Toggle("", useRename);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			if (useRename)
			{

				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Name");
				newName = EditorGUILayout.TextField(newName);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space(5f);
				if (GUILayout.Button("Name as prefix x...", GUILayout.Height(30)))
				{
					if (newName != "" || newName != null)
					{
						AddPrefix(Selection.gameObjects, newName);
					}
				}
				if (GUILayout.Button("Name as suffix ...x", GUILayout.Height(30)))
				{
					if (newName != "" || newName != null)
					{
						AddSuffix(Selection.gameObjects, newName);
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Search");
				searchInName = EditorGUILayout.TextField(searchInName);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Replace");
				replaceName = EditorGUILayout.TextField(replaceName);
				GUILayout.EndHorizontal();
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();

				GUILayout.Space(5f);
				if (GUILayout.Button("Search and Replace", GUILayout.Height(40)))
				{
					if (searchInName != "" || searchInName != null)
					{
						SearchAndReplace(Selection.gameObjects, searchInName, replaceName);
					}
				}

			}
			#endregion

			#region Group
			GUILayout.Space(10f);
			GUI.color = Color.gray;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Group", EditorStyles.selectionRect);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;

			GUILayout.Space(5f);
			if (GUILayout.Button("Group", GUILayout.Height(40)))
			{
				GroupSelectedObjectsPivotCenter(Selection.gameObjects, groupName);

				if (newName != "" || newName != null)
				{

				}
			}
			#endregion

			#region RayPlace
			GUILayout.Space(10f);
			GUI.color = Color.gray;
			GUILayout.BeginHorizontal();
			GUILayout.Label("RayPlace", EditorStyles.selectionRect);
			useRayPlace = EditorGUILayout.Toggle("", useRayPlace);
			GUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (useRayPlace)
			{
				raycastOffset = EditorGUILayout.FloatField("Raycast Offset", raycastOffset);
				useHitRotation = EditorGUILayout.Toggle("Use the Hitpoint rotation", useHitRotation);
				GUILayout.Space(5f);
				if (GUILayout.Button("RayPlace", GUILayout.Height(30)))
				{
					RaycastPlacing(Selection.gameObjects);
				}
			}


			#endregion

			GUILayout.EndVertical();
		}

		#region Offset
		void OffsetRandomAddSub(GameObject[] _selectedObjs)
		{
			if (_selectedObjs.Length == 0)
				Debug.Log("no Objects have been selected");

			foreach (GameObject g in _selectedObjs)
			{

				Vector3 newPosition = g.transform.position;

				if (xOffsetRandom)
				{
					newPosition.x = g.transform.position.x + Random.Range(randomXOffsetMinMax.x, randomXOffsetMinMax.y);
				}
				if (yOffsetRandom)
				{
					newPosition.y = g.transform.position.y + Random.Range(randomYOffsetMinMax.x, randomYOffsetMinMax.y);
				}
				if (zOffsetRandom)
				{
					newPosition.z = g.transform.position.z + Random.Range(randomZOffsetMinMax.x, randomZOffsetMinMax.y);
				}

				if (useGridSnapping)
				{
					Vector3 modoloRemainder = new Vector3(0, 0, 0);

					if (xOffsetRandom && gridsize.x != 0)
					{
						modoloRemainder.x = newPosition.x % gridsize.x;
					}
					if (yOffsetRandom && gridsize.y != 0)
					{
						modoloRemainder.y = newPosition.y % gridsize.y;
					}
					if (zOffsetRandom && gridsize.z != 0)
					{
						modoloRemainder.z = newPosition.z % gridsize.z;
					}

					newPosition = new Vector3(newPosition.x - modoloRemainder.x, newPosition.y - modoloRemainder.y, newPosition.z - modoloRemainder.z);
				}

				Undo.RecordObject(g.transform, "Offset Object random");
				g.transform.position = newPosition;
			}
		}

		#endregion

		#region Rotation
		public void RotateRandomOverwrite(GameObject[] _selectedObjs)
		{
			if (_selectedObjs.Length == 0)
				Debug.Log("no Objects have been selected");

			foreach (GameObject g in _selectedObjs)
			{
				Vector3 newRotation = g.transform.eulerAngles;

				if (xRotateRandom)
				{
					newRotation.x = Random.Range(randomXRotationMinMax.x, randomXRotationMinMax.y);
				}
				if (yRotateRandom)
				{
					newRotation.y = Random.Range(randomYRotationMinMax.x, randomYRotationMinMax.y);
				}
				if (zRotateRandom)
				{
					newRotation.z = Random.Range(randomZRotationMinMax.x, randomZRotationMinMax.y);
				}

				Undo.RecordObject(g.transform, "Rotate Object random");
				g.transform.rotation = Quaternion.Euler(newRotation);
			}
		}

		public void RotateRandomAddSub(GameObject[] _selectedObjs)
		{
			if (_selectedObjs.Length == 0)
				Debug.Log("no Objects have been selected");

			foreach (GameObject g in _selectedObjs)
			{

				Vector3 newRotation = g.transform.eulerAngles;

				if (xRotateRandom)
				{
					newRotation.x = g.transform.eulerAngles.x + Random.Range(randomXRotationMinMax.x, randomXRotationMinMax.y);
				}
				if (yRotateRandom)
				{
					newRotation.y = g.transform.eulerAngles.y + Random.Range(randomYRotationMinMax.x, randomYRotationMinMax.y);
				}
				if (zRotateRandom)
				{
					newRotation.z = g.transform.eulerAngles.z + Random.Range(randomZRotationMinMax.x, randomZRotationMinMax.y);
				}

				Undo.RecordObject(g.transform, "Rotate Object random");
				g.transform.rotation = Quaternion.Euler(newRotation);
			}
		}
		#endregion

		#region Scaling
		public void ScaleRandomOverwrite(GameObject[] _selectedObjs)
		{
			if (_selectedObjs.Length == 0)
				Debug.Log("no Objects have been selected");

			foreach (GameObject g in _selectedObjs)
			{
				Vector3 newScale = g.transform.localScale;

				if (xScaleRandom)
				{
					newScale.x = Random.Range(randomXScaleMinMax.x, randomXScaleMinMax.y);
				}
				if (yScaleRandom)
				{
					newScale.y = Random.Range(randomYScaleMinMax.x, randomYScaleMinMax.y);
				}
				if (zScaleRandom)
				{
					newScale.z = Random.Range(randomZScaleMinMax.x, randomZScaleMinMax.y);
				}

				//Uniform Scale
				newScale = SetUniformScale(newScale);


				Undo.RecordObject(g.transform, "Scale Object random");
				g.transform.localScale = newScale;
			}
		}
		public void ScaleRandomAddSub(GameObject[] _selectedObjs)
		{
			if (_selectedObjs.Length == 0)
				Debug.Log("no Objects have been selected");

			foreach (GameObject g in _selectedObjs)
			{
				Vector3 newScale = g.transform.localScale;

				if (xScaleRandom)
				{
					newScale.x = newScale.x + Random.Range(randomXScaleMinMax.x, randomXScaleMinMax.y);
				}
				if (yScaleRandom)
				{
					newScale.y = newScale.y + Random.Range(randomYScaleMinMax.x, randomYScaleMinMax.y);
				}
				if (zScaleRandom)
				{
					newScale.z = newScale.z + Random.Range(randomZScaleMinMax.x, randomZScaleMinMax.y);
				}

				//Uniform Scale
				newScale = SetUniformScale(newScale);

				Undo.RecordObject(g.transform, "Scale Object random");
				g.transform.localScale = newScale;
			}
		}

		private Vector3 SetUniformScale(Vector3 newScale)
		{
			Vector3 newUniformScale = newScale;

			if (uniformScale)
			{
				if (xUniformScale && yUniformScale && zUniformScale)
				{
					if (xScaleRandom)
					{
						newUniformScale = new Vector3(newScale.x, newScale.x, newScale.x);
					}
					else if (yScaleRandom)
					{
						newUniformScale = new Vector3(newScale.y, newScale.y, newScale.y);
					}
					else if (zScaleRandom)
					{
						newUniformScale = new Vector3(newScale.z, newScale.z, newScale.z);
					}
				}
				else if (xUniformScale && yUniformScale)
				{
					if (xScaleRandom)
					{
						newUniformScale = new Vector3(newScale.x, newScale.x, newScale.z);
					}
					else if (yScaleRandom)
					{
						newUniformScale = new Vector3(newScale.y, newScale.y, newScale.z);
					}
				}
				else if (xUniformScale && zUniformScale)
				{
					if (xScaleRandom)
					{
						newUniformScale = new Vector3(newScale.x, newScale.y, newScale.x);
					}
					else if (zScaleRandom)
					{
						newUniformScale = new Vector3(newScale.z, newScale.y, newScale.z);
					}
				}
				else if (yUniformScale && zUniformScale)
				{
					if (yScaleRandom)
					{
						newUniformScale = new Vector3(newScale.x, newScale.y, newScale.y);
					}
					else if (zScaleRandom)
					{
						newUniformScale = new Vector3(newScale.x, newScale.z, newScale.z);
					}
				}
			}
			return newUniformScale;
		}

		#endregion

		#region Simulation
		private void SimulateSelection(GameObject[] _selectedObjs)
		{
			SetupSelectionForSimulation(_selectedObjs);

			simulatedBodies = FindObjectsOfType<Rigidbody>().Select(rb => new SimulatedBody(rb, _selectedObjs.Contains(rb.gameObject))).ToArray();

			//useForce
			if (useForce)
			{
				foreach (SimulatedBody body in simulatedBodies)
				{
					if (body.isSelected)
					{
						float randomForceAmount = Random.Range(forceMinMax.x, forceMinMax.y);

						float forceAngle = ((randomForce) ? Random.Range(0, 360f) : forceAngleInDegrees) * Mathf.Deg2Rad;

						Vector3 forceDir = new Vector3(Mathf.Sin(forceAngle), 0, Mathf.Cos(forceAngle));
						body.rigidbody.AddForce(forceDir * randomForceAmount, ForceMode.Impulse);
					}
				}
			}

			Physics.autoSimulation = false;

			Transform[] selectedTransforms = new Transform[_selectedObjs.Length];
			for (int i = 0; i < _selectedObjs.Length; i++)
			{
				selectedTransforms[i] = _selectedObjs[i].transform;
			}

			Undo.RecordObjects(selectedTransforms, "Simulate Selected Objects");
			for (int iteration = 0; iteration < maxSimulationIterations; iteration++)
			{
				Physics.Simulate(Time.fixedDeltaTime);
				if (simulatedBodies.All(body => body.rigidbody.IsSleeping() || !body.isSelected))
				{
					Debug.Log("Simulation was done at " + iteration + " iteration");
					break;
				}
			}

			Physics.autoSimulation = true;


			foreach (SimulatedBody body in simulatedBodies)
			{
				if (!body.isSelected)
				{
					body.Reset();
				}
			}

			ResetObjectsFromSimulation();
		}


		private void SetupSelectionForSimulation(GameObject[] _selectedObjs)
		{
			generatedRigidbodies = new List<Rigidbody>();
			generatedColliders = new List<Collider>();

			foreach (GameObject g in _selectedObjs)
			{
				if (!g.GetComponent<Rigidbody>())
				{
					Undo.RecordObject(g.gameObject, "Add Rigidbody");
					generatedRigidbodies.Add(g.AddComponent<Rigidbody>());
				}

				if (!g.GetComponent<Collider>())
				{
					if (simulatedColliderType == SimulatedColiderType.BoxCollider)
					{
						Undo.RecordObject(g.gameObject, "Add Box Collider");
						BoxCollider c = g.AddComponent<BoxCollider>();
						generatedColliders.Add(c);
					}
					if (simulatedColliderType == SimulatedColiderType.MeshCollider)
					{
						Undo.RecordObject(g.gameObject, "Add Mesh Collider");
						MeshCollider c = g.AddComponent<MeshCollider>();
						c.convex = true;
						generatedColliders.Add(c);
					}
				}
			}
		}

		private void ResetObjectsFromSimulation()
		{
			foreach (Rigidbody rb in generatedRigidbodies)
			{
				DestroyImmediate(rb);
			}
			foreach (Collider c in generatedColliders)
			{
				DestroyImmediate(c);
			}
		}

		private void ResetAllBodies()
		{
			if (simulatedBodies != null)
			{
				foreach (SimulatedBody body in simulatedBodies)
				{
					body.Reset();
				}
			}
		}

		struct SimulatedBody
		{
			public readonly Rigidbody rigidbody;
			public readonly bool isSelected;
			readonly Vector3 originalPosition;
			readonly Quaternion originalRotation;
			readonly Transform transform;

			public SimulatedBody(Rigidbody rigidbody, bool isSelected)
			{
				this.rigidbody = rigidbody;
				this.isSelected = isSelected;
				transform = rigidbody.transform;
				originalPosition = rigidbody.position;
				originalRotation = rigidbody.rotation;
			}

			public void Reset()
			{
				transform.position = originalPosition;
				transform.rotation = originalRotation;

				if (rigidbody != null)
				{
					rigidbody.velocity = Vector3.zero;
					rigidbody.angularVelocity = Vector3.zero;
				}
			}
		}
		#endregion

		#region Replace

		private void Replace(GameObject[] _selectedObjs, GameObject newGameObject)
		{
			//GameObject replacedObjectsContainer =  new GameObject();
			//if (keepOldObjects)
			//{
			//	replacedObjectsContainer.name = "Replaced Objects";
			//}
			//else
			//{
			//	DestroyImmediate(replacedObjectsContainer);
			//}

			foreach (GameObject g in _selectedObjs)
			{

				GameObject spawnedGameObject = PrefabUtility.InstantiatePrefab(newGameObject) as GameObject;

				if (g.transform.parent)
					spawnedGameObject.transform.parent = g.transform.parent.transform;
				spawnedGameObject.transform.position = g.transform.position;

				if (!resetRotation)
					spawnedGameObject.transform.rotation = g.transform.rotation;
				if (!resetScale)
					spawnedGameObject.transform.localScale = g.transform.localScale;


				if (!keepOldObjects)
				{
					Undo.DestroyObjectImmediate(g);
				}
				else
				{
					//g.transform.parent = replacedObjectsContainer.transform;
				}

				Undo.RegisterCreatedObjectUndo(spawnedGameObject, "Replaced Selection");
			}

			//if (keepOldObjects)
			//{
			//	replacedObjectsContainer.SetActive(false);
			//}
		}

		#endregion

		#region Rename

		private void AddPrefix(GameObject[] _selectedObjs, string prefix)
		{
			foreach (GameObject g in _selectedObjs)
			{
				string oldname = g.name;
				Undo.RecordObject(g, "Add Prefix to GameObject");
				g.name = prefix + oldname;
			}
		}
		private void AddSuffix(GameObject[] _selectedObjs, string suffix)
		{
			foreach (GameObject g in _selectedObjs)
			{
				string oldname = g.name;
				Undo.RecordObject(g, "Add Suffix to GameObject");
				g.name = oldname + suffix;
			}
		}
		private void SearchAndReplace(GameObject[] _selectedObjs, string search, string replace)
		{
			foreach (GameObject g in _selectedObjs)
			{
				if (g.name.Contains(search))
				{
					string oldname = g.name;
					Undo.RecordObject(g, "Search and Replace parts of Name");
					g.name = oldname.Replace(search, replace);
				}
			}
		}

		#endregion

		#region Group (without undo support)

		//could add
		//toggle to create group always at the world zero
		//toggle to create group but limit an axis  (Group is ways created with a 0 in y making it always stand on the ground)
		//field for gameobject to specify the groups center (making it easy and fast to rotate around something)
		//--> toggle to also use the gameobjects rotation as the base rotation for the group
		//Group by using the selections bounding boxes
		private void GroupSelectedObjectsPivotCenter(GameObject[] _selectedObjs, string _parentName)
		{
			Vector3 pivotCenter = Vector3.zero;

			foreach (GameObject g in _selectedObjs)
			{
				pivotCenter += g.transform.position;
			}

			pivotCenter = pivotCenter / _selectedObjs.Count();

			GameObject groupObject = new GameObject();
			groupObject.transform.position = pivotCenter;
			groupObject.name = _parentName;

			foreach (GameObject g in _selectedObjs)
			{
				g.transform.parent = groupObject.transform;
			}
		}

		#endregion

		#region Raycast Placing
		//shoots a ray down form each object
		//if the ray hits a collider with the chosen layer the objects gets placed
		private void RaycastPlacing(GameObject[] _selectedObjects)
		{
			foreach (GameObject g in _selectedObjects)
			{
				Ray rayDir = new Ray(g.transform.position, Vector3.down);

				RaycastHit hit;
				if (Physics.Raycast(rayDir, out hit, float.PositiveInfinity))
				{
					Undo.RecordObject(g.transform, "Place Object using Raycast");
					g.transform.position = hit.point + new Vector3(0f, raycastOffset, 0f);

					if (useHitRotation)
					{
						Quaternion _slopeRotation = Quaternion.FromToRotation(g.transform.up, hit.normal);
						g.transform.rotation = _slopeRotation;
					}
				}
			}
		}
		#endregion
	}
}
#endif


