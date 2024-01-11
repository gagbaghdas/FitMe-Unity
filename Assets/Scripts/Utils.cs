using System.IO;
using UnityEngine;

public static class Utils
{
    public static void SaveTextureAsPNG(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Image saved to {path}");
    }

    public static Texture2D LoadTextureFromPath(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData); // Automatically resizes the texture.
            return texture;
        }
        return null;
    }
}
