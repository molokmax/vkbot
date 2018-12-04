using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public interface IStateMachineInitializer {
        void Initialize(IStateMachine machine);
        void Save(IList<IState> store);
    }
}
