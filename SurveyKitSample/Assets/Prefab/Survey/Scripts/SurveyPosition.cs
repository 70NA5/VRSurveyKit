using UnityEngine;

public class SurveyPosition : MonoBehaviour
{
    public Camera vrCamera; // Reference to the VR camera
    private float distance;
    private float tilt;
    public QuestionnaireManager manager;

    void Start()
    {
        manager = GameObject.Find("QuestionnaireManager").GetComponent<QuestionnaireManager>();
        distance = manager.surveyDist;
        tilt = manager.surveyTilt;
        if(manager.attachedToHMD){  
        PositionSurvey();
        }
    }

    void LateUpdate()
    {
        if(manager.attachedToHMD){
            
        distance = manager.surveyDist;
        tilt = manager.surveyTilt;
        PositionSurvey();
        }
    }

    void PositionSurvey()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera not assigned.");
            return;
        }

        Vector3 cameraForward = vrCamera.transform.forward;
        Vector3 newPosition = vrCamera.transform.position + cameraForward * distance;

        transform.position = newPosition;

        transform.LookAt(vrCamera.transform);
        transform.Rotate(tilt, 180, 0); // Adjust to face the camera
    }

    public void SetPosition (Vector3 newPosition){
        transform.position = newPosition;
    }
}
