/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NullSpace.SDK;
using System;
using IOHelper;

namespace NullSpace.SDK.Demos
{
	public class LibraryElement : MonoBehaviour
	{
		string fullFilePath = "";

		//For the visual representation of the haptic effect.
		public enum LibraryElementType { Sequence, Pattern, Experience, Folder }
		public LibraryElementType myType = LibraryElementType.Sequence;

		public Image visual;
		public Image myIcon;
		public Text displayName;
		public Button playButton;
		public Button openLocationButton;
		public Button copyButton;
		public string myNamespace;
		public string fileAndExt;
		public string fileName;
		public DateTime lastModified;

		private bool initialized = false;
		private string validationFailureReasons = string.Empty;

		public void Init(string fullPath, string packageName = "")
		{
			if (fullPath.Length == 0)
			{
				Debug.Log("Cleaning up: " + name + " from editor modification\n");
				Destroy(gameObject);
			}
			else if (!initialized)
			{
				fullFilePath = fullPath;
				fileAndExt = LibraryManager.CleanPathToFile(fullFilePath);
				string[] fileParts = fileAndExt.Split('.');
				fileName = fileParts[0];
				displayName.text = fileParts[0];
				myNamespace = packageName;

				if (fullFilePath.Contains(".seq"))
				{
					myType = LibraryElementType.Sequence;
					myIcon.sprite = LibraryManager.Inst.seqIcon;
					visual.color = LibraryManager.Inst.seqColor;
					TooltipDescriptor.AddDescriptor(gameObject, fileParts[0] + " - Sequence", "Plays on all selected pads\nOr when the green haptic trigger touches a pad");
					TooltipDescriptor.AddDescriptor(openLocationButton.gameObject, "<color=#FF4500>Edit File</color>", "View Source of " + fileParts[0] + "\nWe recommend a text editor", new Color32(135, 206, 255, 225));
				}
				else if (fullFilePath.Contains(".pat"))
				{
					myType = LibraryElementType.Pattern;
					myIcon.sprite = LibraryManager.Inst.patIcon;
					visual.color = LibraryManager.Inst.patColor;
					TooltipDescriptor.AddDescriptor(gameObject, fileParts[0] + " - Pattern", "Plays pattern which is composed of sequences on specified areas");
					TooltipDescriptor.AddDescriptor(openLocationButton.gameObject, "<color=#FF4500>Edit File</color>", "View Source of " + fileParts[0] + "\nWe recommend a text editor", new Color32(135, 206, 255, 225));
				}
				else if (fullFilePath.Contains(".exp"))
				{
					myType = LibraryElementType.Experience;
					myIcon.sprite = LibraryManager.Inst.expIcon;
					visual.color = LibraryManager.Inst.expColor;
					TooltipDescriptor.AddDescriptor(gameObject, fileParts[0] + " - Experience", "Plays experience which is composed of multiple Patterns.");
					TooltipDescriptor.AddDescriptor(openLocationButton.gameObject, "<color=#FF4500>Edit File</color>", "View Source of " + fileParts[0] + "\nWe recommend a text editor", new Color32(135, 206, 255, 225));
				}
				else
				{
					myType = LibraryElementType.Folder;
					myIcon.sprite = LibraryManager.Inst.folderIcon;
					visual.color = LibraryManager.Inst.folderColor;
					copyButton.transform.parent.parent.gameObject.SetActive(false);

					TooltipDescriptor.AddDescriptor(gameObject, fileParts[0], "Haptic Package: A collection of sequences, patterns and experiences\nDefined by its config.json");
					TooltipDescriptor.AddDescriptor(openLocationButton.gameObject, "<color=#FF4500>Open Folder</color>", "View directories of " + fileParts[0], new Color32(135, 206, 255, 225));
				}

				//Temporary disabling of the copy-me feature.
				copyButton.transform.parent.parent.gameObject.SetActive(false);

				if (!ValidateFile())
				{
					MarkElementBroken();
				}

				lastModified = FileModifiedHelper.GetLastModified(fullFilePath);



				initialized = true;
			}
		}

		void Start()
		{
			if (openLocationButton != null)
			{
				openLocationButton.onClick.AddListener(() =>
				{
					bool playResult = OpenFile();
					if (!playResult)
					{
						MarkElementBroken();
					}
				});
			}
			if (playButton != null)
			{
				playButton.onClick.AddListener(() =>
				{
					bool playResult = ExecuteLibraryElement();
					if (!playResult)
					{
						MarkElementBroken();
					}
				});
			}
			if (copyButton != null)
			{
				copyButton.onClick.AddListener(() =>
				{
					bool playResult = CopyFile(true);
					if (!playResult)
					{
						MarkElementBroken();
					}
				});

			}
		}

		private void MarkElementBroken()
		{
			Debug.LogError("This element [" + fileAndExt + "] is broken\n");
			//This doesn't prevent the action of the element, but it indicates that the element is broken.

			//Only update the color for now.
			//myIcon.sprite = LibraryManager.Inst.errorIcon;
			visual.color = LibraryManager.Inst.errorColor;
		}

		private void MarkElementChanged()
		{
			Debug.LogError("The element [" + fileAndExt + "] has been changed since it was loaded\nThis functionality is not yet supported. You can reload the scene.");
			visual.color = LibraryManager.Inst.changedColor;

			//Debug.Log("This element [" + fileAndExt + "] is changed\n");
			//This doesn't prevent the action of the element, but it indicates that the element is broken.

			//Only update the color for now.
			//myIcon.sprite = LibraryManager.Inst.errorIcon;
			//visual.color = LibraryManager.Inst.changedColor;
		}

