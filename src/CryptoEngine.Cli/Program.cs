using System;
using CryptoEngine;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Running Crypto CLI");

        var config = new CryptoConfig();
        var encrypted = CryptoFactory.Encrypt("Hello, world!", config);

        Console.WriteLine($"Encrypted: {Convert.ToBase64String(encrypted)}");
    }
}