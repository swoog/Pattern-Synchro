using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pattern.Synchro.Client
{
    public class SynchroConverter : JsonConverter
    {
        private bool canWrite = true;

        public override bool CanWrite => this.canWrite;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            this.canWrite = false;
            
            JToken t = JToken.FromObject(value, serializer);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = (JObject)t;

                var type = value.GetType();
                var assemblyFullName = type.Assembly.FullName.Split(',')[0];
                o.AddFirst(new JProperty("typename", new JValue($"{type.FullName}, {assemblyFullName}")));

                o.WriteTo(writer);
            }
            
            this.canWrite = true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            
            var jObject = JObject.Load(reader);

            object target = this.Create(objectType, jObject);

            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        private object Create(Type objectType, JObject jObject)
        {
            return Activator.CreateInstance(Type.GetType(jObject.Value<string>("typename")));
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntity).IsAssignableFrom(objectType);
        }
    }
}