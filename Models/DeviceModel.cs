using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace clientAPP.Models
{
    public class DeviceModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;

        [JsonPropertyName("problemDescription")]
        public string ProblemDescription { get; set; } = string.Empty;

        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }

        [JsonPropertyName("clientName")]
        public string ClientName { get; set; } = string.Empty;
    }
}
