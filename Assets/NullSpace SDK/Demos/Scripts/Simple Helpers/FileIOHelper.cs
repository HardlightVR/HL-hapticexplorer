/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using System;
using System.IO;
using Diag = System.Diagnostics;
using UnityEngine;

namespace IOHelper
{
	public static class PlatformHelper
	{
		public static bool IsInMacOS
		{ get { return UnityEngine.SystemInfo.operatingSystem.IndexOf("Mac OS") != -1; } }

		public static bool IsInWinOS
		{ get { return UnityEngine.SystemInfo.operatingSystem.IndexOf("Windows") != -1; } }

	}

	public static class CopyHelper
	{
		//[UnityEditor.MenuItem("Window/Test Safe Duplicate")]
		//public static void Test()
		//{
		//	//Just create a simple little text file in the Top Level Directory named thing.txt. It should
		//	SafeFileDuplicate(Application.dataPath);
		//	SafeFileDuplicate(Application.dataPath + "\\thing.txt");
		//}

		public static string SafeDuplicateInMac(string path)
		{
			Debug.LogError("[Mac] Safe Duplicate File is not yet tested on this platform.\nResults may vary.\n");
			string output = string.Empty;
			string addedName = string.Empty;
			// try mac
			string macPath = path.Replace("\\", "/");
			// mac finder doesn't like backward slashes
			if (Directory.Exists(macPath))
			// if path requested is a folder, automatically open insides of that folder
			{
			}
			if (!macPath.StartsWith("\""))
			{
				macPath = "\"" + macPath;
			}
			if (!macPath.EndsWith("\""))
			{
				macPath = macPath + "\"";
			}

			if (Directory.Exists(macPath))
			// if path requested is a folder, automatically open insides of that folder
			{
				Directory.CreateDirectory(macPath);
			}
			try
			{
				//string[] pathSplit = macPath.Split(new char[] { '.' });
				int attempts = 1;
				addedName = " " + attempts;
				bool exists = false;
				//While we havent created something
				while (attempts < 100)
				{
					int lastIndex = macPath.LastIndexOf('.');
					string targetPath = macPath.Insert(lastIndex, addedName);

					//Check if exists
					exists = File.Exists(targetPath);

					//If it does not exist
					if (!exists)
					{
						Debug.Log("Creating [" + targetPath + "]!\n");

						//Make it!
						File.Copy(macPath, targetPath, true);
						output = targetPath;
						attempts = int.MaxValue;
					}
					else
					{
						//Otherwise up the attempt count
						attempts++;
						addedName = " " + attempts;
					}
				}
				return output;
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open mac finder in windows
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this 
				e.HelpLink = "";
				// do anything with this variable to silence warning about not using it

				UnityEngine.Debug.LogError("Error with SafeDuplicateMac on " + path + "\n");

				return string.Empty;
			}
		}
		public static string SafeDuplicateInWin(string path)
		{
			string output = string.Empty;
			// try windows
			string winPath = path.Replace("/", "\\");
			// windows explorer doesn't like forward slashes

			string addedName = string.Empty;

			if (Directory.Exists(winPath))
			// if path requested is a folder, automatically open insides of that folder
			{
				Debug.LogError(winPath + "\n");
				//Directory.CreateDirectory(winPath);
			}
			else
			{
				try
				{
					//string[] pathSplit = winPath.Split(new char[] { '.' });
					int attempts = 1;
					addedName = " " + attempts;
					bool exists = false;
					//While we havent created something
					while (attempts < 100)
					{
						int lastIndex = winPath.LastIndexOf('.');
						string targetPath = winPath.Insert(lastIndex, addedName);

						//Check if exists
						exists = File.Exists(targetPath);

						//If it does not exist
						if (!exists)
						{
							Debug.Log("Creating [" + targetPath + "]!\n");

							//Make it!
							File.Copy(winPath, targetPath, true);
							output = targetPath;
							attempts = int.MaxValue;
						}
						else
						{
							//Otherwise up the attempt count
							attempts++;
							addedName = " " + attempts;
						}
					}
					return output;
				}
				catch (System.ComponentModel.Win32Exception e)
				{
					// tried to open win explorer in mac 
					// just silently skip error
					// we currently have no platform define for the current OS we are in, so we resort to this
					e.HelpLink = "";
					// do anything with this variable to silence warning about not using it

					Debug.LogError("Error with SafeDuplicateWin on " + path + "\n");

					return string.Empty;
				}
			}
			return string.Empty;
		}
		/// <summary>
		/// Safely duplicates the given file path. Will not overwrite other files.
		/// </summary>
		/// <param name="path">The path to a file to be duplicated.</param>
		/// <returns>The new file's path, should be the base path with a number appended</returns>
		public static string SafeFileDuplicate(string path)
		{
			string output = string.Empty;
			if (PlatformHelper.IsInWinOS)
			{
				string result = SafeDuplicateInWin(path);
				output = result.Length > 0 ? result : string.Empty;
			}
			else if (PlatformHelper.IsInMacOS)
			{
				string result = SafeDuplicateInMac(path);
				output = result.Length > 0 ? result : string.Empty;
			}
			else
			// couldn't determine OS 		
			{
				string result = SafeDuplicateInWin(path);
				output = result.Length > 0 ? result : string.Empty;
				result = SafeDuplicateInMac(path);
				output = result.Length > 0 ? result : string.Empty;
			}

#if UNITY_EDITOR
			if (output.Length > 0)
			{
				UnityEditor.AssetDatabase.Refresh();
			}
#endif

			return output;
		}
	}

