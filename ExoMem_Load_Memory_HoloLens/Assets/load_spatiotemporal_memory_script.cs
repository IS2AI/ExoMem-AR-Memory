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

#if ENABLE_WINMD_SUPPORT
using Windows.UI.Xaml;
using HoloLensForCV;
using YoloRuntime;
#endif

#if !UNITY_EDITOR
    using Windows.Storage.Streams;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using System.Threading.Tasks;
    using Windows.Storage.Pickers;
#endif

public class load_spatiotemporal_memory_script : MonoBehaviour
{
	private int fLinesLV_1;
    private int fLinesLV_2;
   
    public TextAsset textFileV_1;
    public TextAsset textFileV_2;
	
	private string valueLineV_1;
    private string valueLineV_2;
	
	private int x_val;
	private int z_val;
	private int y_val;
	private  int x_valO;
	private  int z_valO;
	private  int y_valO;
	
	public List<String> myListObj = new List<String>();
	
	private int _filterCountObj4th;
	private int _filterCountObj5th;
	private int _filterCountObj6th;
	
	private string _labelPath;
	private string _labelObj;
	
	public Transform prefab;
	public Transform prefabObj;
	public Color mycolor_1;  
	public Color mycolor_2; 
	public Color mycolor_3; 
	public Color mycolor_4; 	

	private string _labelObjName;
	private string _labelObjNameColor;
	
	private string textObj = "";
	private string textPath = "";
   
	// Start is called before the first frame update
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
		
		string textV_1 = textFileV_1.text;  //this is the content as string
		string[] fLinesV_1 = Regex.Split(textV_1, "\r\n");
        fLinesLV_1 = fLinesV_1.Length;

        string textV_2 = textFileV_2.text;  //this is the content as string
		string[] fLinesV_2 = Regex.Split(textV_2, "\r\n");
        fLinesLV_2 = fLinesV_2.Length;
		
		for (int i = 0; i < fLinesV_2.Length -1; i = i + 1)
        {
            valueLineV_2 = fLinesV_2[i];
            string[] valuesV_2 = Regex.Split(valueLineV_2, " , "); // your splitter here
			
			x_val = int.Parse(valuesV_2[0]) ;
            z_val = int.Parse(valuesV_2[1]) ;
            y_val = int.Parse(valuesV_2[2]) ;
			_labelPath = x_val.ToString() + " , " + z_val.ToString() + " , " + y_val.ToString();
            
			if (y_val==3)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -9), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas4F").transform, false);
				newButton.name = _labelPath; 
			}
			
			else if (y_val==7)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -9), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas5F").transform, false);
				newButton.name = _labelPath; 
			}
			
			else if (y_val==11)
			{
				Transform newButton = Instantiate(prefab, new Vector3(x_val, z_val, -9), Quaternion.identity);
				newButton.localScale = new Vector3(2, 2, 2);
				newButton.SetParent(GameObject.Find("MapCanvas6F").transform, false);
				newButton.name = _labelPath; 
			}
		}	
	
		for (int i = 0; i < fLinesV_1.Length - 1; i = i + 1)
        {
            valueLineV_1 = fLinesV_1[i];
            string[] valuesV_1 = Regex.Split(valueLineV_1, " , "); // your splitter here
			
			x_valO = int.Parse(valuesV_1[1]) ;
            z_valO = int.Parse(valuesV_1[2]) ;
            y_valO = int.Parse(valuesV_1[3]) ;
			_labelObj = valuesV_1[0] ;//+ x_valO.ToString() + " , " + z_valO.ToString() + " , " + y_valO.ToString();
			
			if (y_valO==3)
			{
				_labelObjName = "Text_4th_" + _filterCountObj4th.ToString();
				_labelObjNameColor = "Sphere_obj_4th_" + _filterCountObj4th.ToString();
				
				GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
				
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -9), Quaternion.identity);
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
			
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -9), Quaternion.identity);
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
			
				Transform newPointObj = Instantiate(prefabObj, new Vector3(x_valO, z_valO, -9), Quaternion.identity);
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
	
	}

#if ENABLE_WINMD_SUPPORT	
	private async void ReadFromTxtObj()
	{
		StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
		
		string desiredName = "dataObj.txt";
		StorageFile sampleFile = await localFolder.GetFileAsync(desiredName);	
		var bufferObj = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
		
		using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(bufferObj))
		{
			textObj = dataReader.ReadString(bufferObj.Length);
		}
		
	}
	
	private async void ReadFromTxtPath()
	{
		StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
		
		string desiredName = "dataPath.txt";
		StorageFile sampleFile = await localFolder.GetFileAsync(desiredName);	
		var bufferObj = await Windows.Storage.FileIO.ReadBufferAsync(sampleFile);
		
		using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(bufferObj))
		{
			textPath = dataReader.ReadString(bufferObj.Length);
		}
		
	}
	
#endif	
}
