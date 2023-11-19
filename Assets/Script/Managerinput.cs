using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Text;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.Windows;
using System.Linq;
using System.Text.RegularExpressions;

public class Managerinput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _Key_Input;
    [SerializeField] private TMP_InputField _ThongDiep_Input;
    [SerializeField] private TMP_InputField _ThongDiep_Output;
    [SerializeField] public Button _RandomKey_Button;
    [SerializeField] public Button _SelectFile_Button;
    [SerializeField] public Button _EncryptButton;
    [SerializeField] public Button _DecryptButton;
    [SerializeField] public Button _confirmButton;
    private string _key;
    private string _thongDiep;
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
    private void SelectFile()
    {
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select File", "", "");

        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string text = reader.ReadToEnd();
                _ThongDiep_Input.text = text;
            }
        }
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
        _thongDiep = _ThongDiep_Input.text;

        byte[] encryptedBytes = RC4.Encrypt(Encoding.UTF8.GetBytes(_thongDiep), Encoding.UTF8.GetBytes(_key));
        string encryptedText = Convert.ToBase64String(encryptedBytes);

        _ThongDiep_Output.text = encryptedText;
    }
    #endregion
    #region DecryptFile
    private void DecryptFile()
    {
        _key = _Key_Input.text;
        _thongDiep = _ThongDiep_Input.text;

        byte[] encryptedBytes = Convert.FromBase64String(_thongDiep);
        byte[] decryptedBytes = RC4.Decrypt(encryptedBytes, Encoding.UTF8.GetBytes(_key));
        string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        _ThongDiep_Output.text = decryptedText;
    }
    #endregion
}
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