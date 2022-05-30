//
//Created by zhanat Makhataeva
//Date: May 2022
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#if ENABLE_WINMD_SUPPORT
using Windows.UI.Xaml;
using HoloLensForCV;
using YoloRuntime;
#endif

using UnityEngine;
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

#if !UNITY_EDITOR
    using Windows.Storage.Streams;
    using Windows.Graphics.Imaging;
    using Windows.Storage;
    using System.Threading.Tasks;
    using Windows.Storage.Pickers;
#endif

namespace CreateSpatiotemporalMemoryHoloLens
{
    public class create_spatiotemporal_memory : MonoBehaviour
    {
        public int deviceType = 1;

        private GestureRecognizer _gestureRecognizer;
        private GestureRecognizer _gestureRecognizerTwo;
        private bool tapped;
		
        int _tapCount;
        int _filterCountObj;
		int _filterCountObj4th;
		int _filterCountObj5th;
		int _filterCountObj6th;
        int _tapCountObj;
        string _input;
        string _labelObj;
		string _labelPath;
		string _labelPathS;
        string _labelObjName;
		string _labelObjNameColor;
        public Transform prefab;
        public Transform prefabObj;
		public Color mycolor_1;  
		public Color mycolor_2; 
		public Color mycolor_3; 
		public Color mycolor_4; 			
		
        public int zObj = 0;
        public int xObj = 0;
		public int yObj = 0;
		
		public string saveInfoPath = "";
		public string saveInfoObj = "";

        private string[] _labels = {
            "person","bicycle","car","motorbike","aeroplane","bus","train","truck","boat","traffic light","fire hydrant","stop sign","parking meter",
             "bench","bird","cat","dog","horse","sheep","cow","elephant","bear","zebra","giraffe","backpack","umbrella","handbag","tie","suitcase",
             "frisbee","skis","snowboard","sports ball","kite","baseball bat","baseball glove","skateboard","surfboard","tennis racket","bottle",
             "wine glass","cup","fork","knife","spoon","bowl","banana","apple","sandwich","orange","broccoli","carrot","hot dog","pizza","donut",
             "cake","chair","sofa","pottedplant","bed","diningtable","toilet","tvmonitor","laptop","mouse","remote","keyboard","cell phone","microwave",
             "oven","toaster","sink","refrigerator","book","clock","vase","scissors","teddy bear","hair drier","toothbrush"
        };

        private string[] firstlistArray = {"teddy bear","bottle",
             "stop sign","clock","umbrella","mouse","pottedplant","keyboard",
             "suitcase","cup"
        };

        private string[] markerlistArray = {"1","2","3",
             "4","5","6","7","8","9","10"
        };
		
        private List<String> firstlist = new List<String>();
		private List<String> staticlist = new List<String>();
        private List<String> objList = new List<String>();
		private List<String> objList4thF = new List<String>();
		private List<String> objList5thF = new List<String>();
		private List<String> objList6thF = new List<String>();
		private List<String> objListS = new List<String>();
		private List<String> pathList = new List<String>();
		private List<String> pathList4thF = new List<String>();
		private List<String> pathList5thF = new List<String>();
		private List<String> pathList6thF = new List<String>();
		private List<String> pathListS = new List<String>();
        

        private bool _holoLensMediaFrameSourceGroupStarted;
        TcpListener listener;
        TcpClient client;

        TcpListener listenerZ;
        TcpClient clientZ;
        
        public enum SensorTypeUnity
        {
            Undefined = -1,
            PhotoVideo = 0
        }

        public SensorTypeUnity sensorTypePv;

#if ENABLE_WINMD_SUPPORT
        
        private MediaFrameSourceGroupType _selectedMediaFrameSourceGroupType = MediaFrameSourceGroupType.PhotoVideoCamera;
        private SensorFrameStreamer _sensorFrameStreamer;
        private SpatialPerception _spatialPerception;
        private HoloLensForCV.DeviceType _deviceType;
        private MediaFrameSourceGroup _holoLensMediaFrameSourceGroup;
        private SensorType _sensorType;
#endif
        PhotoCapture photoCaptureObject = null;
        Texture2D targetTexture = null;

        private int count;

