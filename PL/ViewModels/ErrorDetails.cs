using Newtonsoft.Json;

namespace PL.ViewModels
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
