using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Start9.NodeControl
{
    public enum EntryType
    {
        Receiver,
        Message
    }

    public static class EntryTypeExtensions
    {
        public static EntryType GetForEntry(EntryViewModel m) => m.MessageEntry == null ? EntryType.Receiver : EntryType.Message;
    }
}
