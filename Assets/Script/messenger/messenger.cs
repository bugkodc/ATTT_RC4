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

public class messenger : MonoBehaviour
{
    [SerializeField] public GameObject _messenger_user;
    [SerializeField] public GameObject _messenger_friend;
    [SerializeField] private GameObject _scrollViewContent;
    [SerializeField] public Button _insert;
    [SerializeField] private TMP_InputField _messen;
    [SerializeField] public TextMeshProUGUI _bug;
    [SerializeField] public Button _trolai;
    [SerializeField] public Button _reset;
    private string _key;
    private string _thongDiep;
    private void Start()
    {
        StartCoroutine(Connect());
        _insert.onClick.AddListener(Insert);
        _trolai.onClick.AddListener(Trolai);
        _reset.onClick.AddListener(Reload);
    }
    public void Trolai()
    {
        PlayerPrefs.SetString("RoomID", null);
        PlayerPrefs.SetString("Key", null);
        SceneManager.LoadScene("key");
    }
    public void Reload()
    {
        StartCoroutine(Connect());
    }
    IEnumerator Connect()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("RoomID", PlayerPrefs.GetString("RoomID")));
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_ATTT/messenger.php", formData))
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
        ResetScrollViewContent();
        var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonString);

        foreach (var message in data)
        {
            string userName = message["UserID"];
            string messageText = message["MessageText"];
            string messageTime = message["MessageTime"];
            DecryptFile(messageText);
            if (userName == PlayerPrefs.GetString("userID"))
            {
                _messenger_user.transform.Find("messen").GetComponent<TextMeshProUGUI>().text = _thongDiep;
                _messenger_user.transform.Find("date").GetComponent<TextMeshProUGUI>().text = messageTime;
                Instantiate(_messenger_user, _scrollViewContent.transform);
            }
            else
            {
                _messenger_friend.transform.Find("messen").GetComponent<TextMeshProUGUI>().text = _thongDiep;
                _messenger_friend.transform.Find("date").GetComponent<TextMeshProUGUI>().text = messageTime;
                Instantiate(_messenger_friend, _scrollViewContent.transform);
            }
        }
    }
    private void ResetScrollViewContent()
    {
        Transform content = _scrollViewContent.transform;
        foreach (Transform child in content)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    public void Insert()
    {
        EncryptFile(_messen.text);
        StartCoroutine(Connect_ib());
        StartCoroutine(Connect());
    }
    IEnumerator Connect_ib()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("RoomID", PlayerPrefs.GetString("RoomID")));
        formData.Add(new MultipartFormDataSection("SenderID", PlayerPrefs.GetString("userID")));
        formData.Add(new MultipartFormDataSection("MessageText", _messen.text));
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/Unity_ATTT/insert.php", formData))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseData = www.downloadHandler.text;
                Debug.Log(responseData);
            }
            else
            {
                // Yêu cầu thất bại, hiển thị lỗi
                Debug.Log("Error: " + www.error);
            }
        }
    }
    #region EncryptFile
    private void EncryptFile(string messen)
    {
        _key = PlayerPrefs.GetString("Key"); 
        if (_key.Length != 0)
        {
            _thongDiep = messen;
            try
            {
                byte[] encryptedBytes = RC4.Encrypt(Encoding.UTF8.GetBytes(_thongDiep), Encoding.UTF8.GetBytes(_key));
                string encryptedText = Convert.ToBase64String(encryptedBytes);

                _messen.text = encryptedText;
            }
            catch (FormatException)
            {
                _bug.text = "Thông điệp sai không thể mã hóa";
            }
        }
    }
    #endregion
    #region DecryptFile
    private void DecryptFile(string messen)
    {
        _key = PlayerPrefs.GetString("Key");
        if (_key.Length != 0)
        {
            _thongDiep = messen;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(_thongDiep);
                byte[] decryptedBytes = RC4.Decrypt(encryptedBytes, Encoding.UTF8.GetBytes(_key));
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                _thongDiep = decryptedText;
            }
            catch (FormatException)
            {
                _bug.text = "Thông điệp sai không thể giải mã (không thuộc Base-64)";
            }
        }
    }
    #endregion
}