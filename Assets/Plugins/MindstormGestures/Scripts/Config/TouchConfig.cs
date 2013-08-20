/*
Unity3d-TUIO connects touch tracking from a TUIO to objects in Unity3d.

Copyright 2011 - Mindstorm Limited (reg. 05071596)

Author - Simon Lerpiniere

This file is part of Unity3d-TUIO.

Unity3d-TUIO is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Unity3d-TUIO is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser Public License for more details.

You should have received a copy of the GNU Lesser Public License
along with Unity3d-TUIO.  If not, see <http://www.gnu.org/licenses/>.

If you have any questions regarding this library, or would like to purchase 
a commercial licence, please contact Mindstorm via www.mindstorm.com.
*/

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Xml;
using System.IO;

using Mindstorm.Gesture.Config;

/// <summary>
/// Wraps the TouchHandlerConfig to provide editing of touch configuration from the inspector.
/// </summary>
public class TouchConfig : MonoBehaviour 
{
	public TouchHandlerConfig Config;
	
	public string ConfigFileName = @"GestureConfig.xml";
	public string EditorConfigFileName = @"GestureConfig_Editor.xml";
	public bool LoadFromFile = true;
	public bool ShowMouseCursor = true;
	
	void Awake()
	{
		Screen.showCursor = false;
		if (Application.isEditor) ConfigFileName = EditorConfigFileName;
	}
	
	void Start()
	{
#if WINDOWS_STORE
		LoadFromFile = false;
#else
		// Load from file if set
		if (LoadFromFile) 
		{
			loadConfig();
		}		
		
		// Override from command line
		string[] args = Environment.GetCommandLineArgs();
		
		if (args.Contains("/showcursor"))
		{
			ShowMouseCursor = true;
		}
		
		int i = Array.IndexOf(args, "/tracking");
		if (i != -1 && args.Length > i + 1)
		{
			ParseType(args[i+1]);
		}
		
		// Initialise
		Init();
#endif
	}
	
	void Update()
	{
		if (Config.InputTypeChanged) 
		{
			Init();
		}
	}
	
	void Init()
	{
		Config.Initialise();
		Screen.showCursor = ShowMouseCursor;
	}
	
#if !WINDOWS_STORE
	string getAppPath()
	{
		DirectoryInfo path = new DirectoryInfo(Application.dataPath);
	    if (Application.platform == RuntimePlatform.OSXPlayer) 
		{
	        path = path.Parent.Parent;
	    }
	    else if (Application.platform == RuntimePlatform.WindowsPlayer) 
		{
	        path = path.Parent;
	    }
		return path.ToString();
	}
	
	void loadConfig()
	{
		string configFile = Path.Combine(getAppPath(), ConfigFileName);
		
		if (!File.Exists(configFile))
		{
			Debug.LogWarning("Config file not found: " + configFile);
			return;
		}
		
		XmlDocument config = new XmlDocument();
		config.Load(configFile);
		parseConfig(config);
	}
	
	void parseConfig(XmlDocument config)
	{
		XmlElement root = config.DocumentElement;
		
		XmlNode inputNode = root.SelectSingleNode("//Configuration/Input");
		if (inputNode != null)
		{
			ParseType(inputNode.Attributes["Type"].Value);
			ParseCursor(inputNode.Attributes["ShowCursor"].Value);
		}
		else
		{
			Debug.LogWarning("Input node not found in config, defaulting to " + Config.InputType.ToString());
		}
	}
	
	void ParseType(string s)
	{
		try
		{
			Config.InputType = (TouchHandlerConfig.InputTypeEnum)System.Enum.Parse(typeof(TouchHandlerConfig.InputTypeEnum), s, true);
		}
		catch
		{
			Debug.LogWarning(s + " is not a valid Input type, defaulting to " + Config.InputType.ToString());
		}
	}
	
	void ParseCursor(string s)
	{
		try
		{
			ShowMouseCursor = bool.Parse(s);
		}
		catch
		{
			Debug.LogWarning(s + " is not a valid Input value for ShowCursor, defaulting to " + ShowMouseCursor.ToString());
		}
	}
#endif
}
