using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSection[] sections;
    [SerializeField] private int currentSection = -1;
    public bool finishedTutorial = false;

    public void Init()
    {
        foreach(var section in sections)
        {
            section.Init();
        }
    }

    public void BeginTutorial()
    {
        currentSection = -1;
        Continue();
    }

    private void Continue()
    {
        currentSection++;
        if(currentSection >= sections.Length)
        {
            finishedTutorial = true;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("LI_tutorial_Finished");
            return;
        }

        sections[currentSection].onCompletion += Continue;
        sections[currentSection].Activate();
    }

    public void SavePersistentData(PersistentData persistentData)
    {
        persistentData.finishedTutorial = finishedTutorial;
    }

    public void LoadPersistentData(PersistentData persistentData)
    {
        finishedTutorial = persistentData.finishedTutorial;
    }
}

[System.Serializable]
public class TutorialSection
{
    [SerializeField] private TutorialPopup[] popups;
    [SerializeField] private Requirement[] requirements;
    private int leftRequirements = 0;

    [Tooltip("By default section can be completed before it popups, check this to test completion only after it popups")]
    public bool lateCompletion = false;
    public bool isCompleted = false;

    public void Init()
    {
        leftRequirements = requirements.Length;
        foreach(var popup in popups)
        {
            popup.gameObject.SetActive(false);
            popup.closePopup.Init();
            popup.closePopup.onClick += Complete;
        }

        if (!lateCompletion)
        {
            ConnectRequirements();
        }     
    }

    public void Activate()
    {
        if (isCompleted)
        {
            Complete();
            return;
        }

        ChangePopupsState(true);

        if (lateCompletion)
        {
            ConnectRequirements();
        }
    }

    public UnityAction onCompletion;
    public void Complete()
    {
        foreach (var req in requirements)
        {
            RequirementsManager.Instance.DisconnectRequirementFromValueEvent(req);
        }

        isCompleted = true;
        ChangePopupsState(false);

        onCompletion?.Invoke();     
    }

    private void ConnectRequirements()
    {
        foreach (var req in requirements)
        {
            RequirementsManager.Instance.ConnectRequirementToValueEvent(req);
            req.onStateChanged += CheckIfCompleted;
        }
    }

    private void CheckIfCompleted(bool newRequirementState)
    {
        leftRequirements += newRequirementState ? -1 : 1;

        if(leftRequirements <= 0)
        {
            Complete();
        }
    }

    private void ChangePopupsState(bool state)
    {
        foreach(var popup in popups)
        {
            if (state)
            {
                popup.gameObject.SetActive(true);
            } else if (popup.gameObject.activeSelf){
                popup.StartFinishingSequence();
            }
        }
    }
}
