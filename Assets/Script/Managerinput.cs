using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;
using SFB;
using System.Collections;
using UnityEngine.SceneManagement;

public class Managerinput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _Key_Input;
    [SerializeField] private TMP_InputField _ThongDiep_Input;
    [SerializeField] private TMP_InputField _ThongDiep_Output;
    [SerializeField] public Button _RandomKey_Button;
    [SerializeField] public Button _SelectFile_Button;
    [SerializeField] public Button _EncryptButton;
    [SerializeField] public Button _DecryptButton;
    private string _key;
    private string _thongDiep;
    public Text filePathText;
    #region start
    private void Start()
    {
        _RandomKey_Button.onClick.AddListener(GenerateRandomKey);
        _SelectFile_Button.onClick.AddListener(SelectFile);
        _EncryptButton.onClick.AddListener(EncryptFile);
        _DecryptButton.onClick.AddListener(DecryptFile);
    }
    #endregion
    #region input file
    [Obsolete]
    private void SelectFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }

    }
    private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);
        yield return loader;
        string textt = loader.text;
        _ThongDiep_Input.text = textt;
    }
    #endregion
    #region ramdomkey
    private void GenerateRandomKey()
   {
        _key = GenerateRandomKeyOfLength(16); // Đặt độ dài key tùy ý, ở đây là 16
        _Key_Input.text = _key;
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
    #endregion
    #region EncryptFile
    private void EncryptFile()
    {
        _key = _Key_Input.text;
        if (_key.Length != 0)
        {
            _thongDiep = _ThongDiep_Input.text;
            try
            {
                byte[] encryptedBytes = RC4.Encrypt(Encoding.UTF8.GetBytes(_thongDiep), Encoding.UTF8.GetBytes(_key));
                string encryptedText = Convert.ToBase64String(encryptedBytes);

                _ThongDiep_Output.text = encryptedText;
            }
            catch (FormatException)
            {
                _ThongDiep_Output.text = "Thông điệp sai không thể mã hóa";
            }
        }
    }
    #endregion
    #region DecryptFile
    private void DecryptFile()
    {
        _key = _Key_Input.text;
        if (_key.Length != 0) {
            _thongDiep = _ThongDiep_Input.text;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(_thongDiep);
                byte[] decryptedBytes = RC4.Decrypt(encryptedBytes, Encoding.UTF8.GetBytes(_key));
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                _ThongDiep_Output.text = decryptedText;
            }
            catch (FormatException)
            {
                _ThongDiep_Output.text = "Thông điệp sai không thể giải mã (không thuộc Base-64)";
            }
        }   
    }
    public void EXT()
    {
        Application.Quit();
    }
}
    #endregion
#region RC4
public static class RC4
{
    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        byte[] output = new byte[data.Length];
        byte[] S = new byte[256];
        byte[] K = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            S[i] = (byte)i;
            K[i] = key[i % key.Length];
        }

        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + S[i] + K[i]) % 256;
            Swap(S, i, j);
        }

        int x = 0;
        int y = 0;
        for (int i = 0; i < data.Length; i++)
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;
            Swap(S, x, y);
            int t = (S[x] + S[y]) % 256;
            output[i] = (byte)(data[i] ^ S[t]);
        }

        return output;
    }

    public static byte[] Decrypt(byte[] data, byte[] key)
    {
        return Encrypt(data, key);
    }

    private static void Swap(byte[] array, int i, int j)
    {
        byte temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}
#endregion