using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashingService
{
    class Program
    {
        private static readonly int DefaultSaltLength = 32;
        
        static void Main(string[] args)
        {
            ExecuteAsync().Wait();
        }

        private static async Task ExecuteAsync()
        {
            Console.WriteLine("Do you want to use an existing salt value? (y/n)");

            if (IsYesResponse())
            {
                var existingSalt = GetInputValue("Enter existing base 64 encoded salt value");
                var stringToHash = GetInputValue("Enter a base 64 encoded string to hash");
                var hashModel = GenerateHashWithSalt(existingSalt, stringToHash);
                
                PrintHashResults(hashModel);
            }
            else
            {
                Console.WriteLine();
                var stringToHash = GetInputValue("Enter a base 64 encoded string to hash");
                var hashModel = GenerateHashWithSalt(stringToHash);
                
                PrintHashResults(hashModel);
            }
            
            Console.WriteLine("Hash another value? (y/n)");
            if (IsYesResponse())
            {
                await ExecuteAsync();
            }
            
            Console.WriteLine("Pleasure doing business with you.");
            Environment.Exit(-1);
        }
        
        private static byte[] GenerateSalt()
        {
            Console.WriteLine();
            Console.WriteLine("Generating a new salt value");
            return GenerateSalt(DefaultSaltLength);
        }
        
        private static byte[] GenerateSalt(int maxSaltLength)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var rawByteArray = new byte[maxSaltLength];
       
                rng.GetNonZeroBytes(rawByteArray);

                return rawByteArray;
            }
           
        }

        private static SaltedHashModel GenerateHashWithSalt(string inputValue)
        {
            var salt = GenerateSalt();
            var hashAlgorithm = SHA256.Create();
            var saltedValueBytes = new List<byte>();
            
            saltedValueBytes.AddRange(Convert.FromBase64String(inputValue));
            saltedValueBytes.AddRange(salt);
            
            byte[] digestBytes = hashAlgorithm.ComputeHash(saltedValueBytes.ToArray());
            var saltString = Convert.ToBase64String(salt);
            var bytesString = Convert.ToBase64String(digestBytes);
            
            return new SaltedHashModel
            {
                HashedValue = bytesString,
                Salt = saltString,
                InputValue = inputValue
            };
        }

        private static SaltedHashModel GenerateHashWithSalt(string saltString, string inputValue)
        {
            var hashAlgorithm = SHA256.Create();
            var saltedValueBytes = new List<byte>();

            try
            {
                saltedValueBytes.AddRange(Convert.FromBase64String(inputValue));
                saltedValueBytes.AddRange(Convert.FromBase64String(saltString));
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex); 
            }
            

            byte[] digestBytes = hashAlgorithm.ComputeHash(saltedValueBytes.ToArray());
            var bytesString = Convert.ToBase64String(digestBytes);
            
            return new SaltedHashModel
            {
                HashedValue = bytesString,
                Salt = saltString,
                InputValue = inputValue
            };
        }
        
        private static bool IsYesResponse()
        {
            var response = Console.ReadLine();
            return string.Equals("y", response, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer , out int bytesParsed);
        }
        
        private static string GetInputValue(string prompt)
        {
            Console.WriteLine(prompt);
            var inputValue = Console.ReadLine();

            if (!IsBase64String(inputValue))
            {
                Console.WriteLine("The input value must be base64 encoded. Would you like to encode the value now? (y/n)");
                if (IsYesResponse())
                {
                    return Base64Encode(inputValue);
                }
                
                Console.WriteLine("Exiting program");
                Environment.Exit(-1);
            }

            if (string.IsNullOrWhiteSpace(inputValue))
            {
                throw new InvalidOperationException($"Value not provided for prompt: {prompt}");
            }

            return inputValue;
        }
        
        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        
        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static void PrintHashResults(SaltedHashModel hashModel)
        {
            Console.WriteLine();
            Console.WriteLine($"Original Value: {hashModel.InputValue}");
            Console.WriteLine($"Salt Value: {hashModel.Salt}");
            Console.WriteLine($"Hashed Value: {hashModel.HashedValue}");
            Console.WriteLine();
        }
    }
}