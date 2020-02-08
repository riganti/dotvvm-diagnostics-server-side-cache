﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;

namespace DotVVM.Diagnostics.ServerSideCache.Sample.ViewModels
{
    public class DefaultViewModel : MasterPageViewModel
    {
        public string Title { get; set; }

        public string UserData { get; set; }

        public DefaultViewModel()
        {
            Title = "Hello from DotVVM!";
        }

        public void TestPostBack()
        {
        }
    }
}
