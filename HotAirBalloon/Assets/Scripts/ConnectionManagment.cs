using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

// User user = new User("test"+cnt, "test");
// string json = JsonUtility.ToJson(user);
// dbReference.Child("users").SetRawJsonValueAsync(json);
// cnt++;

public class ConnectionManagment : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject connectionPanel;
    [SerializeField]
    private GameObject createPanel;
    [SerializeField]
    private GameObject usernamePanel;
    [SerializeField]
    private GameObject connectionsPanel;
    [SerializeField]
    private GameObject resetPasswordPanel;
    [SerializeField]
    private GameObject deleteAccountPanel;
    [SerializeField]
    private GameObject settingsPanel;

    private Firebase.Auth.FirebaseAuth auth;

    private int cnt = 0;
    private bool connected = false;
    private bool usernameToCreate = false;
    private bool insetNewUser = false;

    private bool disconnect = false;
    private string errorMessage = "";
    private Text textError;
    private string username;


    private DatabaseReference dbReference;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gamehotairballoon-1.firebaseio.com/");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser == null)
        {
            connectionsPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
        else
        {
            CheckUser();
        }
    }

    void FixedUpdate()
    {
        if (connected)
        {
            connectionsPanel.SetActive(false);
            createPanel.SetActive(false);
            connectionPanel.SetActive(false);
            usernamePanel.SetActive(false);
            mainPanel.SetActive(true);
            connected = false;
        }
        if (usernameToCreate)
        {
            connectionsPanel.SetActive(false);
            createPanel.SetActive(false);
            connectionPanel.SetActive(false);
            usernamePanel.SetActive(true);
            mainPanel.SetActive(false);
            usernameToCreate = false;
        }
        if (insetNewUser)
        {
            insetNewUser = false;
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
                connected = true;
            });
        }
        if(disconnect)
        {
            disconnect=false;
            Disconnect();
        }
        if (errorMessage != "")
        {
            if (createPanel.active)
            {
                textError = createPanel.transform.Find("TextError").GetComponent<Text>();
            }
            else if (connectionPanel.active)
            {
                textError = connectionPanel.transform.Find("TextError").GetComponent<Text>();
            }
            else if (usernamePanel.active)
            {
                textError = usernamePanel.transform.Find("TextError").GetComponent<Text>();
            }
            else if (resetPasswordPanel.active)
            {
                textError = resetPasswordPanel.transform.Find("TextError").GetComponent<Text>();
            }
            textError.text = errorMessage;
            errorMessage = "";
        }
    }

    private void CheckUser()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + StaticClass.GetHashString(auth.CurrentUser.Email)).GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  if (!snapshot.Exists)
                  {
                      usernameToCreate = true;
                  }
              }
          });
    }

    public void Connect()
    {
        string email = connectionPanel.transform.Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;
        string psw = connectionPanel.transform.Find("InputFieldPassword").GetComponent<InputField>().text;

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
                        default:
                            errorMessage = fbEx.Message;
                            break;
                    }
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + fbEx.ErrorCode + "   " + fbEx.Message);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                CheckUser();
                connected = true;
            });
        }
    }

    public void Disconnect()
    {
        auth.SignOut();
        deleteAccountPanel.SetActive(false);
        settingsPanel.SetActive(false);
        connectionsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void CreateUsername()
    {
        username = usernamePanel.transform.Find("InputFieldUsername").Find("Text").GetComponent<Text>().text;

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

                insetNewUser = true;
            });
        }
    }

    public void CreateAccount()
    {
        string email = createPanel.transform.Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;
        string psw1 = createPanel.transform.Find("InputFieldPassword1").GetComponent<InputField>().text;
        string psw2 = createPanel.transform.Find("InputFieldPassword2").GetComponent<InputField>().text;

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
                        default:
                            errorMessage = fbEx.Message;
                            break;
                    }
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + fbEx.ErrorCode + "   " + fbEx.Message);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                usernameToCreate = true;
            });
        }
        else
        {
            errorMessage = "Les mots de passes ne sont pas identiques";
        }
    }

    public void Quit()
    {
        Application.Quit();
        // SendResetEmail();
    }

    public void OpenKeyBoard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void SendResetEmail()
    {
        string emailAddress = resetPasswordPanel.transform.Find("InputFieldEmail").Find("Text").GetComponent<Text>().text;

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
        if (auth.CurrentUser != null)
        {
            auth.CurrentUser.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }


                Debug.Log("User deleted successfully.");
            });
        }
    }
}
