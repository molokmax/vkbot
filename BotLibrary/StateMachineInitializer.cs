using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace BotLibrary {
    public class StateMachineInitializer : IStateMachineInitializer {
        private readonly string configPath;
        public StateMachineInitializer(string configPath = "./config.xml") {
            this.configPath = configPath;
        }

        public void Save(IList<IState> store) {
            XmlDocument xDoc = new XmlDocument();


            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xDoc.DocumentElement;
            xDoc.InsertBefore(xmlDeclaration, root);


            XmlElement rootNode = xDoc.CreateElement(string.Empty, "Root", string.Empty);
            xDoc.AppendChild(rootNode);

            foreach (IState state in store) {
                XmlElement page = xDoc.CreateElement(string.Empty, "Page", string.Empty);
                page.SetAttribute("Name", state.GetName());
                rootNode.AppendChild(page);

                XmlElement msg = xDoc.CreateElement(string.Empty, "Message", string.Empty);
                XmlCDataSection msgText = xDoc.CreateCDataSection(state.GetMessage());
                msg.AppendChild(msgText);
                page.AppendChild(msg);

                foreach (BotTag tag in state.GetTags()) {
                    XmlElement tagNode = xDoc.CreateElement(string.Empty, "Tag", string.Empty);
                    tagNode.SetAttribute("TagName", tag.Name);
                    tagNode.SetAttribute("PageName", tag.NextState);
                    tagNode.SetAttribute("Color", tag.Color);
                    tagNode.SetAttribute("Hidden", tag.Hidden.ToString());
                    page.AppendChild(tagNode);
                }
            }

            xDoc.Save(configPath);
        }

        public void Initialize(IStateMachine machine) {
            if (!File.Exists(configPath)) {
                throw new ApplicationException($"Configuration file '{configPath}' don't exists");
            } else {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(configPath);
                XmlElement xRoot = xDoc.DocumentElement;

                // выбор всех дочерних узлов
                XmlNodeList pages = xRoot.SelectNodes("Page");
                foreach (XmlNode p in pages) {
                    XmlAttribute nameNode = p.Attributes["Name"];
                    if (nameNode == null) {
                        throw new ApplicationException("Page haven't Name element");
                    }
                    XmlNode msgNode = p.SelectSingleNode("Message");
                    if (msgNode == null) {
                        throw new ApplicationException("Page haven't Message element");
                    }
                    IState state = new State(nameNode.Value, msgNode.InnerText);
                    XmlNodeList tags = p.SelectNodes("Tag");
                    foreach (XmlNode t in tags) {
                        XmlAttribute tagNameNode = t.Attributes["TagName"];
                        if (tagNameNode == null) {
                            throw new ApplicationException("Tag haven't TagName element");
                        }
                        XmlAttribute stateNameNode = t.Attributes["PageName"];
                        if (stateNameNode == null) {
                            throw new ApplicationException("Tag haven't PageName element");
                        }
                        XmlAttribute colorNode = t.Attributes["Color"];
                        XmlAttribute hiddenNode = t.Attributes["Hidden"];
                        bool hidden = hiddenNode == null ? false : Boolean.Parse(hiddenNode.Value);
                        state.AddTag(tagNameNode.Value, stateNameNode.Value, colorNode?.Value, hidden);
                    }
                    machine.AddState(state);
                }
            }
        }
    }
}
