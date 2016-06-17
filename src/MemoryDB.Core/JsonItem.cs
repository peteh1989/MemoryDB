using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MemoryDB.Core
{
    public class JsonItem<T> where T: new ()
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public JsonItem(T item)
        {
            
        }


        private JsonItem<T> ConvertToJsonItem(T item)
        {
            var value = JsonConvert.SerializeObject(item);
            var jsonItem = new JsonItem<T>
            {
                Id = GetIdValue(),
                Value = value
            };

            return jsonItem;
        }

        private int GetIdValue()
        {
            var idProperty = GetIdProperty();
            var idValue = idProperty.GetValue(typeof(T), null);
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

    }
}
