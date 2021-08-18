using BLambda.HolaMundo.Domain.TemperatureLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLambda.HolaMundo.Helper
{
    public class StatJsonConverter : JsonConverter<IStat>
    {
        public override IStat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IStat value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
