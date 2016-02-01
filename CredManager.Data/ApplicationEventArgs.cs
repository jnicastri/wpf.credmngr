using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CredManager.Data
{
    public class StatusEventArg : EventArgs
    {
        public string Message { get; set; }
    }
}
