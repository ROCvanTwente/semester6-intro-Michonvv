using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

class Program
{
    private static readonly string key = "0123456789abcdef0123456789abcdef"; // 32 chars = 256-bit key
    private static readonly string iv = "abcdef9876543210"; // 16 chars = 128-bit IV

    static void Main()
    {
        using (var db = new AppDbContext())
        {
            db.Database.Migrate(); // Zorgt ervoor dat de database wordt aangemaakt als deze nog niet bestaat
        }

        Console.Write("Voer voornaam in: ");
        string firstName = Console.ReadLine();

        Console.Write("Voer achternaam in: ");
        string lastName = Console.ReadLine();

        Console.Write("Voer straat in: ");
        string street = Console.ReadLine();

        Console.Write("Voer huisnummer in: ");
        string houseNumber = Console.ReadLine();

        Console.Write("Voer postcode in: ");
        string postalCode = Console.ReadLine();

        Console.Write("Voer woonplaats in: ");
        string city = Console.ReadLine();

        Console.Write("Voer het creditcardnummer in: ");
        string creditCardNumber = Console.ReadLine();

        string encryptedCreditCard = Encrypt(creditCardNumber);

        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Street = street,
            HouseNumber = houseNumber,
            PostalCode = postalCode,
            City = city,
            EncryptedCreditCard = encryptedCreditCard
        };

        using (var db = new AppDbContext())
        {
            db.People.Add(person);
            db.SaveChanges();
        }

        Console.WriteLine("Persoon succesvol opgeslagen!");
    }

    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            // Declare the MemoryStream outside the using block
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            } // Now ms is still accessible

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

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string EncryptedCreditCard { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Person> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.\\MSSQLSERVER01;Database=PeopleDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
