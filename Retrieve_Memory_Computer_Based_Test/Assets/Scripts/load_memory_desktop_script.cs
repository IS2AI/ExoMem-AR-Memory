//
//Created by Zhanat Makhataeva
//Date: May 2022
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System;

using System.Threading.Tasks;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;

using DrawingUtils;
using Photo;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using System.Runtime.InteropServices;

#if !UNITY_EDITOR
    using Windows.Storage.Streams;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using System.Threading.Tasks;
    using Windows.Storage.Pickers;
#endif

public class load_memory_desktop_script : MonoBehaviour
{
    
	public int fLinesL;
	public int fLinesLO;
	
	public string subject_day;
	public string subject_ar;
	public string subject_ar_t;
	public string subject_id;
	
	private int x_val;
	private int z_val;
	private int y_val;
	private  int x_valO;
	private  int z_valO;
	private  int y_valO;
	
	private string valObj;
	
	public List<String> myListObj = new List<String>();
	
	public List<String> fLinesLO_list = new List<String>();
   
	private string valueLineO;
	private string valueLine;
	
	private int _filterCountObj4th;
	private int _filterCountObj5th;
	private int _filterCountObj6th;
	
	public string q_true;
	private string _labelPath;
	public string _labelObj;
	
	public Transform prefab;
	public Transform prefabObj;
	public Color mycolor_1;  
	public Color mycolor_2; 
	public Color mycolor_3; 
	public Color mycolor_4; 	

	public string dir_name;
	public string file_nameO;
	public string file_nameP;
	
	private string _labelObjName;
	private string _labelObjNameColor;
	