	public static class OpenPathHelper
	{
		//[UnityEditor.MenuItem("Window/Test OpenInFileBrowser")]
		//public static void Test()
		//{ Open(UnityEngine.Application.dataPath); }

		public static void OpenInMac(string path)
		{
			bool openInsidesOfFolder = false;
			// try mac
			string macPath = path.Replace("\\", "/");
			// mac finder doesn't like backward slashes
			if (Directory.Exists(macPath))
			// if path requested is a folder, automatically open insides of that folder
			{
				openInsidesOfFolder = true;
			}
			if (!macPath.StartsWith("\""))
			{
				macPath = "\"" + macPath;
			}
			if (!macPath.EndsWith("\""))
			{
				macPath = macPath + "\"";
			}

			string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

			try
			{
				Diag.Process.Start("open", arguments);
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open mac finder in windows
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this 
				e.HelpLink = "";
				// do anything with this variable to silence warning about not using it
			}
		}
		public static void OpenInWin(string path)
		{
			bool openInsidesOfFolder = false;
			// try windows
			string winPath = path.Replace("/", "\\");
			// windows explorer doesn't like forward slashes

			if (System.IO.Directory.Exists(winPath))
			// if path requested is a folder, automatically open insides of that folder
			{
				openInsidesOfFolder = true;
			}

			try
			{
				if (!openInsidesOfFolder)
				{
					Diag.Process.Start(winPath);
				}
				else
				{
					Diag.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
				}
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open win explorer in mac 
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = "";
				// do anything with this variable to silence warning about not using it
			}
		}
		public static void Open(string path)
		{
			if (PlatformHelper.IsInWinOS)
			{
				OpenInWin(path);
			}
			else if (PlatformHelper.IsInMacOS)
			{
				OpenInMac(path);
			}
			else
			// couldn't determine OS 		
			{
				OpenInWin(path); OpenInMac(path);
			}
		}
	}

	public static class FileModifiedHelper
	{
		public static DateTime GetLastModified(string path)
		{
			//Unsure
			DateTime dt = DateTime.MaxValue;
			try
			{
				dt = File.GetLastWriteTime(path);
				//Debug.Log("Path: [" + path + "]\n[" + dt.ToString() + "]\n");
				return dt;
			}
			catch (Exception e)
			{
				Debug.LogError("Exception Thrown while getting last modified\n" + e.Message + "\n");
			}

			return dt;
		}
	}
}