        async void Start()
        {
            count = 0;
            tapped = false;
            _filterCountObj = 1;
			_filterCountObj4th = 1;
			_filterCountObj5th = 1;
			_filterCountObj6th = 1;
            _tapCountObj = 0;
            
            // Adding elements to List
            firstlist.Add("teddy bear");
			firstlist.Add("bottle");
			firstlist.Add("stop sign");
			firstlist.Add("clock");
			firstlist.Add("umbrella");
            firstlist.Add("mouse");
            firstlist.Add("pottedplant");
            firstlist.Add("keyboard");
            firstlist.Add("suitcase");
			firstlist.Add("cup");
			
			staticlist.Add("teddy bear");
			staticlist.Add("bottle");
			staticlist.Add("stop sign");
			staticlist.Add("clock");
			staticlist.Add("umbrella");
            staticlist.Add("mouse");
            staticlist.Add("pottedplant");
            staticlist.Add("keyboard");
            staticlist.Add("suitcase");
			staticlist.Add("cup");
			
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
				
            _holoLensMediaFrameSourceGroupStarted = false;
			
            // Create the gesture handler
            InitializeHandler();

            // Wait for media frame source groups to be initialized
            await StartHoloLensMediaFrameSourceGroup();

            
        }
     

        async void OnApplicationQuit()
        {
            await StopHoloLensMediaFrameSourceGroup();

        }
		
        void startSocket()
        {
#if ENABLE_WINMD_SUPPORT
          
            listener = new TcpListener(IPAddress.Any, 9090);
            listener.Start();
            client = listener.AcceptTcpClient();

            listenerZ = new TcpListener(IPAddress.Any, 9097);
            listenerZ.Start();
            clientZ = listenerZ.AcceptTcpClient();

			CreateTxt("dataPath");
			CreateTxt("dataObj");
#endif
        }

        void Update()
        {
            //UpdateHoloLensMediaFrameSourceGroup();
            if (tapped == true)
            {
				doIt();
            }

        }
        
