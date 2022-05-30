//
//Created by Zhanat Makhataeva
//Date: May 2022
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;


using System.Linq;
using System.IO;

using System.Diagnostics;
using System.Text.RegularExpressions;

using DrawingUtils;
using Photo;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;


public class record_time_script : MonoBehaviour
{
    private float timeNowStart;
	private string q_num ;
	private string file_name;
	private string file_nameRTime;
	public bool file_save;
	
	public List<string> subjectScenes = new List<string>{};
	public List<string> subjectRTime = new List<string>{};
	
    void Start()
    {
        timeNowStart = Time.realtimeSinceStartup;
		GameObject.Find("Text_end").GetComponent<Text>().text = timeNowStart.ToString();
		GameObject.Find("Text_start").GetComponent<Text>().text = timeNowStart.ToString();
		file_save = false;
		
		string textFile = "Responses/file_name_all.txt";  
		string[] lines = File.ReadAllLines(textFile);  
		file_name = lines[0];
		file_nameRTime = lines[1];
		string[] linesQ = File.ReadAllLines(file_name); 
		
		string[] linesRTime = File.ReadAllLines(file_nameRTime); 
        
        foreach (string line in linesQ)
		{
			subjectScenes.Add(line);
		}
		
		foreach (string lineRT in linesRTime)
		{
			subjectRTime.Add(lineRT);
		}
		
		q_num = subjectScenes[5];
		
    }

    // Update is called once per frame
    void Update()
    {
        float timeNowEnd = Time.realtimeSinceStartup;
		float timeRT = timeNowEnd  - timeNowStart;
		
		for (int i = 4; i <subjectRTime.Count; i++) 
		{	
			int my_int = i-3;
			if(q_num.Equals(my_int.ToString()))
			{
				
				subjectRTime[i] = timeRT.ToString();
				file_save = true;
			}	
			
		}
		if (file_save == true)
		{
			StreamWriter sWriter1;
			sWriter1 = new StreamWriter(file_nameRTime);
			
			for (int i = 0; i <subjectRTime.Count; i++) 
			{
				sWriter1.WriteLine(subjectRTime[i]);
			}
			
			sWriter1.Close();  
			
			file_save = false;
		}
		
		GameObject.Find("Text_end").GetComponent<Text>().text = timeNowEnd.ToString();
		GameObject.Find("Text_rt").GetComponent<Text>().text = ((int)timeRT).ToString();
		
		
    }
}
