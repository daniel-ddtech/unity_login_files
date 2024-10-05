using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    // Remove the OnAvatarUpdate event if it's not needed
    // public event Action OnAvatarUpdate;

    private PlayerInfo playerInfo;
    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;

    private bool isInitialized = false;

    private async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
            PlayerAccountService.Instance.SignedIn += SignedIn;
            playerProfile = new PlayerProfile();
            isInitialized = true;
            Debug.Log("Unity Services initialized successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
        }
    }

    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SignIn failed: {ex.Message}");
        }
    }

    public async Task InitSignIn()
    {
        if (!isInitialized)
        {
            Debug.LogError("Unity Services not initialized. Cannot start sign-in process.");
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"InitSignIn failed: {ex.Message}");
        }
    }

    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null after successful sign-in");
                return;
            }

            string name = "Unknown";
            try
            {
                name = await AuthenticationService.Instance.GetPlayerNameAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get player name: {ex.Message}");
            }

            playerProfile = new PlayerProfile(playerInfo, name);

            OnSignedIn?.Invoke(playerProfile);
            TransitionToGameScene();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"Authentication failed: {ex.Message}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error during sign-in: {ex.Message}");
        }
    }

    private void TransitionToGameScene()
    {
        // Load the SampleScene after successful login
        SceneManager.LoadScene("SampleScene");
    }

    private void OnDestroy()
    {
        if (isInitialized && PlayerAccountService.Instance != null)
        {
            PlayerAccountService.Instance.SignedIn -= SignedIn;
        }
    }
}

[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;

    public PlayerProfile(PlayerInfo info, string name)
    {
        playerInfo = info;
        Name = name;
    }
}
