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

#if NETFX_CORE
using System.Xml.Linq;
#endif

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
	
	IEnumerator Start()
	{
		// Load from file if set
		if (LoadFromFile) 
		{
			yield return StartCoroutine(loadConfig());
		}		

#if !NETFX_CORE
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
#endif
		
		// Initialise
		Init();
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
	
	IEnumerator loadConfig()
	{
		string configFile = Application.streamingAssetsPath + "/" + ConfigFileName;
		
		string data = string.Empty;
		if (configFile.Contains("://"))
		{
			var www = new WWW (configFile);
			yield return www;
			data = www.text;
		}
#if !NETFX_CORE		
		else
		{
			if (!File.Exists(configFile))
			{
				Debug.LogWarning("Config file not found: " + configFile);
				yield break;
			}
				
			data = File.ReadAllText(configFile);
		}
		
		if (data == string.Empty)
		{
			Debug.LogWarning("No GestureConfig information found");
			yield break;
		}
#endif

#if !NETFX_CORE
		XmlDocument config = new XmlDocument();
		config.LoadXml(data);
		parseConfig(config);
#else
		XDocument config = XDocument.Load(configFile);
		parseConfig(config);
#endif
	}

#if !NETFX_CORE	
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
#else
	void parseConfig(XDocument config)
	{
		string t = config.Root.Element("Input").Attribute("Type").Value;
		string m = config.Root.Element("Input").Attribute("ShowCursor").Value;
		ParseType(t);
		ParseCursor(m);
	}
#endif
	
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
}
