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

public class input_user_data_script : MonoBehaviour
{
	public string subject_glasses;
	public string subject_age_gender;
	public string subject_day;
	public string subject_ar;
	public string subject_id;
	private string q_order = " ";
	public string q_id;
	private string dir_name;
	private string dir_nameAns;
	private string dir_nameTrue;
	private string file_name;
	private string file_nameRTime;
	private string file_nameERate;
	private string file_nameInfo;
	
	private string file_name_all;
	public List<string> randomScenes = new List<string> {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15"};
	public List<string> subjectScenes = new List<string> {}; 
	public List<string> subjectRTime = new List<string> {};
	public List<string> subjectERate = new List<string> {};
	public List<string> subjectInfo = new List<string> {};
	
    void Start()
    {
		var inputN = GameObject.Find("InputField_Glasses").GetComponent<InputField>();
        var seN= new InputField.SubmitEvent();
        seN.AddListener(SubmitNameN);
        inputN.onEndEdit = seN;
		
		var inputL = GameObject.Find("InputField_Age_Gender").GetComponent<InputField>();
        var seL= new InputField.SubmitEvent();
        seL.AddListener(SubmitNameL);
        inputL.onEndEdit = seL;
		
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
	
		subjectScenes.Add(subject_day);
		subjectScenes.Add(subject_ar);
		subjectScenes.Add(subject_id);
		subjectScenes.Add("trial_question_scene");
		
		subjectRTime.Add(subject_day);
		subjectRTime.Add(subject_ar);
		subjectRTime.Add(subject_id);
		
		subjectERate.Add(subject_day);
		subjectERate.Add(subject_ar);
		subjectERate.Add(subject_id);
		
		subjectInfo.Add(subject_glasses);
		subjectInfo.Add(subject_age_gender);
		subjectInfo.Add(subject_day);
		subjectInfo.Add(subject_ar);
		subjectInfo.Add(subject_id);
		
		
		for (int i = 0; i <randomScenes.Count; i++) 
		{
			string temp = randomScenes[i];
			int randomIndex = Random.Range(i, randomScenes.Count);
			randomScenes[i] = randomScenes[randomIndex];
			randomScenes[randomIndex] = temp;
		}
		
		for (int i = 0; i <randomScenes.Count; i++) 
		{
			subjectScenes.Add(randomScenes[i]);
		}
		subjectScenes.Add("end_test_scene");
		
		for (int i = 0; i <randomScenes.Count; i++) 
		{
			subjectRTime.Add("-");
		}
		
		for (int i = 0; i <randomScenes.Count; i++) 
		{
			subjectERate.Add("-");
		}
		
		GameObject.Find("nextB").GetComponent<Button>().onClick.AddListener(TaskOnClick_next);
		
    }
	
	 private void SubmitNameN(string arg0)
     {
         Debug.Log(arg0);
		 subject_glasses = arg0;
     }
	 
	 private void SubmitNameL(string arg0)
     {
         Debug.Log(arg0);
		 subject_age_gender = arg0;
     }
	 
	 private void SubmitNameD(string arg0)
     {
         Debug.Log(arg0);
		 subject_day = arg0;
     }
	 
	 private void SubmitNameA(string arg0)
     {
         Debug.Log(arg0);
		 subject_ar = arg0;
     }
	 
	 private void SubmitNameI(string arg0)
     {
         Debug.Log(arg0);
		 subject_id = arg0;
     }
	
    void TaskOnClick_next()
    {
        subjectScenes[0] = subject_day;
		subjectScenes[1] = subject_ar;
		subjectScenes[2] = subject_id;
		
		
		subjectRTime[0] = subject_day;
		subjectRTime[1] = subject_ar;
		subjectRTime[2] = subject_id;
		
		subjectERate[0] = subject_day;
		subjectERate[1] = subject_ar;
		subjectERate[2] = subject_id;
		
		subjectInfo[0] = subject_glasses;
		subjectInfo[1] = subject_age_gender;
		subjectInfo[2] = subject_day;
		subjectInfo[3] = subject_ar;
		subjectInfo[4] = subject_id;
		
		dir_name = "Responses" + "/Day_" + subject_day + "/AR_" + subject_ar + "/Subject_" + subject_id;
		dir_nameAns = dir_name + "//ParticipantResponse";
		dir_nameTrue = dir_name + "//RetrievedMemory";
		
		file_name = dir_name + "/Questions_Order" + "_Day_" + subject_day + "_AR_" + subject_ar + "_Subject_" + subject_id + ".txt";
		
		file_nameRTime = dir_name + "/Response_Times" + "_Day_" + subject_day + "_AR_" + subject_ar + "_Subject_" + subject_id  + ".txt";
		file_nameERate = dir_name + "/Error_Rate" + "_Day_" + subject_day + "_AR_" + subject_ar + "_Subject_" + subject_id  + ".txt";
		
		file_nameInfo = dir_name + "/Subject_Info" + "_Day_" + subject_day + "_AR_" + subject_ar + "_Subject_" + subject_id  + ".txt";
		
		file_name_all = "Responses/file_name_all.txt";
		var folder = Directory.CreateDirectory(dir_name);
		
		var folderAns = Directory.CreateDirectory(dir_nameAns);
		var folderTrue = Directory.CreateDirectory(dir_nameTrue);
		
		for (int j = 0; j <subjectScenes.Count; j++) 
		{
			q_order = q_order + subjectScenes[j] + " , ";
		}
		
		
        StreamWriter sWriter1;
        if (!File.Exists(file_name))
        {
            sWriter1 = File.CreateText(file_name);
        }
        else
        {
            sWriter1 = new StreamWriter(file_name);
        }

        sWriter1.WriteLine("Questions Order for day " + subject_day +  ", AR " + subject_ar + ", Subject " + subject_id  + " is: " + q_order);
		
		for (int i = 0; i <subjectScenes.Count; i++) 
		{
			sWriter1.WriteLine(subjectScenes[i]);
		}
		
		sWriter1.Close();
		
		
		StreamWriter sWriter2;
        if (!File.Exists(file_name))
        {
            sWriter2 = File.CreateText(file_name_all);
        }
        else
        {
            sWriter2 = new StreamWriter(file_name_all);
        }

        sWriter2.WriteLine(file_name);
		sWriter2.WriteLine(file_nameRTime);
		sWriter2.WriteLine(file_nameERate);
		sWriter2.WriteLine(file_nameInfo);
		sWriter2.Close();
		
		
		StreamWriter sWriter3;
        if (!File.Exists(file_nameRTime))
        {
            sWriter3 = File.CreateText(file_nameRTime);
        }
        else
        {
            sWriter3 = new StreamWriter(file_nameRTime);
        }

        sWriter3.WriteLine("Response Times for day " + subject_day +  ", AR " + subject_ar  + ", Subject " + subject_id);
		
		for (int i = 0; i <subjectRTime.Count; i++) 
		{
			sWriter3.WriteLine(subjectRTime[i]);
		}
		
		sWriter3.Close();
		
		
		StreamWriter sWriter4;
        if (!File.Exists(file_nameERate))
        {
            sWriter4 = File.CreateText(file_nameERate);
        }
        else
        {
            sWriter4 = new StreamWriter(file_nameERate);
        }

        sWriter4.WriteLine("Error Rate for day " + subject_day +  ", AR " + subject_ar  + ", Subject " + subject_id);
		
		for (int i = 0; i <subjectERate.Count; i++) 
		{
			sWriter4.WriteLine(subjectERate[i]);
		}
		
		sWriter4.Close();
		
		
		StreamWriter sWriter7;
        if (!File.Exists(file_nameInfo))
        {
            sWriter7 = File.CreateText(file_nameInfo);
        }
        else
        {
            sWriter7 = new StreamWriter(file_nameInfo);
        }

        sWriter7.WriteLine("Subject information " + subject_day +  ", AR " + subject_ar  + ", Subject_id " + subject_id);
		
		for (int i = 0; i <subjectInfo.Count; i++) 
		{
			sWriter7.WriteLine(subjectInfo[i]);
		}
		
		sWriter7.Close();
		
	
		q_id = "Responses\\Day_" + subject_day + "\\AR_" + subject_ar + "\\Subject_" + subjectScenes[2]   + "\\user_data_scene" + ".png";
		Debug.Log("You have clicked the button!");
		ScreenCapture.CaptureScreenshot(q_id);
		SceneNext();
    }

	public void SceneNext()
    {
        SceneManager.LoadScene(subjectScenes[3]);
    }
	
	
	
}
