using System;
using System.Collections.Generic;

namespace data
{
    [Serializable]
    public class ResponseData
    {
        public bool success;
        public string message;
        public Dictionary<string, object> data;
        public int status_code;
    }
}
