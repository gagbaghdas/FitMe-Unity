using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using data;

public class WelcomeScreenHandler : MonoBehaviour
{
    public TMP_Dropdown genderDropdown;
    public Toggle[] styleToggles;
    public Slider ageSlider;
    public TMP_Text ageText;

    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.Instance.User == null)
        {
            DataManager.Instance.User = new UserData() { styles = new List<int>() };
        }

        for (int i = 0; i < styleToggles.Length; i++)
        {
            int currentIndex = i;
            styleToggles[i].onValueChanged.AddListener((bool selected) => {
                OnStyleSelect(styleToggles[currentIndex], selected);
            });
        }
        ageText.text = ageSlider.value.ToString();
    }

    public void OnGenderChange()
    {
        DataManager.Instance.User.gender = genderDropdown.value;
    }

    public void OnAgeChange()
    {
        DataManager.Instance.User.age = (int)ageSlider.value;
        ageText.text = ageSlider.value.ToString();
    }

    public void OnStyleSelect(Toggle toggle, bool selected)
    {
        if (selected)
        {
            DataManager.Instance.User.styles.Add(toggle.GetComponent<StyleData>().id);
        }
        else
        {
            DataManager.Instance.User.styles.Remove(toggle.GetComponent<StyleData>().id);
        }
    }

    public void OnCreateButtonClick()
    {
        LoadingAnimationManager.Instance.Show();
        HttpManager.Instance.CreateUser(DataManager.Instance.User, (ResponseData response) =>
        {
            string userId = (string)response.data["user_id"];
            DataManager.Instance.User.id = userId;
            SecurePlayerPrefsManager.SetUserId(userId);

            LoadingAnimationManager.Instance.Hide();
            SceneSwitchManager.Instance.LoadMainScene();
        }, (string b) =>
        {
            LoadingAnimationManager.Instance.Hide();
        });
    }

}
