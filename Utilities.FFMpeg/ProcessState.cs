﻿/*
 * Copyright (C) 2009-2020 Solmead Productions
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 */
using System;

namespace Utilities.MediaConverter
{
    public class ProcessState
    {

        public int Frame { get; set; }
        public int FPS { get; set; }
        public float Q { get; set; }
        public int Size { get; set; }
        public float Time { get; set; }
        public float Bitrate { get; set; }
    }
}
