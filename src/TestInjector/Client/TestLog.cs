// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Text;

namespace TestInjector.Client
{
    public class TestLog
    {
        public static TestLog Current = new TestLog();

        private readonly StringBuilder _messages = new StringBuilder();
        public virtual void Trace(string message)
        {
            _messages.AppendLine(message);
        }

        public string MessageDetails
        {
            get { return _messages.ToString(); }
        }
    }
}
