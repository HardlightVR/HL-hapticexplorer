using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NullSpace.SDK.Demos
{
	public class LibraryManager : MonoBehaviour
	{
		public static LibraryManager Inst;

		public Sprite seqIcon;
		public Color seqColor;
		public Sprite patIcon;
		public Color patColor;
		public Sprite expIcon;
		public Color expColor;
		public Sprite errorIcon;
		public Color errorColor;
		public Sprite folderIcon;
		public Color folderColor;
		public Color changedColor;

		public Dictionary<string, PackageViewer> ContentsDict;
		public PopulateContainer ContentContainer;
		public PopulateContainer FolderContainer;

		public HapticTrigger greenBox;
		public Text greenBoxText;
		public SuitRegionSelectorDemo selector;

		private ScrollRect DirectoryScroll;
		private PackageViewer currentSelected;
		public PackageViewer Selection
		{
			set
			{
				if (value != null)
				{
					//Toggles off the old
					if (currentSelected != null)
					{
						currentSelected.gameObject.SetActive(false);
					}
					//Toggles on the new.
					currentSelected = value;
					currentSelected.gameObject.SetActive(true);
				}
			}
			get { return currentSelected; }
		}

		public string LastPackageAccessed
		{
			set
			{
				//Debug.Log("Setting last accessed to : " + value + "\n");
				PlayerPrefs.SetString("LastPackageAccessed", value);
			}
			get
			{
				if (PlayerPrefs.HasKey("LastPackageAccessed"))
				{
					//Debug.Log("Last Package Accessed [" + PlayerPrefs.GetString("LastPackageAccessed") + "]\n");

					return PlayerPrefs.GetString("LastPackageAccessed");
				}
				return "";
			}
		}

		public bool StopLastPlaying;
		public HapticHandle LastPlayed;

		public string currentFolderSelected = "";
		public string lastFileSelected = "";

		//This is so the project will remember the last thing they had open
		//Not implemented just yet.
		public string LastOpened
		{
			get
			{
				if (PlayerPrefs.HasKey("LastHapticFolder"))
				{
					return PlayerPrefs.GetString("LastHapticFolder");
				}
				return "";
			}
			set
			{
				PlayerPrefs.SetString("LastHapticFolder", value);
			}
		}

		void Awake()
		{
			//For easier referencing in this small scale tool.
			if (Inst == null)
			{
				Inst = this;
			}
			else
			{
				Debug.LogError("Multiple Library Managers.\nDuplicate will now self destructing.\n");
				Destroy(gameObject);
			}

			//This is where we will keep reference to previously opened files/directories.
			ContentsDict = new Dictionary<string, PackageViewer>();
		}

		void Start()
		{
			//Populate the folders that contain packages.
			SetupLibraries();
			DirectoryScroll = transform.FindChild("Folder Viewer").FindChild("Sub Directory").FindChild("Scroll View").GetComponent<ScrollRect>();

			//Minor tweak to get the scroll position to start at the top.
			DirectoryScroll.verticalNormalizedPosition = 1;
		}

		void Update()
		{
			GetInput();
		}

		//This includes a quit condition
		void GetInput()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			//if (Input.GetKeyDown(KeyCode.Delete))
			//{
			//	LastPackageAccessed = "";
			//}
		}

		public void SetupLibraries()
		{
			//Base for the path - has ALL the folders
			string path = Application.streamingAssetsPath;

			//Find the folder with config.json
			List<string> folders = GetSubdirectoriesContainingOnlyFiles(path, "config.json").ToList();
			//This finds subfolders
			List<string> subFolders = GetSubdirectoriesContainingOnlyFiles(path, "config.json", SearchOption.AllDirectories).ToList();

			//Adds ones that are in subfolders
			for (int i = 0; i < subFolders.Count; i++)
			{
				//No double dipping
				if (!folders.Contains(subFolders[i]))
				{
					folders.Add(subFolders[i]);
				}
			}

			//Debug.Log("We have found " + folders.Count + " subfolders\n\t\t" + path);
			for (int i = 0; i < folders.Count; i++)
			{
				//Debug.Log("Directory: " + folders[i] + "\n");

				//A library element represents either a folder or a haptic file. It will configure it's appearance based on its name (if it has .seq/.exp/.pat in its name, it'll adjust accordingly)
				LibraryElement libEle = FolderContainer.AddPrefabToContainerReturn().GetComponent<LibraryElement>();
				libEle.Init(folders[i]);
				libEle.playButton.transform.localScale = Vector3.one;
				libEle.playButton.name = folders[i];
				string folderName = folders[i];
				libEle.playButton.onClick.AddListener(
					() => { SelectDirectory(folderName, libEle.playButton); }
					);

				//Debug.Log(Selection == null);
				//string lastAccessed = LastOpened;

				//If we have something that we last accessed
				if (LastPackageAccessed.Length > 0 && folders.Contains(LastPackageAccessed))
				{
					if (folderName == LastPackageAccessed)
					{
						SelectDirectory(folderName, libEle.playButton);
					}
				}
				else if (folders.Count > 0)
				{
					//Select the first folder
					string first = folders[0];
					SelectDirectory(first, libEle.playButton);
				}
			}
		}

		public AreaFlag GetActiveAreas()
		{
			AreaFlag flag = AreaFlag.None;

			//Safely proceed to avoid broken refs.
			if (selector != null)
			{
				for (int i = 0; i < selector.selected.Count; i++)
				{
					//If this selected element isn't null
					if (selector.selected[i] != null)
					{
						//Add that flag
						flag = flag | selector.selected[i].regionID;
					}
				}
			}
			else
			{
				Debug.LogError("Selector is null. Check Library Manager in the inspector\n");
			}

			return flag;
		}

		public void SetTriggerSequence(string sequenceName)
		{
			greenBox.SetSequence(sequenceName);
			greenBoxText.text = greenBox.fileName;
		}

		//Creates a viewer for the given folder.
		public PackageViewer SelectDirectory(string folderSelected, Button button)
		{
			//If we haven't already opened one, make a new one
			if (!ContentsDict.ContainsKey(folderSelected))
			{
				//LastOpened = folderSelected;

				PackageViewer go = ContentContainer.AddPrefabToContainerReturn().GetComponent<PackageViewer>();
				//So we could change the color of opened folder...
				go.commander = button;

				//Ask the package viewer to do it's own setup.
				go.Init(folderSelected, folderSelected);

				//Hold onto the parent object so we can easily switch between opened ones.
				ContentsDict.Add(folderSelected, go);
			}

			//Open the one we asked for (it was made if it didn't exist, or it already exists)
			//Debug.Log("Entry already exists for: " + folderSelected + "\n");
			Selection = ContentsDict[folderSelected];

			//Remember what they had open
			LastPackageAccessed = folderSelected;
			//Debug.Log("Opened Directory:" + LastPackageAccessed + "\n");

			return ContentsDict[folderSelected];
		}

		#region Static Helper Methods
		//These functions are temporary directory controls to get the appropriate subfolders.
		//Being as this is mostly an editor/helper application, these don't need to be runtime efficient.
		public static IEnumerable<string> GetSubdirectoriesContainingOnlyFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			//This doesn't find directories in other directories quite yet.
			return from subdirectory in Directory.GetDirectories(path, "*", searchOption)
				   where DirectoryContains(subdirectory, searchPattern)
				   select subdirectory;
		}
		public static bool DirectoryContains(string dir, string searchPattern = "*")
		{
			var file = Directory.GetFiles(dir, searchPattern, SearchOption.TopDirectoryOnly)
					.FirstOrDefault();

			return (file != null);
		}

		public static string CleanPathToFile(string fullFilePath)
		{
			string fileName = fullFilePath;
			string[] split = fileName.Split(new char[] { '\\', '/' });
			fileName = split[split.Length - 1];
			return fileName;
		}
		#endregion
	}
}