        void doIt()
        {
#if ENABLE_WINMD_SUPPORT
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var dataBuffer1 = dataReceived.Split(':');

            NetworkStream nwStreamZ = clientZ.GetStream();
            byte[] bufferZ = new byte[clientZ.ReceiveBufferSize];
            int bytesReadZ = nwStreamZ.Read(bufferZ, 0, clientZ.ReceiveBufferSize);
            string dataReceivedZ = Encoding.UTF8.GetString(bufferZ, 0, bytesReadZ);
            dataReceivedZ = dataReceivedZ.Remove(0, 2);
            dataReceivedZ = dataReceivedZ.Remove(dataReceivedZ.Length - 1);
            dataReceivedZ = dataReceivedZ.Replace(":",","); 
            var dataBuffer1Z = dataReceivedZ.Split(',');

            count=int.Parse(dataBuffer1[0]);
            List<BoundingBox> boundingBoxes = new List<BoundingBox>();
            int boxSize = 6;
            int dataLenght = dataBuffer1.Length;
            int dataLenghtZ = dataBuffer1Z.Length;
            int numOfObj = dataLenght / boxSize;
			
            if (dataReceivedZ!="0")
            {
                xObj = -(int)double.Parse(dataBuffer1Z[0]);
                zObj = (int)double.Parse(dataBuffer1Z[2]);
				yObj = (int)double.Parse(dataBuffer1Z[1]);
				
				if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 0<=yObj && yObj<=4)
				{
					_labelPath = xObj.ToString() + " , " + zObj.ToString() + " , " + "3";
					if (pathList.Contains(_labelPath))
					{
						_input = " already included ";
					}
					else
					{	
						Transform newButton = Instantiate(prefab, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newButton.localScale = new Vector3(2, 2, 2);
						newButton.SetParent(GameObject.Find("MapCanvas4F").transform, false);
						newButton.name = _labelPath; 
						pathList.Add(_labelPath);
						
						foreach (var myPath in pathList)
						{
							saveInfoPath = saveInfoPath + myPath + "\r\n";
						}
						SaveToTxt(saveInfoPath, "dataPath"); 
						saveInfoPath = "";
					}
				}
				
				else if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 5<=yObj && yObj<=9)
				{
					_labelPath = xObj.ToString() + " , " + zObj.ToString() + " , " + "7";
					if (pathList.Contains(_labelPath))
					{
						_input = " already included ";
					}
					else
					{
						Transform newButton = Instantiate(prefab, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newButton.localScale = new Vector3(2, 2, 2);
						newButton.SetParent(GameObject.Find("MapCanvas5F").transform, false);
						newButton.name = _labelPath; 
						pathList.Add(_labelPath);
				
						foreach (var myPath in pathList)
						{
							saveInfoPath = saveInfoPath + myPath + "\r\n";
						}
						SaveToTxt(saveInfoPath, "dataPath"); 
						saveInfoPath = "";
					}
				}
				
				else if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 10<=yObj && yObj<=15)
				{
					_labelPath = xObj.ToString() + " , " + zObj.ToString() + " , " + "11";
					if (pathList.Contains(_labelPath))
					{
						_input = " already included ";
					}
					else
					{
						Transform newButton = Instantiate(prefab, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newButton.localScale = new Vector3(2, 2, 2);
						newButton.SetParent(GameObject.Find("MapCanvas6F").transform, false);
						newButton.name = _labelPath; 
						pathList.Add(_labelPath);
					
						foreach (var myPath in pathList)
						{
							saveInfoPath = saveInfoPath + myPath + "\r\n";
						}
						SaveToTxt(saveInfoPath, "dataPath"); 
						saveInfoPath = "";
					}
				}
            }
			
			if (dataBuffer1[1] != "00")
            {
                _labelObj = _labels[int.Parse(dataBuffer1[1])];
                if (firstlist.Contains(_labelObj))
                {
					if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 0<=yObj && yObj<=4)
					{
						_labelObjName = "Text_4th_" + _filterCountObj4th.ToString();
						_labelObjNameColor = "Sphere_obj_4th_" + _filterCountObj4th.ToString();
						
						GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
						
						Transform newPointObj = Instantiate(prefabObj, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newPointObj.localScale = new Vector3(5, 5, 5);
						newPointObj.SetParent(GameObject.Find("MapCanvas4F").transform, false);
						newPointObj.name = _labelObj; 
						
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
						
						objList.Add(_labelObj);
						firstlist.Remove(_labelObj);
						_filterCountObj += 1;
						_filterCountObj4th += 1;
						
						if (objListS.Contains(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "3"))
						{
							_input = " already included ";
						}
						else
						{
							objListS.Add(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "3");
							foreach (var myObjS in objListS)
							{
								saveInfoObj = saveInfoObj + myObjS + "\r\n";
							}
							SaveToTxt(saveInfoObj, "dataObj"); 
							saveInfoObj = "";
						}
					}
					
					else if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 5<=yObj && yObj<=9)
					{
						_labelObjName = "Text_5th_" + _filterCountObj5th.ToString();
						_labelObjNameColor = "Sphere_obj_5th_" + _filterCountObj5th.ToString();
						GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
						
						Transform newPointObj = Instantiate(prefabObj, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newPointObj.localScale = new Vector3(5, 5, 5);
						newPointObj.SetParent(GameObject.Find("MapCanvas5F").transform, false);
						newPointObj.name = _labelObj; 
						
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
						
						objList.Add(_labelObj);
						firstlist.Remove(_labelObj);
						_filterCountObj += 1;
						_filterCountObj5th += 1;
						
						if (objListS.Contains(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "7"))
						{
							_input = " already included ";
						}
						else
						{
							objListS.Add(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "7");
							foreach (var myObjS in objListS)
							{
								saveInfoObj = saveInfoObj + myObjS + "\r\n";
							}
							SaveToTxt(saveInfoObj, "dataObj"); 
							saveInfoObj = "";
						}
					}
					
					else if(-300<zObj && zObj<300 && -300<xObj && xObj<300 && 10<=yObj && yObj<=15)
					{
						_labelObjName = "Text_6th_" + _filterCountObj6th.ToString();
						GameObject.Find(_labelObjName).GetComponent<Text>().text = _labelObj;
						_labelObjNameColor = "Sphere_obj_6th_" + _filterCountObj6th.ToString();
						
						Transform newPointObj = Instantiate(prefabObj, new Vector3(xObj, zObj, -9), Quaternion.identity);
						newPointObj.localScale = new Vector3(5, 5, 5);
						newPointObj.SetParent(GameObject.Find("MapCanvas6F").transform, false);
						newPointObj.name = _labelObj; 
						
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
						
						objList.Add(_labelObj);
						firstlist.Remove(_labelObj);
						_filterCountObj += 1;
						_filterCountObj6th += 1;
						
						if (objListS.Contains(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "11"))
						{
							_input = " already included ";
						}
						else
						{
							objListS.Add(_labelObj + " , " + xObj.ToString() + " , " + zObj.ToString() + " , " + "11");
							foreach (var myObjS in objListS)
							{
								saveInfoObj = saveInfoObj + myObjS + "\r\n";
							}
							SaveToTxt(saveInfoObj, "dataObj"); 
							saveInfoObj = "";
						}
					}
                }
                
            }
			
#endif
        }

