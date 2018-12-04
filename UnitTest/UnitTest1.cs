using BotLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
    [TestClass]
    public class UnitTest1 {

        [TestMethod]
        public void MachineInitializerTest() {
            IStateMachineInitializer initializer = new StateMachineInitializer("./config.xml");
            IStateMachine machine = new StateMachine();

            initializer.Initialize(machine);

            string userId = "1";
            IState st = machine.GetState(userId, "tag1");
            Assert.AreEqual("state1", st.GetName());
            Assert.AreEqual("first state\r\ndef - go to default state\r\ntag2 - go to state 2\r\ntag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "def");
            Assert.AreEqual("default", st.GetName());
            Assert.AreEqual("default state\r\ntag1 - go to state 1\r\ntag2 - go to state 2\r\ntag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "tag2");
            Assert.AreEqual("state2", st.GetName());
            Assert.AreEqual("second state\r\ndef - go to default state\r\ntag1 - go to state 1\r\ntag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "tag3");
            Assert.AreEqual("state3", st.GetName());
            Assert.AreEqual("third state\r\ndef - go to default state\r\ntag1 - go to state 1\r\ntag2 - go to state 2", st.GetMessage());

            st = machine.GetState(userId, "empty");
            Assert.AreEqual("state3", st.GetName());
            Assert.AreEqual("third state\r\ndef - go to default state\r\ntag1 - go to state 1\r\ntag2 - go to state 2", st.GetMessage());

        }

        [TestMethod]
        public void StateMachineTestMethod1() {
            IStateMachine machine = new StateMachine();
            IState defaultState = new State("default", "default state\r\n tag1 - go to state 1\r\n tag2 - go to state 2\r\n tag3 - go to state 3");
            defaultState.AddTag("tag1", "state1");
            defaultState.AddTag("tag2", "state2");
            defaultState.AddTag("tag3", "state3");
            machine.AddState(defaultState);

            IState state1 = new State("state1", "first state\r\n def - go to default state\r\n tag2 - go to state 2\r\n tag3 - go to state 3");
            state1.AddTag("tag2", "state2");
            state1.AddTag("tag3", "state3");
            state1.AddTag("def", "default");
            machine.AddState(state1);

            IState state2 = new State("state2", "second state\r\n def - go to default state\r\n tag1 - go to state 1\r\n tag3 - go to state 3");
            state2.AddTag("tag1", "state1");
            state2.AddTag("tag3", "state3");
            state2.AddTag("def", "default");
            machine.AddState(state2);

            IState state3 = new State("state3", "third state\r\n def - go to default state\r\n tag1 - go to state 1\r\n tag2 - go to state 2");
            state3.AddTag("tag1", "state1");
            state3.AddTag("tag2", "state2");
            state3.AddTag("def", "default");
            machine.AddState(state3);

            string userId = "1";
            IState st = machine.GetState(userId, "tag1");
            Assert.AreEqual("state1", st.GetName());
            Assert.AreEqual("first state\r\n def - go to default state\r\n tag2 - go to state 2\r\n tag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "def");
            Assert.AreEqual("default", st.GetName());
            Assert.AreEqual("default state\r\n tag1 - go to state 1\r\n tag2 - go to state 2\r\n tag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "tag2");
            Assert.AreEqual("state2", st.GetName());
            Assert.AreEqual("second state\r\n def - go to default state\r\n tag1 - go to state 1\r\n tag3 - go to state 3", st.GetMessage());

            st = machine.GetState(userId, "tag3");
            Assert.AreEqual("state3", st.GetName());
            Assert.AreEqual("third state\r\n def - go to default state\r\n tag1 - go to state 1\r\n tag2 - go to state 2", st.GetMessage());

            st = machine.GetState(userId, "empty");
            Assert.AreEqual("state3", st.GetName());
            Assert.AreEqual("third state\r\n def - go to default state\r\n tag1 - go to state 1\r\n tag2 - go to state 2", st.GetMessage());

        }
    }
}
