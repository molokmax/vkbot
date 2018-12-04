using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public class StateMachine : IStateMachine {
        public StateMachine(string defaultStateName = "default") {
            DefaultState = defaultStateName;
        }

        public void AddState(IState state) {
            stateStore[state.GetName()] = state;
        }

        private readonly string DefaultState;

        private readonly IDictionary<string, IState> stateStore = new Dictionary<string, IState>();

        private readonly IDictionary<string, IState> userStore = new Dictionary<string, IState>();

        public IEnumerable<IState> GetStates() {
            return stateStore.Values;
        }

        public IState GetState(string userId, string tag) {
            IState state;
            if (!userStore.TryGetValue(userId, out state)) {
                state = stateStore[DefaultState];
                userStore[userId] = state;
            }
            string nextStateName = state.GetNextState(tag);
            if (String.IsNullOrEmpty(nextStateName)) { // Не нашли перехода по заданному тэгу
                return state;
            } else {
                IState next = stateStore[nextStateName];
                userStore[userId] = next;
                return next;
            }
        }
    }
}
