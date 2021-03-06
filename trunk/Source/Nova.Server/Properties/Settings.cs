﻿// This file is Copyright 2005 OpenArrow Software.
//
// OpenArrow Software grants you a license to copy, modify, 
// use, and distribute this file according to the terms of the 
// Common Public License version 1.0. You should have a copy 
// of this license in the license.txt file located at the root
// of this project. If not, Visit 
// http://www.opensource.org/licenses/cpl1.0.php or 
// send e-mail to openarrow@gmail.com to view a copy of this license

//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version:2.0.40607.85
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace Nova.Server.Properties {
    
    
    sealed partial class Settings : System.Configuration.ApplicationSettingsBase {
        
        private static Settings m_Value;
        
        private static object m_SyncObject = new object();
        
        [System.Diagnostics.DebuggerNonUserCode()]
        public static Settings Value {
            get {
                if ((Settings.m_Value == null)) {
                    System.Threading.Monitor.Enter(Settings.m_SyncObject);
                    if ((Settings.m_Value == null)) {
                        try {
                            Settings.m_Value = new Settings();
                        }
                        finally {
                            System.Threading.Monitor.Exit(Settings.m_SyncObject);
                        }
                    }
                }
                return Settings.m_Value;
            }
        }
    }
}
