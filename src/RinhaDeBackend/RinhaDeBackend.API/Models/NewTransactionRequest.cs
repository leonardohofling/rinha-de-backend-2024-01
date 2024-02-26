using RinhaDeBackend.API.Validators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RinhaDeBackend.API.Models
{
    public class NewTransactionRequest
    {
        [JsonPropertyName("valor")]
        [Range(0, int.MaxValue)]
        [Required]
        public int Amount { get; set; }

        [JsonPropertyName("tipo")]
        [Required]
        [TypeValidator]
        public string Type {  get; set; }

        [JsonPropertyName("descricao")]
        [Required]
        [MaxLength(10)]
        public String Description {  get; set; }
    }
}
