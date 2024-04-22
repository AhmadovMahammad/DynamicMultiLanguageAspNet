using MultiLanguageProvider.AppCode.Infrastructure;
using MultiLanguageProvider.AppCode.Providers;
using Newtonsoft.Json;
using System.Reflection;

namespace MultiLanguageProvider.AppCode.Extensions
{
    public enum LanguageOptions
    {
        Aze,
        Eng
    }
    public class LanguageProvider
    {
        private readonly string? _assemblyLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        private readonly string _fileName = string.Empty;
        private readonly string _directoryName = string.Empty;
        public LanguageProvider(string fileName, string directoryName)
        {
            _fileName = fileName;
            _directoryName = Path.Combine(_assemblyLocation!, "DynamicResources", directoryName);
        }
        public void CreateDirectory()
        {
            if (!Directory.Exists(_directoryName))
                Directory.CreateDirectory(_directoryName);
        }

        public void WritePairs(IMultiLanguage model)
        {
            WriteEntityById(model, LanguageOptions.Eng);
            WriteEntityById(model, LanguageOptions.Aze);
        }
        public void RemovePairs(int id)
        {
            RemoveEntityById(id, LanguageOptions.Eng);
            RemoveEntityById(id, LanguageOptions.Aze);
        }
        public void UpdatePairs(int id, IMultiLanguage updatedModel)
        {
            UpdateEntityById(id, updatedModel, LanguageOptions.Eng);
            UpdateEntityById(id, updatedModel, LanguageOptions.Aze);
        }
        public List<Dictionary<string, string>>? ReadFullJson(LanguageOptions languageOptions)
        {
            string jsonPath = GenerateJsonFilePath(languageOptions);
            if (!File.Exists(jsonPath))
                return null;

            string jsonContent = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);
        }


        #region CRUD OPERATIONS
        private void WriteEntityById(IMultiLanguage model, LanguageOptions languageOptions)
        {
            Dictionary<string, string> dictionary = GetDictionary(model, languageOptions);
            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            string jsonPath = GenerateJsonFilePath(languageOptions);

            if (File.Exists(jsonPath))
            {
                string existingJson = File.ReadAllText(jsonPath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    existingJson = existingJson.TrimEnd(',', ']', '\r', '\n');
                    existingJson += ",";
                    json = $"\n{json}\n]";
                    json = $"{existingJson}{json}";
                }
            }
            else
                json = $"[{Environment.NewLine}{json}{Environment.NewLine}]";

            File.WriteAllText(jsonPath, json);
        }
        private void RemoveEntityById(int id, LanguageOptions languageOptions)
        {
            string jsonPath = GenerateJsonFilePath(languageOptions);
            if (!File.Exists(jsonPath))
                return;

            string jsonContent = File.ReadAllText(jsonPath);
            List<Dictionary<string, string>>? previousEntities = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);
            if (previousEntities == null)
                return;

            List<Dictionary<string, string>> updatedEntities = previousEntities.Where(entity => entity.ContainsKey("Id") && int.TryParse(entity["Id"], out int entityId) && entityId != id).ToList();
            string updatedJson = JsonConvert.SerializeObject(updatedEntities, Formatting.Indented);

            if (updatedEntities.Count == 0)
                File.Delete(jsonPath);
            else
                File.WriteAllText(jsonPath, updatedJson);
        }
        private void UpdateEntityById(int id, IMultiLanguage updatedModel, LanguageOptions languageOptions)
        {
            string jsonPath = GenerateJsonFilePath(languageOptions);
            if (!File.Exists(jsonPath))
                return;

            string jsonContent = File.ReadAllText(jsonPath);
            List<Dictionary<string, string>>? entities = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);
            if (entities == null)
                return;

            Dictionary<string, string>? entityToUpdate = entities
                .FirstOrDefault(entity => entity.ContainsKey("Id") && int.TryParse(entity["Id"], out int entityId) && entityId == id);

            foreach (PropertyInfo property in updatedModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(string))
                    continue;

                if (Attribute.IsDefined(property, typeof(LocalizedPropertyAttribute)))
                {
                    string propName = property.Name;
                    object? propValue = property.GetValue(updatedModel);

                    if (languageOptions == LanguageOptions.Eng && propName.EndsWith("Eng"))
                    {
                        propName = propName.Remove(propName.Length - 3);
                        entityToUpdate![propName] = (string)propValue!;
                    }
                    else if (languageOptions == LanguageOptions.Aze && !propName.EndsWith("Eng"))
                        entityToUpdate![propName] = (string)propValue!;
                }
            }
            string updatedJson = JsonConvert.SerializeObject(entities, Formatting.Indented);
            File.WriteAllText(jsonPath, updatedJson);
        }
        #endregion

        #region HELPERS
        private string GenerateJsonFilePath(LanguageOptions options)
        {
            string languageSuffix = options == LanguageOptions.Eng ? ".en" : ".az";
            string jsonFileName = $"{_fileName}{languageSuffix}.json";
            return Path.Combine(_directoryName, jsonFileName);
        }
        private Dictionary<string, string> GetDictionary(IMultiLanguage model, LanguageOptions languageOptions)
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Id", model.Id.ToString() }
            };

            foreach (var property in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(string))
                    continue;

                if (Attribute.IsDefined(property, typeof(LocalizedPropertyAttribute)))
                {
                    string propName = property.Name;
                    object? propValue = property.GetValue(model);

                    if (languageOptions == LanguageOptions.Eng && propName.EndsWith("Eng"))
                    {
                        propName = propName.Remove(propName.Length - 3);
                        dictionary.Add(propName, (string)propValue!);
                    }
                    else if (languageOptions == LanguageOptions.Aze && !propName.EndsWith("Eng"))
                        dictionary.Add(propName, (string)propValue!);
                }
            }
            return dictionary;
        }
        #endregion
    }
}
