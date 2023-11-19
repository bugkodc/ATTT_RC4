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
    private byte[] key;
    private byte[] fileData;
    #region start
    private void Start()
    {
        _confirmButton.onClick.AddListener(OnConfirmButtonClick);
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
        key = new byte[16]; // Key length in bytes
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(key);
        }
        string keyHex = BitConverter.ToString(key).Replace("-", string.Empty);
        string keyString = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyHex));
        _Key_Input.text = keyString;
    }
    #endregion
    #region EncryptFile
    private void EncryptFile()
    {
        if (_Key_Input.text == null || key.Length != 16)
        {
            _ThongDiep_Output.text = "Please generate a key first!";
            return;
        }
        RC4 rc4 = new RC4(key);
        byte[] encryptedData = rc4.Encrypt(fileData);
        string keyString = ConvertByteArrayToString(encryptedData);
        _ThongDiep_Output.text = keyString;
    }
    #endregion
    #region DecryptFile
    private void DecryptFile()
    {
        if (_Key_Input.text == null || key.Length != 16)
        {
            _ThongDiep_Output.text = "Please generate a key first!";
            return;
        }
        RC4 rc4 = new RC4(key);
        byte[] decryptedData = rc4.Decrypt(fileData);
        string keyString = Encoding.UTF8.GetString(decryptedData);
        _ThongDiep_Output.text = keyString;
    }
    #endregion
    #region string to byte and byte to string
    private string ConvertByteArrayToString(byte[] byteArray)
    {
        string hexString = Encoding.UTF8.GetString(byteArray);
        return hexString;
    }

    private byte[] ConvertToByteArray(string hexString)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(hexString);
        return byteArray;
    }
    private bool IsValidHexString(string hexString)
    {
        Regex regex = new Regex("^[0-9A-F]+$");
        return regex.IsMatch(hexString);
    }
    #endregion
    #region xacnhan input
    private void OnConfirmButtonClick()
    {
        string keyString = _Key_Input.text;
        key = ConvertToByteArray(keyString);
        string DataString = _ThongDiep_Input.text;
        fileData = ConvertToByteArray(DataString);
    }
    #endregion
}
#region RC4
public class RC4
{
    private byte[] S;
    private byte[] T;
    private int keyLength;

    public RC4(byte[] key)
    {
        keyLength = key.Length;
        S = new byte[256];
        T = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            S[i] = (byte)i;
            T[i] = key[i % keyLength];
        }

        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + S[i] + T[i]) % 256;
            Swap(S, i, j);
        }
    }

    public byte[] Encrypt(byte[] data)
    {
        byte[] encryptedData = new byte[data.Length];
        int i = 0;
        int j = 0;

        for (int k = 0; k < data.Length; k++)
        {
            i = (i + 1) % 256;
            j = (j + S[i]) % 256;

            Swap(S, i, j);

            int t = (S[i] + S[j]) % 256;

            encryptedData[k] = (byte)(data[k] ^ S[t]);
        }

        return encryptedData;
    }

    public byte[] Decrypt(byte[] encryptedData)
    {
        // Decrypt the encrypted data using the RC4 algorithm
        byte[] decryptedData = Encrypt(encryptedData);

        // Convert the decrypted data back to a string using the correct encoding
        string decryptedString = Encoding.UTF8.GetString(decryptedData);
        return decryptedString;
    }

    private void Swap(byte[] array, int i, int j)
    {
        byte temp = array[i];
        array[i] = array[j];
        array[j] = temp;
      }
}
#endregion