using UnityEngine;
using TMPro; //Keep, needed for text

public class UI_Manager : MonoBehaviour
{
    public UnityEngine.UI.Slider temperatureSlider;
    public TextMeshProUGUI temperatureSliderDisplay;

    public UnityEngine.UI.Slider humiditySlider;
    public TextMeshProUGUI humiditySliderDisplay;

    public UnityEngine.UI.Slider windSpeedSlider;
    public TextMeshProUGUI windSpeedDisplay;

    public TMP_Dropdown windDirectionDropDown;
    public UnityEngine.UI.Button startSimButton;

    //Used to indicate when user values have been received
    private bool simulatorStarted = false;

    UserData userInputData;
    public BurningCellManager burnCellManager;


    public void InitializeUIManager()
    {
      userInputData = new UserData();
      SetUpEventListeners();
    }

    public bool UserInputReceived()
    {
        return simulatorStarted;
    }

    void SetUpEventListeners()
    {
         //Slider output make an event, when slider is moved the text output is updated
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

       startSimButton.onClick.AddListener(()=>
       {
            DeactivateUI();
            GatherUserData(); 
            //Get the data to the probability generator
            burnCellManager.igniteProbGen.SetUserInput(userInputData);
            simulatorStarted = true;
       });
    }

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

    void DeactivateUI()
    {
        windSpeedSlider.interactable = false;
        temperatureSlider.interactable = false;
        humiditySlider.interactable = false;
        windDirectionDropDown.interactable = false;
        startSimButton.interactable = false;
    }
}
