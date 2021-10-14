using System;
using System.Collections.Generic;
//using Unity.Collections;
//using System;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
//using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

//using System.Threading;



[RequireComponent(typeof(ARRaycastManager))]
public class UIScript : MonoBehaviour
{

    ARCameraManager CameraManager;
    public Camera currentCamera;
    private ARRaycastManager aRRaycastManager;
    int totalLum = 0;
    public Text lumDisplay;
    Animator anim;
    public Text lightWarningLabel;
    //public Text monsterText;

    int currentMonsterIndex;


    int[] eventIntervals;

    public GameObject audioMeter;

    public UnityEngine.UI.Image screenStatus;
    public Text screenStatusMessage;
    public Button startScanButton;
    public Button stopScanButton;
    public GameObject activeLightAudioWarning;
    Animator activeWarningAnim;
    public Sprite lightbulb;
    public Sprite soundImage;

    public Text activeWarningText;
    public Text activeAudioText;

    public Image activeWarningImage;

    public GameObject monsterArray;

    static List<ARRaycastHit> hits;

    XRCameraSubsystem camSys;


   

    List<GameObject> instantiatedMonsters;
    System.Random random;
    //Thread findNewMonster;


 

    private void Start()
    {
        for (int i = 0; i < monsterArray.gameObject.transform.childCount; i++)
        {
            Debug.Log("Monster Started");
            instantiatedMonsters.Add(Instantiate(monsterArray.gameObject.transform.GetChild(i).gameObject, new Vector3(0, -500, 0), Quaternion.LookRotation(Camera.main.transform.position, Vector3.forward)));

            monstersAreActive.Add(false);


        }
        //instantiatedMonsters[0].gameObject.transform.position = new Vector3(100, 100, 100);
        Debug.Log("Instance " + instantiatedMonsters[0].name);
        Debug.Log("Next Instance " + instantiatedMonsters[1].name);
        currentMonsterIndex = random.Next(instantiatedMonsters.Count);
        MakeEvent();
    }

