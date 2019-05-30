using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class ConnectionManagment : MonoBehaviour
{
    [SerializeField]
    private GameObject canvasPanel;


    private string state = "";
    private string errorMessage = "";
    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    private string username;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gamehotairballoon-1.firebaseio.com/");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            errorMessage = "Pas de connexion internet, veuillez vous connecter et relancer le jeu";
            canvasPanel.transform.Find("ConnectionsPanel").Find("ButtonConnection").GetComponent<Button>().interactable = false;
            canvasPanel.transform.Find("ConnectionsPanel").Find("ButtonCreate").GetComponent<Button>().interactable = false;
        }
        else
        {
            canvasPanel.transform.Find("ConnectionsPanel").Find("ButtonConnection").GetComponent<Button>().interactable = true;
            canvasPanel.transform.Find("ConnectionsPanel").Find("ButtonCreate").GetComponent<Button>().interactable = true;
        }

        if (auth.CurrentUser == null || Application.internetReachability == NetworkReachability.NotReachable)
        {
            state = "notConnected";
        }
        else
        {
            CheckUser();
        }
    }

    void Update()
    {
        switch (state)
        {
            case "connected":
                ShowPanel("MainPanel");
                break;
            case "usernameToCreate":
                ShowPanel("UsernamePanel");
                break;
            case "insertNewUser":
                InsertNewUser();
                break;
            case "disconnect":
                Disconnect();
                break;
            case "notConnected":
                ShowPanel("ConnectionsPanel");
                break;
        }
        state = "";

        if (errorMessage != "")
        {
            for (int i = 0; i < canvasPanel.transform.childCount; i++)
            {
                GameObject panel = canvasPanel.transform.GetChild(i).gameObject;
                if (panel.active)
                {
                    panel.transform.Find("TextError").GetComponent<Text>().text = errorMessage;
                }
            }
            errorMessage = "";
        }
    }

    private void CheckUser()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + StaticClass.GetHashString(auth.CurrentUser.Email)).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    state = "connected";
                }
                else
                {
                    state = "usernameToCreate";
                }
            }
        });
    }

    private void InsertNewUser()
    {
        User user = new User(username, auth.CurrentUser.Email);
        string json = JsonUtility.ToJson(user);
        dbReference.Child("users").Child(StaticClass.GetHashString(auth.CurrentUser.Email)).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error: " + task.Exception);
                return;
            }
            Debug.LogFormat("Username successfully inserted");
            state = "connected";
        });
    }

    private void ShowPanel(string[] panelsNames)
    {
        for (int i = 0; i < canvasPanel.transform.childCount; i++)
        {
            GameObject panel = canvasPanel.transform.GetChild(i).gameObject;
            panel.SetActive(Array.IndexOf(panelsNames, panel.name) != -1);
        }
    }

    private void ShowPanel(string panelName)
    {
        ShowPanel(new string[] { panelName });
    }

    public void Connect()
    {
        string email = canvasPanel.transform.Find("ConnectionPanel").Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;
        string psw = canvasPanel.transform.Find("ConnectionPanel").Find("InputFieldPassword").GetComponent<InputField>().text;

        if (email == "" || psw == "")
        {
            errorMessage = "Tous les champs doivent être rempli";
        }
        else
        {
            auth.SignInWithEmailAndPasswordAsync(email, psw).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    FirebaseException fbEx = (FirebaseException)task.Exception.Flatten().InnerExceptions[0];
                    switch (fbEx.ErrorCode)
                    {
                        case (int)Firebase.Auth.AuthError.WrongPassword:
                            errorMessage = "Mot de passe invalide";
                            break;
                        case (int)Firebase.Auth.AuthError.UserNotFound:
                            errorMessage = "Aucun utilisateur lié à cette adresse email";
                            break;
                        case (int)Firebase.Auth.AuthError.InvalidEmail:
                            errorMessage = "Email invalide";
                            break;
                        case (int)Firebase.Auth.AuthError.NetworkRequestFailed:
                            errorMessage = "Pas de connexion internet";
                            break;
                        default:
                            errorMessage = "Une erreure est survenue, veuillez réessayer plus tard";
                            break;
                    }
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + fbEx.ErrorCode + "   " + fbEx.Message);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                CheckUser();
                state = "connected";
            });
        }
    }

    public void Disconnect()
    {
        auth.SignOut();
        state="notConnected";
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenKeyBoard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void CreateAccount()
    {
        string email = canvasPanel.transform.Find("CreateAccountPanel").Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;
        string psw1 = canvasPanel.transform.Find("CreateAccountPanel").Find("InputFieldPassword1").GetComponent<InputField>().text;
        string psw2 = canvasPanel.transform.Find("CreateAccountPanel").Find("InputFieldPassword2").GetComponent<InputField>().text;

        if (email == "" || psw1 == "" || psw2 == "")
        {
            errorMessage = "Tous les champs doivent être rempli";
        }
        else if (psw1 == psw2)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, psw1).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    FirebaseException fbEx = (FirebaseException)task.Exception.Flatten().InnerExceptions[0];
                    switch (fbEx.ErrorCode)
                    {
                        case (int)Firebase.Auth.AuthError.WeakPassword:
                            errorMessage = "Mot de passe trop cours (minimum 6 caractères)";
                            break;
                        case (int)Firebase.Auth.AuthError.EmailAlreadyInUse:
                            errorMessage = "Email déjà lié à un compte";
                            break;
                        case (int)Firebase.Auth.AuthError.InvalidEmail:
                            errorMessage = "Email invalide";
                            break;
                        case (int)Firebase.Auth.AuthError.NetworkRequestFailed:
                            errorMessage = "Pas de connexion internet";
                            break;
                        default:
                            errorMessage = "Une erreure est survenue, veuillez réessayer plus tard";
                            break;
                    }
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + fbEx.ErrorCode + "   " + fbEx.Message);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                state = "usernameToCreate";
            });
        }
        else
        {
            errorMessage = "Les mots de passes ne sont pas identiques";
        }
    }

    public void CreateUsername()
    {
        username = canvasPanel.transform.Find("UsernamePanel").transform.Find("InputFieldUsername").Find("Text").GetComponent<Text>().text;

        if (username == "")
        {
            errorMessage = "Veuillez saisir un nom d'utilisateur";
        }
        else
        {
            dbReference.Child("usernames").Child(username).SetRawJsonValueAsync("\"" + auth.CurrentUser.Email + "\"").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    errorMessage = "Nom d'utilisateur déjà utilisé";
                    Debug.LogError(" error: " + task.Exception);
                    return;
                }

                Debug.LogFormat("Username successfully inserted");

                state = "insertNewUser";
            });
        }
    }

    public void SendResetEmail()
    {
        string emailAddress = canvasPanel.transform.Find("ResetPasswordPanel").transform.Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;

        auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
            errorMessage = "Un email de réinitialisation a été envoyé";
        });
    }

    public void DeleteAccount()
    {
        int nbLevel = canvasPanel.transform.Find("LevelPanel").GetComponent<LevelPanel>().GetNbLevel();
        string id = StaticClass.GetHashString(auth.CurrentUser.Email);

        for (int i = 0; i < nbLevel; i++)
        {
            dbReference.Child("generalscores").Child("race" + i).Child(id).RemoveValueAsync();
            dbReference.Child("usersscores").Child("race" + i).Child(id).RemoveValueAsync();
        }

        dbReference.Child("users").Child(id).RemoveValueAsync();

        dbReference.Child("usernames").OrderByValue().EqualTo(auth.CurrentUser.Email).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error in getting usernames for deletion : " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    Debug.Log(snapshot.ChildrenCount);
                    foreach (var item in snapshot.Children)
                    {
                        dbReference.Child("usernames").Child(item.Key).RemoveValueAsync();
                        auth.CurrentUser.DeleteAsync().ContinueWith(task2 =>
                        {
                            if (task2.IsCanceled)
                            {
                                Debug.LogError("DeleteAsync was canceled.");
                                return;
                            }
                            if (task2.IsFaulted)
                            {
                                Debug.LogError("DeleteAsync encountered an error: " + task2.Exception);
                                return;
                            }

                            Debug.Log("User deleted successfully.");
                        });
                        state = "notConnected";
                    }
                }

            }
        });
    }
}
