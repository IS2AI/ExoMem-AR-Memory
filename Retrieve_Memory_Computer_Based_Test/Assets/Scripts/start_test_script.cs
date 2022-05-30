//
//Created by Zhanat Makhataeva
//Date: May 2022
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
using System.IO;
using DrawingUtils;
using Photo;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;

public class start_test_script : MonoBehaviour
{
	private string file_name;
	public List<string> subjectScenes = new List<string>{} ; 
    
    void Start()
    {
        string textFile = "Responses/file_name_all.txt";  
		string[] lines = File.ReadAllLines(textFile);  
		file_name = lines[0];
		string[] linesQ = File.ReadAllLines(file_name); 
        
        foreach (string line in linesQ)
		{
			subjectScenes.Add(line);
		}
		GameObject.Find("nextB").GetComponent<Button>().onClick.AddListener(TaskOnClick_next);
    }
	
    void TaskOnClick_next()
    {
		Debug.Log("You have clicked the button!");
		SceneNext();
    }

	public void SceneNext()
    {
        SceneManager.LoadScene(subjectScenes[5]);
    }
	
}
