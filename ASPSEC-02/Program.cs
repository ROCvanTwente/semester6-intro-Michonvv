namespace ASPSEC_02;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    private static readonly string key = "0123456789abcdef0123456789abcdef"; // 32 chars = 256-bit key
    private static readonly string iv = "abcdef9876543210"; // 16 chars = 128-bit IV

    static void Main()
    {
        Console.Write("Voer het creditcardnummer in: ");
        string creditCardNumber = Console.ReadLine();

        string encrypted = Encrypt(creditCardNumber);
        Console.WriteLine($"Versleuteld: {encrypted}");

        string decrypted = Decrypt(encrypted);
        Console.WriteLine($"Ontsleuteld: {decrypted}");
    }

    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public static string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
