using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NINA.Contradrift {
    [Export(typeof(ResourceDictionary))]
    partial class Options : ResourceDictionary {

        public Options() {
            InitializeComponent();
        }
        public void UpdateLog(string message) {
            if (string.IsNullOrEmpty(message)) { return; }
            //NINA.Contradrift.Options

        }
    }
}