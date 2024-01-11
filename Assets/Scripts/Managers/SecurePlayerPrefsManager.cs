using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public static class SecurePlayerPrefsManager
{
    private static readonly byte[] encryptionKey = new byte[] 
    {
        0xfd, 0xab, 0x3c, 0xc6, 0x28, 0x41, 0x26, 0xe8, 
        0x4b, 0x81, 0x37, 0xa5, 0xe8, 0xb4, 0x08, 0x17, 
        0x42, 0x27, 0x43, 0xc0, 0xf5, 0x96, 0x0d, 0xc4, 
        0x86, 0x15, 0x79, 0x6c, 0x1f, 0xd6, 0x5b, 0xc3
    };


    private const string keyUserId = "user_id";

    private static void SetString(string key, string value)
    {
        var encryptedValue = EncryptString(value);
        PlayerPrefs.SetString(key, encryptedValue);
    }

    private static string GetString(string key, string defaultValue = "")
    {
        var value = PlayerPrefs.GetString(key, defaultValue);
        return value == string.Empty ? string.Empty : DecryptString(value);
    }

    private static string EncryptString(string text)
    {

        using var aesAlg = Aes.Create();
        using var encryptor = aesAlg.CreateEncryptor(encryptionKey, aesAlg.IV);
        using var msEncrypt = new System.IO.MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
        {
            swEncrypt.Write(text);
        }

        var iv = aesAlg.IV;
        var decryptedContent = msEncrypt.ToArray();
        var result = new byte[iv.Length + decryptedContent.Length];

        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        return Convert.ToBase64String(result);
    }

    private static string DecryptString(string cipherText)
    {
        // Convert base64-encoded text to bytes
        var fullCipher = Convert.FromBase64String(cipherText);

        // Extract the IV from the beginning of the full cipher
        var iv = new byte[16];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);

        // Extract the actual cipher text from after the IV
        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        using var aesAlg = Aes.Create();
        aesAlg.Key = encryptionKey;
        aesAlg.IV = iv;
        aesAlg.Padding = PaddingMode.PKCS7; // Make sure padding mode matches the encryption

        // Create a decryptor to perform the stream transform.
        using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for decryption.
        using var msDecrypt = new MemoryStream(cipher);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        // Read the decrypted bytes from the decrypting stream and place them in a string.
        return srDecrypt.ReadToEnd();
    }


    public static void SetUserId(string userId)
    {
        SetString(keyUserId, userId);
    }

    public static string GetUserId()
    {
        return GetString(keyUserId);
    }


}
