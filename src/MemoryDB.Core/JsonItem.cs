using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace MemoryDB.Core
{
    public class JsonItem<T> where T: new ()
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public JsonItem()
        {
            
        } 

        public JsonItem(T item)
        {
            Value = JsonConvert.SerializeObject(item);
            Id = GetIdValue(item);
        }

        private int GetIdValue(T item)
        {
            var idProperty = GetIdProperty();
            var idValue = idProperty.GetValue(item, null);
            return (int) idValue;
        }

        private  PropertyInfo GetIdProperty()
        {
            var myObject = new T();
            var myType = myObject.GetType();
            var myProperties = myType.GetProperties();
            var objectTypeName = myType.Name;

            // Is there a property named id (case irrelevant)?
            var idProperty = myProperties.FirstOrDefault(n => n.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            if (idProperty == null)
            {
                // Is there a property named TypeNameId (case irrelevant)?
                string findName = $"{objectTypeName}{"id"}";
                idProperty = myProperties.FirstOrDefault(n => n.Name.Equals(findName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (idProperty != null) return idProperty;
            var keyNotDefinedMessageFormat = ""
                                                + "No Id property is defined on {0}. Please define a property which forms a unique key for objects of this type.";
            throw new Exception(string.Format(keyNotDefinedMessageFormat, objectTypeName));
        }

        public T Deserialize()
        {
            return JsonConvert.DeserializeObject<T>(Value);
        }
    }
}
