using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public class BotTag {
        public int Order { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public string NextState { get; set; }
        public bool Hidden { get; set; }
    }
}
