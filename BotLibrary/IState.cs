using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public interface IState : ICloneable {
        string GetName();
        void AddTag(string tag, string stateName, string color = null, bool hidden = false);
        bool DeleteTag(string tag);
        string GetNextState(string tag);
        string GetMessage();
        IEnumerable<BotTag> GetTags();
    }
}
