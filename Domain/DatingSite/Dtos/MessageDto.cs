using System.Text.Json.Serialization;

namespace Domain.DatingSite.Dtos
{
    public  class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderPhototUrl { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientPhototUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

        [JsonIgnore]
        public bool SenderDeleted { get; set; }
        [JsonIgnore]
        public bool RecipientDeletedd { get; set; }

    }
}
