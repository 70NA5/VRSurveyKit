using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionnaireManager : MonoBehaviour
{
    public Transform leftIndexFinger;
    public Transform rightIndexFinger;
    public GameObject endPage;
    public float checkDistance = 0.05f;
    public float holdTime = 2f;
    public float surveyDist = 0.5F;
    public float surveyTilt = 30F;
    public bool attachedToHMD;
    private PageNavigator navigator;

    
    public List<PageNavigator> surveys;

    private float timer = 0;
    private Button targetButton;

    void Start(){
       
    }

    void Update()
    {
        bool leftClose = CheckProximity(leftIndexFinger);
        bool rightClose = CheckProximity(rightIndexFinger);

        if (!leftClose && !rightClose)
        {
            targetButton = null;
            timer = 0;
        }
    }

    // Method to check proximity between answer button and finger
    // used to check answers and click buttons
    // With hold time in inspector we can set the time that is needed to stay at button to trigger action
    // Hold distance is the distance threshold from the button and finger to trigger action

    bool CheckProximity(Transform finger)
    {
        foreach (var button in FindObjectsOfType<Button>())
        {
            float distance = Vector3.Distance(finger.position, button.transform.position);
            if (distance < checkDistance)
            {
                if (button == targetButton)
                {
                    timer += Time.deltaTime;
                    if (timer >= holdTime)
                    {
                        button.onClick.Invoke();
                        timer = 0;
                    }
                }
                else
                {
                    targetButton = button;
                    timer = 0;
                }
                return true;
            }
        }
        return false;
    }


    // method to disable HMD bool
    public void DisableSurveyToHMD(){
        attachedToHMD = false;
    }

    // Function to spawn survey at given position
    // if we have different surveys prepared we can use id to distinguish between surveys

    public void SpawnSurvey(Vector3 position, int surveyID){
        surveys[surveyID].ShowPage(0);
        surveys[surveyID].GetComponentInChildren<SurveyPosition>().SetPosition(position);


    }

    // Function to spawn custom End page at given position
    public void SpawnEndPage(Vector3 position){
        Instantiate(endPage, position, Quaternion.identity);

    }

    public void CloseWithoutSave(int surveyID)
    {
        surveys[surveyID].CloseWithoutSaving();
    }
}
