using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public interface IStateMachine {
        void AddState(IState state);
        IState GetState(string userId, string tag);

        IEnumerable<IState> GetStates();
    }
}
