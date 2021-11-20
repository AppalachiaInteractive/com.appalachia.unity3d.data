using System;
using System.Text;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Utility.Encryption;
using Appalachia.Utility.Logging;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Data.Core.Configuration
{
    [Serializable, SmartLabelChildren]
    public class DatabaseConfigurationSettings
    {
        #region Constants and Static Readonly

        private const int KEY_SIZE = 40;
        private const string APPA = "appa";
        private const string APPAEDCS = APPA + "e" + DCS;
        private const string APPAUDCS = APPA + "u" + DCS;
        private const string DCS = "dcs";

        #endregion

        #region Static Fields and Autoproperties

        [JsonIgnore] private static char[] _chars;
        [JsonIgnore] private static Random _random;

        #endregion

        #region Fields and Autoproperties

        public bool isEncrypted;
        public DatabaseTechnology technology;
        [ReadOnly] public string key;
        public string name;

        #endregion

        public static DatabaseConfigurationSettings Deserialize(string dataStoragePath)
        {
            using (_PRF_Deserialize.Auto())
            {
                var path = GetDCSPath(dataStoragePath, true);

                var content = AppaFile.ReadAllText(path);
                var cleanContent = AppaCipher.Decrypt(
                    content,
                    APPASTR.EncryptionKeys.DB_CONFIG_SETTINGS
                );
                var deserialized = JsonConvert.DeserializeObject<DatabaseConfigurationSettings>(cleanContent);

                return deserialized;
            }
        }

        [ButtonGroup(APPASTR.Keys)]
        public void GenerateNewKey()
        {
            Initialize();

            var output = new StringBuilder(KEY_SIZE);

            for (var i = 0; i < KEY_SIZE; i++)
            {
                var random = _random.Next(0, _chars.Length - 1);

                output.Append(random);
            }

            key = output.ToString();
        }

        public void Serialize(string dataStoragePath)
        {
            using (_PRF_Serialize.Auto())
            {
                var pathE = GetDCSPath(dataStoragePath, true);
                var pathU = GetDCSPath(dataStoragePath, false);

                var contentE = Serialize(true);
                var contentU = Serialize(false);

                AppaFile.WriteAllText(pathE, contentE);
                AppaFile.WriteAllText(pathU, contentU);

#if UNITY_EDITOR
                AssetDatabaseManager.ImportAsset(pathE);
                AssetDatabaseManager.ImportAsset(pathU);
#endif
            }
        }

        [ButtonGroup(APPASTR.Keys)]
        public void Test()
        {
            var plainTextJson = Serialize(false);
            var encryptedJson = Serialize(true);
            var decryptedJson = AppaCipher.Decrypt(
                encryptedJson,
                APPASTR.EncryptionKeys.DB_CONFIG_SETTINGS
            );

            var match = plainTextJson == decryptedJson;

            var output =
                $"Match: {match}\r\nOriginal: {plainTextJson}\r\nEncrypted: {encryptedJson}\r\nDecrypted: {decryptedJson}";
            if (match)
            {
                AppaLog.Info(output);
            }
            else
            {
                AppaLog.Error(output);
            }
        }

        private static string GetDCSPath(string dataStoragePath, bool encrypted)
        {
            var fileName = AppaPath.GetFileNameWithoutExtension(dataStoragePath);
            var directoryPath = AppaPath.GetDirectoryName(dataStoragePath);

            if (encrypted)
            {
                return AppaPath.Combine(directoryPath, $"{fileName}.{APPAEDCS}");
            }

            return AppaPath.Combine(directoryPath, $"{fileName}.{APPAUDCS}");
        }

        private static void Initialize()
        {
            _random ??= new Random();
            _chars ??= new[]
            {
                'a',
                'b',
                'c',
                'd',
                'e',
                'f',
                'g',
                'h',
                'i',
                'j',
                'k',
                'l',
                'm',
                'n',
                'o',
                'p',
                'q',
                'r',
                's',
                't',
                'u',
                'v',
                'w',
                'x',
                'y',
                'z',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'X',
                'Y',
                'Z',
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9'
            };
        }

        private string Serialize(bool encrypt)
        {
            using (_PRF_Serialize.Auto())
            {
                var serialized = JsonConvert.SerializeObject(this);

                if (!encrypt)
                {
                    return serialized;
                }

                var encrypted = AppaCipher.Encrypt(
                    serialized,
                    APPASTR.EncryptionKeys.DB_CONFIG_SETTINGS
                );

                return encrypted;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(DatabaseConfigurationSettings) + ".";

        private static readonly ProfilerMarker _PRF_Deserialize =
            new ProfilerMarker(_PRF_PFX + nameof(Deserialize));

        private static readonly ProfilerMarker _PRF_Serialize =
            new ProfilerMarker(_PRF_PFX + nameof(Serialize));

        #endregion
    }
}