    void Awake()
    {

        Debug.Log("Provolone");
        CameraManager = gameObject.transform.GetChild(0).gameObject.GetComponent<ARCameraManager>();

       




        Debug.Log("Cam Mgr Found");
       


        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            audSource.clip = Microphone.Start("iPhone audio input", true, 1, 11025);
        }
        else
        {
            audSource.clip = Microphone.Start("Android camcorder input", true, 1, 11025);
        }
        Debug.Log("Audio Loaded");
        //anim = lightWarningLabel.GetComponent<Animator>();
        camSys = CameraManager.subsystem;
        Debug.Log("Captain Hook");
        instantiatedMonsters = new List<GameObject>();
        random = new System.Random();
        sampleData = new float[4096];
        monstersAreActive = new List<bool>();
        camSys.Start();
        Debug.Log("pork");
        eventIntervals = new int[] { 5, 10, 15, 20 };
        //findNewMonster = new Thread(SetNewMonsterValue);
        hits = new List<ARRaycastHit>();
        anim = lightWarningLabel.GetComponent<Animator>();
        activeWarningAnim = activeLightAudioWarning.GetComponent<Animator>();
        activeLightAudioWarning.SetActive(false);
        activeAudioText.gameObject.SetActive(false);
        ghostPhrases = new List<string> { "Hello " + PlayerPrefs.GetString("player_name"), PlayerPrefs.GetString("player_name") + " , I can see you", "Look behind you" };
        Debug.Log("mustard1");
        //activeWarningAnim.SetTrigger("isFlashing");
        //activeWarningAnim.SetTrigger("startDouble");
        Debug.Log("mayo1");
        aRRaycastManager = GetComponent<ARRaycastManager>();
        Debug.Log("AR is" + aRRaycastManager);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        isLightOn = false;
       




    }
    
  
   

    public bool isLightOn;
    
    public void TurnOnLight()
    {




        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
           // _turnOnLight();
        }


    }





    public void StartScanning(bool scanning)
    {

        Debug.Log("Offer 10");
        if (scanning ==  false)
        {
            foreach(GameObject thisMonster in instantiatedMonsters)
            {
                thisMonster.GetComponent<CheckIfVisible>().Die();
            }
        }
        Debug.Log("Offer 20");
        isScanning = scanning;
        Debug.Log("Scan status " + isScanning);

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Offer 30");
            //_turnOnLight();
        }



        isLightOn = scanning;

    }



    void setStatusLabel(int status)
    {
        switch (status)
        {
            case 0:
                Debug.Log("Status Standby");
                startScanButton.gameObject.SetActive(true);
                stopScanButton.gameObject.SetActive(false);
                screenStatus.GetComponent<UnityEngine.UI.Image>().sprite = Resources.LoadAll<Sprite>("GHOSTSPRITE")[7];
                screenStatusMessage.text = "STANDBY";
                break;
            case 1:
                Debug.Log("Status Error");
                startScanButton.gameObject.SetActive(false);
                stopScanButton.gameObject.SetActive(false);
                screenStatus.GetComponent<UnityEngine.UI.Image>().sprite = Resources.LoadAll<Sprite>("GHOSTSPRITE")[1];
                screenStatusMessage.text = "ERROR";
                break;

            case 2:
                Debug.Log("start Scannnning");
                startScanButton.gameObject.SetActive(false);
                stopScanButton.gameObject.SetActive(true);
                screenStatus.GetComponent<UnityEngine.UI.Image>().sprite = Resources.LoadAll<Sprite>("GHOSTSPRITE")[6];
                screenStatusMessage.text = "SCANNING";
                break;
        }

    }


    int topLight=0;
    bool LightUpdate()
    {
        totalLum = 0;
        int topLum = 0;
        int index = 0;
        Debug.Log("Camera Point 11");
        camSys.TryGetLatestImage(out XRCameraImage theImage);
        Debug.Log("Camera Point 22");
        XRCameraImagePlane imgpPlane = theImage.GetPlane(0);
        Debug.Log("Camera Point 33");
        foreach (int value in imgpPlane.data)
        {
            if(index <= 1080)
            {
                topLum += value;
            }
            totalLum += value;
        }
        Debug.Log("Level is " + totalLum);
        topLight = totalLum / 1000;
        Debug.Log("Camera Point 44");
        Debug.Log("Top line is " + totalLum / 1000);
        Debug.Log("Illum status is " + !isLightOn);
        theImage.Dispose();

        int lowLum, hiLum;

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            lowLum = 150000;
            hiLum = 320000;
        }
        else
        {
            lowLum = 20000;
            hiLum = 40000;
        }

        if((totalLum / 1000 > lowLum && !isLightOn) || (totalLum / 1000 > hiLum && isLightOn))
        {

            Debug.Log("too much light");
            return true;
        }
       
        else
        {

            Debug.Log("good light");
            return false;
        }

        


    }

    bool setObject = false;

    public void SetObject()
    {
        setObject = true;
        Debug.Log("Object set");
    }


    int count = 0;

    GameObject currentMonster;

    void IsMonsterVisible()
    {
        Debug.Log("chedder");
        
        Ray leftRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));

        Debug.Log("swiss");
        
        RaycastHit leftRayHit;

        Debug.Log("muenster");

        if (Physics.Raycast(leftRay, out leftRayHit))
        {
            Debug.Log(currentMonster + " was hit");
            currentMonster = leftRayHit.collider.gameObject;

           currentMonster.GetComponent<CheckIfVisible>().SetRaycastHit(true);
            Debug.Log(currentMonster + " still hit");
        }
        else
        {
            if (currentMonster)
            {
                Debug.Log("Saturn Hit");
                currentMonster.GetComponent<CheckIfVisible>().SetRaycastHit(false);
                currentMonster = null;
                monsterCreatedTime = DateTime.Now;
            }
        }


           
        
      

    }

    DateTime monsterCreatedTime;
    
   
    bool monsterWasPlaced;

   

    public void ShowHideMonster()
    {

        Debug.Log("pressed butts");
        instantiatedMonsters[currentMonsterOut].GetComponent<CheckIfVisible>().ShowOrHide();
        Debug.Log("salsa");
            //isMonsterVis = false;
        
        
    }

    public GameObject directionScreen;

    public void ShowDirections()
    {
        directionScreen.SetActive(true);
    }


    /*void CheckMonster()
    {
        if(monsterWasPlaced == false && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            currentPos = hits[0];
            monsterWasPlaced = true;
        }
    }

    void PutMonster()
    {
        instantiatedMonsters[currentMonsterIndex].GetComponent<CheckIfVisible>().Place(currentPos.pose);
        currentMonsterOut = currentMonsterIndex;
        lastEventTime = DateTime.Now;
        while (true)
        {
            int index = random.Next(instantiatedMonsters.Count);
            if (index != currentMonsterIndex)
            {
                currentMonsterIndex = index;
                Debug.Log("Current Monster Is " + instantiatedMonsters[currentMonsterIndex].name);
                break;
            }
        }
        monsterIsAlive = true;
    }*/

    int currentMonsterOut;
    
    bool CheckToPlaceMonster()
    {
        //if (DateTime.Now.Subtract(currentTime).Seconds > 1 && moveOn == true)
        Debug.Log("point 1");
        
        Debug.Log(instantiatedMonsters.Count);
        Debug.Log("Instance "+instantiatedMonsters[0].name);
        

        if (monsterWasPlaced == false && aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            monsterWasPlaced = true;

            Debug.Log("lion king " + instantiatedMonsters[currentMonsterIndex].name);
            instantiatedMonsters[currentMonsterIndex].GetComponent<CheckIfVisible>().Place(hits[0].pose);
            //monsterText.text = instantiatedMonsters[currentMonsterIndex].name;
            Debug.Log("aladdin");
            monstersAreActive[currentMonsterIndex] = true;
            Debug.Log("cinderalla");
            //currentPlacedMonster = instantiatedMonsters[currentMonsterIndex];
            currentMonsterOut = currentMonsterIndex;
            Debug.Log("We put it in");
            
            //isMonsterVis = true;
            lastEventTime = DateTime.Now;
            Debug.Log("Yup we did it");


            int index = 0;

            while(index == currentMonsterIndex)
            {
                index = random.Next(instantiatedMonsters.Count);
            }
            currentMonsterIndex = index;


           
            
            return true;

          
        }
        return false;

        
    }
    //ARCameraManager camMgr;

    public static bool isOn;
    // Update is called once per frame
    DateTime lastEventTime;
    bool error, isScanning;
    int currentEventInterval;

    List<bool> monstersAreActive;

    List<string> ghostPhrases;
    int phraseIndex;
    int lowAudio;
    int highAudio;
    public Text ghostSpeakDisplay;

    


   

    public Text bothWarningStatus;

    void MakeEvent()
    {
        //Do Event
        currentEventInterval = eventIntervals[random.Next(0, 3)];
        int newPhraseIndex = 0;
        while(newPhraseIndex == phraseIndex)
        {
            newPhraseIndex = random.Next(0, 3);
        }
        phraseIndex = newPhraseIndex;
        //phraseIndex = 2;
        lastEventTime = DateTime.Now;
        monsterWasPlaced = false;
        
       
        
        //monsterWasPlacedForSure = false;
    }

    //bool monsterWasPlacedForSure = false;


    void Speak(int input)
    {
        if (DateTime.Now.Subtract(lastEventTime).Seconds < 3)
        {
            Debug.Log("Spoken");
            lowAudio = 7;
            highAudio = 10;

            ghostSpeakDisplay.text = ghostPhrases[input];


        }
        else
        {
            Debug.Log("Spoken 3");
            lowAudio = 1;
            highAudio = 3;
            ghostSpeakDisplay.text = "";
        }
    }

    

    void Update()
    {

        

        bool audUpdate = AudioUpdate();
        bool lightUpdate = LightUpdate();

        Debug.Log("Status issss " + isScanning);
        lumDisplay.text = "Val is "+topLight.ToString();

        


      

        if(isScanning == true)
        {

            setStatusLabel(2);
            lightWarningLabel.gameObject.SetActive(false);
            soundWarningLabel.gameObject.SetActive(false);
            Debug.Log("Event is " + monsterWasPlaced);

            int timeOffset;

            if(phraseIndex == 2 && monsterWasPlaced == false)
            {
                Debug.Log("put monster out");
                CheckToPlaceMonster();
                //CheckMonster();
                
            }
            else
            {
                

                Speak(phraseIndex);
            }

            if(monsterWasPlaced == true)
            {
                
                    if (DateTime.Now.Subtract(lastEventTime).Seconds > 10)
                    {
                        Debug.Log("scrape");
                        instantiatedMonsters[currentMonsterOut].GetComponent<CheckIfVisible>().Die();
                        Debug.Log("paint");
                        monsterWasPlaced = false;
                        //monsterIsAlive = false;
                        //currentMonsterIndex = -1;
                    }
                timeOffset = 10;
                    
            }
            else
            {
                
                timeOffset = 0;
            }

            


            if (((DateTime.Now.Subtract(lastEventTime).Seconds + timeOffset) >= (currentEventInterval + timeOffset)) && monsterWasPlaced == false)
            {
                Debug.Log("new event");
                MakeEvent();
            }




          


            
            audioMeter.GetComponent<FlashControl>().ChangeLevel(random.Next(lowAudio, highAudio));
            //activeLightAudioWarning.SetActive(false);
            Debug.Log("scanning");
            //CheckToPlaceMonster();
            IsMonsterVisible();

            if (audUpdate == true && lightUpdate == true)
            {
                Debug.Log("both problems");
                activeLightAudioWarning.SetActive(true);
                activeWarningAnim.SetTrigger("goBack");
                activeWarningAnim.SetTrigger("startDualWarning");
                Debug.Log("both problems1");
                bothWarningStatus.text = "Yup";
            }
            else if(audUpdate == true)
            {
                activeLightAudioWarning.SetActive(true);
                activeWarningAnim.SetTrigger("goBack");
                activeWarningAnim.SetTrigger("startSoundWarning");
            }
            else if(lightUpdate == true)
            {
                activeLightAudioWarning.SetActive(true);
                Debug.Log("chocolate1");
                activeWarningAnim.SetTrigger("goBack");
                activeWarningAnim.SetTrigger("startLightWarning");
                Debug.Log("vanilla1");
            }
            else
            {
                activeLightAudioWarning.SetActive(false);
            }
       
        }
        else if(audUpdate == true || lightUpdate == true)
        {
            setStatusLabel(1);
            Debug.Log("charlie brown");
            if(audUpdate == true)
            {
                Debug.Log("bad sounds");
                soundWarningLabel.gameObject.SetActive(true);
                soundWarningLabel.enabled = true;
                Animator soundAnimation = soundWarningLabel.GetComponent<Animator>();
                soundAnimation.enabled = true;
                soundAnimation.SetTrigger("soundIsFlashing");
                error = true;
            }
            else
            {
                soundWarningLabel.gameObject.SetActive(false);
            }
            if(lightUpdate == true)
            {
                lightWarningLabel.gameObject.SetActive(true);
                Debug.Log("bad lighting1");
                lightWarningLabel.enabled = true;
                anim.enabled = true;
                anim.SetTrigger("isFlashing");
                error = true;
            }
            else
            {
                lightWarningLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            setStatusLabel(0);
            Debug.Log("Linus George");
            //lightWarningLabel.gameObject.SetActive(false); 
            activeLightAudioWarning.SetActive(false);
            lightWarningLabel.gameObject.SetActive(false);
            soundWarningLabel.gameObject.SetActive(false);
            //soundWarningLabel.gameObject.SetActive(false);
            audioMeter.GetComponent<FlashControl>().ChangeLevel(random.Next(0, 0));
            foreach(GameObject thisMonster in instantiatedMonsters)
            {
                thisMonster.GetComponent<CheckIfVisible>().Die();
            }
            Debug.Log("standby");
        }
        

    }

    




    public void GetCamImage()
    {


        Debug.Log("place 1");
        camSys.TryGetLatestImage(out XRCameraImage theImage);
        Debug.Log("place 2 "+theImage.format);
        XRCameraImagePlane imgpPlane = theImage.GetPlane(0);
        Debug.Log("place 3 "+imgpPlane);
        Debug.Log("Pixel Length: "+imgpPlane.data.Length);
        foreach(int value in imgpPlane.data)
        {
            totalLum += value;
        }
        Debug.Log("place 4 "+totalLum);
        totalLum = 0;
        theImage.Dispose();
    }


    

    /*  Audio Fields  */
    float[] sampleData;
    float updateStep = 0.03f;
    float currentUpdateTime = 0f;
    float loudness = 0f;
    bool audioReturnVal = false;
    int audioIndex = 0;
    public AudioSource audSource;

    public Text soundWarningLabel;


    bool AudioUpdate()
    {
        
        currentUpdateTime += Time.deltaTime;
       
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            Debug.Log("sound1");
            audSource.clip.GetData(sampleData, Microphone.GetPosition("iPhone audio input"));
            Debug.Log("sound2");
            Debug.Log(sampleData);
            foreach (var sample in sampleData)
            {
                loudness += Math.Abs(sample);
            }
            Debug.Log("sound3");
            if ((loudness / 4096) < .07)
            {
                if (audioIndex != 0)
                {
                    audioIndex--;
                }

                
                Debug.Log("sound good");
                audioReturnVal = false;

            }
            else
            {
                audioIndex++;
                if (audioIndex >= 80)
                {
                    //soundWarningLabel.enabled = true;
                    Debug.Log("sound bad");
                    audioReturnVal = true;
                }
             

            }

            loudness = 0;
      
        }

        return audioReturnVal;
    }
}
