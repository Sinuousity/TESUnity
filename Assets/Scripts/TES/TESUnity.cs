using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity
{
	public class TESUnity : MonoBehaviour
	{
		public static TESUnity instance;
		private static LocalSettingsObject __settingsFile;
		public static LocalSettingsObject Settings
		{
			// any access to this property will attempt to find the Settings File if it is not cached.
			// this means the Settings file reference will work in both play mode and the editor
			get
			{
				if ( __settingsFile == null ) TryFindSettings();
				if ( __settingsFile == null ) return new LocalSettingsObject();
				return __settingsFile;
			}
			set
			{
				__settingsFile = value;
			}
		}
		static bool warned = false;

		#region Inspector-set Members

		public string dataPath;

		public SFXCollection debugCollection;
		public Sprite UIBackgroundImg;
		public Sprite UICheckmarkImg;
		public Sprite UIDropdownArrowImg;
		public Sprite UIInputFieldBackgroundImg;
		public Sprite UIKnobImg;
		public Sprite UIMaskImg;
		public Sprite UISpriteImg;

		public GameObject waterPrefab;
		#endregion

		public static bool FoundSettings		{ get { return __settingsFile != null; } }
		public static string MWDataPath			{ get { if ( FoundSettings ) return Settings.engine.dataFilesPath; else return LocalSettingsObject.dataPathOverride; } }
		public static bool UseRigidbodies		{ get { return Settings.engine.useKinematicRigidbodies; } }
		public static bool UseSphereCast		{ get { return Settings.engine.useSphereCast; } }
		public static bool ShowObjectNames		{ get { return Settings.engine.showObjectNames; } }
		public static bool EnableMusic			{ get { return Settings.audio.enableMusic; } }
		public static float AmbientIntensity	{ get { return Settings.graphics.ambientIntensity; } }
		public static bool EnableSunShadows			{ get { return Settings.graphics.sunShadows; } }
		public static bool EnableLightShadows			{ get { return Settings.graphics.lightShadows; } }
		public static RenderingPath RenderPath			{ get { return Settings.graphics.preferredRenderMode; } }
		public static bool EnableExteriorLights		{ get { return Settings.graphics.exteriorCellLights; } }
		public static bool EnableAnimatedLights		{ get { return Settings.graphics.animatedLights; } }

		private MorrowindDataReader MWDataReader;
		private MorrowindEngine MWEngine;
		private MusicPlayer musicPlayer;
		
		private GameObject testObj;
		private string testObjPath;

		private void Awake()
		{
			instance = this;
		}

		public static void TryFindSettings()
		{
			var foundSettings = Resources.LoadAll<LocalSettingsObject>("");
			if ( foundSettings.Length > 0 )
				Settings = foundSettings[ 0 ]; // search for and load the first found Settings file from a Resources folder
			else if ( !warned )
			{
				Debug.LogWarning("No TESUnity Settings File found in any Resources Folder. \n Create one through Assets>Create>TES Unity>Settings");
				warned = true;
			}
		}

		private void Start()
		{
			MWDataReader = new MorrowindDataReader(MWDataPath);
			MWEngine = new MorrowindEngine(MWDataReader);

			if ( EnableMusic )
			{// Start the music.
				musicPlayer = new MusicPlayer();

				foreach ( var songFilePath in Directory.GetFiles( MWDataPath + "/Music/Explore" ) )
				{
					if ( !songFilePath.Contains( "Morrowind Title" ) )
					{
						musicPlayer.AddSong( songFilePath );
					}
				}
				musicPlayer.Play();
			}

			// Spawn the player.
			MWEngine.SpawnPlayerInside("Imperial Prison Ship", new Vector3(0.8f, -0.25f, -1.4f));
		}
		private void OnDestroy()
		{
			if(MWDataReader != null)
			{
				MWDataReader.Close();
				MWDataReader = null;
			}
		}

		private void Update()
		{
			MWEngine.Update();
			if ( EnableMusic ) musicPlayer.Update();

			if(Input.GetKeyDown(KeyCode.P))
			{
				if(MWEngine.currentCell == null || !MWEngine.currentCell.isInterior)
				{
					Debug.Log(MWEngine.GetExteriorCellIndices(Camera.main.transform.position));
				}
				else
				{
					Debug.Log(MWEngine.currentCell.NAME.value);
				}
			}

			if (Input.GetKeyDown(KeyCode.O))
			{
				SFXLoader.PlaySFX(debugCollection.RandomFile() , debugCollection.localPath , Camera.allCameras[ 0 ].transform.position);
			}
		}

		private void CreateBSABrowser()
		{
			var MWArchiveFile = MWDataReader.MorrowindBSAFile;

			var scrollView = GUIUtils.CreateScrollView(MWEngine.canvasObj);
			scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 400);

			var scrollViewContent = GUIUtils.GetScrollViewContent(scrollView);
			scrollViewContent.AddComponent<VerticalLayoutGroup>();
			var scrollViewContentTransform = scrollViewContent.GetComponent<RectTransform>();
			scrollViewContentTransform.sizeDelta = new Vector2(scrollViewContentTransform.sizeDelta.x, 128000);

			float x = 0;
			float y0 = 0;
			float width = 400;
			float height = 20;
			float yMarginBottom = 0;
			int drawI = 0;

			for(int i = 0; i < MWArchiveFile.fileMetadatas.Length; i++)
			{
				var filePath = MWArchiveFile.fileMetadatas[i].path;

				if(Path.GetExtension(filePath) == ".nif")
				{
					int iCopy = i;
					float y = y0 - drawI * (height + yMarginBottom);

					var button = GUIUtils.CreateTextButton(filePath, scrollViewContent);
					button.GetComponent<Button>().onClick.AddListener(() =>
					{
						if(testObj != null)
						{
							Destroy(testObj);
							testObj = null;
						}

						testObj = MWEngine.theNIFManager.InstantiateNIF(filePath);
						testObjPath = filePath;
					});

					drawI++;
				}
			}
		}
		private void WriteBSAFilePaths(string parentDirectoryPath)
		{
			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/MorrowindBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.MorrowindBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/BloodmoonBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.BloodmoonBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/TribunalBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.TribunalBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}
		}
		private void ExtractFileFromMorrowind(string filePathInBSA, string parentDirectoryPath)
		{
			File.WriteAllBytes(parentDirectoryPath + '/' + Path.GetFileName(filePathInBSA), MWDataReader.MorrowindBSAFile.LoadFileData(filePathInBSA));
		}
		private void TestAllCells(string resultsFilePath)
		{
			using(StreamWriter writer = new StreamWriter(resultsFilePath))
			{
				foreach(var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
				{
					var CELL = (ESM.CELLRecord)record;

					try
					{
						var cellInfo = MWEngine.InstantiateCell(CELL);
						MWEngine.temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);
						DestroyImmediate(cellInfo.gameObject);

						writer.Write("Pass: ");
					}
					catch
					{
						writer.Write("Fail: ");
					}

					if(!CELL.isInterior)
					{
						writer.WriteLine(CELL.gridCoords.ToString());
					}
					else
					{
						writer.WriteLine(CELL.NAME.value);
					}
				}
			}
		}
	}
}