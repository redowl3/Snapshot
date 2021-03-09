using System;
namespace LaunchPad.Mobile.Models
{
    public class ConcernNote
    {
        public Guid ID { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
