using System;
using System.Collections.Generic;
using System.Linq;

namespace BotLibrary {
    public class State : IState {
        private readonly string Name;
        private readonly string Message;
        private readonly IDictionary<string, BotTag> Tags;

        private const string DefaultColor = "WHITE";

        public State(string name, string message) {
            Name = name;
            Message = message;
            Tags = new Dictionary<string, BotTag>();
        }

        public void AddTag(string tagName, string stateName, string color = null, bool hidden = false) {
            BotTag tag = new BotTag() {
                Name = tagName,
                NextState = stateName,
                Order = Tags.Count,
                Color = color ?? DefaultColor,
                Hidden = hidden
            };
            Tags[tagName.ToLowerInvariant()] = tag;
        }

        public bool DeleteTag(string tag) {
            return Tags.Remove(tag.ToLowerInvariant());
        }

        public string GetName() {
            return Name;
        }

        public string GetMessage() {
            return Message;
        }

        public IEnumerable<BotTag> GetTags() {
            return Tags.Values.OrderBy(t => t.Order);
        }

        public string GetNextState(string tag) {
            if (Tags.TryGetValue(tag.ToLowerInvariant(), out BotTag next)) {
                return next.NextState;
            } else {
                return null;
            }
        }

        public object Clone() {
            IState clone = new State(Name, Message);
            foreach (BotTag t in Tags.Values) {
                clone.AddTag(t.Name, t.NextState, t.Color, t.Hidden);
            }
            return clone;
        }
    }
}
