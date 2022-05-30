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


public class trial_question_script : MonoBehaviour
{
	
    void Start()
    {
		GameObject.Find("notB").GetComponent<Button>().onClick.AddListener(TaskOnClick_not);
		GameObject.Find("nextB").GetComponent<Button>().onClick.AddListener(TaskOnClick_next);
		GameObject.Find("againB").GetComponent<Button>().onClick.AddListener(TaskOnClick_again);
    }
	
    void TaskOnClick_next()
    {
		Debug.Log("You have clicked the button!");
		
		ScreenCapture.CaptureScreenshot("start_test_scene");
		
		SceneNext();
    }
	
	void TaskOnClick_again()
    {
		SceneAgain();
		
	}	
	
	public void SceneNext()
    {
        SceneManager.LoadScene("start_test_scene");
    }
	
	public void SceneAgain()
    {
		ScreenCapture.CaptureScreenshot("trial_additional_trial.png");
		SceneManager.LoadScene("trial_question_scene");
    }
	
	void TaskOnClick_not()
    {
        GameObject.Find("Text_not").GetComponent<Text>().text = "Didn't see!";
    }
	
	
}