        async Task StartHoloLensMediaFrameSourceGroup()
        {
#if ENABLE_WINMD_SUPPORT
            
            //Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up sensor frame streamer");
            _sensorType = (SensorType)sensorTypePv;
            _sensorFrameStreamer = new SensorFrameStreamer();
            _sensorFrameStreamer.Enable(_sensorType);

            //Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up spatial perception");
            _spatialPerception = new SpatialPerception();

            //Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Setting up the media frame source group");
            
            // Cast device type 
            _deviceType = (HoloLensForCV.DeviceType)deviceType;
            _holoLensMediaFrameSourceGroup = new MediaFrameSourceGroup(
                _selectedMediaFrameSourceGroupType,
                _spatialPerception,
                _deviceType,
                _sensorFrameStreamer);
            _holoLensMediaFrameSourceGroup.Enable(_sensorType);

            //Debug.Log("YoloDetection.Detection.StartHoloLensMediaFrameSourceGroup: Starting the media frame source group");
            await _holoLensMediaFrameSourceGroup.StartAsync();
            _holoLensMediaFrameSourceGroupStarted = true;

#endif
        }

        async Task StopHoloLensMediaFrameSourceGroup()
        {
#if ENABLE_WINMD_SUPPORT
           
            await _holoLensMediaFrameSourceGroup.StopAsync();
            _holoLensMediaFrameSourceGroup = null;
            _sensorFrameStreamer = null;
            _holoLensMediaFrameSourceGroupStarted = false;
#endif
        }

        #region ComImport
        // https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/imaging
        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }
        #endregion

#if ENABLE_WINMD_SUPPORT
       
        unsafe byte* GetByteArrayFromSoftwareBitmap(SoftwareBitmap sb)
        {
            SoftwareBitmap sbCopy = new SoftwareBitmap(sb.BitmapPixelFormat, sb.PixelWidth, sb.PixelHeight);
            Interlocked.Exchange(ref sbCopy, sb);
            using (var input = sbCopy.LockBuffer(BitmapBufferAccessMode.Read))
            using (var inputReference = input.CreateReference())
            {
                byte* inputBytes;
                uint inputCapacity;
                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputBytes, out inputCapacity);
                return inputBytes;
            }
        }
		
		private async void CreateTxt(string filename)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string desiredNameZ = filename +".txt";
            StorageFile newFile = await localFolder.CreateFileAsync(desiredNameZ, CreationCollisionOption.ReplaceExisting);
		}

		private async void SaveToTxt(string sometext, string filename)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string desiredName = filename +".txt";
			StorageFile newFile = await localFolder.GetFileAsync(desiredName);
			
			//await Windows.Storage.FileIO.WriteTextAsync(newFile, sometext);
			var stream = await newFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
			
			using (var outputStream = stream.GetOutputStreamAt(0))
			{
				using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
				{
					dataWriter.WriteString(sometext);
					await dataWriter.StoreAsync();
				}
				
				await outputStream.FlushAsync();
				// We'll add more code here in the next step.
			}
			stream.Dispose(); 
		}
		
#endif

        #region TapGestureHandler
        private void InitializeHandler()
        {
            // New recognizer class
            _gestureRecognizer = new GestureRecognizer();

            // Set tap as a recognizable gesture
            _gestureRecognizer.SetRecognizableGestures(GestureSettings.DoubleTap);

            // Begin listening for gestures
            _gestureRecognizer.StartCapturingGestures();

            // Capture on gesture events with delegate handler

            _gestureRecognizer.Tapped += GestureRecognizer_Tapped;


            //Debug.Log("Gesture recognizer initialized.");
        }

        public void GestureRecognizer_Tapped(TappedEventArgs obj)
        {
            tapped = true;
            startSocket();
        }

        void CloseHandler()
        {
            _gestureRecognizer.StopCapturingGestures();
            _gestureRecognizer.Dispose();
        }
        #endregion

    }
}




