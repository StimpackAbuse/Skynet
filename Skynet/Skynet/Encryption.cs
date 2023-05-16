using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Encryption;

namespace Skynet
{
    public class Encryption
    {
        //https://github.com/NServiceBusExtensions/Newtonsoft.Json.Encryption#usage

        private static readonly int iteration = 100000;
        private static readonly byte[] key = Encoding.UTF8.GetBytes("Xx1O4WBoJTUFCcECE3Mof2eG0vmtBjgv");
        private static byte[] initVector;

        public static Rfc2898DeriveBytes CreateKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = SHA512.Create().ComputeHash(keyBytes);

            return new Rfc2898DeriveBytes(keyBytes, saltBytes, iteration);
        }

        public static Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorBytes = Encoding.UTF8.GetBytes(vector);
            byte[] saltBytes = SHA512.Create().ComputeHash(vectorBytes);

            return new Rfc2898DeriveBytes(vectorBytes, saltBytes, iteration);
        }

        public static string Encrypt<T>(string origin, T instance)
        {
            // per app domain
            var factory = new EncryptionFactory();
            var serializer = new JsonSerializer
            {
                ContractResolver = factory.GetContractResolver()
            };

            // transferred as meta data with the serialized payload
            

            string serialized;
            using (var algorithm = new RijndaelManaged
            {
                Key = key
            })
            {
                //TODO: store initVector for use in deserialization
                initVector = algorithm.IV;
                using (factory.GetEncryptSession(algorithm))
                {
                    var builder = new StringBuilder();
                    using (var writer = new StringWriter(builder))
                    {
                        serializer.Serialize(writer, instance);
                    }

                    serialized = builder.ToString();
                }
            }

            return serialized;
        }

        public static T Decrypt<T>(string origin)
        {
            // per app domain
            var factory = new EncryptionFactory();
            var serializer = new JsonSerializer
            {
                ContractResolver = factory.GetContractResolver()
            };

            // per deserialize session
            using (var algorithm = new RijndaelManaged
            {
                IV = initVector,
                Key = key
            })
            {
                using (factory.GetDecryptSession(algorithm))
                {
                    var stringReader = new StringReader(origin);
                    var jsonReader = new JsonTextReader(stringReader);
                    var deserialized = serializer.Deserialize<T>(jsonReader);
                    return deserialized;
                }
            }
        }
    }
}
