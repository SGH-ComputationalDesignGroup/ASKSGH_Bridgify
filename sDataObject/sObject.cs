﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject
{
    public interface sObject
    {
       Guid objectGUID { get; set; }
    }
}
