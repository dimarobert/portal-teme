using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface IJsonSerializer {

        string Serialize(object obj);
        T Deserialize<T>(string value);
    }

    public class JsonNetSerializer : IJsonSerializer {
        public string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string value) {
            return JsonConvert.DeserializeObject<T>(value);
        }

    }
}
