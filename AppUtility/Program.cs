// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hello, Exim Bank Hash Utility!");
string key = "DEV-INTERVIEW-KEY-2025";

using var sha = SHA512.Create();
var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
string hashed = Convert.ToHexString(hashBytes);

Console.WriteLine(hashed);
