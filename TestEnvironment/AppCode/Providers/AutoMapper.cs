using System.Reflection;

namespace MultiLanguageProvider.AppCode.Providers
{
    #region Test Entities
    //public class BookEntity : Old
    //{
    //	public string Title { get; set; }
    //	public string Author { get; set; }
    //	public string Description { get; set; }
    //	public decimal price { get; set; }
    //}

    //public class BookDetails : New
    //{
    //	public string Title { get; set; }
    //	public string Author { get; set; }
    //}
    #endregion
    public static class AutoMapper
    {
        public static TNew Map<TNew>(Object oldObject) where TNew : class, new()
        {
            TNew newObject = new();
            try
            {
                if (oldObject is null)
                    return newObject;

                Type typeOfOldObject = oldObject.GetType();  // BookEntity
                foreach (PropertyInfo propertyOfNewObject in typeof(TNew).GetProperties())   // 2 props : (Title və Author)
                {
                    //try to find current prop in Old Object
                    PropertyInfo? propertyOfOldObject = typeOfOldObject.GetProperty(propertyOfNewObject.Name, BindingFlags.Public | BindingFlags.Instance);

                    //if same properties is existed in 2 sides...
                    if (propertyOfOldObject != null)
                    {
                        (Type oldPropertyType, Type newPropertyType) = (propertyOfOldObject.PropertyType, propertyOfNewObject.PropertyType); // string, string
                        if (newPropertyType == oldPropertyType)
                            propertyOfNewObject.SetValue(newObject, propertyOfOldObject.GetValue(oldObject));
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Error occured while mapping objects each other!");
            }
            return newObject;
        }
    }
}
