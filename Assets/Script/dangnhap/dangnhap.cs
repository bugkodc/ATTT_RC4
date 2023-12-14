using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using SFB;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json;
public class dangnhap : MonoBehaviour
{
    [SerializeField] public Button _dangnhap;
    [SerializeField] public Button _trolai;
    [SerializeField] public TMP_InputField _TK;
    [SerializeField] public TMP_InputField _MK;
    [SerializeField] public TextMeshProUGUI _bug;
    void Start()
    {
        _dangnhap.onClick.AddListener(Dangnhap);
        _trolai.onClick.AddListener(Trolai);
    }
    public void Trolai()
    {
        SceneManager.LoadScene("RC4");
    }
    public void Dangnhap()
    {
        StartCoroutine(Connect());
    }
    IEnumerator Connect()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("UserName", _TK.text));
        formData.Add(new MultipartFormDataSection("Password", _MK.text));
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_ATTT/dangnhap.php", formData))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseData = www.downloadHandler.text;
                Debug.Log(responseData);
                ProcessData(responseData);
            }
            else
            {
                // Yêu cầu thất bại, hiển thị lỗi
                Debug.Log("Error: " + www.error);
            }
        }
    }
    public void ProcessData(string jsonString)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        string message = data["message"];
        Debug.Log(message);

        if (message == "Login successful")
        {
            if (data.ContainsKey("userID"))
            {
                string userID = data["userID"];
                PlayerPrefs.SetString("userID", userID);
                Debug.Log(userID);
                SceneManager.LoadScene("key");
            }
            else
            {
                Debug.Log("No userID found in the response");
            }
        }
        else if (message == "Login failed")
        {
            _bug.text = "Tài khoản hoặc mật khẩu không đúng";
        }
    }
}