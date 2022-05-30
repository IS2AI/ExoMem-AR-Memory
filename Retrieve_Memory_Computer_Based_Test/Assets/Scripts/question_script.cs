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


public class question_script : MonoBehaviour
{
	public string q_id;
	public string q_id_add;
	public string m_id;
	private string q_num ;
	private int save_num ;
	private int load_num ;
	private string file_name;
	public List<string> subjectScenes = new List<string>{};
	
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
		
		q_num = subjectScenes[5];
		q_id = "Responses\\Day_" + subjectScenes[1] + "\\AR_" + subjectScenes[2] + "\\Subject_" + subjectScenes[3] +  "\\ParticipantResponse\\Question_" + q_num + ".png";
		subjectScenes.RemoveAt(5);
	
		GameObject.Find("notB").GetComponent<Button>().onClick.AddListener(TaskOnClick_not);
		GameObject.Find("nextB").GetComponent<Button>().onClick.AddListener(TaskOnClick_next);
		GameObject.Find("againB").GetComponent<Button>().onClick.AddListener(TaskOnClick_again);
		
    }
	
    void TaskOnClick_next()
    {
        
		Debug.Log("You have clicked the button!");
		
		StreamWriter sWriter1;
		sWriter1 = new StreamWriter(file_name);
		
		for (int i = 0; i <subjectScenes.Count; i++) 
		{
			sWriter1.WriteLine(subjectScenes[i]);
		}
		
		sWriter1.Close();  
		
		ScreenCapture.CaptureScreenshot(q_id);
		
		SceneNext();
    }
	
	void TaskOnClick_again()
    {
		SceneAgain();
	}	
	
	public void SceneAgain()
    {
		q_id_add = q_id +"_additional_trial.png";
		ScreenCapture.CaptureScreenshot(q_id_add);
		SceneManager.LoadScene(q_num);
    }

	public void SceneNext()
    {
        SceneManager.LoadScene(subjectScenes[5]);
    }
	
	void TaskOnClick_not()
    {
        GameObject.Find("Text_not").GetComponent<Text>().text = "Didn't see!";
    }
	
}
