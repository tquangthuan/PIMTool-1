﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PIMTool.Services.Resource
{
    public abstract class BaseResource
    {
        public virtual int Id { get; set; }
        public virtual int RowVersion { get; set; }
    }
}