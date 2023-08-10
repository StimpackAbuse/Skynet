using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Skynet
{
    static class JSONUtility
    {
        private static readonly string jsonFormat = ".json";

        /// <summary>
        /// Creates JSON file.
        /// </summary>
        public static void Serialize<T>(string path, string fileName, T item)
        {
            Serialize(Path.Combine(path, fileName), item);
        }

        /// <summary>
        /// Creates JSON file.
        /// </summary>
        public static void Serialize<T>(string fileName, T item)
        {
            //In case fileName doesn't ends with .json
            if (!fileName.EndsWith(jsonFormat))
                fileName += jsonFormat;

            string jsonContent = JsonConvert.SerializeObject(item);
            File.WriteAllText(fileName, jsonContent);
        }

        /// <summary>
        /// Reads JSON file and returns it.
        /// </summary>
        public static T? Deserialize<T>(string path, string fileName)
        {
            return Deserialize<T>(Path.Combine(path, fileName));
        }

        /// <summary>
        /// Reads JSON file and returns it.
        /// </summary>
        public static T? Deserialize<T>(string fileName)
        {
            //In case fileName doesn't ends with .json
            if (!fileName.EndsWith(jsonFormat))
                fileName += jsonFormat;

            string fullPath = fileName;
            if (!File.Exists(fullPath))
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_JSON_FILE_NOT_FOUND);
                return default;
            }

            string jsonContent = File.ReadAllText(fullPath);
            T? result = JsonConvert.DeserializeObject<T>(jsonContent);

            if (result == null)
            {
                ErrorManager.WriteErrorMessage(ErrorCode.MAINMDL_JSON_FILE_DESERIALIZATION_FAILED);
                return default;
            }

            return result;
        }
    }
}
