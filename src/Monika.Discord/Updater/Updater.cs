// Fucking Monikammmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
// mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
// this on MIT btw uwu

using System;
using System.Net.Http;
using Squirrel.Windows;

namespace Monika.Discord.Updater
{
    public class UpdateService 
    {
        public static void Main () 
        {
            // check for Operating system first
            var os = Environment.OSVersion;
            var platform = os.Platform;
            switch(platform)
            {
                #region Windows Updater case
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                //TODO: Squirrel stuff here
                 break;
               #endregion
               
               #region Unix Updater case
                case PlatformID.Unix :
                //TODO: System.Http stuff here
                break;
               #endregion
            }
            
        }
    }
}