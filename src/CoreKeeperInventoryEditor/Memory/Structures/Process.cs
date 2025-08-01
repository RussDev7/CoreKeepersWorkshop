#pragma warning disable
/*
===============================================================================
  Memory.dll – Process-memory manipulation utilities for .NET.
  (Adapted and integrated from https://github.com/erfg12/memory.dll)

  ▸ Original License: GNU Lesser General Public License v2.1 (LGPL-2.1)
    See: https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html

  Summary:
  --------
  This file is part of a suite of classes providing cross-platform methods for
  reading, writing, and scanning memory in external processes. Adapted from the
  open-source Memory.dll library by erfg12, widely used for trainers, modding
  tools, and debugging utilities.

  Note:
  -----
  This code is integrated as source to help reduce antivirus false positives
  associated with distributing Memory.dll as a binary.

  Modifications:
  --------------
    • Disabled compiler warnings for third-party code.

  Attribution:
  ------------
  © 2016-2025 erfg12   – Creator of Memory.dll.
  © 2025      RussDev7 – Adaptations for CoreKeeperInventoryEditor.
===============================================================================
*/

using System;
using System.Diagnostics;

namespace Memory
{
    /// <summary>
    /// Information about the opened process.
    /// </summary>
    public class Proc
    {
        public Process Process { get; set; }
        public IntPtr Handle { get; set; }
        public bool Is64Bit { get; set; }
        //public ConcurrentDictionary<string, IntPtr> Modules { get; set; } // Use mProc.Process.Modules instead
        public ProcessModule MainModule { get; set; }
    }
}
