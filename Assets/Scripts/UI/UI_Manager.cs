using UnityEngine;
using TMPro; //Keep, needed for text

public class UI_Manager : MonoBehaviour
{
    //class from: https://docs.unity3d.com/2018.2/Documentation/ScriptReference/UI.Slider.html
    public UnityEngine.UI.Slider temperatureSlider;
    //Class from: https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.1/api/TMPro.TextMeshProUGUI.html
    public TextMeshProUGUI temperatureSliderDisplay;

    public UnityEngine.UI.Slider humiditySlider;
    public TextMeshProUGUI humiditySliderDisplay;

    public UnityEngine.UI.Slider windSpeedSlider;
    public TextMeshProUGUI windSpeedDisplay;

    //Class from: https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Dropdown.html
    public TMP_Dropdown windDirectionDropDown;
    //Class from: https://docs.unity3d.com/2018.2/Documentation/ScriptReference/UI.Button.html
    public UnityEngine.UI.Button startSimButton;

    //Used to indicate when user values have been received
    private bool simulatorStarted = false;

    //This holds the user data to be past to the simulator engine following user input
    UserData userInputData;
    //needed to pass user input from UI to SimEngine
    public BurningCellManager burnCellManager;


    public void InitializeUIManager()
    {
      userInputData = new UserData();
      SetUpEventListeners();
    }

    //This is used to check when to start update loop. 
    //Not starting until user data is received. 
    public bool UserInputReceived()
    {
        return simulatorStarted;
    }

    void SetUpEventListeners()
    {
        //Slider output make an event, when slider is moved the text output is updated for all sliders
       temperatureSlider.onValueChanged.AddListener((value) =>
       {
          temperatureSliderDisplay.text = value.ToString(); 
       });
        
        humiditySlider.onValueChanged.AddListener((value) =>
        {
          humiditySliderDisplay.text = value.ToString(); 
        });

       windSpeedSlider.onValueChanged.AddListener((value)=>
       {
          windSpeedDisplay.text = value.ToString(); 
       });

        //When start button clicked, we can now gather and share the user parameters received.
       startSimButton.onClick.AddListener(()=>
       {
            DeactivateUI();
            GatherUserData(); 
            //Get the data to the probability generator
            burnCellManager.igniteProbGen.SetUserInput(userInputData);
            //This triggers start of the update loop
            simulatorStarted = true;
       });
    }

    //Gets parameters enterred and stores them in the userData object
    void GatherUserData()
    {
        //We would then get all the values for the input elements here***
        //Temp = temperatureInput.parse....
        userInputData.windDirection = windDirectionDropDown.value;
        userInputData.windSpeed = windSpeedSlider.value;
        userInputData.humidity = humiditySlider.value;

        //This gets the max value from the actual slider object in unity
        //This was we always have a correct wind speed calculation
        userInputData.maxWindSpeed = windSpeedSlider.maxValue;
        userInputData.temperature = temperatureSlider.value;
        userInputData.maxTemperature = temperatureSlider.maxValue;
    }

    //This allows the UI items to be deactivated so that they are not appearing modifiable during run 
    void DeactivateUI()
    {
        windSpeedSlider.interactable = false;
        temperatureSlider.interactable = false;
        humiditySlider.interactable = false;
        windDirectionDropDown.interactable = false;
        startSimButton.interactable = false;
    }
}
