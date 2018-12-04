using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public class UserMessage {
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
        public string Message { get; set; }
    }
}
