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

public class rooms : MonoBehaviour
{
    [SerializeField] public Button _vao;
    [SerializeField] public Button _trolai;
    [SerializeField] public Button _key_ramdom;
    [SerializeField] public TMP_InputField _rooms;
    [SerializeField] public TMP_InputField _key;
    [SerializeField] public TextMeshProUGUI _bug;
    void Start()
    {
        _vao.onClick.AddListener(Vao);
        _key_ramdom.onClick.AddListener(GenerateRandomKey);
        _trolai.onClick.AddListener(Trolai);
    }

   public void Vao()
    {
        StartCoroutine(Connect());
    }
    public void Trolai()
    {
        PlayerPrefs.SetString("userID", null) ;
        SceneManager.LoadScene("dangnhap");
    }
    private void GenerateRandomKey()
    {
        _key.text = GenerateRandomKeyOfLength(16); // Đặt độ dài key tùy ý, ở đây là 16
    }
    private string GenerateRandomKeyOfLength(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string key = "";
        for (int i = 0; i < length; i++)
        {
            key += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return key;
    }
    IEnumerator Connect()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("UserID", PlayerPrefs.GetString("userID")));
        formData.Add(new MultipartFormDataSection("rooms", _rooms.text));
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_ATTT/rooms.php", formData))
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
                PlayerPrefs.SetString("RoomID", _rooms.text);
                PlayerPrefs.SetString("Key", _key.text);
                Debug.Log(userID);
                SceneManager.LoadScene("messger");
            }
            else
            {
                Debug.Log("No userID found in the response");
            }
        }
        else if (message == "Login failed")
        {
            _bug.text = "phòng sai";
        }
    }
}
