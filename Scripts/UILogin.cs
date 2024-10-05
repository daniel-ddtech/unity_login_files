using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text userIdText;
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Transform loginPanel, userPanel;
    [SerializeField] private LoginController loginController;

    private PlayerProfile playerProfile;

    private void Start()
    {
        Debug.Log("UILogin Start method called");
        ValidateReferences();
        
        loginButton.onClick.AddListener(OnLoginButtonClick);
        loginController.OnSignedIn += LoginController_OnSignedIn;
    }

    private void ValidateReferences()
    {
        if (loginButton == null) Debug.LogError("Login button is null");
        if (loginController == null) Debug.LogError("LoginController is null");
        if (userIdText == null) Debug.LogError("userIdText is null");
        if (userNameText == null) Debug.LogError("userNameText is null");
        if (loginPanel == null) Debug.LogError("loginPanel is null");
        if (userPanel == null) Debug.LogError("userPanel is null");
    }

    private async void OnLoginButtonClick()
    {
        Debug.Log("Login button clicked");
        loginButton.interactable = false;
        try
        {
            await loginController.InitSignIn();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login failed: {ex.Message}");
        }
        finally
        {
            loginButton.interactable = true;
        }
    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        Debug.Log("LoginController_OnSignedIn called");
        this.playerProfile = profile;
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("UpdateUI called");
        ValidateReferences();
        
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);
        
        if (playerProfile.playerInfo != null)
        {
            if (userIdText != null) userIdText.text = $"User ID: {playerProfile.playerInfo.Id}";
            if (userNameText != null) userNameText.text = $"Name: {playerProfile.Name}";
        }
        else
        {
            Debug.LogError("PlayerInfo is null");
            if (userIdText != null) userIdText.text = "User ID: N/A";
            if (userNameText != null) userNameText.text = "Name: N/A";
        }
    }

    private void OnDestroy()
    {
        if (loginButton != null) loginButton.onClick.RemoveListener(OnLoginButtonClick);
        if (loginController != null) loginController.OnSignedIn -= LoginController_OnSignedIn;
    }
}
