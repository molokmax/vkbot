using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public class BotMessage {
        public string UserId { get; set; }
        public string Message { get; set; }
        public IEnumerable<BotTag> Tags { get; set; }
    }
}
