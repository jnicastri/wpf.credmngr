﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CredManager.Data
{
    [Serializable]
    public class CredItem
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
