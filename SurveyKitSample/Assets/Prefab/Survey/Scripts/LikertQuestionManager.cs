using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LikertQuestionManager : MonoBehaviour
{
    [System.Serializable]
    public class LikertQuestion
    {
        public GameObject questionPrefab;
        [HideInInspector]
        public TextMeshProUGUI questionText;
        [HideInInspector]
        public Button[] radioButtons;
        [HideInInspector]
        public Button selectedButton;
    }

    public LikertQuestion[] questions;
    public Sprite checkedSprite;
    public Sprite uncheckedSprite;
    public Button nextButton;

    private List<string> pageAnswers = new List<string>();

    void Start()
    {
        InitializeQuestions();

        foreach (var question in questions)
        {
            foreach (var button in question.radioButtons)
            {
                button.onClick.AddListener(() => OnRadioButtonClicked(question, button));
                button.image.sprite = uncheckedSprite;
            }
        }
        SetNextButtonState(false);
    }

    // Initializes questions from question Array
    // Likert Question Objects can be assigned in inspector
    // Automatically recogniztes valid answer buttons -> works for all answer scales that can be answered via button or scale
    void InitializeQuestions()
    {
        foreach (var question in questions)
        {
            if (question.questionPrefab != null)
            {
                question.questionText = question.questionPrefab.GetComponentInChildren<TextMeshProUGUI>();

                var buttons = question.questionPrefab.GetComponentsInChildren<Button>();
                question.radioButtons = new Button[buttons.Length];
                for (int i = 0; i < buttons.Length; i++)
                {
                    question.radioButtons[i] = buttons[i];
                }
            }
        }
    }

    // handles logic if radio button is clicked
    // button changes sprite to indicate selection
    // assigns button as valid answer
    // calls CheckAllQuestionsAnswered to see if "next" button should appear
    void OnRadioButtonClicked(LikertQuestion question, Button clickedButton)
    {
        Debug.Log($"Button clicked: {clickedButton.name}");

        if (question.selectedButton != null)
        {
            question.selectedButton.image.sprite = uncheckedSprite;
            Debug.Log($"Deselected button: {question.selectedButton.name}");
        }

        clickedButton.image.sprite = checkedSprite;
        question.selectedButton = clickedButton;
        Debug.Log($"Selected button: {clickedButton.name}");

        CheckAllQuestionsAnswered();
    }

    // checks if all questions are answerd
    /// if yes button is shown
    void CheckAllQuestionsAnswered()
    {
        foreach (var question in questions)
        {
            if (question.selectedButton == null)
            {
                SetNextButtonState(false);
                return;
            }
        }
        SetNextButtonState(true);
        Debug.Log("All questions answered, next button enabled.");
    }

    // function to set next button active
    // used to display button only if all questions are answered
    void SetNextButtonState(bool state)
    {
        nextButton.interactable = state;
        nextButton.gameObject.SetActive(state);
    }

    // function to retrieve page answers
    public List<string> GetPageAnswers(int pageIndex)
    {
        pageAnswers.Clear();
        for (int i = 0; i < questions.Length; i++)
        {
            int selectedIndex = System.Array.IndexOf(questions[i].radioButtons, questions[i].selectedButton) + 1;
            string questionText = questions[i].questionText.text;

            string formattedQuestionText = questionText.Replace(",", "");

            pageAnswers.Add($"{formattedQuestionText},{selectedIndex}");
        }

        Debug.Log($"Page {pageIndex + 1} answers: {string.Join(", ", pageAnswers)}");
        return new List<string>(pageAnswers);
    }

    // Automatically assign components from the prefab in the inspector
    void OnValidate()
    {
        foreach (var question in questions)
        {
            if (question.questionPrefab != null)
            {
                question.questionText = question.questionPrefab.GetComponentInChildren<TextMeshProUGUI>();

                var buttons = question.questionPrefab.GetComponentsInChildren<Button>();
                question.radioButtons = new Button[buttons.Length];
                for (int i = 0; i < buttons.Length; i++)
                {
                    question.radioButtons[i] = buttons[i];
                }
            }
        }
    }

    // Resets all page answer values to null
    // resets button states
    public void ResetAnswers()
    {
        foreach (var question in questions)
        {
            if (question.selectedButton != null)
            {
                question.selectedButton.image.sprite = uncheckedSprite;
                question.selectedButton = null;
            }
        }
        SetNextButtonState(false);
    }
}
