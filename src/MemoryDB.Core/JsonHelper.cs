using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MemoryDB.Core
{
    public class JsonHelper<T> where T : new()
    {
        public List<JsonItem> SerializeList(List<T> list)
        {
            var jsonList = new List<JsonItem>();
            foreach (var item in list)
            {
                var jsonItem = new JsonItem {Value = JsonConvert.SerializeObject(item)};
                jsonList.Add(jsonItem);
            }
            return jsonList;
        }


        public List<T> DeserilizeList(List<JsonItem> jsonlist)
        {
            var js = new JsonSerializer();
            var list = new List<T>();

            foreach (var jsonItem in jsonlist)
            {
                using (var stringReader = new StringReader(jsonItem.Value))
                {
                    using (var jsonReader = new JsonTextReader(stringReader))
                    {
                        var item = js.Deserialize<T>(jsonReader);
                       list.Add(item);
                    }
                }
            }

            return list;
        }
    }


}
