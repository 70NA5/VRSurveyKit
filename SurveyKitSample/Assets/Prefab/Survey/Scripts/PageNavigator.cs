using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class PageNavigator : MonoBehaviour
{
    public GameObject[] pages;
    private List<LikertQuestionManager> likertManagers = new List<LikertQuestionManager>();
    public string fileName = "SurveyAnswers.csv";
    public string customPath = "";
    private int currentPageIndex = 0;
    private List<string> allAnswers = new List<string>();
    public QuestionnaireManager managerQ;
    //public ExperimentManager experimentManager;
    private long surveyStartTime;
    long currentSurveyTime;
    private long lastSurveyTime;
    //public Recorder recorder;

    void Start()
    {
        //experimentManager = GameObject.Find("LocalWorldManager").GetComponent<ExperimentManager>();
        managerQ = GameObject.Find("QuestionnaireManager").GetComponent<QuestionnaireManager>();
        //recorder = GameObject.Find("MicroManager").GetComponent<Recorder>();

        foreach (var page in pages)
        {
            var manager = page.GetComponent<LikertQuestionManager>();
            if (manager != null)
            {
                likertManagers.Add(manager);
            }
        }

        // Record the initial start time for the first survey
        surveyStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //recorder.StartRecording();
        lastSurveyTime = surveyStartTime;
    }

    //functio to display next page
    public void ShowNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            CollectCurrentPageAnswers();
            ShowPage(++currentPageIndex);
        }
        else
        {
            CollectCurrentPageAnswers();
            SaveAllAnswersToFile();
            UnityEngine.Debug.Log("Reached the last page. Answers saved.");
        }
    }
    //show distict page
    public void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length)
            return;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        if (pageIndex == 0)
        {
            currentSurveyTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(); ;
            //recorder.StopRecordingAuto();
        }
        pages[pageIndex].SetActive(true);
        currentPageIndex = pageIndex;
    }

    //loads answers from pages
    void CollectCurrentPageAnswers()
    {
        var currentPage = pages[currentPageIndex];
        var manager = currentPage.GetComponent<LikertQuestionManager>();

        if (manager != null)
        {
            var answers = manager.GetPageAnswers(currentPageIndex);
            allAnswers.AddRange(answers);

            UnityEngine.Debug.Log($"Collected answers from page {currentPageIndex + 1}: {string.Join(", ", answers)}");
        }
        else
        {
            UnityEngine.Debug.Log($"No LikertQuestionManager found on page {currentPageIndex + 1}");
        }
    }

    //saves answers to csv, filename can be adjustes
    void SaveAllAnswersToFile()
    {
        //int currentConditionIndex = experimentManager.currentConditionIndex - 1;
        //int conditionId = experimentManager.conditions[currentConditionIndex].idCondition;
        //int playerID = experimentManager.playerID;
        string distinctFilename = fileName +  ".csv";

        string path = string.IsNullOrEmpty(customPath) ? Application.persistentDataPath : customPath;
        string filePath = GetUniqueFilePath(Path.Combine(path, distinctFilename));

        Directory.CreateDirectory(path);

        List<string> csvLines = new List<string>();

        long duration = currentSurveyTime - lastSurveyTime;
        lastSurveyTime = currentSurveyTime;


        for (int i = 0; i < allAnswers.Count; i++)
        {
            csvLines.Add($"Q{i + 1},{allAnswers[i]}");
        }

        csvLines.Add("T, task time in Millis," + duration);

        File.WriteAllLines(filePath, csvLines.ToArray());
        UnityEngine.Debug.Log($"Answers saved to {filePath}");

        allAnswers.Clear();
    }

    //ensures that all answers are saved in individual files
    string GetUniqueFilePath(string baseFilePath)
    {
        string filePath = baseFilePath;
        int count = 1;

        while (File.Exists(filePath))
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFilePath);
            string extension = Path.GetExtension(baseFilePath);
            filePath = Path.Combine(Path.GetDirectoryName(baseFilePath), $"{fileNameWithoutExtension}_{count}{extension}");
            count++;
        }

        return filePath;
    }

    //function to deactivate all pages and continue with experiment
    //can be called from external experiment manager 
    public void CloseAndSave()
    {
        if (currentPageIndex == pages.Length - 1)
        {
            CollectCurrentPageAnswers();
            SaveAllAnswersToFile();
            UnityEngine.Debug.Log("Questionnaire completed and answers saved.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("CloseAndSave called before reaching the last page.");
        }

        foreach (var manager in likertManagers)
        {
            manager.ResetAnswers();
        }

        foreach (var page in pages)
        {
            page.SetActive(false);
        }

        //recorder.StartRecording();
    }

    // function to close without saving for "redo"-functionality if questionnaire has to be repeated
    public void CloseWithoutSaving()
    {
        foreach (var manager in likertManagers)
        {
            manager.ResetAnswers();
        }

        foreach (var page in pages)
        {
            page.SetActive(false);
        }

        currentPageIndex = 0;

        UnityEngine.Debug.Log("Survey closed without saving and reset for next use.");
    }
}