		private bool ValidateFile()
		{
			//Open the file

			//Check each of the validation conditions
			//Keep track of the returned reasons.
			validationFailureReasons += ValidateForCommaAfterLastElement();
			validationFailureReasons += ValidateFileName();

			//Then return true or false.
			if (validationFailureReasons.Length > 0)
			{
				return false;
			}
			return true;
		}

		private bool FileHasBeenModified()
		{
			DateTime dt = FileModifiedHelper.GetLastModified(fullFilePath);
			//Debug.Log(dt.ToString() + "\n");

			return dt != lastModified;
		}

		private string ValidateFileName()
		{
			//Debug.Log("[" + myType.ToString() + "]  [" + fileName + "]  [" + fileAndExt + "]  [" + myNamespace + "]\n", this);

			return string.Empty;

			//return "Failed due to comma after the last element\n";
		}

		private string ValidateForCommaAfterLastElement()
		{
			//Test case to check the broken marking is working.
			//return UnityEngine.Random.Range(0, 40) > 35 ? "FAILED" : string.Empty;

			//Do some JSON validation here?
			//Possibly get API calls to validate file format?

			return string.Empty;

			//return "Failed due to comma after the last element\n";
		}

		private string EvaluateName(string packageName)
		{
			string fileName = fullFilePath;
			string[] split = fileName.Split(new char[] { '\\', '/' });
			fileName = split[split.Length - 1];
			return fileName;
		}

		public bool CopyFile(bool editImmediately = false)
		{
			try
			{
				string copyResult = CopyHelper.SafeFileDuplicate(fullFilePath);

				if (copyResult.Length > 0)
				{
					//Debug.Log("Successful copy: [" + copyResult + "]\n");

					//Ask PackageViewer to create a representation of that file.
					LibraryManager.Inst.Selection.CreateRepresentations(copyResult);
					LibraryManager.Inst.Selection.SortElements();

					if (editImmediately)
					{
						//If we want to edit it immediately, open it.
						OpenFile(copyResult);
					}
				}
			}

			catch (Exception e)
			{
				Debug.LogError("Failure to duplicate file \n\t[" + fullFilePath + "]\n" + e.Message);
				return false;
			}

			return true;
		}

		//Make this return a bool and it'll mark the LibraryElement as broken.
		public bool ExecuteLibraryElement()
		{
			if (FileHasBeenModified())
			{
				MarkElementChanged();
			}

			//Debug.Log("File has been modified since load: [" + FileHasBeenModified() + "]\n");
			try
			{
				HapticHandle newHandle = null;
				if (LibraryManager.Inst.LastPlayed != null && LibraryManager.Inst.StopLastPlaying)
				{
					LibraryManager.Inst.LastPlayed.Pause();
					LibraryManager.Inst.LastPlayed.Dispose();
				}

				//Get the file path
				//Debug.Log("[" + myNamespace + "] [" + fileName + "]\n" + myNamespace + "" + fileName);
				if (myType == LibraryElementType.Sequence)
				{
					//If sequence, use the specific pads selected (unsupported atm)
					AreaFlag flag = LibraryManager.Inst.GetActiveAreas();
					newHandle = new Sequence(myNamespace + fileName).CreateHandle(flag);
					LibraryManager.Inst.SetTriggerSequence(myNamespace + fileName);
				}
				if (myType == LibraryElementType.Pattern)
				{
					newHandle = new Pattern(myNamespace + fileName).CreateHandle();
				}
				if (myType == LibraryElementType.Experience)
				{
					newHandle = new Experience(myNamespace + fileName).CreateHandle();
				}

				LibraryManager.Inst.LastPlayed = newHandle;

				if (LibraryManager.Inst.LastPlayed != null)
				{
					LibraryManager.Inst.LastPlayed.Play();
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to play haptic [" + fullFilePath + "]" + "\n" + e.Message);
				return false;
			}
			return true;
		}

		public bool OpenFile(string requestedFilePath = "")
		{
			//Test case to check the broken marking is working.
			//return UnityEngine.Random.Range(0, 40) > 35 ? false : true;

			try
			{
				requestedFilePath = requestedFilePath.Length > 0 ? requestedFilePath : fullFilePath;
				OpenPathHelper.Open(requestedFilePath);
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to open file path [" + fullFilePath + "]" + "\n" + e.Message);
				return false;
			}
			return true;
		}

		public HapticHandle CreateCodeHaptic()
		{
			//Debug.Log("Hit\n");
			CodeSequence seq = new CodeSequence();
			seq.AddEffect(0.0f, new CodeEffect("buzz", .2f));
			seq.AddEffect(0.3f, new CodeEffect("click", 0.0f));
			//seq.Play(AreaFlag.All_Areas);

			CodePattern pat = new CodePattern();
			pat.AddSequence(0.5f, AreaFlag.Lower_Ab_Both, seq);
			pat.AddSequence(1.0f, AreaFlag.Mid_Ab_Both, seq);
			pat.AddSequence(1.5f, AreaFlag.Upper_Ab_Both, seq);
			pat.AddSequence(2.0f, AreaFlag.Chest_Both, seq);
			pat.AddSequence(2.5f, AreaFlag.Shoulder_Both, seq);
			pat.AddSequence(2.5f, AreaFlag.Back_Both, seq);
			pat.AddSequence(3.0f, AreaFlag.Upper_Arm_Both, seq);
			pat.AddSequence(3.5f, AreaFlag.Forearm_Both, seq);
			return pat.Play();
		}

		void Update()
		{
			if (!initialized)
				Init("");
		}
	}
}