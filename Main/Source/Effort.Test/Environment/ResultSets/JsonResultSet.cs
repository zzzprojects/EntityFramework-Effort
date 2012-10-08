
namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO;

    public class JsonResultSet : IResultSet
    {
        private IResultSet innerResultSet;

        public JsonResultSet(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader textReader = new StringReader(json);
            JsonReader jsonReader = new JsonTextReader(textReader);

            IDictionary<string, object>[] deserialized =
                serializer.Deserialize<IDictionary<string, object>[]>(jsonReader);

            this.innerResultSet = new DictionaryResultSet(deserialized);
        }

        public IEnumerable<IResultSetElement> Elements
        {
            get { return this.innerResultSet.Elements; }
        }
    }
}
