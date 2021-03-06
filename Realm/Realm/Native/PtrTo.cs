////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Realms
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PtrTo<T> where T : struct
    {
        private void* ptr;

        internal T? Value
        {
            get
            {
                if (ptr == null)
                {
                    return null;
                }

                var @struct = default(T);
                Unsafe.CopyBlock(Unsafe.AsPointer(ref @struct), ptr, (uint)Unsafe.SizeOf<T>());
                return @struct;
            }
        }

        public PtrTo(IntPtr intPtr)
        {
            ptr = intPtr.ToPointer();
        } 
    }
}