    void Start()
    {
        GameObject.Find("Sphere_obj_4th_1").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_4th_2").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_4th_3").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_4th_4").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_5th_1").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_5th_2").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_5th_3").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_5th_4").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_6th_1").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_6th_2").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_6th_3").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Sphere_obj_6th_4").GetComponent<Renderer>().enabled = false;
		
		_filterCountObj4th = 1;
		_filterCountObj5th = 1;
		_filterCountObj6th = 1;
	
		var inputD = GameObject.Find("InputField_Day").GetComponent<InputField>();
        var seD= new InputField.SubmitEvent();
        seD.AddListener(SubmitNameD);
        inputD.onEndEdit = seD;
		
		var inputA = GameObject.Find("InputField_AR").GetComponent<InputField>();
        var seA= new InputField.SubmitEvent();
        seA.AddListener(SubmitNameA);
        inputA.onEndEdit = seA;
		
		var inputI = GameObject.Find("InputField_id").GetComponent<InputField>();
        var seI= new InputField.SubmitEvent();
        seI.AddListener(SubmitNameI);
        inputI.onEndEdit = seI;
		
		GameObject.Find("BSave").GetComponent<Button>().onClick.AddListener(TaskOnClick_Save);
		GameObject.Find("BRead").GetComponent<Button>().onClick.AddListener(TaskOnClick_Read);
		GameObject.Find("q1").GetComponent<Button>().onClick.AddListener(TaskOnClick_q1);
		GameObject.Find("q2").GetComponent<Button>().onClick.AddListener(TaskOnClick_q2);
		GameObject.Find("q3").GetComponent<Button>().onClick.AddListener(TaskOnClick_q3);
		GameObject.Find("q4").GetComponent<Button>().onClick.AddListener(TaskOnClick_q4);
		GameObject.Find("q5").GetComponent<Button>().onClick.AddListener(TaskOnClick_q5);
		GameObject.Find("q6").GetComponent<Button>().onClick.AddListener(TaskOnClick_q6);
		GameObject.Find("q7").GetComponent<Button>().onClick.AddListener(TaskOnClick_q7);
		GameObject.Find("q8").GetComponent<Button>().onClick.AddListener(TaskOnClick_q8);
		GameObject.Find("q9").GetComponent<Button>().onClick.AddListener(TaskOnClick_q9);
		GameObject.Find("q10").GetComponent<Button>().onClick.AddListener(TaskOnClick_q10);
		
    }
	
	private void SubmitNameD(string arg0)
     {
		 subject_day = arg0;
     }
	 
	 private void SubmitNameA(string arg0)
     {
		 subject_ar = arg0;
     }
	 
	 private void SubmitNameI(string arg0)
     {
		 subject_id = arg0;
     }
	 
	void TaskOnClick_Read()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		
		dir_name = "Responses" + "/Day_" + subject_day + "/AR_" + subject_ar + "/Subject_" + subject_id;
		file_nameO = dir_name + "/dataObj.txt";
		file_nameP = dir_name + "/dataPath.txt";
		
		string textP = file_nameP;  
		string[] fLines = File.ReadAllLines(textP);  
        fLinesL = fLines.Length;
		

        string textO = file_nameO; 
        string[] fLinesO = File.ReadAllLines(textO);  
        fLinesLO = fLinesO.Length;
		
		for (int i = 0; i < fLinesLO; i = i + 1)
        {
            valueLineO = fLinesO[i];
            string[] valuesO = Regex.Split(valueLineO, " , "); // your splitter here
            x_valO = int.Parse(valuesO[1]) ;
            z_valO = int.Parse(valuesO[2]) ;
            y_valO = int.Parse(valuesO[3]) ;
			_labelObj = valuesO[0] ;
			
			if (y_valO==3)
			{
				
				_labelObjName = "Text_4th_" + _filterCountObj4th.ToString();
				_labelObjNameColor = "Sphere_obj_4th_" + _filterCountObj4th.ToString();
				
				GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
				
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -26), Quaternion.identity);
				newPointObj.localScale = new Vector3(5, 5, 5);
				newPointObj.SetParent(GameObject.Find("MapCanvas4F").transform, false);
				newPointObj.name = _labelObj; 
				myListObj.Add(_labelObj);
				if (_filterCountObj4th==1)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_1;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_1;
				}
				
				else if (_filterCountObj4th==2)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_2;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_2;
				}
				
				else if (_filterCountObj4th==3)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_3;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_3;
				}
				
				else if (_filterCountObj4th==4)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_4;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_4;
				}
				
				_filterCountObj4th += 1;
			
			}
			
			else if (y_valO==7)
			{
				_labelObjName = "Text_5th_" + _filterCountObj5th.ToString();
				_labelObjNameColor = "Sphere_obj_5th_" + _filterCountObj5th.ToString();
				GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
			
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -26), Quaternion.identity);
				newPointObj.localScale = new Vector3(5, 5, 5);
				newPointObj.SetParent(GameObject.Find("MapCanvas5F").transform, false);
				newPointObj.name = _labelObj; 
				myListObj.Add(_labelObj);
				if (_filterCountObj5th==1)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_1;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_1;
				}
				
				else if (_filterCountObj5th==2)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_2;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_2;
				}
				
				else if (_filterCountObj5th==3)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_3;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_3;
				}
				
				else if (_filterCountObj5th==4)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_4;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_4;
				}
				
				_filterCountObj5th += 1;
			}
			else if (y_valO==11)
			{
				_labelObjName = "Text_6th_" + _filterCountObj6th.ToString();
				GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
				_labelObjNameColor = "Sphere_obj_6th_" + _filterCountObj6th.ToString();
			
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -40), Quaternion.identity);
				newPointObj.localScale = new Vector3(5, 5, 5);
				newPointObj.SetParent(GameObject.Find("MapCanvas6F").transform, false);
				newPointObj.name = _labelObj; 
				myListObj.Add(_labelObj);
				if (_filterCountObj6th==1)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_1;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_1;
				}
				
				else if (_filterCountObj6th==2)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_2;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_2;
				}
				
				else if (_filterCountObj6th==3)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_3;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_3;
				}
				
				else if (_filterCountObj6th==4)
				{
					GameObject.Find(_labelObj).GetComponent<Renderer>().material.color = mycolor_4;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().enabled = true;
					GameObject.Find(_labelObjNameColor).GetComponent<Renderer>().material.color = mycolor_4;
				}
				
				_filterCountObj6th += 1;
			}
			
			
		}
		for (int i = 0; i < fLinesL; i = i + 1)
        {
            valueLine = fLines[i];
            string[] values = Regex.Split(valueLine, " , "); // your splitter here
            x_val = int.Parse(values[0]) ;
            z_val = int.Parse(values[1]) ;
            y_val = int.Parse(values[2]) ;
			_labelPath = x_val.ToString() + " , " + z_val.ToString() + " , " + y_val.ToString();
            
			if (y_val==3)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -26), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas4F").transform, false);
				newButton.name = _labelPath; 
				
				
			}
			else if (y_val==7)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -26), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas5F").transform, false);
				newButton.name = _labelPath; 
				
			}
			else if (y_val==11)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -40), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas6F").transform, false);
				newButton.name = _labelPath; 
				
			}
           
		}
		
		
    }
	
	void TaskOnClick_Save()
    {
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\retrieved_memory_scene" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
       
    }
	
	void TaskOnClick_q1()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("teddy bear").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q1" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	void TaskOnClick_q2()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("bottle").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q2" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	void TaskOnClick_q3()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("stop sign").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q3" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	void TaskOnClick_q4()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("clock").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q4" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	void TaskOnClick_q5()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("umbrella").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q5" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
	void TaskOnClick_q6()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("mouse").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q6" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
	void TaskOnClick_q7()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("pottedplant").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q7" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
	void TaskOnClick_q8()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("keyboard").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q8" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
	void TaskOnClick_q9()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("suitcase").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id + "\\RetrievedMemory\\RetrievedMemory_q9" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
	void TaskOnClick_q10()
    {
		foreach (var myObj in myListObj)
		{
			GameObject.Find(myObj).GetComponent<Renderer>().enabled = false;
		}
		GameObject.Find("cup").GetComponent<Renderer>().enabled = true;
		
		q_true = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subject_id  + "\\RetrievedMemory\\RetrievedMemory_q10" + ".png";
        ScreenCapture.CaptureScreenshot(q_true);
        
    }
	
}
