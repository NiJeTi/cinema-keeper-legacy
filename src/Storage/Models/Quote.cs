using System;

namespace CinemaKeeper.Storage.Models
{
    public class Quote : DatabaseEntity
    {
        public Quote(
            ulong author,
            string text,
            DateTimeOffset createdAt,
            ulong createdBy,
            ulong createdOn)
        {
            Author = author;
            Text = text;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
        }

        public ulong Author { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ulong CreatedBy { get; set; }
        public ulong CreatedOn { get; set; }
    }
}
